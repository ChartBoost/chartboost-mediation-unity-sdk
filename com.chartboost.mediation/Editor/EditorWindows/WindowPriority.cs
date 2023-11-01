using System;

namespace Chartboost.Editor.EditorWindows
{
    internal struct WindowPriority
    {
        public readonly Type Type;

        public readonly int Priority;

        public WindowPriority(Type t, int priority)
        {
            Type = t;
            Priority = priority;
        }
    }
}
