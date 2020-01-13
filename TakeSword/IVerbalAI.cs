using System.Collections.Generic;

namespace TakeSword
{
    public interface IVerbalAI : IRoutine
    {
        //AI routines that can provide token resolution for verbs.
        new PhysicalActor GetActor();
        //IRoutine already provites a GetActor, but it only gaurantees an IActor
        //We hide the original to provide a more specific return type.
        IEnumerable<GameObject> ObjectsWithName(string name);
        GameObject ChooseObject(string input, IEnumerable<GameObject> objects);
    }
}