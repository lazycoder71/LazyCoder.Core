using System.Collections.Generic;

namespace LazyCoder
{
    public static class LCollectDestinationHelper
    {
        private static Dictionary<LCollectConfig, Stack<LCollectDestination>> s_destinationDict;

        public static void Push(LCollectDestination destination)
        {
            if (s_destinationDict == null)
                s_destinationDict = new Dictionary<LCollectConfig, Stack<LCollectDestination>>();

            if (!s_destinationDict.ContainsKey(destination.Config))
                s_destinationDict.Add(destination.Config, new Stack<LCollectDestination>());

            s_destinationDict[destination.Config].Push(destination);
        }

        public static void Pop(LCollectDestination destination)
        {
            if (s_destinationDict == null)
                return;

            Stack<LCollectDestination> stack;

            s_destinationDict.TryGetValue(destination.Config, out stack);

            if (stack == null || stack.Count == 0)
                return;

            stack.Pop();
        }

        public static LCollectDestination Get(LCollectConfig config)
        {
            if (s_destinationDict == null)
                return null;

            if (!s_destinationDict.ContainsKey(config))
                return null;

            Stack<LCollectDestination> stack = s_destinationDict[config];

            if(stack == null || stack.Count == 0) 
                return null;

            return stack.Peek();
        }
    }
}
