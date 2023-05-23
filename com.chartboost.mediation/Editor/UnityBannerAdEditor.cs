using System;
using System.Collections;
using System.Collections.Generic;
using Chartboost.Banner.Unity;
using UnityEditor;
using UnityEngine;

namespace Chartboost.Editor
{
    [CustomEditor(typeof(UnityBannerAd))]
    public class UnityBannerAdEditor : UnityEditor.Editor
    {
        private SerializedProperty _viusulaizeSP;
        private void OnEnable()
        {
            _viusulaizeSP = serializedObject.FindProperty("_visualize");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var unityBannerAd = target as UnityBannerAd;
            unityBannerAd.AdjustSize();

            unityBannerAd.Visualize = _viusulaizeSP.boolValue;
        }
    }
}
