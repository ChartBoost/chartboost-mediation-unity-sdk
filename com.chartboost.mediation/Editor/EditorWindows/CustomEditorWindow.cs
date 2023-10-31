using UnityEditor;

namespace Chartboost.Editor.EditorWindows
{
    public interface ICustomWindow
    {
        void SetInstance(object instance);
    }

    public abstract class CustomEditorWindow<T> : EditorWindow, ICustomWindow where T : EditorWindow
    {
        protected static T Instance;


        public void SetInstance(object instance)
        {
            Instance = instance as T;
        }
    }
}
