using System;
using System.Linq;
using System.Web.Mvc;

namespace Arebis.Web.Mvc
{
    /// <summary>
    /// A DefaultModelBinder that can update complex model graphs.
    /// </summary>
    public class DefaultGraphModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            // Bind indexed collections without replacing member instances:
            if ((bindingContext.Model.IsCollection())
                && (bindingContext.Model.CollectionGetCount() > 0)
                && (!controllerContext.RequestContext.HttpContext.Request.Form.AllKeys.Contains(bindingContext.ModelName)))
                return this.BindCollection(controllerContext, bindingContext);

            // Bind flag-enumerations:
            if (bindingContext.ValueProvider.ContainsPrefix(bindingContext.ModelName)
                && bindingContext.ModelType.UnWrapNullable().IsBitFlagsEnum())
                return BindBitFlagsEnum(controllerContext, bindingContext);

            // Default binding:
            return base.BindModel(controllerContext, bindingContext);
        }

        private object BindBitFlagsEnum(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var enumType = bindingContext.ModelType.UnWrapNullable();

            // Get all defined enumeration values
            var values = ((string[])bindingContext.ValueProvider.GetValue(bindingContext.ModelName)
                .ConvertTo(typeof(string[])))
                .Where(v => Enum.IsDefined(enumType, v));

            // ASP.NET handles 1 value correctly
            if (values.Count() <= 1) return base.BindModel(controllerContext, bindingContext);

            // Aggregate the enum values
            var resultingBitFlags = values.Aggregate<string, long>(0, (current, value) => current | Convert.ToInt64(Enum.Parse(enumType, value)));

            return Enum.Parse(enumType, resultingBitFlags.ToString());
        }

        private object BindCollection(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            object collection = bindingContext.Model;
            Type collectionMemberType = typeof(Object);
            if (collection.GetType().IsGenericType)
                collectionMemberType =
                    collection.GetType().GetGenericArguments()[0];
            int count = collection.CollectionGetCount();
            for (int index = 0; index < count; index++)
            {
                // Create a BindingContext for the collection member:
                ModelBindingContext innerContext = new ModelBindingContext();
                object member = collection.CollectionGetItem(index);
                Type memberType =
                    (member == null) ? collectionMemberType : member.GetType();
                innerContext.ModelMetadata =
                    ModelMetadataProviders.Current.GetMetadataForType(
                        delegate() { return member; },
                        memberType);
                innerContext.ModelName =
                    String.Format("{0}[{1}]", bindingContext.ModelName, index);
                innerContext.ModelState = bindingContext.ModelState;
                innerContext.PropertyFilter = bindingContext.PropertyFilter;
                innerContext.ValueProvider = bindingContext.ValueProvider;

                // Bind the collection member:
                IModelBinder binder = Binders.GetBinder(memberType);
                object boundMember =
                    binder.BindModel(controllerContext, innerContext) ?? member;
                collection.CollectionSetItem(index, boundMember);
            }

            // Return the collection:
            return collection;
        }
    }

    internal static class DefaultModelGraphBinderCollectionExtensions
    {
        public static bool IsNullable(this Type type)
        {
            return type.IsGenericType &&
                   type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static bool IsBitFlagsEnum(this Type type)
        {
            return type.IsEnum &&
                   type.IsDefined(typeof(FlagsAttribute), false);
        }

        public static Type UnWrapNullable(this Type type)
        {
            return !type.IsNullable() ? type : Nullable.GetUnderlyingType(type);
        }


        public static bool IsCollection(this object obj)
        {
            return (obj != null)
                && (obj.GetType() != typeof(String))
                && (typeof(System.Collections.IEnumerable).IsInstanceOfType(obj));
        }

        public static int CollectionGetCount(this object collection)
        {
            if (collection.GetType().IsArray)
                return ((Array)collection).GetLength(0);
            else
                return (int)collection.GetType().GetProperty("Count")
                    .GetValue(collection, null);
        }

        public static object CollectionGetItem(this object collection, int index)
        {
            if (collection.GetType().IsArray)
                return ((Array)collection).GetValue(index);
            else
                return collection.GetType().GetProperty("Item")
                    .GetValue(collection, new object[] { index });
        }

        public static void CollectionSetItem(this object collection, int index, object value)
        {
            if (collection.GetType().IsArray)
                ((Array)collection).SetValue(value, index);
            else
                collection.GetType().GetProperty("Item")
                    .SetValue(collection, value, new object[] { index });
        }
    }
}
