@file:Suppress("PackageDirectoryMismatch")
package com.chartboost.mediation.unity.ilrd

interface UnityILRDConsumer {
    fun onImpression(uniqueId: Int, ilrdJson: String, completer: UnityILRDCompleter)
}
