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
    [MetadataType(typeof(Claim_Metadata))]
    public partial class Claim
    {
        public IDomainObjectContainer Container { set; protected get; }
        public IXmlSnapshotService XmlSnapshotService { set; protected get; }

        public string Title()
        {
            var t = new TitleBuilder();
            t.Append(Id).AppendSpace().Append(Name);
            return t.ToString();
        }


        public void AddAllowance(string name)
        {
            var allowance = Container.NewTransientInstance<Allowance>();
            allowance.Name = name;
            allowance.Claim = this;
            Container.Persist(ref allowance);

            this.Allowances.Add(allowance);
        }

        public void RemoveAllowance(Allowance allowance)
        {
            this.Allowances.Remove(allowance);
            Container.DisposeInstance(allowance);
        }
        public List<Allowance> Choices0RemoveAllowance()
        {
            return this.Allowances.ToList();
        }
        public string ValidateRemoveAllowance(Allowance allowance)
        {
            return this.Allowances.Contains(allowance) ? null : "Not an allowance of this claim";
        }

        public void Decide(string status, string rationale)
        {
            this.Status = status;

            var ra = Container.NewTransientInstance<RecordedActionForClaim>();

            ra.Claim = this;
            ra.Summary = "status is: " + status;
            ra.Rationale = rationale;
            ra.User = Container.Principal.Identity.Name;
            var snapshot = XmlSnapshotService.GenerateSnapshot(this);
            snapshot.Include("Allowances");
            ra.XmlSnapshot = snapshot.Xml;


            Container.Persist(ref ra);
        }
        public List<string> ChoicesDecide(string status)
        {
            return new List<string>() {"Awarded", "Disallowed", "Suspended", "Stopped"};
        }


        public IQueryable<RecordedActionForClaim> ListRecordedActions()
        {
            return from ra in Container.Instances<RecordedActionForClaim>() 
                   where ra.Claim.Id == this.Id 
                   select ra;
        }

    }

    #region 'Buddy class'
    //This is the so-called 'buddy class' for Employer.  It should have the 
    //same properties as the auto-generated partial Employer class, to which 
    //property-level attributes such as <Hidden> may be added.
    public class Claim_Metadata
    {

    }
    #endregion

}