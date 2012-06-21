// Copyright � Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Demo.Dom.Claims;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;
using NakedObjects.Core.Context;
using NakedObjects.Core.Security;
using NakedObjects.Reflector.Spec;
using NakedObjects.Web.Mvc.Controllers;
using NakedObjects.Web.Mvc.Html;
using NakedObjects.Web.Mvc.Models;

namespace Demo.App.Mvc.Controllers {

    [Authorize] 
    public class RecordedActionController : GenericControllerImpl {

        #region actions

        [HttpGet]
        public override ActionResult Details(ObjectAndControlData controlData) {
            return base.Details(controlData);
        }

        [HttpGet]
        public override ActionResult EditObject(ObjectAndControlData controlData) {
            return base.EditObject(controlData);
        }

        [HttpGet]
        public override ActionResult Action(ObjectAndControlData controlData) {
            return base.Action(controlData);
        }

        [HttpPost]
        public override ActionResult Details(ObjectAndControlData controlData, FormCollection form) {
            return base.Details(controlData, form);
        }

        [HttpPost]
        public override  ActionResult EditObject(ObjectAndControlData controlData, FormCollection form) {
            return base.EditObject(controlData, form);
        }

        [HttpPost]
        public override ActionResult Edit(ObjectAndControlData controlData, FormCollection form) {
            return base.Edit(controlData, form);
        }

        [HttpPost]
        public override ActionResult Action(ObjectAndControlData controlData, FormCollection form) {
            return base.Action(controlData, form);
        }

        [HttpGet]
        public FileContentResult ViewSnapshot(string id)
        {
            //NakedObjectsContext.Instance.SetSession(new WindowsSession(User));
            //try
            //{
                var repository = GetService<RecordedActionRepository>();
                var ra = repository.Find(Int32.Parse(id));
                if (ra == null)
                {
                    return null;
                }
                var encoding = new System.Text.UTF8Encoding();
                var bytes = encoding.GetBytes(ra.XmlSnapshot);
                return new FileContentResult(bytes, "application/xml");

            //}
            //finally
            //{
            //    NakedObjectsContext.CloseSession();
            //}
        }

        public static T GetService<T>()
        {
            foreach (var s in GetAllServices())
            {
                if (s is T)
                {
                    return (T)s;
                }
            }
            return default(T);
        }

        public static List<object> GetAllServices()
        {
            return NakedObjectsContext.ObjectPersistor.GetServices()
                    .Where(x => GetActions(x).Any()).Select(x => x.Object).ToList();
        }

        public static IEnumerable<INakedObjectAction> GetActions(INakedObject nakedObject)
        {
            return nakedObject.Specification.GetObjectActions(NakedObjectActionConstants.USER)
                .OfType<NakedObjectActionImpl>()
                .Union(nakedObject.Specification.GetObjectActions(NakedObjectActionConstants.USER)
                .OfType<NakedObjectActionSet>()
                .SelectMany(set => (IEnumerable<INakedObjectAction>)set.Actions))
                .Where(a => a.IsUsable(CurrentSession, nakedObject).IsAllowed)
                .Where(a => a.IsVisible(CurrentSession, nakedObject));
        }

        static ISession CurrentSession
        {
            get
            {
                return NakedObjectsContext.Session;
            }
        }


        #endregion

    }
}