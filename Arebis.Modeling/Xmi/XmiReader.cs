using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Linq.Expressions;

namespace Arebis.Modeling.Xmi
{
    public class XmiReader : IDisposable
    {
        private Dictionary<string, ModelBase>  elementIndex = new Dictionary<string, ModelBase>();
        private List<Action<Dictionary<string, ModelBase>>> resolvers = new List<Action<Dictionary<string,ModelBase>>>();

        public XmiReader(string filename)
            : this(new FileStream(filename, FileMode.Open))
        { }

        public XmiReader(Stream stream)
        {
            this.BaseStream = stream;
        }

        public Stream BaseStream { get; private set; }

        public Model ReadModel()
        {

            var doc = new XmlDocument();
            doc.Load(this.BaseStream);

            // Add all namespaces of the document:
            var nsmgr = new XmlNamespaceManager(doc.NameTable);
            foreach (XmlAttribute item in doc.DocumentElement.Attributes)
                if (item.Name.StartsWith("xmlns:"))
                    nsmgr.AddNamespace(item.LocalName, item.Value);

            // Read model:
            var result = this.ReadModelElement((XmlElement)doc.DocumentElement.SelectSingleNode("//XMI.content/UML:Model", nsmgr), nsmgr, elementIndex);

            // Apply resolvers:
            foreach (var resolver in this.resolvers)
                resolver(elementIndex);

            // Read tagged values:
            foreach(XmlElement valueElement in doc.DocumentElement.SelectNodes("//XMI.content/UML:TaggedValue", nsmgr))
            {
                var item = GetById(elementIndex, valueElement.Attributes["modelElement"].Value);
                item.TaggedValues[valueElement.Attributes["tag"].Value] = valueElement.Attributes["value"].ValueOr(null);
            }

            return result;
        }

        private ModelBase ReadElement(XmlElement xmlElement, XmlNamespaceManager nsmgr, Dictionary<string, ModelBase> elementIndex)
        {
            if (xmlElement.LocalName == "Model")
                return this.ReadModelElement(xmlElement, nsmgr, elementIndex);
            else if (xmlElement.LocalName == "Package")
                return this.ReadPackageElement(xmlElement, nsmgr, elementIndex);
            else if (xmlElement.LocalName == "Class")
                return this.ReadClassElement(xmlElement, nsmgr, elementIndex);
            else if (xmlElement.LocalName == "Attribute")
                return this.ReadAttributeElement(xmlElement, nsmgr, elementIndex);
            else if (xmlElement.LocalName == "Association")
                return this.ReadAssociationElement(xmlElement, nsmgr, elementIndex);
            else if (xmlElement.LocalName == "AssociationEnd")
                return this.ReadAssociationEndElement(xmlElement, nsmgr, elementIndex);
            else if (xmlElement.LocalName == "DataType")
                return this.ReadDataTypeElement(xmlElement, nsmgr, elementIndex);
            else if (xmlElement.LocalName == "Dependency")
                return this.ReadDependencyElement(xmlElement, nsmgr, elementIndex);
            else if (xmlElement.LocalName == "Stereotype")
                return this.ReadStereotypeElement(xmlElement, nsmgr, elementIndex);
            else
                return null;
        }

        internal virtual Model ReadModelElement(XmlElement xmlElement, XmlNamespaceManager nsmgr, Dictionary<string, ModelBase> elementIndex)
        {
            var result = new Model();
            SetById(elementIndex, xmlElement, result);
            result.Name = xmlElement.Attributes["name"].ValueOr(null);
            if (String.IsNullOrWhiteSpace(result.Name)) result.Name = null;
            result.Visibility = xmlElement.Attributes["visibility"].ValueOr("public");

            foreach (XmlElement subnode in xmlElement.SelectSingleNode("UML:Namespace.ownedElement", nsmgr).ChildNodes)
            {
                var element = ReadElement(subnode, nsmgr, elementIndex) as ModelElement;
                if (element != null)
                {
                    element.Owner = result;
                }
            }

            return result;
        }

        private ModelElement ReadPackageElement(XmlElement xmlElement, XmlNamespaceManager nsmgr, Dictionary<string, ModelBase> elementIndex)
        {
            var result = new Package();
            SetById(elementIndex, xmlElement, result);
            result.Name = xmlElement.Attributes["name"].ValueOr(null);
            if (String.IsNullOrWhiteSpace(result.Name)) result.Name = null;
            result.Visibility = xmlElement.Attributes["visibility"].ValueOr("public");

            foreach (XmlElement subnode in xmlElement.SelectSingleNode("UML:Namespace.ownedElement", nsmgr).ChildNodesOrNone())
            {
                var element = (IOwned<Package>)ReadElement(subnode, nsmgr, elementIndex);
                if (element != null)
                {
                    element.Owner = result;
                }
            }

            return result;
        }

        private Class ReadClassElement(XmlElement xmlElement, XmlNamespaceManager nsmgr, Dictionary<string, ModelBase> elementIndex)
        {
            var result = new Class();
            SetById(elementIndex, xmlElement, result);
            result.Name = xmlElement.Attributes["name"].ValueOr(null);
            if (String.IsNullOrWhiteSpace(result.Name)) result.Name = null;
            result.Visibility = xmlElement.Attributes["visibility"].ValueOr("public");
            result.IsAbstract = Convert.ToBoolean(xmlElement.Attributes["isAbstract"].ValueOr("false"));

            foreach (XmlElement subnode in xmlElement.SelectSingleNode("UML:Classifier.feature", nsmgr).ChildNodesOrNone())
            {
                var element = (IOwned<Class>)ReadElement(subnode, nsmgr, elementIndex);
                if (element != null)
                {
                    element.Owner = result;
                }
            }

            return result;
        }

        private Attribute ReadAttributeElement(XmlElement xmlElement, XmlNamespaceManager nsmgr, Dictionary<string, ModelBase> elementIndex)
        {
            var result = new Attribute();
            SetById(elementIndex, xmlElement, result);
            result.Name = xmlElement.Attributes["name"].ValueOr(null);
            if (String.IsNullOrWhiteSpace(result.Name)) result.Name = null;
            result.Visibility = xmlElement.Attributes["visibility"].ValueOr("public");
            resolvers.Add((ix) => result.Type = GetById(ix, xmlElement.Attributes["type"].ValueOr(null)) as IModelType);

            var mul = xmlElement.SelectSingleNode("UML:StructuralFeature.multiplicity/UML:Multiplicity/UML:Multiplicity.range/UML:MultiplicityRange", nsmgr);
            if (mul != null)
                result.Multiplicity = mul.Attributes["lower"].ValueOr("0") + ".." + mul.Attributes["upper"].ValueOr("-1").Replace("-1", "*");

            var iv = xmlElement.SelectSingleNode("UML:Attribute.initialValue/UML:Expression/@body", nsmgr);
            if (iv != null)
                result.InitialValue = iv.Value;

            return result;
        }

        private Association ReadAssociationElement(XmlElement xmlElement, XmlNamespaceManager nsmgr, Dictionary<string, ModelBase> elementIndex)
        {
            var result = new Association();
            SetById(elementIndex, xmlElement, result);
            result.Name = xmlElement.Attributes["name"].ValueOr(null);
            if (String.IsNullOrWhiteSpace(result.Name)) result.Name = null;
            result.Visibility = xmlElement.Attributes["visibility"].ValueOr("public");

            foreach (XmlElement subnode in xmlElement.SelectSingleNode("UML:Association.connection", nsmgr).ChildNodesOrNone())
            {
                var element = (IOwned<Association>)ReadElement(subnode, nsmgr, elementIndex);
                if (element != null)
                {
                    element.Owner = result;
                }
            }

            return result;
        }

        private AssociationEnd ReadAssociationEndElement(XmlElement xmlElement, XmlNamespaceManager nsmgr, Dictionary<string, ModelBase> elementIndex)
        {
            var result = new AssociationEnd();
            SetById(elementIndex, xmlElement, result);
            result.Name = xmlElement.Attributes["name"].ValueOr(null);
            if (String.IsNullOrWhiteSpace(result.Name)) result.Name = null;
            result.Visibility = xmlElement.Attributes["visibility"].ValueOr("public");
            result.Aggregation = xmlElement.Attributes["aggregation"].ValueOr("none");
            resolvers.Add((ix) => result.Type = GetById(ix, xmlElement.Attributes["type"].ValueOr(null)) as IModelType);

            var mul = xmlElement.SelectSingleNode("UML:AssociationEnd.multiplicity/UML:Multiplicity/UML:Multiplicity.range/UML:MultiplicityRange", nsmgr);
            if (mul != null)
                result.Multiplicity = mul.Attributes["lower"].ValueOr("0") + ".." + mul.Attributes["upper"].ValueOr("-1").Replace("-1", "*");

            return result;
        }

        private DataType ReadDataTypeElement(XmlElement xmlElement, XmlNamespaceManager nsmgr, Dictionary<string, ModelBase> elementIndex)
        {
            var result = new DataType();
            SetById(elementIndex, xmlElement, result);
            result.Name = xmlElement.Attributes["name"].ValueOr(null);
            if (String.IsNullOrWhiteSpace(result.Name)) result.Name = null;
            result.Visibility = xmlElement.Attributes["visibility"].ValueOr("public");
            result.IsAbstract = Convert.ToBoolean(xmlElement.Attributes["isAbstract"].ValueOr("false"));

            return result;
        }

        private Relationship ReadDependencyElement(XmlElement xmlElement, XmlNamespaceManager nsmgr, Dictionary<string, ModelBase> elementIndex)
        {
            var result = new Relationship();
            result.Type = "Dependency";
            result.Name = xmlElement.Attributes["name"].ValueOr(null);
            if (String.IsNullOrWhiteSpace(result.Name)) result.Name = null;
            result.Visibility = xmlElement.Attributes["visibility"].ValueOr("public");
            SetById(elementIndex, xmlElement, result);

            foreach (var item in xmlElement.Attributes["client"].ValueOr("").Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries))
            {
                resolvers.Add((ix) => result.Sources.Add(GetById(ix, item)));
            }

            foreach (var item in xmlElement.Attributes["supplier"].ValueOr("").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                resolvers.Add((ix) => result.Targets.Add(GetById(ix, item)));
            }

            return result;
        }

        private ModelBase ReadStereotypeElement(XmlElement xmlElement, XmlNamespaceManager nsmgr, Dictionary<string, ModelBase> elementIndex)
        {
            string result = xmlElement.Attributes["name"].Value;
            string[] ids = xmlElement.Attributes["extendedElement"].Value.Split(' ');
            foreach (var item in ids)
            {
                var id = item;
                resolvers.Add((ix) => GetById(ix, id).Stereotypes.Add(result));
            }

            return null;
        }
        private void SetById(Dictionary<string, ModelBase> index, XmlNode xmiElement, ModelBase modelElement)
        {
            var id = xmiElement.Attributes["xmi.id"].ValueOr(null);
            if (id != null)
                index[id] = modelElement;
        }

        private ModelBase GetById(Dictionary<string, ModelBase> index, string id)
        {
            ModelBase result;
            if (id == null)
                return null;
            else if (index.TryGetValue(id, out result))
                return result;
            else
                return null;
        }

        public void Dispose()
        {
            if (this.BaseStream != null)
            {
                this.BaseStream.Dispose();
                this.BaseStream = null;
            }
        }
    }

    internal static class HelperExtensions
    {
        public static string ValueOr(this XmlAttribute attribute, string defaultValue)
        {
            if (attribute == null)
                return defaultValue;
            else
                return attribute.Value;
        }

        public static IEnumerable ChildNodesOrNone(this XmlNode node)
        {
            if (node == null)
                yield break;
            else
                foreach (var subnode in node.ChildNodes)
                    yield return subnode;
        }
    }
}
