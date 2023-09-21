#if UNITY_EDITOR
using System;
using Chartboost.AdFormats.Banner;
using Chartboost.AdFormats.Banner.Unity;
using Chartboost.Banner;
using Chartboost.Utilities;
using UnityEditor;
using UnityEngine;

namespace Chartboost.Editor
{
    [CustomEditor(typeof(ChartboostMediationUnityBannerAd))]
    internal class ChartboostMediationUnityBannerAdEditor : UnityEditor.Editor
    {
        private SerializedProperty _sizeTypeSP;
        private SerializedProperty _resizeOptionSP;
        private SerializedProperty _horizontalAlignmentSP;
        private SerializedProperty _verticalAlignmentSP;

        private ChartboostMediationBannerSizeType _sizeType = ChartboostMediationBannerSizeType.Standard;
        private ResizeOption _resizeOption = ResizeOption.NoResize;
        private ChartboostMediationBannerHorizontalAlignment _horizontalAlignment = ChartboostMediationBannerHorizontalAlignment.Center;
        private ChartboostMediationBannerVerticalAlignment _verticalAlignment = ChartboostMediationBannerVerticalAlignment.Center;

        private void OnEnable()
        {
            _sizeTypeSP = serializedObject.FindProperty("sizeType");
            _resizeOptionSP = serializedObject.FindProperty("resizeOption");
            _horizontalAlignmentSP = serializedObject.FindProperty("horizontalAlignment");
            _verticalAlignmentSP = serializedObject.FindProperty("verticalAlignment");

            _sizeType = (ChartboostMediationBannerSizeType)_sizeTypeSP.intValue;
            _resizeOption = (ResizeOption)_resizeOptionSP.intValue;
            _horizontalAlignment = (ChartboostMediationBannerHorizontalAlignment)_horizontalAlignmentSP.intValue;
            _verticalAlignment = (ChartboostMediationBannerVerticalAlignment)_verticalAlignmentSP.intValue;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            _sizeType = (ChartboostMediationBannerSizeType)EditorGUILayout.EnumPopup("Size", _sizeType);
            
            if (_sizeType == ChartboostMediationBannerSizeType.Adaptive)
            {
                _resizeOption = (ResizeOption) EditorGUILayout.EnumPopup("Resize", _resizeOption);
                _horizontalAlignment = (ChartboostMediationBannerHorizontalAlignment)EditorGUILayout.EnumPopup("Horizontal Alignment", _horizontalAlignment);
                _verticalAlignment = (ChartboostMediationBannerVerticalAlignment)EditorGUILayout.EnumPopup("Vertical Alignment", _verticalAlignment);
            }
            else
            {
                var unityBannerAd = target as ChartboostMediationUnityBannerAd;
                // ReSharper disable once PossibleNullReferenceException
                unityBannerAd.LockToFixedSize(_sizeType);
            }
            
            _sizeTypeSP.intValue = (int)_sizeType;
            _resizeOptionSP.intValue = (int)_resizeOption;
            _horizontalAlignmentSP.intValue = (int)_horizontalAlignment;
            _verticalAlignmentSP.intValue = (int)_verticalAlignment;
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif
