// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Text;
using System.Xml;
using System.Xml.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Core.Persist;

namespace NakedObjects.Snapshot {
    public class XmlSnapshot : IXmlSnapshot {
        private readonly Xml.Utility.XmlSnapshot snapshot;

        public XmlSnapshot(object obj) {
            INakedObject nakedObject = PersistorUtils.GetAdapterForElseCreateAdapterForTransient(obj);
            snapshot = new Xml.Utility.XmlSnapshot(nakedObject);
        }

        #region IXmlSnapshot Members

        public string Xml {
            get {
                XElement element = snapshot.XmlElement;
                var sb = new StringBuilder();
                using (XmlWriter writer = XmlWriter.Create(sb)) {
                    element.WriteTo(writer);
                }
                return sb.ToString();
            }
        }

        public string Xsd {
            get {
                XElement element = snapshot.XsdElement;
                var sb = new StringBuilder();            
                using (XmlWriter writer = XmlWriter.Create(sb)) {
                    element.WriteTo(writer);
                }
                return sb.ToString();
            }
        }

        public string SchemaLocationFileName {
            get { return snapshot.SchemaLocationFileName; }
        }

        public void Include(string path) {
            snapshot.Include(path);
        }

        public void Include(string path, string annotation) {
            snapshot.Include(path, annotation);
        }

        public string TransformedXml(string transform) {
            return snapshot.TransformXml(transform);
        }

        public string TransformedXsd(string transform) {
            return snapshot.TransformXsd(transform);
        }

        #endregion
    }
}