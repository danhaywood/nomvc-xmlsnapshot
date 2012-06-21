﻿// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace NakedObjects.Snapshot.Xml.Utility {
    public class XmlSchema {
        public static string DefaultPrefix = "app";
        private string uri;

        // The base part of the namespace prefix to use if none explicitly supplied in the constructor.

        public XmlSchema() : this(NofMetaModel.DefaultUriBase, DefaultPrefix) {}

        // uriBase the prefix for the application namespace's URIs
        // prefix the prefix for the application namespace's prefix

        public XmlSchema(string uriBase, string prefix) {
            uriBase = Helper.TrailingSlash(uriBase);
            if (uriBase == XNamespace.Xmlns.NamespaceName) {
                throw new ArgumentException("Namespace URI reserved for w3.org XMLNS namespace");
            }
            if (prefix == XsMetaModel.W3OrgXmlnsPrefix) {
                throw new ArgumentException("Namespace prefix reserved for w3.org XMLNS namespace.");
            }
            if (uriBase == XsMetaModel.xs.NamespaceName) {
                throw new ArgumentException("Namespace URI reserved for w3.org XML schema namespace.");
            }
            if (prefix == XsMetaModel.W3OrgXsPrefix) {
                throw new ArgumentException("Namespace prefix reserved for w3.org XML schema namespace.");
            }
            if (uriBase == XsMetaModel.xsi.NamespaceName) {
                throw new ArgumentException("Namespace URI reserved for w3.org XML schema-instance namespace.");
            }
            if (prefix  == XsMetaModel.W3OrgXsiPrefix) {
                throw new ArgumentException("Namespace prefix reserved for w3.org XML schema-instance namespace.");
            }
            if (uriBase == NofMetaModel.nof.NamespaceName) {
                throw new ArgumentException("Namespace URI reserved for NOF metamodel namespace.");
            }
            if (prefix == NofMetaModel.NofMetamodelNsPrefix) {
                throw new ArgumentException("Namespace prefix reserved for NOF metamodel namespace.");
            }
            UriBase = uriBase;
            Prefix = prefix;
        }

        // The base of the Uri in use.  All namespaces are concatenated with this. 
        //
        // The namespace string will be the concatenation of the plus the
        // package name of the class of the object being referenced.
        //
        // If not specified in the constructor, then {@link #DEFAULT_URI_PREFIX} is used.

        public string UriBase { get; private set; }

        // Returns the namespace URI for the class.
        public void SetUri(string fullyQualifiedClassName) {
            if (uri != null) {
                throw new InvalidOperationException("URI has already been specified.");
            }
            uri = UriBase + Helper.PackageNameFor(fullyQualifiedClassName) + "/" + Helper.ClassNameFor(fullyQualifiedClassName);
        }

        // The URI of the application namespace.
        //
        // The value returned will be <code>null</code> until a Snapshot}is created.
        public string GetUri() {
            if (uri == null) {
                throw new InvalidOperationException("URI has not been specified.");
            }
            return uri;
        }

        // The prefix to the namespace for the application.

        public string Prefix { get; private set; }

        // Creates an element with the specified localName, in the appropriate namespace for the NOS.
        //
        // If necessary the namespace definition is added to the root element of the doc used to
        // create the element.  The element is not parented but to avoid an error can only be added
        // as a child of another element in the same doc.

        public XElement CreateElement(XDocument doc, string localName, string fullyQualifiedClassName, string singularName, string pluralName) {
 
            XNamespace nsUri = GetUri();

            var element = new XElement(nsUri + localName);

            element.SetAttributeValue(NofMetaModel.nof + "fqn", fullyQualifiedClassName);
            element.SetAttributeValue(NofMetaModel.nof + "singular", singularName);
            element.SetAttributeValue(NofMetaModel.nof + "plural", pluralName);
            NofMetaModel.AddNamespace(element); // good a place as any

            AddNamespace(element, Prefix, GetUri());
            return element;
        }

        //  Sets the target namespace for the XSD document to a URI derived from the
        //  fully qualified class name of the supplied object

        public void SetTargetNamespace(XDocument xsdDoc, string fullyQualifiedClassName) {
            XElement xsSchemaElement = xsdDoc.Root;
            if (xsSchemaElement == null) {
                throw new ArgumentException("XSD XDocument must have <xs:schema> element attached");
            }

            //	targetNamespace="http://www.nakedobjects.org/ns/app/<fully qualified class name>"
            xsSchemaElement.SetAttributeValue("targetNamespace", GetUri());

            AddNamespace(xsSchemaElement, Prefix, GetUri());
        }

        //  Creates an &lt;xs:element&gt; element defining the presence of the named element
        //  representing a class

        public XElement CreateXsElementForNofClass(XDocument xsdDoc, XElement element, bool addCardinality) {
            // gather details from XML element
            string localName = element.Name.LocalName;

            //	<xs:element name="AO11ConfirmAnimalRegistration">
            //		<xs:complexType>
            //			<xs:sequence>
            //             <xs:element ref="nof:title"/>
            //             <!-- placeholder -->
            //			</xs:sequence>
            //			<xs:attribute ref="nof:feature"
            //			              default="class"/>
            //			<xs:attribute ref="nof:oid"/>
            //			<xs:attribute ref="nof:annotation"/>
            //			<xs:attribute ref="nof:fqn"/>
            //	    </xs:complexType>
            //	</xs:element>

            // xs:element/@name="class name"
            // add to XML schema as a global attribute
            XElement xsElementForNofClassElement = XsMetaModel.CreateXsElementElement(xsdDoc, localName, addCardinality);

            // xs:element/xs:complexType
            // xs:element/xs:complexType/xs:sequence
            XElement xsComplexTypeElement = XsMetaModel.ComplexTypeFor(xsElementForNofClassElement);
            XElement xsSequenceElement = XsMetaModel.SequenceFor(xsComplexTypeElement);

            // xs:element/xs:complexType/xs:sequence/xs:element ref="nof:title"
            XElement xsTitleElement = XsMetaModel.CreateXsElement(Helper.DocFor(xsSequenceElement), "element");
            xsTitleElement.SetAttributeValue("ref", NofMetaModel.NofMetamodelNsPrefix + ":" + "title");
            xsSequenceElement.Add(xsTitleElement);
            XsMetaModel.SetXsCardinality(xsTitleElement, 0, 1);

            // xs:element/xs:complexType/xs:sequence/xs:element ref="extensions"
            //addXsElementForAppExtensions(xsSequenceElement, extensions);

            // xs:element/xs:complexType/xs:attribute ...
            XsMetaModel.AddXsNofFeatureAttributeElements(xsComplexTypeElement, "class");
            XsMetaModel.AddXsNofAttribute(xsComplexTypeElement, "oid");
            XsMetaModel.AddXsNofAttribute(xsComplexTypeElement, "fqn");
            XsMetaModel.AddXsNofAttribute(xsComplexTypeElement, "singular");
            XsMetaModel.AddXsNofAttribute(xsComplexTypeElement, "plural");
            XsMetaModel.AddXsNofAttribute(xsComplexTypeElement, "annotation");

            element.AddAnnotation(xsElementForNofClassElement);

            return xsElementForNofClassElement;
        }

        //  Creates an <code>xs:element</code> element to represent a value field in a class.
        //  
        //  The returned element should be appended to <code>xs:sequence</code> element of the
        //  xs:element representing the type of the owning object.

        public XElement CreateXsElementForNofValue(XElement parentXsElementElement, XElement xmlValueElement) {
            // gather details from XML element
      
            var datatype = xmlValueElement.Attribute(NofMetaModel.nof + "datatype");
            string fieldName = xmlValueElement.Name.LocalName;

            // <xs:element name="%owning object%">
            //		<xs:complexType>
            //			<xs:sequence>
            //				<xs:element name="%%field object%%">
            //					<xs:complexType>
            //						<xs:sequence>
            //				            <xs:element name="nof-extensions">
            //					            <xs:complexType>
            //						            <xs:sequence>
            //                                      <xs:element name="%extensionClassShortName%" default="%extensionObjString" minOccurs="0"/>
            //                                      <xs:element name="%extensionClassShortName%" default="%extensionObjString" minOccurs="0"/>
            //                                      ...
            //                                      <xs:element name="%extensionClassShortName%" default="%extensionObjString" minOccurs="0"/>
            //						            </xs:sequence>
            //					            </xs:complexType>
            //				            </xs:element>
            //						</xs:sequence>
            //						<xs:attribute ref="nof:feature" fixed="value"/>
            //						<xs:attribute ref="nof:datatype" fixed="nof:%datatype%"/>
            //						<xs:attribute ref="nof:isEmpty"/>
            //			            <xs:attribute ref="nof:annotation"/>
            //					</xs:complexType>
            //				</xs:element>
            //			</xs:sequence>
            //		</xs:complexType>
            //	</xs:element>

            // xs:element/xs:complexType/xs:sequence
            XElement parentXsComplexTypeElement = XsMetaModel.ComplexTypeFor(parentXsElementElement);
            XElement parentXsSequenceElement = XsMetaModel.SequenceFor(parentXsComplexTypeElement);

            // xs:element/xs:complexType/xs:sequence/xs:element name="%%field object%"
            XElement xsFieldElementElement = XsMetaModel.CreateXsElementElement(Helper.DocFor(parentXsSequenceElement), fieldName);
            parentXsSequenceElement.Add(xsFieldElementElement);

            // xs:element/xs:complexType/xs:sequence/xs:element/xs:complexType
            XElement xsFieldComplexTypeElement = XsMetaModel.ComplexTypeFor(xsFieldElementElement);

            // NEW CODE TO SUPPORT EXTENSIONS;
            // uses a complexType/sequence

            // xs:element/xs:complexType/xs:sequence/xs:element/xs:complexType/xs:sequence
            //XElement xsFieldSequenceElement = XsMetaModel.SequenceFor(xsFieldComplexTypeElement);

            // xs:element/xs:complexType/xs:sequence/xs:element/xs:complexType/xs:sequence/xs:element name="nof-extensions"
            // xs:element/xs:complexType/xs:sequence/xs:element/xs:complexType/xs:sequence/xs:element/xs:complexType/xs:sequence
            //addXsElementForAppExtensions(xsFieldSequenceElement, extensions);

            XsMetaModel.AddXsNofFeatureAttributeElements(xsFieldComplexTypeElement, "value");
            XsMetaModel.AddXsNofAttribute(xsFieldComplexTypeElement, "datatype", datatype == null ? "" : datatype.Value);
            XsMetaModel.AddXsNofAttribute(xsFieldComplexTypeElement, "isEmpty");
            XsMetaModel.AddXsNofAttribute(xsFieldComplexTypeElement, "annotation");

            // ORIGINAL CODE THAT DIDN'T EXPORT EXTENSIONS
            // uses a simpleContent
            // (I've left this code in in case there is a need to regenerate schemas the "old way").

            // <xs:element name="%owning object%">
            //		<xs:complexType>
            //			<xs:sequence>
            //				<xs:element name="%%field object%%">
            //					<xs:complexType>
            //						<xs:simpleContent>
            //							<xs:extension base="xs:string">
            //								<xs:attribute ref="nof:feature" fixed="value"/>
            //								<xs:attribute ref="nof:datatype" fixed="nof:%datatype%"/>
            //								<xs:attribute ref="nof:isEmpty"/>
            //			                    <xs:attribute ref="nof:annotation"/>
            //							</xs:extension>
            //						</xs:simpleContent>
            //					</xs:complexType>
            //				</xs:element>
            //			</xs:sequence>
            //		</xs:complexType>
            //	</xs:element>

            // xs:element/xs:complexType/xs:sequence/xs:element/xs:complexType/xs:simpleContent/xs:extension
            //		XElement xsFieldSimpleContentElement = XsMetaModel.simpleContentFor(xsFieldComplexTypeElement);
            //		XElement xsFieldExtensionElement = XsMetaModel.extensionFor(xsFieldSimpleContentElement, "string");
            //		XsMetaModel.addXsNofFeatureAttributeElements(xsFieldExtensionElement, "value");
            //		XsMetaModel.addXsNofAttribute(xsFieldExtensionElement, "datatype", datatype);
            //		XsMetaModel.addXsNofAttribute(xsFieldExtensionElement, "isEmpty");
            //		XsMetaModel.addXsNofAttribute(xsFieldExtensionElement, "annotation");

            return xsFieldElementElement;
        }

        //  Creates an &lt;xs:element&gt; element defining the presence of the named element
        //  representing a reference to a class; appended to xs:sequence element

        public XElement CreateXsElementForNofReference(XElement parentXsElementElement, XElement xmlReferenceElement, string referencedClassName) {
            // gather details from XML element
            string fieldName = xmlReferenceElement.Name.LocalName;

            // <xs:element name="%owning object%">
            //		<xs:complexType>
            //			<xs:sequence>
            //				<xs:element name="%%field object%%">
            //					<xs:complexType>
            //						<xs:sequence>
            //							<xs:element ref="nof:title" minOccurs="0"/>
            //				            <xs:element name="nof-extensions">
            //					            <xs:complexType>
            //						            <xs:sequence>
            //				                        <xs:element name="app:%extension class short name%" minOccurs="0" maxOccurs="1" default="%value%"/>
            //				                        <xs:element name="app:%extension class short name%" minOccurs="0" maxOccurs="1" default="%value%"/>
            //				                        ...
            //				                        <xs:element name="app:%extension class short name%" minOccurs="0" maxOccurs="1" default="%value%"/>
            //						            </xs:sequence>
            //					            </xs:complexType>
            //				            </xs:element>
            //							<xs:sequence minOccurs="0" maxOccurs="1"/>
            //						</xs:sequence>
            //						<xs:attribute ref="nof:feature" fixed="reference"/>
            //						<xs:attribute ref="nof:type" default="%%appX%%:%%type%%"/>
            //						<xs:attribute ref="nof:isEmpty"/>
            //						<xs:attribute ref="nof:annotation"/>
            //					</xs:complexType>
            //				</xs:element>
            //			</xs:sequence>
            //		</xs:complexType>
            //	</xs:element>

            // xs:element/xs:complexType/xs:sequence
            XElement parentXsComplexTypeElement = XsMetaModel.ComplexTypeFor(parentXsElementElement);
            XElement parentXsSequenceElement = XsMetaModel.SequenceFor(parentXsComplexTypeElement);

            // xs:element/xs:complexType/xs:sequence/xs:element name="%%field object%"
            XElement xsFieldElementElement = XsMetaModel.CreateXsElementElement(Helper.DocFor(parentXsSequenceElement), fieldName);
            parentXsSequenceElement.Add(xsFieldElementElement);

            // xs:element/xs:complexType/xs:sequence/xs:element/xs:complexType/xs:sequence
            XElement xsFieldComplexTypeElement = XsMetaModel.ComplexTypeFor(xsFieldElementElement);
            XElement xsFieldSequenceElement = XsMetaModel.SequenceFor(xsFieldComplexTypeElement);

            // xs:element/xs:complexType/xs:sequence/xs:element/xs:complexType/xs:sequence/xs:element ref="nof:title"
            XElement xsFieldTitleElement = XsMetaModel.CreateXsElement(Helper.DocFor(xsFieldSequenceElement), "element");
            xsFieldTitleElement.SetAttributeValue("ref", NofMetaModel.NofMetamodelNsPrefix + ":" + "title");
            xsFieldSequenceElement.Add(xsFieldTitleElement);
            XsMetaModel.SetXsCardinality(xsFieldTitleElement, 0, 1);

            // xs:element/xs:complexType/xs:sequence/xs:element/xs:complexType/xs:sequence/xs:element name="nof-extensions"
            //addXsElementForAppExtensions(xsFieldSequenceElement, extensions);

            // xs:element/xs:complexType/xs:sequence/xs:element/xs:complexType/xs:sequence/xs:sequence   // placeholder
            XElement xsReferencedElementSequenceElement = XsMetaModel.SequenceFor(xsFieldSequenceElement);
            XsMetaModel.SetXsCardinality(xsReferencedElementSequenceElement, 0, 1);

            XsMetaModel.AddXsNofFeatureAttributeElements(xsFieldComplexTypeElement, "reference");
            XsMetaModel.AddXsNofAttribute(xsFieldComplexTypeElement, "type", "app:" + referencedClassName, false);
            XsMetaModel.AddXsNofAttribute(xsFieldComplexTypeElement, "isEmpty");
            XsMetaModel.AddXsNofAttribute(xsFieldComplexTypeElement, "annotation");
            return xsFieldElementElement;
        }

        ///**
        // * Creates an &lt;xs:element&gt; element defining the presence of the named element
        // * representing a collection in a class; appended to xs:sequence element
        // */ 
        public XElement CreateXsElementForNofCollection(XElement parentXsElementElement, XElement xmlCollectionElement, string referencedClassName) {
            // gather details from XML element
            string fieldName = xmlCollectionElement.Name.LocalName;

            // <xs:element name="%owning object%">
            //		<xs:complexType>
            //			<xs:sequence>
            //				<xs:element name="%%field object%%">
            //					<xs:complexType>
            //						<xs:sequence>
            //							<xs:element ref="nof:oids" minOccurs="0" maxOccurs="1"/>
            //						    <!-- nested element definitions go here -->
            //						</xs:sequence>
            //						<xs:attribute ref="nof:feature" fixed="collection"/>
            //						<xs:attribute ref="nof:type" fixed="%%appX%%:%%type%%"/>
            //						<xs:attribute ref="nof:size"/>
            //						<xs:attribute ref="nof:annotation"/>
            //					</xs:complexType>
            //				</xs:element>
            //			</xs:sequence>
            //		</xs:complexType>
            //	</xs:element>

            // xs:element/xs:complexType/xs:sequence
            XElement parentXsComplexTypeElement = XsMetaModel.ComplexTypeFor(parentXsElementElement);
            XElement parentXsSequenceElement = XsMetaModel.SequenceFor(parentXsComplexTypeElement);

            // xs:element/xs:complexType/xs:sequence/xs:element name="%field object%%"
            XElement xsFieldElementElement = XsMetaModel.CreateXsElementElement(Helper.DocFor(parentXsSequenceElement), fieldName);
            parentXsSequenceElement.Add(xsFieldElementElement);

            // xs:element/xs:complexType/xs:sequence/xs:element/xs:complexType
            XElement xsFieldComplexTypeElement = XsMetaModel.ComplexTypeFor(xsFieldElementElement);
            // xs:element/xs:complexType/xs:sequence/xs:element/xs:complexType/xs:sequence
            XElement xsFieldSequenceElement = XsMetaModel.SequenceFor(xsFieldComplexTypeElement);

            // xs:element/xs:complexType/xs:sequence/xs:element/xs:complexType/xs:sequence/xs:element ref="nof:oids"
            XElement xsFieldOidsElement = XsMetaModel.CreateXsElement(Helper.DocFor(xsFieldSequenceElement), "element");
            xsFieldOidsElement.SetAttributeValue("ref", NofMetaModel.NofMetamodelNsPrefix + ":" + "oids");
            xsFieldSequenceElement.Add(xsFieldOidsElement);
            XsMetaModel.SetXsCardinality(xsFieldOidsElement, 0, 1);

            // extensions
            // addXsElementForAppExtensions(xsFieldSequenceElement, extensions);

//		// xs:element/xs:complexType/xs:sequence/xs:element/xs:complexType/xs:sequence/xs:choice
//		XElement xsFieldChoiceElement = XsMetaModel.choiceFor(xsFieldComplexTypeElement); // placeholder
//		XsMetaModel.setXsCardinality(xsFieldChoiceElement, 0, Integer.MAX_VALUE);

//		XElement xsFieldTitleElement = addXsNofRefElementElement(xsFieldSequenceElement, "title");

//		XElement xsReferencedElementSequenceElement = sequenceFor(xsFieldSequenceElement);
//		setXsCardinality(xsReferencedElementSequenceElement, 0, 1);

            XsMetaModel.AddXsNofFeatureAttributeElements(xsFieldComplexTypeElement, "collection");
            XsMetaModel.AddXsNofAttribute(xsFieldComplexTypeElement, "type", "app:" + referencedClassName, false);
            XsMetaModel.AddXsNofAttribute(xsFieldComplexTypeElement, "size");
            XsMetaModel.AddXsNofAttribute(xsFieldComplexTypeElement, "annotation");

            return xsFieldElementElement;
        }

        //  <pre>
        //  xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
        //  xsi:schemaLocation="http://www.nakedobjects.org/ns/app/sdm.common.fixture.schemes.ao.communications ddd.xsd"
        //  </pre>
        //  
        //  Assumes that the URI has been specified. 
        //  
        //  xmlDoc
        //  fullyQualifiedClassName
        //  schemaLocationFileName

        public void AssignSchema(XDocument xmlDoc, string fullyQualifiedClassName, string schemaLocationFileName) {
            string xsiSchemaLocationAttrValue = GetUri() + " " + schemaLocationFileName;

            XElement rootElement = xmlDoc.Root;

            // xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
            AddNamespace(rootElement, XsMetaModel.W3OrgXsiPrefix, XsMetaModel.xsi.NamespaceName);

            // xsi:schemaLocation="http://www.nakedobjects.org/ns/app/<fully qualified class name> sdm.common.fixture.schemes.ao.communications sdm.common.fixture.schemes.ao.communications.AO11ConfirmAnimalRegistration.xsd"

            rootElement.SetAttributeValue(XsMetaModel.xsi + "schemaLocation", xsiSchemaLocationAttrValue);
        }

        //  Adds a previously created &lt;xs:element&gt; element (representing a field of an object)
        //  to the supplied element (presumed to be a <code>complexType/sequence</code>).

        public void AddFieldXsElement(XElement xsElement, XElement xsFieldElement) {
            if (xsFieldElement == null) {
                return;
            }
            XElement sequenceElement = XsMetaModel.SequenceForComplexTypeFor(xsElement);
            sequenceElement.Add(xsFieldElement);
        }

        //  Adds a namespace using the supplied prefix and the supplied URI to the
        //  root element of the document that is the parent of the supplied element.
        //  
        //  If the namespace declaration already exists but has a different URI (shouldn't
        //  normally happen) overwrites with supplied URI.

        private static void AddNamespace(XElement element, string prefix, string nsUri) {
           
            XElement rootElement = Helper.RootElementFor(element);
            // see if we have the NS prefix there already
            var existingNsUri = rootElement.Attribute(XNamespace.Xmlns + prefix);
            // if there is none (or it is different from what we want), then set the attribute
            if (existingNsUri == null || !existingNsUri.Value.Equals(nsUri)) {
                rootElement.SetAttributeValue(XNamespace.Xmlns + prefix, nsUri);
            }
        }

        public XElement AddXsElementIfNotPresent(XElement parentXsElement, XElement childXsElement) {
            XElement parentChoiceOrSequenceElement = XsMetaModel.ChoiceOrSequenceFor(XsMetaModel.ComplexTypeFor(parentXsElement));

            if (parentChoiceOrSequenceElement == null) {
                throw new ArgumentException("Unable to locate complexType/sequence or complexType/choice under supplied parent XSD element");
            }

            XAttribute childXsElementAttr = childXsElement.Attribute("name");
            string localName = childXsElementAttr.Value;

            XNamespace ns = "*";

            IEnumerable<XElement> existingElements = parentChoiceOrSequenceElement.Descendants(ns + childXsElement.Name.LocalName);
            foreach (XElement xsElement in existingElements) {
                XAttribute attr = xsElement.Attribute("name");
                if (attr != null && attr.Value.Equals(localName)) {
                    return xsElement;
                }
            }

            parentChoiceOrSequenceElement.Add(childXsElement);
            return childXsElement;
        }
    }
}