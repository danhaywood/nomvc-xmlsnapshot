using System.Linq;
using NakedObjects;

//Namespace must match that of the auto-generated partial class
namespace Demo.Dom.Claims
{
    //Use this partial class to define actions for the Employer class.
    //Also for methods to enrich the behaviour of properties - such as Validate,
    //Default, or Choices and for class-level attributes such as <IconName>.
    public class ClaimFactory 
    {

        public IDomainObjectContainer Container { set; protected get; }

        public Claim NewClaim(int claimId, string claimName)
        {
            var claim = Container.NewTransientInstance<Claim>();
            claim.Id = claimId;
            claim.Name = claimName;
            claim.Status = "Pending";

            Container.Persist(ref claim);

            return claim;
        }


        public Claim Find(int id)
        {
            return Container.Instances<Claim>().FirstOrDefault(c => c.Id == id);
        }
    }

}