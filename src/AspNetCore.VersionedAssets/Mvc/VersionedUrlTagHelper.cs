using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.WebEncoders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace AspNetCore.VersionedAssets.Mvc
{
    [HtmlTargetElement("script", Attributes = VersionedAssetAttributeName)]
    [HtmlTargetElement("img", Attributes = VersionedAssetAttributeName)]
    [HtmlTargetElement("link", Attributes = VersionedAssetAttributeName)]
    public class VersionedUrlTagHelper : TagHelper
    {
        private const string VersionedAssetAttributeName = "asset-version";

        private static readonly Dictionary<string, string[]> elementAttributeLookup =
            new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
            {
                { "img", new[] { "src", "srcset" } },
                { "link", new[] { "href" } },
                { "script", new[] { "src" } }                               
            };

        private readonly HtmlEncoder htmlEncoder;
        private readonly Lazy<FileHashProvider> lazyHashProvider;
        private readonly VersionedAssetsOptions options;

        public VersionedUrlTagHelper(
            IHostingEnvironment hostingEnvironment,
            IMemoryCache cache,
            IOptions<VersionedAssetsOptions> options,
            HtmlEncoder htmlEncoder)
        {
            if (hostingEnvironment == null)
                throw new ArgumentNullException(nameof(hostingEnvironment));
            if (cache == null)
                throw new ArgumentNullException(nameof(cache));
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (htmlEncoder == null)
                throw new ArgumentNullException(nameof(htmlEncoder));

            lazyHashProvider = new Lazy<FileHashProvider>(() => new FileHashProvider(
                hostingEnvironment.WebRootFileProvider,
                cache,
                ViewContext.HttpContext.Request.PathBase));

            this.options = options.Value;
            this.htmlEncoder = htmlEncoder;
        }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(VersionedAssetAttributeName)]
        public VersionKind VersionKind { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (output == null)
                throw new ArgumentNullException(nameof(output));

            if (!options.IsEnabled)
                return;

            string[] attributeNames;
            if (elementAttributeLookup.TryGetValue(output.TagName, out attributeNames))
            {
                for (var i = 0; i < attributeNames.Length; i++)
                {
                    ProcessUrlAttribute(attributeNames[i], output);
                }
            }
        }

        protected void ProcessUrlAttribute(string attributeName, TagHelperOutput output)
        {
            var attIdx = output.Attributes.IndexOfName(attributeName);
            var attr = output.Attributes[attIdx];

            var path = GetAttributeValue(attr);
            var versionedPath = BuildVersionedUrl(path);

            output.Attributes[attIdx] = new TagHelperAttribute(attributeName, versionedPath);
        }

        private string BuildVersionedUrl(string path)
        {
            string versionPart;

            switch (VersionKind)
            {
                case VersionKind.GlobalVersion:
                    if (string.IsNullOrWhiteSpace(options.GlobalVersion))
                        return path;
                    versionPart = options.GlobalVersion;
                    break;

                case VersionKind.FileVersion:
                    versionPart = lazyHashProvider.Value.GetContentHash(path);
                    break;

                default:
                    throw new InvalidOperationException($"Unsupported version kind - {VersionKind}");
            }
            return string.Format($"{options.UrlPrefix}/{versionPart}{path}");
        }

        private string GetAttributeValue(TagHelperAttribute attribute)
        {
            var stringValue = attribute.Value as string;
            if (stringValue != null)
            {
                return stringValue;
            }
            else
            {
                var htmlContent = attribute.Value as IHtmlContent;
                if (htmlContent != null)
                {
                    var htmlString = htmlContent as HtmlString;
                    if (htmlString != null)
                    {
                        // No need for a StringWriter in this case.
                        stringValue = htmlString.ToString();
                    }
                    else
                    {
                        using (var writer = new StringWriter())
                        {
                            htmlContent.WriteTo(writer, htmlEncoder);
                            stringValue = writer.ToString();
                        }
                    }
                    return stringValue;
                }
            }
            throw new InvalidOperationException();
        }
    }

    public enum VersionKind
    {
        /// <summary>
        /// Use file content hash version
        /// </summary>
        FileVersion,

        /// <summary>
        /// Use gloval version
        /// </summary>
        GlobalVersion
    }
}
