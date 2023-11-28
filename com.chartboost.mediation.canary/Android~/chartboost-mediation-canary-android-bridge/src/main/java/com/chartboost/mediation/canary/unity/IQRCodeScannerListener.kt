package com.chartboost.mediation.canary.unity

interface IQRCodeScannerListener {
    fun QrCodeScannerDidScan(appId: String, appSignature: String)
}