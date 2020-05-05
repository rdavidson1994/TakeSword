namespace TakeSword
{
    public class StationaryEnemyAI : ActivityRoutine<PhysicalActor>
    {
        public StationaryEnemyAI(PhysicalActor actor)
        {
            Actor = actor;
        }
        public override IActivity<PhysicalActor> NextActivity()
        {
            foreach (GameObject thing in Actor.ItemsInReach())
            {
                if (thing.Is<Player>())
                {
                    return Do<BestWeaponStrike>(thing);
                }
            }
            return Do<WaitAction>();
        }
    }
}