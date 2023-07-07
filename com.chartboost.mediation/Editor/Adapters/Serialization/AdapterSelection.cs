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
        /// <summary>
        /// string defining adapter id.
        /// </summary>
        public string id;
        
        /// <summary>
        /// Current Android adapter version selection.
        /// </summary>
        public string android = AdapterWindowConstants.Unselected;
        
        /// <summary>
        /// Current IOS adapter version selection.
        /// </summary>
        public string ios = AdapterWindowConstants.Unselected;
            
        #nullable enable
        /// <summary>
        /// AndroidDropdown for Android Versions.
        /// </summary>
        [NonSerialized]
        public ToolbarMenu? androidDropdown;
        
        /// <summary>
        /// IOSDropdown for IOS Versions.
        /// </summary>
        [NonSerialized]
        public ToolbarMenu? iosDropdown;
        #nullable disable

        public AdapterSelection(string id) => this.id = id;
    }
}
