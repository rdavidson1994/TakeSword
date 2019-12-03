using System;
using System.Collections.Generic;
using System.Text;

namespace TakeSword
{
    public interface IName
    {
        bool Matches(IGameObject viewer, string text);
        string GetName(IGameObject viewer);
        string DisplayName(IGameObject viewer);
    }
    public class SimpleName : IName
    {
        private bool isProper;
        private string name;
        public SimpleName(string name, bool isProper=false)
        {
            this.name = name;
            this.isProper = isProper;
        }

        public string DisplayName(IGameObject viewer)
        {
            if (isProper)
            {
                return name;
            }
            else
            {
                return "the " + name;
            }
        }

        public string GetName(IGameObject viewer)
        {
            return name;
        }

        public bool Matches(IGameObject viewer, string text)
        {
            return text == name;
        }
    }
}
