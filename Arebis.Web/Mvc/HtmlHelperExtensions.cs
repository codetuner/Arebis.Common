using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Arebis.Web.Mvc
{
    /// <summary>
    /// General extension methods on HtmlHelper.
    /// </summary>
    public static class HtmlHelperExtensions
    {
        #region Model State

        /// <summary>
        /// Renders the model state in the current form.
        /// </summary>
        public static MvcHtmlString ModelState(this HtmlHelper html)
        {
            string serializedModel = new MvcSerializer().Serialize(html.ViewData.Model);
            return html.Hidden(BaseController<object>.ModelStateKey, serializedModel);
        }

        #endregion
    }
}
