using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TakeSword
{
    public interface IName
    {
        bool Matches(IGameObject viewer, string text);
        string GetName(IGameObject viewer);
        string NameWithArticle(IGameObject viewer);
    }

    public class Name : IName
    {
        private string displayName;
        private HashSet<string> synoyms;
        private bool isProper;
        public Name(string displayName, string otherSynonyms="", bool isProper = false)
        {
            this.displayName = displayName;
            IEnumerable<string> allSynonyms = Enumerable.Concat(
                displayName.Split(), otherSynonyms.Split()
            );
            synoyms = new HashSet<string>(allSynonyms);
            this.isProper = isProper;
        }
        public string NameWithArticle(IGameObject viewer)
        {
            if (isProper)
            {
                return displayName;
            }
            else
            {
                return "the " + displayName;
            }
        }

        public string GetName(IGameObject viewer)
        {
            return displayName;
        }

        public bool Matches(IGameObject viewer, string text)
        {
            string[] tokens = text.Split();
            return tokens.All((token) => synoyms.Contains(token));
        }
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

        public string NameWithArticle(IGameObject viewer)
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
