// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Xml.Linq;
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Snapshot.Xml.Utility {
    public static class NofMetaModel {
        //The generated XML schema references the NOF metamodel schema. This is the
        //default location for this schema.

        public static string DefaultNofSchemaLocation = "nof.xsd";

        // The base of the namespace URI to use for application namespaces if none
        // explicitly supplied in the constructor.

        public static string DefaultUriBase = "http://www.nakedobjects.org/ns/app/";

        // Enumeration of nof:feature attribute representing a class

        public static string NofMetamodelFeatureClass = "class";

        // Enumeration of nof:feature attribute representing a collection (1:n
        // association)

        public static string NofMetamodelFeatureCollection = "collection";

        // Enumeration of nof:feature attribute representing a reference (1:1
        // association)

        public static string NofMetamodelFeatureReference = "reference";

        // Enumeration of nof:feature attribute representing a value field

        public static string NofMetamodelFeatureValue = "value";


        #region nof 

        // Namespace prefix for NofMetamodelNsUri.
        // The NamespaceManager will not allow any namespace to use this prefix.

        public static string NofMetamodelNsPrefix = "nof";

        // URI representing the namespace of NakedObject framework's metamodel.
        // The NamespaceManager will not allow any namespaces with this URI to be
        // added.

        public static string NofMetamodelNsUri = "http://www.nakedobjects.org/ns/0.1/metamodel";

        public static XNamespace nof = NofMetamodelNsUri;

        #endregion 

        public static void AddNamespace(XElement element) {
            Helper.RootElementFor(element).SetAttributeValue(XNamespace.Xmlns + NofMetamodelNsPrefix,  nof.NamespaceName);
        }

        // Creates an element in the NOF namespace, appends to parent, and adds NOF
        // namespace to the root element if required.

        public static XElement AppendElement(XElement parentElement, string localName) {
            var element = new XElement(nof + localName);
            parentElement.Add(element);
            return element;
        }

        // Appends an <code>nof:title</code> element with the supplied title
        // string to the provided element.

        public static void AppendNofTitle(XElement element, string titleStr) {
            XElement titleElement = AppendElement(element, "title");
            titleElement.Add(new XText(titleStr));
        }

        // Gets an attribute with the supplied name in the NOF namespace from the
        // supplied element

        public static string GetAttribute(XElement element, string attributeName) {
            XAttribute attribute = element.Attribute(nof + attributeName);
            return attribute == null ? string.Empty : attribute.ToString();
        }

        // Adds an <code>nof:annotation</code> attribute for the supplied class to
        // the supplied element.

        public static void SetAnnotationAttribute(XElement element, string annotation) {
            SetAttribute(element, "annotation", NofMetamodelNsPrefix + ":" + annotation);
        }

        // Sets an attribute of the supplied element with the attribute being in the
        // NOF namespace.

        public static void SetAttribute(XElement element, string attributeName, string attributeValue) {      
            element.SetAttributeValue(nof + attributeName, attributeValue);
        }

        // Adds <code>nof:feature=&quot;class&quot;</code> attribute and
        // <code>nof:oid=&quote;...&quot;</code> for the supplied element.

        public static void SetAttributesForClass(XElement element, string oid) {
            SetAttribute(element, "feature", NofMetamodelFeatureClass);
            SetAttribute(element, "oid", oid);
        }

        // Adds <code>nof:feature=&quot;reference&quot;</code> attribute and
        // <code>nof:type=&quote;...&quot;</code> for the supplied element.

        public static void SetAttributesForReference(XElement element, string prefix, string fullyQualifiedClassName) {
            SetAttribute(element, "feature", NofMetamodelFeatureReference);
            SetAttribute(element, "type", prefix + ":" + fullyQualifiedClassName);
        }

        // Adds <code>nof:feature=&quot;value&quot;</code> attribute and
        // <code>nof:datatype=&quote;...&quot;</code> for the supplied element.

        public static void SetAttributesForValue(XElement element, string datatypeName) {
            SetAttribute(element, "feature", NofMetamodelFeatureValue);
            SetAttribute(element, "datatype", NofMetamodelNsPrefix + ":" + datatypeName);
        }

        // Adds an <code>nof:isEmpty</code> attribute for the supplied class to
        // the supplied element.

        public static void SetIsEmptyAttribute(XElement element, bool isEmpty) {
            SetAttribute(element, "isEmpty", "" + isEmpty);
        }

        // Adds <code>nof:feature=&quot;collection&quot;</code> attribute, the
        // <code>nof:type=&quote;...&quot;</code> and the
        // <code>nof:size=&quote;...&quot;</code> for the supplied element.

        public static void SetNofCollection(XElement element,
                                            string prefix,
                                            string fullyQualifiedClassName,
                                            INakedObject collection) {
            SetAttribute(element, "feature", NofMetamodelFeatureCollection);
            SetAttribute(element, "type", prefix + ":" + fullyQualifiedClassName);

            // Size is no longer an attribute of collection, replaced by page
            //var collectionFacetFromSpec = collection.GetCollectionFacetFromSpec();
            //SetAttribute(element, "size", "" + collectionFacetFromSpec.Size(collection));
        }
    }
}