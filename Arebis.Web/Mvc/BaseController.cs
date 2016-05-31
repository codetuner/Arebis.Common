using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Arebis.Web.Mvc
{
    /// <summary>
    /// Base class for extendable ASP.NET MVC controllers.
    /// </summary>
    public abstract class BaseController : Controller
    {
    }

    /// <summary>
    /// Base class for extendable ASP.NET MVC controllers holding a model.
    /// </summary>
    /// <typeparam name="TModel">The model type.</typeparam>
    public abstract class BaseController<TModel> : BaseController
        where TModel : class
    {
        public const string ModelStateKey = "__modelState";

        /// <summary>
        /// The controllers model.
        /// </summary>
        public virtual TModel Model { get; set; }

        /// <summary>
        /// Initializes and returns a new model instance.
        /// </summary>
        protected abstract TModel InitializeModel();

        /// <summary>
        /// Called before the action method is invoked.
        /// </summary>
        protected override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // Deserialize the model state:
            if (filterContext.HttpContext.Request[ModelStateKey] != null)
            {
                MvcSerializer serializer = new MvcSerializer();
                Model = serializer.Deserialize(filterContext.HttpContext.Request[ModelStateKey]) as TModel;
                this.OnModelDeserialized();
            }
            else if (TempData[ModelStateKey] != null)
            {
                Model = TempData[ModelStateKey] as TModel;
            }

            // If none found, initialize a new one, else, try updating it from request:
            if (Model == null)
            {
                Model = this.InitializeModel();
            }
            else
            {
                this.TryUpdateModel(Model);
            }
        }

        /// <summary>
        /// Called after the action method is invoked.
        /// </summary>
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // Pass model to view (if no other model was passed):
            if (filterContext.Controller.ViewData.Model == null)
                filterContext.Controller.ViewData.Model = Model;

            // Proceed:
            base.OnActionExecuted(filterContext);
        }

        /// <summary>
        /// Called after deserializing the model from request modelstate.
        /// </summary>
        protected virtual void OnModelDeserialized()
        { 
        }

        /// <summary>
        /// Redirects to the specified action using the action name, controller name,
        /// and route dictionary passing the model through TempData.
        /// </summary>
        protected override RedirectToRouteResult RedirectToAction(string actionName, string controllerName, RouteValueDictionary routeValues)
        {
            // Pass model through TempData:
            TempData[ModelStateKey] = Model;

            // Proceed:
            return base.RedirectToAction(actionName, controllerName, routeValues);
        }
    }
}
