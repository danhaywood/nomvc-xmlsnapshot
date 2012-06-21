using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace Demo.Dom.Claims
{
    public partial class Claim
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
        #region Status (String)
    [MemberOrder(120)]
        public virtual string  Status {get; set;}

        #endregion

        #endregion
        #region Navigation Properties
        #region Allowances (Collection of Allowance)
    		
    	    private ICollection<Allowance> _allowances = new List<Allowance>();
    		
    		[MemberOrder(130), Disabled]
        public virtual ICollection<Allowance> Allowances
        {
            get
            {
                return _allowances;
            }
    		set
    		{
    		    _allowances = value;
    		}
        }

        #endregion

        #endregion
    }
}
