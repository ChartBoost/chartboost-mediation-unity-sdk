package com.chartboost.mediation.canary.unity

import android.widget.Toast
import com.unity3d.player.UnityPlayer

class ToastManager {

    companion object {
        @JvmStatic
        fun showToast(message: String) {
            UnityPlayer.currentActivity?.let {
                Toast.makeText(it, message, Toast.LENGTH_SHORT).show()
            }
        }
    }
}
