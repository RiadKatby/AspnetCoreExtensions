﻿using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspnetCoreExtensions.FeatureStructure
{
    public class FeatureViewLocationExpander : IViewLocationExpander
    {
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (viewLocations == null)
                throw new ArgumentNullException(nameof(viewLocations));

            var properties = context.ActionContext.ActionDescriptor.Properties;
            var featureName = properties["feature"] as string;

            //var controllerActionDescriptor = context.ActionContext.ActionDescriptor as ControllerActionDescriptor;
            //if (controllerActionDescriptor == null)
            //{
                //foreach (var item in viewLocations)
                  //  yield return item;
                //yield break;
            //}
            //throw new NullReferenceException("ControllerActionDescriptor cannot be null.");

            //string featureName = controllerActionDescriptor.Properties["feature"] as string;
            foreach (var location in viewLocations)
                yield return location.Replace("{3}", featureName);
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }
    }
}
