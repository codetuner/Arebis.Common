using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Web.Mvc.DataTables
{
    public class DataTableModel
    {
        public DataTableModel(DataTableDescriptor descriptor, string[][] data)
            : this(descriptor)
        {
            this.Data = data;
        }

        public DataTableModel(DataTableDescriptor descriptor, System.Uri dataUri)
            : this(descriptor)
        {
            this.DataUri = dataUri;
        }

        public DataTableModel(DataTableDescriptor descriptor, string actionName, string controllerName = null, object routeValues = null)
            : this(descriptor)
        {
            this.DataAction = actionName;
            this.DataController = controllerName;
            this.DataRouteValues = routeValues;
        }

        private DataTableModel(DataTableDescriptor descriptor)
        {
            this.Id = "DataTable" + Guid.NewGuid().ToString().Replace("-", "");
            this.Descriptor = descriptor;
        }

        public DataTableDescriptor Descriptor { get; set; }

        public string Id { get; set; }

        public string[][] Data { get; set; }

        public Uri DataUri { get; set; }

        public string DataAction { get; set; }

        public string DataController { get; set; }

        public object DataRouteValues { get; set; }
    }
}
