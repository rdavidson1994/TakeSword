namespace TakeSword
{
    internal class OffscreenLocation : ILocation
    {
        public bool BeEntered(GameObject gameObject)
        {
            return true;
        }

        public bool BeExited(GameObject gameObject)
        {
            return true;
        }

        public void HandleAnnouncement(object announcement)
        {
            // do nothing
        }
    }
}