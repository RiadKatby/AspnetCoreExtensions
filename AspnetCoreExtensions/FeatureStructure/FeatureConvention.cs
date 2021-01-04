using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AspnetCoreExtensions.FeatureStructure
{
    public class FeatureConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var featureName = GetFeatureName(controller.ControllerType);
            controller.Properties.Add("feature", featureName);
        }

        private string GetFeatureName(TypeInfo controllerType)
        {
            string[] tokens = controllerType.FullName.Split('.');
            if (!tokens.Any(t => t == "Features"))
                return "";

            string featureName = tokens
                .SkipWhile(t => !t.Equals("features", StringComparison.CurrentCultureIgnoreCase))
                .Skip(1)
                .Take(1)
                .FirstOrDefault();

            return featureName;
        }
    }

    public class PageFeatureConvention : IPageApplicationModelConvention
    {
        public void Apply(PageApplicationModel model)
        {
            var featureName = GetFeatureName(model.ModelType);
            model.Properties.Add("feature", featureName);
        }

        private string GetFeatureName(TypeInfo controllerType)
        {
            string[] tokens = controllerType.FullName.Split('.');
            if (!tokens.Any(t => t == "Features"))
                return "";

            string featureName = tokens
                .SkipWhile(t => !t.Equals("features", StringComparison.CurrentCultureIgnoreCase))
                .Skip(1)
                .Take(1)
                .FirstOrDefault();

            return featureName;
        }
    }
}
