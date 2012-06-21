// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using log4net;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Actcoll.Typeof;
using NakedObjects.Architecture.Facets.Collections.Modify;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Snapshot.Xml.Utility {
    public class XmlSnapshot {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (XmlSnapshot));

        private readonly Place rootPlace;

        private bool topLevelElementWritten;

        //  Start a snapshot at the root object, using own namespace manager.

        public XmlSnapshot(INakedObject rootObject) : this(rootObject, new XmlSchema()) {}

        // Start a snapshot at the root object, using supplied namespace manager.

        public XmlSnapshot(INakedObject rootObject, XmlSchema schema) {
            LOG.Debug(".ctor(" + Log("rootObj", rootObject) + AndLog("schema", schema) + AndLog("addOids", "" + true) + ")");

            Schema = schema;

            try {
                XmlDocument = new XDocument();
                XsdDocument = new XDocument();

                XsdElement = XsMetaModel.CreateXsSchemaElement(XsdDocument);

                rootPlace = AppendXml(rootObject);
            }
            catch (ArgumentException e) {
                LOG.Error("unable to build snapshot", e);
                throw new NakedObjectSystemException(e);
            }
        }

        //  The name of the <code>xsi:schemaLocation</code> in the XML document.
        //  
        //  Taken from the <code>fullyQualifiedClassName</code> (which also is used
        //  as the basis for the <code>targetNamespace</code>.
        //  
        //  Populated in AppendXml(NakedObject).
        public string SchemaLocationFileName { get; private set; }
        public XDocument XmlDocument { get; private set; }

        //  The root element of GetXmlDocument(). Returns <code>null</code>
        //  until the snapshot has actually been built.

        public XElement XmlElement { get; private set; }

        public XDocument XsdDocument { get; private set; }

        //  The root element of GetXsdDocument(). Returns <code>null</code>
        //  until the snapshot has actually been built.

        public XElement XsdElement { get; private set; }
        public XmlSchema Schema { get; private set; }

        // Start a snapshot at the root object, using own namespace manager.

        private static string AndLog(string label, INakedObject nakedObject) {
            return ", " + Log(label, nakedObject);
        }

        private static string AndLog(string label, Object nakedObject) {
            return ", " + Log(label, nakedObject);
        }

        // Creates an XElement representing this object, and appends it as the root
        // element of the Document.
        // 
        // The Document must not yet have a root element Additionally, the supplied
        // schemaManager must be populated with any application-level namespaces
        // referenced in the document that the parentElement resides within.
        // (Normally this is achieved simply by using AppendXml passing in a new
        // schemaManager - see ToXml() or XmlSnapshot).

        private Place AppendXml(INakedObject nakedObject) {
            LOG.Debug("appendXml(" + Log("obj", nakedObject) + "')");

            string fullyQualifiedClassName = nakedObject.Specification.FullName;

            Schema.SetUri(fullyQualifiedClassName); // derive URI from fully qualified name

            Place place = ObjectToElement(nakedObject);

            XElement element = place.XmlElement;
            var xsElementElement = element.Annotation<XElement>();

            LOG.Debug("appendXml(NO): add as element to XML doc");
            XmlDocument.Add(element);

            LOG.Debug("appendXml(NO): add as xs:element to xs:schema of the XSD document");
            XsdElement.Add(xsElementElement);

            LOG.Debug("appendXml(NO): set target name in XSD, derived from FQCN of obj");
            Schema.SetTargetNamespace(XsdDocument, fullyQualifiedClassName);

            LOG.Debug("appendXml(NO): set schema location file name to XSD, derived from FQCN of obj");
            string schemaLocationFileName = fullyQualifiedClassName + ".xsd";
            Schema.AssignSchema(XmlDocument, fullyQualifiedClassName, schemaLocationFileName);

            LOG.Debug("appendXml(NO): copy into snapshot obj");
            XmlElement = element;
            SchemaLocationFileName = schemaLocationFileName;

            return place;
        }

        // Creates an XElement representing this object, and appends it to the
        // supplied parentElement, provided that an element for the object is not
        // already appended.
        // 
        // The method uses the OID to determine if an object's element is already
        // present. If the object is not yet persistent, then the hashCode is used
        // instead.
        // 
        // The parentElement must have an owner document, and should define the nof
        // namespace. Additionally, the supplied schemaManager must be populated
        // with any application-level namespaces referenced in the document that the
        // parentElement resides within. (Normally this is achieved simply by using
        // appendXml passing in a rootElement and a new schemaManager - see
        // ToXml() or XmlSnapshot).

        private XElement AppendXml(Place parentPlace, INakedObject childObject) {
            LOG.Debug("appendXml(" + Log("parentPlace", parentPlace) + AndLog("childObj", childObject) + ")");

            XElement parentElement = parentPlace.XmlElement;
            var parentXsElement = parentElement.Annotation<XElement>();

            if (parentElement.Document != XmlDocument) {
                throw new ArgumentException("parent XML XElement must have snapshot's XML document as its owner");
            }

            LOG.Debug("appendXml(Pl, NO): invoking objectToElement() for " + Log("childObj", childObject));
            Place childPlace = ObjectToElement(childObject);
            XElement childElement = childPlace.XmlElement;
            var childXsElement = childElement.Annotation<XElement>();

            LOG.Debug("appendXml(Pl, NO): invoking mergeTree of parent with child");
            childElement = MergeTree(parentElement, childElement);

            LOG.Debug("appendXml(Pl, NO): adding XS XElement to schema if required");
            Schema.AddXsElementIfNotPresent(parentXsElement, childXsElement);

            return childElement;
        }

        private bool AppendXmlThenIncludeRemaining(Place parentPlace, INakedObject referencedObject, IList<string> fieldNames,
                                                   string annotation) {
            LOG.Debug("appendXmlThenIncludeRemaining(: " + Log("parentPlace", parentPlace)
                      + AndLog("referencedObj", referencedObject) + AndLog("fieldNames", fieldNames) + AndLog("annotation", annotation)
                      + ")");

            LOG.Debug("appendXmlThenIncludeRemaining(..): invoking appendXml(parentPlace, referencedObject)");

            XElement referencedElement = AppendXml(parentPlace, referencedObject);
            var referencedPlace = new Place(referencedObject, referencedElement);

            bool includedField = IncludeField(referencedPlace, fieldNames, annotation);

            LOG.Debug("appendXmlThenIncludeRemaining(..): invoked includeField(referencedPlace, fieldNames)"
                      + AndLog("returned", "" + includedField));

            return includedField;
        }

        private static IEnumerable<XElement> ElementsUnder(XElement parentElement, string localName) {
            return parentElement.Descendants().Where(element => localName.Equals("*") || element.Name.LocalName.Equals(localName));
        }

        public INakedObject GetObject() {
            return rootPlace.NakedObject;
        }

        public void Include(string path) {
            Include(path, null);
        }

        public void Include(string path, string annotation) {
            // tokenize into successive fields
            List<string> fieldNames = path.Split('/').Select(tok => tok.ToLower()).ToList();

            fieldNames.ForEach(s => LOG.Debug("include(..): " + Log("token", s)));

            LOG.Debug("include(..): " + Log("fieldNames", fieldNames));

            // navigate first field, from the root.
            LOG.Debug("include(..): invoking includeField");
            IncludeField(rootPlace, fieldNames, annotation);
        }

        //  return true if able to navigate the complete vector of field names
        //                  successfully; false if a field could not be located or it turned
        //                  out to be a value.

        private bool IncludeField(Place place, IList<string> fieldNames, string annotation) {
            LOG.DebugFormat("includeField(: {0})", Log("place", place) + AndLog("fieldNames", fieldNames) + AndLog("annotation", annotation));

            INakedObject nakedObject = place.NakedObject;
            XElement xmlElement = place.XmlElement;

            // we use a copy of the path so that we can safely traverse collections
            // without side-effects
            fieldNames = fieldNames.ToList();

            // see if we have any fields to process
            if (!fieldNames.Any()) {
                return true;
            }

            // take the first field name from the list, and remove
            string fieldName = fieldNames.First();
            fieldNames.Remove(fieldName);

            LOG.Debug("includeField(Pl, Vec, Str):" + Log("processing field", fieldName) + AndLog("left", "" + fieldNames.Count()));

            // locate the field in the object's class
            INakedObjectSpecification nos = nakedObject.Specification;
            INakedObjectAssociation field = nos.Properties.Where(p => p.Id.ToLower() == fieldName).SingleOrDefault();

            if (field == null) {
                LOG.Info("includeField(Pl, Vec, Str): could not locate field, skipping");
                return false;
            }

            // locate the corresponding XML element
            // (the corresponding XSD element will later be attached to xmlElement
            // as its userData)
            LOG.Debug("includeField(Pl, Vec, Str): locating corresponding XML element");
            IEnumerable<XElement> xmlFieldElements = ElementsUnder(xmlElement, field.Id);
            if (xmlFieldElements.Count() != 1) {
                LOG.Info("includeField(Pl, Vec, Str): could not locate " + Log("field", field.Id) + AndLog("xmlFieldElements.size", "" + xmlFieldElements.Count()));
                return false;
            }
            XElement xmlFieldElement = xmlFieldElements.First();

            if (!fieldNames.Any() && annotation != null) {
                // nothing left in the path, so we will apply the annotation now
                NofMetaModel.SetAnnotationAttribute(xmlFieldElement, annotation);
            }

            var fieldPlace = new Place(nakedObject, xmlFieldElement);

            if (field.Specification.IsParseable) {
                LOG.Debug("includeField(Pl, Vec, Str): field is value; done");
                return false;
            }
            if (field is IOneToOneAssociation) {
                LOG.Debug("includeField(Pl, Vec, Str): field is 1->1");

                var oneToOneAssociation = ((IOneToOneAssociation) field);
                INakedObject referencedObject = oneToOneAssociation.GetNakedObject(fieldPlace.NakedObject);

                if (referencedObject == null) {
                    return true; // not a failure if the reference was null
                }

                bool appendedXml = AppendXmlThenIncludeRemaining(fieldPlace, referencedObject, fieldNames, annotation);

                LOG.Debug("includeField(Pl, Vec, Str): 1->1: invoked appendXmlThenIncludeRemaining for " + Log("referencedObj", referencedObject) + AndLog("returned", "" + appendedXml));

                return appendedXml;
            }
            if (field is IOneToManyAssociation) {
                LOG.Debug("includeField(Pl, Vec, Str): field is 1->M");

                var oneToManyAssociation = (IOneToManyAssociation) field;
                var collection = oneToManyAssociation.GetNakedObject(fieldPlace.NakedObject);
                var facet = collection.GetCollectionFacetFromSpec();

                LOG.Debug("includeField(Pl, Vec, Str): 1->M: " /*+ Log("collection.size", "" + facet.Size(collection))*/);
                var allFieldsNavigated = true;

                for (IEnumerator<INakedObject> enumer = facet.AsEnumerable(collection).GetEnumerator(); enumer.MoveNext();) {
                    var referencedObject = enumer.Current;

                    var appendedXml = AppendXmlThenIncludeRemaining(fieldPlace, referencedObject, fieldNames, annotation);

                    LOG.Debug("includeField(Pl, Vec, Str): 1->M: + invoked appendXmlThenIncludeRemaining for " + Log("referencedObj", referencedObject) + AndLog("returned", "" + appendedXml));

                    allFieldsNavigated = allFieldsNavigated && appendedXml;
                }
                LOG.Debug("includeField(Pl, Vec, Str): " + Log("returning", "" + allFieldsNavigated));

                return allFieldsNavigated;
            }

            return false; // fall through, shouldn't get here but just in case.
        }

        private static string Log(string label, INakedObject nakedObject) {
            return Log(label, (nakedObject == null ? "(null)" : nakedObject.TitleString() + "[" + OidOrHashCode(nakedObject) + "]"));
        }

        private static string Log(string label, object obj) {
            return (label ?? "?") + "='" + (obj == null ? "(null)" : obj.ToString()) + "'";
        }

        //  Merges the tree of Elements whose root is <code>childElement</code>
        //  underneath the <code>parentElement</code>.
        //  
        //  If the <code>parentElement</code> already has an element that matches
        //  the <code>childElement</code>, then recursively attaches the
        //  grandchildren instead.
        //  
        //  The element returned will be either the supplied
        //  <code>childElement</code>, or an existing child element if one already
        //  existed under <code>parentElement</code>.

        private static XElement MergeTree(XElement parentElement, XElement childElement) {
            LOG.Debug("mergeTree(" + Log("parent", parentElement) + AndLog("child", childElement));

            string childElementOid = NofMetaModel.GetAttribute(childElement, "oid");

            LOG.Debug("mergeTree(El,El): " + Log("childOid", childElementOid));
            if (childElementOid != null) {
                // before we add the child element, check to see if it is already
                // there
                LOG.Debug("mergeTree(El,El): check if child already there");
                IEnumerable<XElement> existingChildElements = ElementsUnder(parentElement, childElement.Name.LocalName);
                foreach (XElement possibleMatchingElement in existingChildElements) {
                    string possibleMatchOid = NofMetaModel.GetAttribute(possibleMatchingElement, "oid");
                    if (possibleMatchOid == null || !possibleMatchOid.Equals(childElementOid)) {
                        continue;
                    }

                    LOG.Debug("mergeTree(El,El): child already there; merging grandchildren");

                    // match: transfer the children of the child (grandchildren) to the
                    // already existing matching child
                    XElement existingChildElement = possibleMatchingElement;
                    IEnumerable<XElement> grandchildrenElements = ElementsUnder(childElement, "*");
                    foreach (XElement grandchildElement in grandchildrenElements) {
                        grandchildElement.Remove();

                        LOG.Debug("mergeTree(El,El): merging " + Log("grandchild", grandchildElement));

                        MergeTree(existingChildElement, grandchildElement);
                    }
                    return existingChildElement;
                }
            }

            parentElement.Add(childElement);
            return childElement;
        }

        public Place ObjectToElement(INakedObject nakedObject) {
            LOG.Debug("objectToElement(" + Log("object", nakedObject) + ")");

            INakedObjectSpecification nos = nakedObject.Specification;

            LOG.Debug("objectToElement(NO): create element and nof:title");
            XElement element = Schema.CreateElement(XmlDocument, nos.ShortName, nos.FullName, nos.SingularName, nos.PluralName);
            NofMetaModel.AppendNofTitle(element, nakedObject.TitleString());

            LOG.Debug("objectToElement(NO): create XS element for NOF class");
            XElement xsElement = Schema.CreateXsElementForNofClass(XsdDocument, element, topLevelElementWritten);

            // hack: every element in the XSD schema apart from first needs minimum cardinality setting.
            topLevelElementWritten = true;

            var place = new Place(nakedObject, element);

            NofMetaModel.SetAttributesForClass(element, OidOrHashCode(nakedObject));

            INakedObjectAssociation[] fields = nos.Properties;
            LOG.Debug("objectToElement(NO): processing fields");

            var seenFields = new List<string>();

            foreach (INakedObjectAssociation field in fields) {
                string fieldName = field.Id;

                LOG.Debug("objectToElement(NO): " + Log("field", fieldName));

                // Skip field if we have seen the name already
                // This is a workaround for getLastActivity(). This method exists
                // in AbstractNakedObject, but is not (at some level) being picked up
                // by the dot-net reflector as a property. On the other hand it does
                // exist as a field in the meta model (NakedObjectSpecification).
                //
                // Now, to re-expose the lastactivity field for .Net, a deriveLastActivity()
                // has been added to BusinessObject. This caused another field ofthe
                // same name, ultimately breaking the XSD.

                if (seenFields.Contains(fieldName)) {
                    LOG.Debug("objectToElement(NO): " + Log("field", fieldName) + " SKIPPED");
                    continue;
                }
                seenFields.Add(fieldName);

                XNamespace ns = Schema.GetUri();

                var xmlFieldElement = new XElement(ns + fieldName);

                XElement xsdFieldElement;

                if (field.Specification.IsParseable) {
                    LOG.Debug("objectToElement(NO): " + Log("field", fieldName) + " is value");

                    INakedObjectSpecification fieldNos = field.Specification;
                    // skip fields of type XmlValue
                    if (fieldNos != null &&
                        fieldNos.FullName != null &&
                        fieldNos.FullName.EndsWith("XmlValue")) {
                        continue;
                    }

                    var oneToOneAssociation = ((IOneToOneAssociation) field);
                    XElement xmlValueElement = xmlFieldElement; // more meaningful locally scoped name

                    INakedObject value;
                    try {
                        value = oneToOneAssociation.GetNakedObject(nakedObject);

                        // a null value would be a programming error, but we protect
                        // against it anyway
                        if (value == null) {
                            continue;
                        }

                        INakedObjectSpecification valueNos = value.Specification;

                        // XML
                        NofMetaModel.SetAttributesForValue(xmlValueElement, valueNos.ShortName);

                        bool notEmpty = (value.TitleString().Length > 0);
                        if (notEmpty) {
                            string valueStr = value.TitleString();
                            xmlValueElement.Add(new XText(valueStr));
                        }
                        else {
                            NofMetaModel.SetIsEmptyAttribute(xmlValueElement, true);
                        }
                    }
                    catch (Exception) {
                        LOG.Warn("objectToElement(NO): " + Log("field", fieldName) + ": getField() threw exception - skipping XML generation");
                    }

                    // XSD
                    xsdFieldElement = Schema.CreateXsElementForNofValue(xsElement, xmlValueElement);
                }
                else if (field is IOneToOneAssociation) {
                    LOG.Debug("objectToElement(NO): " + Log("field", fieldName) + " is IOneToOneAssociation");

                    var oneToOneAssociation = ((IOneToOneAssociation) field);

                    XElement xmlReferenceElement = xmlFieldElement; // more meaningful locally scoped name

                    INakedObject referencedNakedObject;

                    try {
                        referencedNakedObject = oneToOneAssociation.GetNakedObject(nakedObject);
                        string fullyQualifiedClassName = field.Specification.FullName;

                        // XML
                        NofMetaModel.SetAttributesForReference(xmlReferenceElement, Schema.Prefix, fullyQualifiedClassName);

                        if (referencedNakedObject != null) {
                            NofMetaModel.AppendNofTitle(xmlReferenceElement, referencedNakedObject.TitleString());
                        }
                        else {
                            NofMetaModel.SetIsEmptyAttribute(xmlReferenceElement, true);
                        }
                    }
                    catch (Exception) {
                        LOG.Warn("objectToElement(NO): " + Log("field", fieldName) + ": getAssociation() threw exception - skipping XML generation");
                    }

                    // XSD
                    xsdFieldElement = Schema.CreateXsElementForNofReference(xsElement, xmlReferenceElement, oneToOneAssociation.Specification.FullName);
                }
                else if (field is IOneToManyAssociation) {
                    LOG.Debug("objectToElement(NO): " + Log("field", fieldName) + " is IOneToManyAssociation");

                    var oneToManyAssociation = (IOneToManyAssociation) field;
                    XElement xmlCollectionElement = xmlFieldElement; // more meaningful locally scoped name

                    try {
                        INakedObject collection = oneToManyAssociation.GetNakedObject(nakedObject);
                        ITypeOfFacet facet = collection.GetTypeOfFacetFromSpec();

                        INakedObjectSpecification referencedTypeNos = facet.ValueSpec;
                        string fullyQualifiedClassName = referencedTypeNos.FullName;

                        // XML
                        NofMetaModel.SetNofCollection(xmlCollectionElement, Schema.Prefix, fullyQualifiedClassName, collection);
                    }
                    catch (Exception) {
                        LOG.Warn("objectToElement(NO): " + Log("field", fieldName) + ": get(obj) threw exception - skipping XML generation");
                    }

                    // XSD
                    xsdFieldElement = Schema.CreateXsElementForNofCollection(xsElement, xmlCollectionElement, oneToManyAssociation.Specification.FullName);
                }
                else {
                    LOG.Info("objectToElement(NO): " + Log("field", fieldName) + " is unknown type; ignored");
                    continue;
                }

                if (xsdFieldElement != null) {
                    xmlFieldElement.AddAnnotation(xsdFieldElement);
                }

                // XML
                LOG.Debug("objectToElement(NO): invoking mergeTree for field");
                MergeTree(element, xmlFieldElement);

                // XSD
                if (xsdFieldElement != null) {
                    LOG.Debug("objectToElement(NO): adding XS element for field to schema");
                    Schema.AddFieldXsElement(xsElement, xsdFieldElement);
                }
            }

            return place;
        }

        private static string OidOrHashCode(INakedObject nakedObject) {
            IOid oid = nakedObject.Oid;
            if (oid == null) {
                return "" + nakedObject.GetHashCode();
            }
            return oid.ToString();
        }

        private static string Transform(XDocument xDoc, string transform) {
            var sb = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(sb)) {
                var compiledTransform = new XslCompiledTransform();
                compiledTransform.Load(XmlReader.Create(new StringReader(transform)));
                compiledTransform.Transform(xDoc.CreateReader(), writer);
            }

            return sb.ToString();
        }

        public string TransformXml(string transform) {
            return Transform(XmlDocument, transform);
        }

        public string TransformXsd(string transform) {
            return Transform(XsdDocument, transform);
        }
    }
}