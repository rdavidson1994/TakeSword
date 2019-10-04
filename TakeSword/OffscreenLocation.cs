using System.Collections.Generic;

namespace TakeSword
{
    class OffscreenSchedule : ISchedule
    {
        public void Add(IEvent event1, long delay)
        {

        }

        public bool RunFor(long deltaTime)
        {
            return true;
        }

        public bool RunOnce()
        {
            return true;
        }
    }
    internal class OffscreenLocation : ILocation
    {
        public ISchedule Schedule => new OffscreenSchedule();

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

        public IEnumerable<GameObject> NearbyObjects(long range)
        {
            return new List<GameObject>();
        }
    }
}