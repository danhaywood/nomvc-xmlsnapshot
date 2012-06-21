using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace Demo.Dom.Claims
{
    public partial class Allowance
    {
    
        #region Primitive Properties
        #region Id (Int32)
    [MemberOrder(100)]
        public virtual int  Id {get; set;}

        #endregion
        #region Name (String)
    [MemberOrder(110)]
        public virtual string  Name {get; set;}

        #endregion
        #region ClaimId (Int32)
    [MemberOrder(120)]
        public virtual int  ClaimId {get; set;}

        #endregion

        #endregion
        #region Navigation Properties
        #region Claim (Claim)
    		
    [MemberOrder(130)]
    	public virtual Claim Claim {get; set;}

        #endregion

        #endregion
    }
}
