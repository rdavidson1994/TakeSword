using System;
using System.Collections.Generic;
using System.Text;

namespace TakeSword
{
    public enum Direction
    {
        North = 1,
        South = -1,
        East = 2,
        West = -2,
        Up = 3,
        Down = -3
    }
    public static class DirectionConverter
    {
        public static Direction Opposite(Direction direction)
        {
            return (Direction)(-(int)direction);
        }
    }

    public class PortalVertex : GameObject
    {
        public Direction Direction { get; set; }
        private PortalEdge edge;
        public PortalVertex(PortalEdge edge)
        {
            this.edge = edge;
        }
        public PortalVertex Opposite()
        {
            return edge.Opposite(this);
        }
    }

    public class PortalEdge
    {
        public PortalVertex First { get; set; }
        public PortalVertex Second { get; set; }
        public PortalVertex Opposite(PortalVertex vertex)
        {
            if (vertex == First)
            {
                return Second;
            }
            else if (vertex == Second)
            {
                return First;
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }

    public static class PortalFactory
    {
        public static PortalEdge CreatePortal()
        {
            PortalEdge portal = new PortalEdge();
            portal.First = new PortalVertex(portal);
            portal.Second = new PortalVertex(portal);
            return portal;
        }
        /*
        public PortalEdge CreatePortal<T1, T2>(ILocation loc1, ILocation loc2)
            where T1 : PortalVertex, new() where T2 : PortalVertex, new()
        {
            PortalEdge outEdge = new PortalEdge();
            outEdge.First = new T1(outEdge);
        }*/
    }
}
