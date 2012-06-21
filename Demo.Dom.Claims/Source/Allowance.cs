using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;
using NakedObjects.Snapshot;

//Namespace must match that of the auto-generated partial class
namespace Demo.Dom.Claims
{
    //Use this partial class to define actions for the Employer class.
    //Also for methods to enrich the behaviour of properties - such as Validate,
    //Default, or Choices and for class-level attributes such as <IconName>.
    [MetadataType(typeof(Allowance_Metadata))]
    public partial class Allowance
    {
        public string Title()
        {
            var t = new TitleBuilder();
            t.Append(Id).AppendSpace().Append(Name);
            return t.ToString();
        }


    }

    #region 'Buddy class'
    //This is the so-called 'buddy class' for Employer.  It should have the 
    //same properties as the auto-generated partial Employer class, to which 
    //property-level attributes such as <Hidden> may be added.
    public class Allowance_Metadata
    {

    }
    #endregion

}