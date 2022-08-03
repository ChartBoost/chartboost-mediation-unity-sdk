package com.chartboost.heliumsdk.unity;

/**
 * Interface for sending real-time lifecycle events
 */
public interface ILifeCycleEventListener
{
    void DidStart(int errorCode, String errorDescription);
    void DidReceiveILRD(String impressionDataJson);
}
