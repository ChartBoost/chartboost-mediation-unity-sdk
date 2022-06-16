package com.chartboost.heliumsdk.unity;

/**
 * Interface for sending real-time background events while the Unity Player is paused (which will happen when
 * a fullscreen ad is being displayed).
 */
public interface IBackgroundEventListener
{
    void onBackgroundEvent(String event, String json);
}
