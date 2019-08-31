using System.Collections.Generic;

namespace TakeSword
{
    public interface IGameObject
    {
        string GetName(IGameObject viewer);
        ILocation Location { get; }
        bool AddTrait(Trait trait);
        TraitType As<TraitType>() where TraitType : Trait;
        void Move(ILocation location);
        bool RemoveTrait(Trait trait);
    }
}