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
        IName Extend(Func<string, string> displayTransform, params string[] extraSynonyms);
    }

    public static class ExtensionsForIName
    {
        public static IName Possessive(this IName name, string objectName)
        {
            Func<string, string> displayTransform = (str) =>
                $"{str}'s {objectName}";
            return name.Extend(displayTransform, objectName);
        }
    }

    public class Name : IName
    {
        private string displayName;
        private HashSet<string> synonyms;
        private bool isProper;
        public Name(string displayName, string otherSynonyms="", bool isProper = false)
        {
            this.displayName = displayName;
            IEnumerable<string> allSynonyms = Enumerable.Concat(
                displayName.Split(), otherSynonyms.Split()
            );
            synonyms = new HashSet<string>(allSynonyms);
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
            return tokens.All((token) => synonyms.Contains(token));
        }

        public IName PlusSuffix(FormattableString suffix)
        {
            Name output = (Name)MemberwiseClone();
            output.synonyms.Add(suffix.ToString());
            output.displayName += suffix;
            return output;
        }

        public IName Extend(Func<string, string> displayTransform, params string[] extraSynonyms)
        {
            Name output = (Name)MemberwiseClone();
            output.synonyms.UnionWith(extraSynonyms);
            output.displayName = displayTransform(displayName);
            return output;
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

        public IName Extend(Func<string, string> displayTransform, params string[] extraSynonyms)
        {
            Name output = new Name(name, "", isProper);
            return output.Extend(displayTransform, extraSynonyms);
        }
    }
}
