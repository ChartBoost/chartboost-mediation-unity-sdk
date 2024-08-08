using Chartboost.Mediation.Ad.Banner.Enums;
using Chartboost.Mediation.Ad.Banner.Unity;
using UnityEditor;

namespace Chartboost.Mediation.Editor.CustomEditors
{
    [CustomEditor(typeof(UnityBannerAd))]
    internal class UnityBannerAdEditor : UnityEditor.Editor
    {
        private const string MenuItemUnityBannerAd = "GameObject/Chartboost Mediation/UnityBannerAd";

        [MenuItem(MenuItemUnityBannerAd)]
        public static void CreateAd()
        {
            UnityBannerAd.InstantiateUnityBannerAd(Selection.activeTransform);
        }
        
        private SerializedProperty _horizontalAlignmentSP;
        private SerializedProperty _verticalAlignmentSP;

        private BannerHorizontalAlignment _horizontalAlignment = BannerHorizontalAlignment.Center;
        private BannerVerticalAlignment _verticalAlignment = BannerVerticalAlignment.Center;

        private void OnEnable()
        {
            _horizontalAlignmentSP = serializedObject.FindProperty("horizontalAlignment");
            _verticalAlignmentSP = serializedObject.FindProperty("verticalAlignment");

            _horizontalAlignment = (BannerHorizontalAlignment)_horizontalAlignmentSP.intValue;
            _verticalAlignment = (BannerVerticalAlignment)_verticalAlignmentSP.intValue;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            _horizontalAlignment = (BannerHorizontalAlignment)EditorGUILayout.EnumPopup("Horizontal Alignment", _horizontalAlignment);
            _verticalAlignment = (BannerVerticalAlignment)EditorGUILayout.EnumPopup("Vertical Alignment", _verticalAlignment);
            
            _horizontalAlignmentSP.intValue = (int)_horizontalAlignment;
            _verticalAlignmentSP.intValue = (int)_verticalAlignment;
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
