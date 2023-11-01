using System;

namespace Chartboost.Editor.EditorWindows
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class CustomEditorWindowAttribute : Attribute
    {
        public readonly Type Type;
        public readonly int Priority;
        
        public CustomEditorWindowAttribute(Type t, int priority)
        {
            Type = t;
            Priority = priority;
        }
    }
}
