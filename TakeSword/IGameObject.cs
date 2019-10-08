using System.Collections.Generic;

namespace TakeSword
{
    public interface IGameObject
    {
        string GetName(IGameObject viewer);
        ILocation Location { get; }
        bool AddTrait<T>(T trait) where T: Trait;
        TraitType As<TraitType>() where TraitType : Trait;
        void Move(ILocation location);
        bool RemoveTrait<T>() where T : Trait;
    }
}