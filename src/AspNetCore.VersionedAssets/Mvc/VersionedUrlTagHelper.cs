using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Mvc.ViewFeatures;
using Microsoft.AspNet.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.WebEncoders;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private readonly Lazy<FileHashProvider> lazyHashProvider;
        private readonly VersionedAssetsOptions options;

        public VersionedUrlTagHelper(
            IHostingEnvironment hostingEnvironment,
            IMemoryCache cache,
            IHtmlEncoder htmlEncoder,
            IUrlHelper urlHelper,
            Func<VersionedAssetsOptions> optionsProvider)
        {
            lazyHashProvider = new Lazy<FileHashProvider>(() => new FileHashProvider(
                hostingEnvironment.WebRootFileProvider,
                cache,
                ViewContext.HttpContext.Request.PathBase));

            options = optionsProvider.Invoke();
        }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(VersionedAssetAttributeName)]
        public string VersionType { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (output == null)
                throw new ArgumentNullException(nameof(output));

            if (!options.IsEnabled || string.IsNullOrWhiteSpace(VersionType))
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
            var attr = output.Attributes[attributeName];

            var path = attr.Value.ToString();
            var hash = lazyHashProvider.Value.GetContentHash(path);
           
            attr.Value = string.Format($"{options.UrlPrefix}/{hash}{path}");
        }
    }
}
