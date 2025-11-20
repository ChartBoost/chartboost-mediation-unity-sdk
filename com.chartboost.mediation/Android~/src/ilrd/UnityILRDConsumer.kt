@file:Suppress("PackageDirectoryMismatch")
package com.chartboost.mediation.unity.ilrd

/**
 * Consumer interface for handling ILRD (Impression Level Revenue Data) events from the native layer.
 *
 * This interface is implemented by the Unity C# layer (via bridge) to receive impression
 * events captured by the Chartboost Mediation SDK. The consumer must call the completer
 * callback when processing is finished to allow the native layer to cleanup cached data.
 *
 * @see UnityILRDCompleter
 * @see UnityILRDObserver
 */
interface UnityILRDConsumer {
    /**
     * Called when a new impression event is available for Unity to process.
     *
     * Implementations should process the impression data and invoke [completer.completed]
     * when finished to signal that the data can be removed from the native cache.
     *
     * @param uniqueId Unique identifier (hash code) for this impression event
     * @param ilrdJson JSON string containing impression data including placement and ILRD info
     * @param completer Callback to invoke when processing is complete
     */
    fun onImpression(uniqueId: Int, ilrdJson: String, completer: UnityILRDCompleter)
}
