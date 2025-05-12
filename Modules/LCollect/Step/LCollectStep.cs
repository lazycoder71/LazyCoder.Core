using System;
using System.Text.RegularExpressions;

namespace LFramework
{
    [Serializable]
    public abstract class LCollectStep
    {
        [Serializable]
        public enum AddType
        {
            Append = 0,
            Join = 1,
            Insert = 2,
        }

        public virtual string DisplayName { get { return Regex.Replace(ToString().Replace(typeof(LCollectStep).ToString(), ""), "(?<!^)([A-Z])", " $1"); } }

        public abstract void Apply(LCollectItem item);
    }
}
