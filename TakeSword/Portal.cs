namespace TakeSword
{
    public class Portal : GameObject
    {
        public Direction Direction { get; protected set; }
        public Portal Opposite { get; protected set; }
        public Portal(Direction direction) : base ()
        {
            Direction = direction;
        }
        public Portal(Portal portal)
        {
            Opposite = portal;
            portal.Opposite = this;
            Direction = DirectionConverter.Opposite(portal.Direction);
        }
    }
}
