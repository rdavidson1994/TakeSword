using System;
using System.Collections.Generic;
using System.Text;

namespace TakeSword
{
    public interface IName
    {
        bool Matches(IGameObject viewer);
        string GetText(IGameObject viewer);
    }
    public class SimpleName : IName
    {
        private string name;
        public SimpleName(string name)
        {
            this.name = name;
        }
        public string GetText(IGameObject viewer)
        {
            return name;
        }

        public bool Matches(IGameObject viewer)
        {
            throw new NotImplementedException();
        }
    }
}
