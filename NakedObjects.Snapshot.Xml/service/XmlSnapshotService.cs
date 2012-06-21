// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;

namespace NakedObjects.Snapshot {
    [Named("XML Snapshot")]
    public class XmlSnapshotService : IXmlSnapshotService {
        #region IXmlSnapshotService Members

        [NotContributedAction]
        public IXmlSnapshot GenerateSnapshot(object domainObject) {
            return new XmlSnapshot(domainObject);
        }

        #endregion
    }
}