#if UNITY_EDITOR


using System;
using Chartboost.AdFormats.Banner;
using Chartboost.AdFormats.Banner.Unity;
using Chartboost.Utilities;
using UnityEditor;
using UnityEngine;
using static Chartboost.Utilities.Constants;

namespace Chartboost.Editor
{
    [CustomEditor(typeof(ChartboostMediationUnityBannerAd))]
    internal class ChartboostMediationUnityBannerAdEditor : UnityEditor.Editor
    {
        private SerializedProperty _sizeSP;
        private SerializedProperty _horizontalAlignmentSP;
        private SerializedProperty _verticalAlignmentSP;
        private SerializedProperty _resizeToFitSP;
        
        private UnityBannerAdSize _size = UnityBannerAdSize.Standard;
        private bool _resizeToFit;
        private ChartboostMediationBannerHorizontalAlignment _horizontalAlignment = ChartboostMediationBannerHorizontalAlignment.Center;
        private ChartboostMediationBannerVerticalAlignment _verticalAlignment = ChartboostMediationBannerVerticalAlignment.Center;

        private void OnEnable()
        {
            _sizeSP = serializedObject.FindProperty("size");
            _horizontalAlignmentSP = serializedObject.FindProperty("horizontalAlignment");
            _verticalAlignmentSP = serializedObject.FindProperty("verticalAlignment");
            _resizeToFitSP = serializedObject.FindProperty("resizeToFit");

            _size = (UnityBannerAdSize)_sizeSP.intValue;
            _resizeToFit = _resizeToFitSP.boolValue;
            _horizontalAlignment = (ChartboostMediationBannerHorizontalAlignment)_horizontalAlignmentSP.intValue;
            _verticalAlignment = (ChartboostMediationBannerVerticalAlignment)_verticalAlignmentSP.intValue;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            _size = (UnityBannerAdSize)EditorGUILayout.EnumPopup("Size", _size);
            
            if (_size == (int)UnityBannerAdSize.Adaptive)
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
                AdjustSize();
            }
            
            _sizeSP.intValue = (int)_size;
            _resizeToFitSP.boolValue = _resizeToFit;
            _horizontalAlignmentSP.intValue = (int)_horizontalAlignment;
            _verticalAlignmentSP.intValue = (int)_verticalAlignment;
            
            serializedObject.ApplyModifiedProperties();
        }

        private void AdjustSize()
        {
            var unityBannerAd = target as ChartboostMediationUnityBannerAd;
            // ReSharper disable once PossibleNullReferenceException
            var canvas =  unityBannerAd.GetComponentInParent<Canvas>();
            var rect = unityBannerAd.GetComponent<RectTransform>();
            
            var canvasScale = canvas.transform.localScale.x;
            float width;
            float height;

            switch (_size)
            {
                case UnityBannerAdSize.Adaptive: 
                    return;
                case UnityBannerAdSize.Standard: 
                    width = ChartboostMediationConverters.NativeToPixels(BannerSize.STANDARD.Item1)/canvasScale;
                    height = ChartboostMediationConverters.NativeToPixels(BannerSize.STANDARD.Item2)/canvasScale;
                    break;
                case UnityBannerAdSize.Medium:
                    width = ChartboostMediationConverters.NativeToPixels(BannerSize.MEDIUM.Item1)/canvasScale;
                    height = ChartboostMediationConverters.NativeToPixels(BannerSize.MEDIUM.Item2)/canvasScale;
                    break;
                case UnityBannerAdSize.Leaderboard:
                    width = ChartboostMediationConverters.NativeToPixels(BannerSize.LEADERBOARD.Item1)/canvasScale;
                    height = ChartboostMediationConverters.NativeToPixels(BannerSize.LEADERBOARD.Item2)/canvasScale;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var temp = unityBannerAd.transform.position;
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            unityBannerAd.transform.position = temp;
        }
    }
}

#endif
