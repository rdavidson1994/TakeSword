using System.Collections.Generic;

namespace TakeSword
{
    public interface IVerbalAI<TActor> : IRoutine<TActor>
    {
        //AI routines that can provide token resolution for verbs.
        IEnumerable<GameObject> ObjectsWithName(string name);
        GameObject ChooseObject(string input, IEnumerable<GameObject> objects);
    }
}