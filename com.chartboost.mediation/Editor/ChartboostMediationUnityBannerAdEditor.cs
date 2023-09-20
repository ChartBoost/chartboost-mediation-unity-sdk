#if UNITY_EDITOR
using System;
using Chartboost.AdFormats.Banner;
using Chartboost.AdFormats.Banner.Unity;
using Chartboost.Utilities;
using UnityEditor;
using UnityEngine;

namespace Chartboost.Editor
{
    [CustomEditor(typeof(ChartboostMediationUnityBannerAd))]
    internal class ChartboostMediationUnityBannerAdEditor : UnityEditor.Editor
    {
        private SerializedProperty _sizeNameSP;
        private SerializedProperty _horizontalAlignmentSP;
        private SerializedProperty _verticalAlignmentSP;
        private SerializedProperty _resizeToFitSP;
        private SerializedProperty _resizeAxisSP;
        
        private bool _resizeToFit;
        private ChartboostMediationBannerResizeAxis _resizeAxis = ChartboostMediationBannerResizeAxis.Both;
        private ChartboostMediationBannerSizeType _sizeType = ChartboostMediationBannerSizeType.Standard;
        private ChartboostMediationBannerHorizontalAlignment _horizontalAlignment = ChartboostMediationBannerHorizontalAlignment.Center;
        private ChartboostMediationBannerVerticalAlignment _verticalAlignment = ChartboostMediationBannerVerticalAlignment.Center;

        private void OnEnable()
        {
            _sizeNameSP = serializedObject.FindProperty("sizeType");
            _horizontalAlignmentSP = serializedObject.FindProperty("horizontalAlignment");
            _verticalAlignmentSP = serializedObject.FindProperty("verticalAlignment");
            _resizeToFitSP = serializedObject.FindProperty("resizeToFit");
            _resizeAxisSP = serializedObject.FindProperty("resizeAxis");

            _sizeType = (ChartboostMediationBannerSizeType)_sizeNameSP.intValue;
            _resizeToFit = _resizeToFitSP.boolValue;
            _resizeAxis = (ChartboostMediationBannerResizeAxis)_resizeAxisSP.intValue;
            _horizontalAlignment = (ChartboostMediationBannerHorizontalAlignment)_horizontalAlignmentSP.intValue;
            _verticalAlignment = (ChartboostMediationBannerVerticalAlignment)_verticalAlignmentSP.intValue;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            _sizeType = (ChartboostMediationBannerSizeType)EditorGUILayout.EnumPopup("Size", _sizeType);
            
            if (_sizeType == (int)ChartboostMediationBannerSizeType.Adaptive)
            {
                _resizeToFit = EditorGUILayout.Toggle("Resize To Fit", _resizeToFit);
                
                if (_resizeToFitSP.boolValue)
                {
                    _resizeAxis = (ChartboostMediationBannerResizeAxis)EditorGUILayout.EnumPopup("Resize Axis", _resizeAxis);
                    switch (_resizeAxis)
                    {
                        case ChartboostMediationBannerResizeAxis.Horizontal:
                            _verticalAlignment = (ChartboostMediationBannerVerticalAlignment)EditorGUILayout.EnumPopup("Vertical Alignment", _verticalAlignment);
                            break;
                        case ChartboostMediationBannerResizeAxis.Vertical:
                            _horizontalAlignment = (ChartboostMediationBannerHorizontalAlignment)EditorGUILayout.EnumPopup("Horizontal Alignment", _horizontalAlignment);
                            break;
                    }
                }
                else
                {
                    _horizontalAlignment = (ChartboostMediationBannerHorizontalAlignment)EditorGUILayout.EnumPopup("Horizontal Alignment", _horizontalAlignment);
                    _verticalAlignment = (ChartboostMediationBannerVerticalAlignment)EditorGUILayout.EnumPopup("Vertical Alignment", _verticalAlignment);
                }
            }
            else
            {
                var unityBannerAd = target as ChartboostMediationUnityBannerAd;
                unityBannerAd.LockToFixedSize(_sizeType);
            }
            
            _sizeNameSP.intValue = (int)_sizeType;
            _resizeToFitSP.boolValue = _resizeToFit;
            _resizeAxisSP.intValue = (int)_resizeAxis;
            _horizontalAlignmentSP.intValue = (int)_horizontalAlignment;
            _verticalAlignmentSP.intValue = (int)_verticalAlignment;
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif
