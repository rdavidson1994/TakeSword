namespace TakeSword
{
    public interface ILocation
    {
        bool BeEntered(GameObject gameObject);
        bool BeExited(GameObject gameObject);
        void HandleAnnouncement(object announcement);
    }
}