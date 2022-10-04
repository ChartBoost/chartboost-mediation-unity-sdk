package com.chartboost.heliumsdk.unity

import android.util.Log
import com.chartboost.heliumsdk.ad.HeliumAdError
import org.json.JSONException
import org.json.JSONObject

object HeliumEventProcessor {
    private val TAG = HeliumEventProcessor::class.java.simpleName

    @JvmStatic
    fun serializeHeliumEvent(
        placementName: String,
        error: HeliumAdError?,
        eventConsumer: HeliumEventConsumer<String, Int, String>
    ) {
        eventConsumer.accept(placementName, error?.getCode() ?: -1, error?.getMessage() ?: "")
    }

    @JvmStatic
    fun serializeHeliumBidEvent(
        placementName: String,
        data: HashMap<String, String>,
        eventConsumer: HeliumBidEventConsumer<String, String, String, Double>
    ) {
        val partnerId = data["partner_id"] ?: ""
        val auctionId = data["auction-id"] ?: ""
        val price = try {
            data["price"]?.toDouble() ?: 0.0
        } catch (e: NumberFormatException) {
            Log.d(TAG, "HeliumBidEvent failed to serialize price, defaulting to 0.0", e)
            0.0
        }
        eventConsumer.accept(placementName, auctionId, partnerId, price)
    }

    @JvmStatic
    fun serializeHeliumRewardEvent(
        placementName: String,
        reward: String,
        eventConsumer: HeliumRewardEventConsumer<String, Int>
    ) {
        val rewardAmount = try {
            reward.toInt()
        } catch (e: NumberFormatException) {
            Log.d(TAG, "HeliumRewardEvent failed to serialize reward amount, defaulting", e)
            1
        }
        eventConsumer.accept(placementName, rewardAmount)
    }

    @JvmStatic
    fun serializePlacementIlrdData(placementName: String, ilrdInfo: JSONObject?): String {
        return try {
            JSONObject().apply {
                put("placementName", placementName)
                put("ilrd", ilrdInfo)
            }.toString()
        } catch (e: JSONException) {
            Log.d(TAG, "serializeError", e)
            ""
        }
    }

    fun interface HeliumEventConsumer<PlacementName, ErrorCode, ErrorDescription> {
        fun accept(placementName: PlacementName, errorCode: ErrorCode, errorDescription: ErrorDescription)
    }

    fun interface HeliumBidEventConsumer<PlacementName, AuctionId, PartnerId, Price> {
        fun accept(placementName: PlacementName, auctionId: AuctionId, partnerId: PartnerId, price: Price)
    }

    fun interface HeliumRewardEventConsumer<PlacementName, Reward> {
        fun accept(placementName: PlacementName, reward: Reward)
    }
}
