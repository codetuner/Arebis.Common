using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Linq.Expressions;
using Arebis.Linq;

namespace Arebis.Web.Mvc.DataTables
{
    public class DataTableDescriptor
    {
        private bool compacted = false;

        public DataTableDescriptor()
        {
            this.Extended = true;
            this.Paged = true;
            this.Columns = new List<DataTableColumn>();
            this.Actions = new List<DataTableAction>();
            this.Filters = new List<DataTableFilter>();
            this.NamedFilters = new List<DataTableNamedFilter>();
            this.DefaultSortedColumn = 0;
            this.DefaultSortedAscending = true;
        }

        /// <summary>
        /// Sets options. The same instance is returned to allow fluent syntax.
        /// </summary>
        /// <param name="Extended">Whether extended (full-featured) rendering of the dataTable is to be used.</param>
        /// <param name="Paged">Whether the dataTable should be paged.</param>
        /// <param name="HasGlobalFilter">Whether the dataTable should have a global search field.</param>
        /// <param name="CssClass">CSS class for the option.</param>
        public DataTableDescriptor WithOptions(bool Extended = true, bool Paged = true, bool HasGlobalFilter = false, string CssClass = null)
        {
            this.Extended = Extended;
            this.Paged = Paged;
            this.HasGlobalFilter = HasGlobalFilter;
            this.CssClass = CssClass;
            return this;
        }

        /// <summary>
        /// Defines the column to sort on by default, and the sort direction.
        /// </summary>
        public DataTableDescriptor WithDefaultSorting(int columnIndex, bool ascending = true)
        {
            this.DefaultSortedColumn = columnIndex;
            this.DefaultSortedAscending = ascending;
            return this;
        }

        /// <summary>
        /// Compacts and preprocesses the DataTable before rendering.
        /// </summary>
        public DataTableDescriptor Compact()
        {
            if (this.compacted) return this;

            foreach (var action in Actions)
            {
                if (action.ConditionColumn is String)
                { 
                    action.ConditionColumn = this.Columns.IndexOf(this.Columns.FirstOrDefault(c => c.Name == (string)action.ConditionColumn));
                }
                else if (action.ConditionColumn is DataTableColumn)
                {
                    action.ConditionColumn = this.Columns.IndexOf((DataTableColumn)action.ConditionColumn);
                }
            }

            this.compacted = true;

            return this;
        }

        public string CssClass { get; set; }

        public int DefaultSortedColumn { get; set; }

        public bool DefaultSortedAscending { get; set; }

        /// <summary>
        /// Whether extended (full-featured) rendering of the datatable is choosen.
        /// </summary>
        public bool Extended { get; set; }

        public bool Paged { get; set; }

        public bool HasGlobalFilter { get; set; }

        public bool HasActions
        {
            get
            {
                return this.Actions.Count > 0;
            }
        }

        public bool HasFilters
        {
            get
            {
                return this.Filters.Count > 0;
            }
        }

        public List<DataTableColumn> Columns { get; set; }

        public List<DataTableAction> Actions { get; set; }

        public List<DataTableFilter> Filters { get; set; }

        public List<DataTableNamedFilter> NamedFilters { get; set; }

        public Object Settings { get; set; }

        /// <summary>
        /// Retrieves a particular property of the Settings object.
        /// </summary>
        public T GetSetting<T>(string settingName, T defaultValue = default(T))
        {
            if (this.Settings == null) return defaultValue;

            var propertyInfo = this.Settings.GetType().GetProperty(settingName);
            if (propertyInfo == null) return defaultValue;

            return (T)propertyInfo.GetValue(this.Settings, null);
        }

        public virtual DataTableDescriptor AddColumn(DataTableColumn column)
        {
            this.Columns.Add(column);
            column.DataTableModel = this;
            return this;
        }

        public virtual DataTableDescriptor AddColumn<T>(Expression<Func<T, dynamic>> property, string Label = null, string CssClass = null, int Alignment = -1, bool Sortable = false, bool Visible = true, string Width = null, Expression<Func<T, dynamic>> AltRendering = null)
        {
            var name = property.GetPropertyPath();

            return this.AddColumn(new DataTableColumn() { Name = name, Label = Label, CssClass = CssClass, Alignment = Alignment, Sortable = Sortable, Visible = Visible, Width = Width, Rendering = (AltRendering ?? property).Compile() });
        }

        public virtual DataTableDescriptor AddAction(DataTableAction action)
        {
            this.Actions.Add(action);
            return this;
        }

        public virtual DataTableDescriptor AddFilter(DataTableFilter filter)
        {
            this.Filters.Add(filter);
            return this;
        }

        public DataTableDescriptor AddFilter<T>(Expression<Func<T, dynamic>> property, string Label = null, string CssClass = null, string FieldName = null, bool Visible = true, string[][] List = null, string ListUrl = null, bool IsLargeList = false, object AdditionalData = null)
        {
            var name = property.GetPropertyPath();

            this.AddFilter(new DataTableFilter()
            {
                Name = name,
                ValueType = typeof(T).GetProperty(name).PropertyType,
                Label = Label,
                CssClass = CssClass,
                FieldName = FieldName,
                Visible = Visible,
                List = List,
                ListUrl = ListUrl,
                IsLargeList = IsLargeList,
                Settings = AdditionalData
            });

            return this;
        }

        public DataTableDescriptor AddNamedFilter(string name)
        {
            return this.AddNamedFilter(name, name);
        }

        public DataTableDescriptor AddNamedFilter(string filtername, string caption)
        {
            this.NamedFilters.Add(new DataTableNamedFilter() {
                Caption = caption,
                FilterName = filtername
            });

            return this;
        }

        public DataTableModel MakeModel(string[][] data)
        { 
            return new DataTableModel(this, data);
        }

        public DataTableModel MakeModel(System.Uri dataUri)
        { 
            return new DataTableModel(this, dataUri);
        }

        public DataTableModel MakeModel(string actionName, string controllerName = null, object routeValues = null)
        { 
            return new DataTableModel(this, actionName, controllerName, routeValues);
        }
    }

    public class DataTableColumn
    {
        internal DataTableDescriptor DataTableModel;

        public DataTableColumn()
        {
            this.Alignment = -1;
            this.Visible = true;
        }

        /// <summary>
        /// Column name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Column label or resource key.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Column width.
        /// </summary>
        public string Width { get; set; }

        /// <summary>
        /// Alignment: -1=left, 0=center, 1=right.
        /// </summary>
        public int Alignment { get; set; }

        /// <summary>
        /// Classes to set on all cells of this column.
        /// </summary>
        public string CssClass { get; set; }

        /// <summary>
        /// Sortable.
        /// </summary>
        public bool Sortable { get; set; }

        /// <summary>
        /// If false, asc en desc sortable, if true, asc sortable only.
        /// </summary>
        public bool AscendingSortableOnly { get; set; }

        /// <summary>
        /// The column index to perform sort on when this column is sortable.
        /// Can be used for sorting on hidden columns via a visible column.
        /// </summary>
        public int? SortColumnRedirection { get; set; }

        /// <summary>
        /// Whether this column is visible or not.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Searchable.
        /// </summary>
        public bool Searchable { get; set; }

        /// <summary>
        /// Editable.
        /// </summary>
        public bool Editable { get; set; }

        /// <summary>
        /// Type to use to identify the EditorTemplate to use.
        /// </summary>
        public Type EditorType { get; set; }

        /// <summary>
        /// Name of the EditorTemplate to use. Null for default.
        /// </summary>
        public string EditorTemplate { get; set; }

        /// <summary>
        /// Func&lt;TRow, dynamic&gt; to use to render the column value for the row object.
        /// </summary>
        public object Rendering { get; set; }

        /// <summary>
        /// Type to use to identify the EditorTemplate to use for the search field.
        /// </summary>
        public Type SearchFieldType { get; set; }

        /// <summary>
        /// Name of the EditorTemplate to use for the search field. Null for default.
        /// </summary>
        public string SearchFieldTemplate { get; set; }

        /// <summary>
        /// Url to a JSON call returning allowed values (i.e. for editing or searching).
        /// </summary>
        public string ValueListUrl { get; set; }

        /// <summary>
        /// Object with additional settings.
        /// </summary>
        public Object Settings { get; set; }

        /// <summary>
        /// The index of the column.
        /// </summary>
        public int Index {
            get
            {
                if (this.DataTableModel == null)
                    return -1;
                else
                    return this.DataTableModel.Columns.IndexOf(this);
            }
        }

        /// <summary>
        /// Retrieves a particular property of the Settings object.
        /// </summary>
        public T GetSetting<T>(string settingName, T defaultValue = default(T))
        {
            if (this.Settings == null) return defaultValue;

            var propertyInfo = this.Settings.GetType().GetProperty(settingName);
            if (propertyInfo == null) return defaultValue;

            return (T)propertyInfo.GetValue(this.Settings, null);
        }
    }

    public class DataTableAction
    {
        public const int RenderTextOnly = 0;
        public const int RenderIconAndText = 1;
        public const int RenderIconOnly = 2;

        /// <summary>
        /// Column name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Column label or resource key.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// A (relative or absolute) URL to link to.
        /// If null, performs a postback.
        /// If starts with "function:" indicates a JavaScript function to call with args (rowid, tableid).
        /// </summary>
        public string Href { get; set; }

        /// <summary>
        /// Classes to set on all cells of this column.
        /// </summary>
        public string CssClass { get; set; }

        /// <summary>
        /// Whether this action can act on multiple items at once.
        /// </summary>
        public bool Multiple { get; set; }

        /// <summary>
        /// Whether this is the default action.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Icon image file.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Rendering mode of this action.
        /// Use constants defined on the DataTableAction class.
        /// </summary>
        public int RenderingMode { get; set; }

        /// <summary>
        /// If set, message to ask for confirmation. Use {0}...{n} to render column values.
        /// For Multiple actions, {count} will be replaced by the number of selected rows.
        /// </summary>
        public string ConfirmationMessage { get; set; }

        /// <summary>
        /// Whether this action is only shown/enabled when conditions are met.
        /// </summary>
        public bool Conditional
        {
            get
            {
                return (ConditionColumn != null) && (ConditionValue != null);
            }
        }

        /// <summary>
        /// Index or name of the column containing the value to test the ConditionValue with.
        /// </summary>
        public object ConditionColumn { get; set; }

        /// <summary>
        /// If this value appears as substring in the value of the ConditionColumn, this action is enabled/shown.
        /// </summary>
        public string ConditionValue { get; set; }

        /// <summary>
        /// Object with additional settings.
        /// </summary>
        public Object Settings { get; set; }

        /// <summary>
        /// Retrieves a particular property of the Settings object.
        /// </summary>
        public T GetSetting<T>(string settingName, T defaultValue = default(T))
        {
            if (this.Settings == null) return defaultValue;

            var propertyInfo = this.Settings.GetType().GetProperty(settingName);
            if (propertyInfo == null) return defaultValue;

            return (T)propertyInfo.GetValue(this.Settings, null);
        }

        /// <summary>
        /// Whether this action has an icon.
        /// </summary>
        public bool HasIcon
        {
            get
            {
                return (this.Icon != null);
            }
        }

        /// <summary>
        /// Whether a confirmation message is set for this action.
        /// </summary>
        public bool ConfirmationRequired
        {
            get
            {
                return (this.ConfirmationMessage != null);
            }
        }

        /// <summary>
        /// Gets the full icon path, combined with AppSetting 'DataTableIconsPath' if not starting
        /// with '~' or '/'.
        /// </summary>
        public string GetFullIconPath()
        {
            if (HasIcon)
            {
                if (Icon.StartsWith("~") || Icon.StartsWith("/"))
                    return Icon;
                else
                    return (ConfigurationManager.AppSettings["DataTableIconsPath"] + "/" + Icon);
            }
            else
            {
                return null;
            }
        }
    }

    public class DataTableFilter
    {
        public DataTableFilter()
        {
            this.Visible = true;
            this.ValueType = typeof(String);
        }

        /// <summary>
        /// Column name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Type of values of this filter.
        /// </summary>
        public Type ValueType { get; set; }

        /// <summary>
        /// Column label or resource key.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Name of the HTML field.
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Classes to set on all cells of this column.
        /// </summary>
        public string CssClass { get; set; }

        /// <summary>
        /// Whether the filter is visible.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// List of possible values (value and displayvalue per row).
        /// </summary>
        public string[][] List { get; set; }

        /// <summary>
        /// Url of list of possible values.
        /// </summary>
        public string ListUrl { get; set; }

        /// <summary>
        /// Whether the list of possible values should be paged and/or autocompleted.
        /// </summary>
        public bool IsLargeList { get; set; }

        /// <summary>
        /// Object with additional settings.
        /// </summary>
        public Object Settings { get; set; }

        /// <summary>
        /// Retrieves a particular property of the Settings object.
        /// </summary>
        public T GetSetting<T>(string settingName, T defaultValue = default(T))
        {
            if (this.Settings == null) return defaultValue;

            var propertyInfo = this.Settings.GetType().GetProperty(settingName);
            if (propertyInfo == null) return defaultValue;

            return (T)propertyInfo.GetValue(this.Settings, null);
        }
    }

    public class DataTableNamedFilter
    {
        public string Caption { get; set; }

        public string FilterName { get; set; }
    }
}