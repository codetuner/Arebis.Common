using System;
using System.Web.Mvc;

namespace Arebis.Web.Mvc
{
    // From: http://ivonna.biz/blog/2012/2/2/custom-aspnet-model-binders-series,-part-3-subclassing-your-models.aspx

    /// <summary>
    /// A ModelBinder that looks for a ModelType value containing the effective (fully qualified) typename
    /// of the effective model type. To be used when model type is a(n abstract) base type.
    /// Can substitute the default model binder.
    /// </summary>
    public class SubclassModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            // Take ModelType into account:
            if (bindingContext.ValueProvider.ContainsPrefix("ModelType"))
            {
                // Get the model type:
                var typeName = (string)bindingContext
                    .ValueProvider
                    .GetValue("ModelType")
                    .ConvertTo(typeof(string));
                var modelType = Type.GetType(typeName);

                // Verify validity:
                if (modelType == null)
                    throw new InvalidOperationException(String.Format("Failed to load model type \"{0}\".", typeName));
                if (!bindingContext.ModelType.IsAssignableFrom(modelType))
                    throw new InvalidOperationException(String.Format("Cannot bind instance of type \"{1}\" to type \"{0}\".", bindingContext.ModelType, modelType));

                // Tell the binder to use it:
                bindingContext.ModelMetadata =
                    ModelMetadataProviders
                    .Current
                    .GetMetadataForType(null, modelType);
            }

            // Proceed with default behavior:
            return base.BindModel(controllerContext, bindingContext);
        }
    }
}