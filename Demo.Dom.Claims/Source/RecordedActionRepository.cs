using System.Linq;
using NakedObjects;

//Namespace must match that of the auto-generated partial class
namespace Demo.Dom.Claims
{
    //Use this partial class to define actions for the Employer class.
    //Also for methods to enrich the behaviour of properties - such as Validate,
    //Default, or Choices and for class-level attributes such as <IconName>.
    public class RecordedActionRepository
    {

        public IDomainObjectContainer Container { set; protected get; }

        public RecordedActionForClaim Find(int id)
        {
            return
                (from ra in Container.Instances<RecordedActionForClaim>() 
                 where ra.Id == id 
                 select ra).FirstOrDefault();
        }

    }

}