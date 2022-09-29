package com.chartboost.heliumsdk.unity

/**
 * Interface for sending real-time lifecycle events
 */
interface ILifeCycleEventListener {
    fun DidStart(errorCode: Int, errorDescription: String?)
    fun DidReceiveILRD(impressionDataJson: String?)
}
