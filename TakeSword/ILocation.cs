using System.Collections.Generic;

namespace TakeSword
{
    public interface ILocation
    {
        bool BeEntered(GameObject gameObject);
        bool BeExited(GameObject gameObject);
        void HandleAnnouncement(ActionAnnouncement announcement);
        IEnumerable<GameObject> NearbyObjects(long range);
        ISchedule Schedule { get; }
    }
}