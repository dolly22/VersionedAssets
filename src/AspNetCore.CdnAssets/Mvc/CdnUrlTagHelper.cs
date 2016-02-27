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

namespace AspNetCore.CdnAssets.Mvc
{
    [HtmlTargetElement("script", Attributes = CdnAssetAttributeName)]
    public class CdnUrlTagHelper : TagHelper
    {
        private const string CdnAssetAttributeName = "asp-cdn-asset";

        private static readonly Dictionary<string, string[]> elementAttributeLookup =
            new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
            {
                { "script", new[] { "src" } },
            };

        private Lazy<FileHashProvider> lazyHashProvider;

        public CdnUrlTagHelper(
            IHostingEnvironment hostingEnvironment,
            IMemoryCache cache,
            IHtmlEncoder htmlEncoder,
            IUrlHelper urlHelper)
        {
            this.lazyHashProvider = new Lazy<FileHashProvider>(() => new FileHashProvider(
                hostingEnvironment.WebRootFileProvider,
                cache,
                ViewContext.HttpContext.Request.PathBase));
        }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(CdnAssetAttributeName)]
        public string CndUrl { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (output == null)
                throw new ArgumentNullException(nameof(output));

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

            attr.Value = string.Format($"{CndUrl}/{hash}{path}");
        }
    }
}
