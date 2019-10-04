namespace TakeSword
{
    public static class PortalFactory
    {
        public static Portal CreatePortal(ILocation location, Direction direction)
        {
            Portal portal1 = new Portal(direction);
            portal1.Move(location);
            Portal portal2 = new Portal(portal1);
            return portal1;
        }

        public static Portal CreatePortal(ILocation location, Direction direction, ILocation location2)
        {
            Portal portal = CreatePortal(location, direction);
            portal.Opposite.Move(location2);
            return portal;
        }
    }
}
