using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Arebis.Web.Mvc;
using Arebis.Contract;

namespace Arebis.Web.Mvc.DataTables
{
    public static class ControllerExtensions
    {
        public static ActionResult DataTableResult<T>(this ControllerBase controller, DataTableDescriptor dataTableDescriptor, PagedResponse<T> result)
            where T : class
        {
            if (!dataTableDescriptor.GetSetting<bool>("allowPageCaching", false))
                controller.DisablePageCaching();

            var data = result.Results.Select(r => dataTableDescriptor.Columns.Select(c => Convert.ToString(((Func<T, dynamic>)c.Rendering)(r))).ToList().ToArray()).ToArray();

            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new
                {
                    draw = result.Echo,
                    recordsTotal = result.TotalCount,
                    recordsFiltered = result.TotalFilteredCount,
                    data = data
                }
            };
        }
    }
}
