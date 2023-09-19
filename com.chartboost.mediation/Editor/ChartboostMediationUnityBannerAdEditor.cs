#if UNITY_EDITOR


using System;
using Chartboost.AdFormats.Banner;
using Chartboost.AdFormats.Banner.Unity;
using Chartboost.Banner;
using Chartboost.Utilities;
using UnityEditor;
using UnityEngine;
using static Chartboost.Utilities.Constants;

namespace Chartboost.Editor
{
    [CustomEditor(typeof(ChartboostMediationUnityBannerAd))]
    internal class ChartboostMediationUnityBannerAdEditor : UnityEditor.Editor
    {
        private SerializedProperty _sizeNameSP;
        private SerializedProperty _horizontalAlignmentSP;
        private SerializedProperty _verticalAlignmentSP;
        private SerializedProperty _resizeToFitSP;
        
        private bool _resizeToFit;
        private ChartboostMediationBannerName _sizeName = ChartboostMediationBannerName.Standard;
        private ChartboostMediationBannerHorizontalAlignment _horizontalAlignment = ChartboostMediationBannerHorizontalAlignment.Center;
        private ChartboostMediationBannerVerticalAlignment _verticalAlignment = ChartboostMediationBannerVerticalAlignment.Center;

        private void OnEnable()
        {
            _sizeNameSP = serializedObject.FindProperty("sizeName");
            _horizontalAlignmentSP = serializedObject.FindProperty("horizontalAlignment");
            _verticalAlignmentSP = serializedObject.FindProperty("verticalAlignment");
            _resizeToFitSP = serializedObject.FindProperty("resizeToFit");

            _sizeName = (ChartboostMediationBannerName)_sizeNameSP.intValue;
            _resizeToFit = _resizeToFitSP.boolValue;
            _horizontalAlignment = (ChartboostMediationBannerHorizontalAlignment)_horizontalAlignmentSP.intValue;
            _verticalAlignment = (ChartboostMediationBannerVerticalAlignment)_verticalAlignmentSP.intValue;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            _sizeName = (ChartboostMediationBannerName)EditorGUILayout.EnumPopup("Size", _sizeName);
            
            if (_sizeName == (int)ChartboostMediationBannerName.Adaptive)
            {
                _resizeToFit = EditorGUILayout.Toggle("Resize To Fit", _resizeToFit);
                
                if (!_resizeToFitSP.boolValue)
                {
                    _horizontalAlignment = (ChartboostMediationBannerHorizontalAlignment)EditorGUILayout.EnumPopup("Horizontal Alignment", _horizontalAlignment);
                    _verticalAlignment = (ChartboostMediationBannerVerticalAlignment)EditorGUILayout.EnumPopup("Vertical Alignment", _verticalAlignment);
                }
            }
            else
            {
                var unityBannerAd = target as ChartboostMediationUnityBannerAd;
                unityBannerAd.LockToFixedSize(_sizeName);
            }
            
            _sizeNameSP.intValue = (int)_sizeName;
            _resizeToFitSP.boolValue = _resizeToFit;
            _horizontalAlignmentSP.intValue = (int)_horizontalAlignment;
            _verticalAlignmentSP.intValue = (int)_verticalAlignment;
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif
