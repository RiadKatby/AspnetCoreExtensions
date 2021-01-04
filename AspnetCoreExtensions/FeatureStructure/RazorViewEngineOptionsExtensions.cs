using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspnetCoreExtensions.FeatureStructure
{
    static public class RazorViewEngineOptionsExtensions
    {
        /// <summary>
        /// I have used following reference to use feature based structure instead of Model-View-Controller structure
        /// https://docs.microsoft.com/en-us/archive/msdn-magazine/2016/september/asp-net-core-feature-slices-for-asp-net-core-mvc
        /// https://github.com/ardalis/OrganizingAspNetCore
        /// </summary>
        /// <param name="options"></param>
        public static void ConfigureFeatureFolders(this RazorViewEngineOptions options)
        {
            // {0} - Action Name
            // {1} - Controller Name
            // {2} - Area Name
            // {3} - Feature Name
            options.AreaViewLocationFormats.Clear();
            options.AreaViewLocationFormats.Add("/Areas/{2}/Features/{3}/{1}/{0}.cshtml");
            options.AreaViewLocationFormats.Add("/Areas/{2}/Features/{3}/{0}.cshtml");
            options.AreaViewLocationFormats.Add("/Areas/{2}/Features/Shared/{0}.cshtml");
            options.AreaViewLocationFormats.Add("/Areas/Shared/{0}.cshtml");

            // replace normal view location entirely
            options.ViewLocationFormats.Clear();
            options.ViewLocationFormats.Add("/Features/{3}/Views/{0}.cshtml");  // Features/AccountFeature/Views/Login.cshtml
            //options.ViewLocationFormats.Add("/Features/{3}/{1}/{0}.cshtml");    // Features/Account/Account/Login.cshtml
            //options.ViewLocationFormats.Add("/Features/{3}/{0}.cshtml");        // Features/Account/Login.cshtml
            options.ViewLocationFormats.Add("/Features/SharedFeature/Views/{0}.cshtml");

            options.PageViewLocationFormats.Clear();
            options.PageViewLocationFormats.Add("/Features/{3}/Views/{0}.cshtml");
            options.PageViewLocationFormats.Add("/Features/SharedFeature/Views/{0}.cshtml");


            options.ViewLocationExpanders.Add(new FeatureViewLocationExpander());
        }

        public static void ConfigurePageFeatureFolders(this RazorPagesOptions options)
        {
            options.RootDirectory = "/Features";
        }
    }
}
