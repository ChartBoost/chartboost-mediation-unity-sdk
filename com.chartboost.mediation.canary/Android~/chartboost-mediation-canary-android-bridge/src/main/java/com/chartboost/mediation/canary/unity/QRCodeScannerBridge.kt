package com.chartboost.mediation.canary.unity

import com.chartboost.mediation.canary.unity.IQRCodeScannerListener
import com.chartboost.mediation.canary.unity.QRCodeScannerActivity
import com.unity3d.player.UnityPlayer
import android.content.BroadcastReceiver
import android.content.Context
import android.content.Intent
import android.content.IntentFilter
import android.widget.Toast
import androidx.localbroadcastmanager.content.LocalBroadcastManager
import java.net.URI

class QRCodeScannerBridge {
    companion object {
        val ACTION_QR_CODE_SCANNED = "ACTION_QR_CODE_SCANNED"
        private var qrCodeScannerListener: IQRCodeScannerListener? = null

        @JvmStatic
        fun presentQRCodeScanner(listener: IQRCodeScannerListener) {
            UnityPlayer.currentActivity?.let {
                qrCodeScannerListener = listener

                LocalBroadcastManager.getInstance(it)
                    .registerReceiver(messageReceiver, IntentFilter(ACTION_QR_CODE_SCANNED))

                val intent = Intent(it, QRCodeScannerActivity::class.java)
                it.startActivity(intent)
            }
        }

        private var messageReceiver = object : BroadcastReceiver() {
            override fun onReceive(context: Context, intent: Intent) {
                when (intent.action) {
                    ACTION_QR_CODE_SCANNED -> handleQrCodeScannedAction(context, intent)
                }
            }
        }

        private fun handleQrCodeScannedAction(context: Context, intent: Intent) {
            val message: String? = intent.getStringExtra("message")
            val uri = URI(message)
            val appId = uri.findParameterValue("appId") ?: ""
            val appSignature = uri.findParameterValue("appSignature") ?: ""
            if (!appId.isEmpty() && !appSignature.isEmpty()) {
                Toast.makeText(
                    context,
                    "appId: $appId, appSignature: $appSignature",
                    Toast.LENGTH_SHORT
                ).show()
                UnityPlayer.currentActivity?.let {
                    it.runOnUiThread {
                        qrCodeScannerListener?.QrCodeScannerDidScan(appId, appSignature)
                    }
                }
            }

            qrCodeScannerListener = null
            UnityPlayer.currentActivity?.let {
                LocalBroadcastManager.getInstance(it)
                    .unregisterReceiver(messageReceiver)
            }
        }

        private fun URI.findParameterValue(parameterName: String): String? {
            return rawQuery.split('&').map {
                val parts = it.split('=')
                val name = parts.firstOrNull() ?: ""
                val value = parts.drop(1).firstOrNull() ?: ""
                Pair(name, value)
            }.firstOrNull { it.first == parameterName }?.second
        }
    }
}
