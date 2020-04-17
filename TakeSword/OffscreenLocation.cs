using System;
using System.Collections.Generic;

namespace TakeSword
{
    public class OffscreenSchedule : ISchedule
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
    public class OffscreenLocation : ILocation
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

        public FormattableString DescriptionForInhabitant(GameObject viewer)
        {
            return $"If you're seeing this text, it means your character got unloaded from the game somehow. :( Sorry!";
        }

        public void HandleAnnouncement(ActionAnnouncement announcement)
        {
            // do nothing
        }

        public IEnumerable<GameObject> NearbyObjects(long range)
        {
            return new List<GameObject>();
        }
    }
}