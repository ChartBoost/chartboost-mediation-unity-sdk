using System;
using UnityEditor.UIElements;

namespace Chartboost.Editor.Adapters.Serialization
{
    /// <summary>
    /// Contains adapter selected versions in the project, used to update, modify, existing selections. 
    /// </summary>
    [Serializable]
    public class AdapterSelection
    {
        public string id;
        public string android = Constants.Unselected;
        public string ios = Constants.Unselected;
            
        [NonSerialized]
        public ToolbarMenu androidDropdown;
        [NonSerialized]
        public ToolbarMenu iosDropdown;

        public AdapterSelection(string id)
        {
            this.id = id;
        }
    }
}
