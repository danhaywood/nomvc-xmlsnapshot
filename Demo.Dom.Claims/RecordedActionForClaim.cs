using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace Demo.Dom.Claims
{
    public partial class RecordedActionForClaim
    {
    
        #region Primitive Properties
        #region Id (Int32)
    [MemberOrder(100)]
        public virtual int  Id {get; set;}

        #endregion
        #region Summary (String)
    [MemberOrder(110)]
        public virtual string  Summary {get; set;}

        #endregion
        #region Rationale (String)
    [MemberOrder(120)]
        public virtual string  Rationale {get; set;}

        #endregion
        #region User (String)
    [MemberOrder(130)]
        public virtual string  User {get; set;}

        #endregion
        #region XmlSnapshot (String)
    [MemberOrder(140)]
        public virtual string  XmlSnapshot {get; set;}

        #endregion

        #endregion
        #region Navigation Properties
        #region Claim (Claim)
    		
    [MemberOrder(150)]
    	public virtual Claim Claim {get; set;}

        #endregion

        #endregion
    }
}
