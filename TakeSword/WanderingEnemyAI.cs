using System.Linq;

namespace TakeSword
{
    public class WanderingEnemyAI : ActivityRoutine<PhysicalActor>
    {
        public WanderingEnemyAI(PhysicalActor actor)
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
            Portal? portal = Actor.ItemsInReach()
                .OfType<Portal>()
                .RandomChoice();
            if (portal != null)
            {
                var act = Do<Enter>(portal);
                if (act.IsValid()) return act;
            }
            return Do<WaitAction>();
        }
    }
}