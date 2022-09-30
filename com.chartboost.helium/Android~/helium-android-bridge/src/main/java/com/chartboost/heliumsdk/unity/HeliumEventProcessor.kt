package com.chartboost.heliumsdk.unity

import android.util.Log
import com.chartboost.heliumsdk.ad.HeliumAdError
import org.json.JSONException
import org.json.JSONObject

object HeliumEventProcessor {
    private const val TAG = "HeliumEventProcessor"

    @JvmStatic
    fun serializeHeliumEvent(
        placementName: String,
        heliumAdError: HeliumAdError?,
        eventConsumer: HeliumEventConsumer<String, Int, String>
    ) {
        eventConsumer.accept(placementName, heliumAdError?.getCode() ?: -1, heliumAdError?.getMessage() ?: "")
    }

    @JvmStatic
    fun serializeHeliumBidEvent(
        placementName: String,
        dataMap: HashMap<String, String>,
        eventConsumer: HeliumBidEventConsumer<String, String, String, Double>
    ) {
        val partnerId = dataMap["partner_id"] ?: ""
        val auctionId = dataMap["auction-id"] ?: ""
        val priceAsString = dataMap["price"] ?: "0"
        var price = 0.0
        try {
            price = priceAsString.toDouble()
        } catch (e: NumberFormatException) {
            Log.d(TAG, "bidFetchingInformationError", e)
        }
        eventConsumer.accept(placementName, auctionId, partnerId, price)
    }

    @JvmStatic
    fun serializeHeliumRewardEvent(
        placementName: String,
        reward: String,
        eventConsumer: HeliumRewardEventConsumer<String, Int>
    ) {
        var rewardAsInt = 1
        // some rewards are coming as JSON or other values not pure numbers, so this is in place to catch such scenarios
        try {
            rewardAsInt = reward.toInt()
        } catch (exception: NumberFormatException) {
            Log.e(TAG, "Failed to Parse Reward Information: Reward: $reward")
        }
        eventConsumer.accept(placementName, rewardAsInt)
    }

    @JvmStatic
    fun serializePlacementILRDData(placementName: String?, ilrdInfo: JSONObject?): String {
        val serializedString = JSONObject()
        try {
            return serializedString.apply {
                put("placementName", placementName)
                put("ilrd", ilrdInfo)
            }.toString()
        } catch (e: JSONException) {
            Log.d(TAG, "serializeError", e)
        }
        return serializedString.toString()
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
