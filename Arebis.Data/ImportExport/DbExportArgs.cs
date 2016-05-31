using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Arebis.Data.ImportExport
{
    [DataContract(Namespace = "urn:arebis.be:data:importexport")]
    public class DbExportArgs
    {
        public DbExportArgs()
        {
            this.Queries = new List<DbExportArgsQuery>();
            this.TableFilters = new List<DbExportArgsQuery>();
            this.RelationsToExclude = new List<string>();
            this.RelationsToInclude = new List<string>();
            this.QueryArguments = new Dictionary<string, string>();
            this.OnImportBefore = new List<string>();
            this.OnImportAfter = new List<string>();
            this.ConnectionStrings = new List<ConnectionStringSettings>();
        }

        public static DbExportArgs FromXmlFile(string filename)
        {
            var result = new DbExportArgs();
            var xdoc = new XmlDocument();
            xdoc.Load(filename);
            if (xdoc.DocumentElement.Attributes["mode"] != null && !String.IsNullOrWhiteSpace(xdoc.DocumentElement.Attributes["mode"].Value))
                result.ExportMode = (DbExportMode)Enum.Parse(typeof(DbExportMode), xdoc.DocumentElement.Attributes["mode"].Value, true);
            if (xdoc.DocumentElement.Attributes["connectionName"] != null && !String.IsNullOrWhiteSpace(xdoc.DocumentElement.Attributes["connectionName"].Value))
                result.FromConnectionName = xdoc.DocumentElement.Attributes["connectionName"].Value;
            if (xdoc.DocumentElement.Attributes["outputfile"] != null && !String.IsNullOrWhiteSpace(xdoc.DocumentElement.Attributes["outputfile"].Value))
                result.FromConnectionName = xdoc.DocumentElement.Attributes["outputfile"].Value;
            foreach (XmlElement element in xdoc.DocumentElement.SelectNodes("/*/connectionStrings/add"))
            { 
                var item = new ConnectionStringSettings();
                item.Name = element.Attributes["name"].Value;
                item.ConnectionString = element.Attributes["connectionString"].Value;
                if (element.Attributes["providerName"] != null && !String.IsNullOrWhiteSpace(element.Attributes["providerName"].Value))
                    item.ProviderName = element.Attributes["providerName"].Value;
                result.ConnectionStrings.Add(item);
            }
            foreach (XmlElement element in xdoc.DocumentElement.SelectNodes("/*/queries/add"))
            {
                var item = new DbExportArgsQuery();
                item.TableName = element.Attributes["table"].Value;
                item.WhereCondition = (element.Attributes["where"] == null) ? "(1=1)" : element.Attributes["where"].Value;
                result.Queries.Add(item);
            }
            foreach (XmlElement element in xdoc.DocumentElement.SelectNodes("/*/filters/add"))
            {
                if (element.Attributes["where"] != null)
                {
                    var item = new DbExportArgsQuery();
                    item.TableName = element.Attributes["table"].Value;
                    item.WhereCondition = element.Attributes["where"].Value;
                    result.TableFilters.Add(item);
                }
            }
            foreach (XmlElement element in xdoc.DocumentElement.SelectNodes("/*/includes/add"))
            {
                result.RelationsToInclude.Add(element.Attributes["relation"].Value);
            }
            foreach (XmlElement element in xdoc.DocumentElement.SelectNodes("/*/excludes/add"))
            {
                result.RelationsToExclude.Add(element.Attributes["relation"].Value);
            }
            foreach (XmlElement element in xdoc.DocumentElement.SelectNodes("/*/import/before"))
            {
                result.OnImportBefore.Add(element.InnerText);
            }
            foreach (XmlElement element in xdoc.DocumentElement.SelectNodes("/*/import/after"))
            {
                result.OnImportAfter.Add(element.InnerText);
            }

            return result;
        }

        public static DbExportArgs FromDataContractFile(string filename)
        {
            var settings = new DataContractSerializerSettings();
            var serializer = new DataContractSerializer(typeof(DbExportArgs), settings);
            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                return (DbExportArgs)serializer.ReadObject(stream);
            }
        }

        public static DbExportArgs Merge(DbExportArgs args, DbExportArgs defaultArgs)
        {
            var result = new DbExportArgs();
            result.ConnectionStrings.AddRange(defaultArgs.ConnectionStrings);
            result.ConnectionStrings.AddRange(args.ConnectionStrings);
            result.ExportMode = args.ExportMode ?? defaultArgs.ExportMode;
            result.FromConnectionName = args.FromConnectionName ?? defaultArgs.FromConnectionName;
            result.Queries.AddRange(defaultArgs.Queries);
            result.Queries.AddRange(args.Queries);
            foreach (var key in defaultArgs.QueryArguments.Keys)
                result.QueryArguments[key] = defaultArgs.QueryArguments[key];
            foreach (var key in args.QueryArguments.Keys)
                result.QueryArguments[key] = args.QueryArguments[key];
            result.RelationsToExclude.AddRange(defaultArgs.RelationsToExclude);
            result.RelationsToExclude.AddRange(args.RelationsToExclude);
            result.RelationsToInclude.AddRange(defaultArgs.RelationsToInclude);
            result.RelationsToInclude.AddRange(args.RelationsToInclude);
            result.TableFilters.AddRange(defaultArgs.TableFilters);
            result.TableFilters.AddRange(args.TableFilters);
            result.OnImportBefore.AddRange(defaultArgs.OnImportBefore);
            result.OnImportBefore.AddRange(args.OnImportBefore);
            result.OnImportAfter.AddRange(defaultArgs.OnImportAfter);
            result.OnImportAfter.AddRange(args.OnImportAfter);
            result.ToFileName = args.ToFileName ?? defaultArgs.ToFileName;
            result.Repeat = args.Repeat ?? defaultArgs.Repeat;
            result.Transactional = args.Transactional || defaultArgs.Transactional;
            return result;
        }

        public ConnectionStringSettings FromConnection
        {
            get
            {
                return this.ConnectionStrings.FirstOrDefault(cs => String.Equals(cs.Name, this.FromConnectionName, StringComparison.OrdinalIgnoreCase));
            }
        }

        [DataMember(EmitDefaultValue = false)]
        public DbExportMode? ExportMode { get; set; }

        [DataMember]
        public string FromConnectionName { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<ConnectionStringSettings> ConnectionStrings { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string ToFileName { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<DbExportArgsQuery> Queries { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<DbExportArgsQuery> TableFilters { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<string> RelationsToInclude { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<string> RelationsToExclude { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<string> OnImportBefore { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<string> OnImportAfter { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Dictionary<string, string> QueryArguments { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int? Repeat { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool Transactional { get; set; }
    }

    [DataContract(Namespace = "urn:arebis.be:data:importexport")]
    public class DbExportArgsQuery
    {
        [DataMember(EmitDefaultValue = false)]
        public string TableName { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string WhereCondition { get; set; }
    }
}
