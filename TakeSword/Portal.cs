using System;

namespace TakeSword
{
    public class Portal : GameObject
    {
        public Direction Direction { get; protected set; }
        public Portal Opposite { get; protected set; }

        public Portal(
            Direction direction,
            ILocation location = null,
            FrozenTraitStore traits = null
        ) : base(location, traits)
        {
            Direction = Direction;
        }


        public Portal(
            Portal opposite,
            ILocation location=null,
            FrozenTraitStore traits=null,
            Direction? direction = null
        ) : base (location, traits)
        {
            Opposite = opposite;
            Opposite.Opposite = this;
            if (direction.HasValue)
            {
                Direction = direction.Value;
            }
            else
            {
                Direction = DirectionConverter.Opposite(Opposite.Direction);
            }
        }
    }
}
