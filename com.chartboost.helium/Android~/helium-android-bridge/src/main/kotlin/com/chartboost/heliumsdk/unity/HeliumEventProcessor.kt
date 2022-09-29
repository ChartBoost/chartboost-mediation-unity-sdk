package com.chartboost.heliumsdk.unity

import android.util.Log
import com.chartboost.heliumsdk.ad.HeliumAdError
import org.json.JSONException
import org.json.JSONObject

object HeliumEventProcessor {
    private const val TAG = "HeliumEventProcessor"
    private const val EMPTY_STRING = ""

    @JvmStatic
    fun serializeHeliumEvent(
        placement: String?,
        heliumAdError: HeliumAdError?,
        eventConsumer: HeliumEventConsumer<String?, Int, String?>
    ) {
        var placementName: String? = EMPTY_STRING
        if (placement != null) placementName = placement
        var errorCode = -1
        var errorDescription = EMPTY_STRING
        if (heliumAdError != null) {
            val tempCode = heliumAdError.getCode()
            errorDescription = heliumAdError.getMessage()

            // we do this in order to bypass lint issues
            if (errorCode != tempCode) errorCode = tempCode
        }
        eventConsumer.accept(placementName, errorCode, errorDescription)
    }

    @JvmStatic
    fun serializeHeliumBidEvent(
        placement: String?,
        dataMap: HashMap<String, String>,
        eventConsumer: HeliumBidEventConsumer<String?, String?, String?, Double>
    ) {
        try {
            var placementName: String? = EMPTY_STRING
            if (placement != null) placementName = placement
            var partnerId = dataMap["partner_id"]
            partnerId = partnerId ?: EMPTY_STRING
            var auctionId = dataMap["auction-id"]
            auctionId = auctionId ?: EMPTY_STRING
            var priceAsString = dataMap["price"]
            priceAsString = priceAsString ?: "0"
            val price = priceAsString.toDouble()
            eventConsumer.accept(placementName, auctionId, partnerId, price)
        } catch (e: Exception) {
            Log.d(TAG, "bidFetchingInformationError", e)
        }
    }

    @JvmStatic
    fun serializeHeliumRewardEvent(
        placement: String?,
        reward: String?,
        eventConsumer: HeliumRewardEventConsumer<String?, Int>
    ) {
        var placementName: String? = EMPTY_STRING
        if (placement != null) placementName = placement
        try {
            var rewardAsInt = 0
            if (reward != null) rewardAsInt = reward.toInt()
            eventConsumer.accept(placementName, rewardAsInt)
        } catch (exception: Exception) {
            Log.e(TAG, "Failed to Parse Reward Information: Reward: $reward")
            eventConsumer.accept(placementName, 1)
        }
    }

    @JvmStatic
    fun serializePlacementILRDData(placementName: String?, ilrdInfo: JSONObject?): String {
        val serializedString = JSONObject()
        try {
            serializedString.put("placementName", placementName)
            serializedString.put("ilrd", ilrdInfo)
        } catch (e: JSONException) {
            Log.d(TAG, "serializeError", e)
        }
        return serializedString.toString()
    }

    fun interface HeliumEventConsumer<T, V, S> {
        fun accept(placementName: T, errorCode: V, errorDescription: S)
    }

    fun interface HeliumBidEventConsumer<T, V, S, X> {
        fun accept(placementName: T, auctionId: V, partnerId: S, price: X)
    }

    fun interface HeliumRewardEventConsumer<T, V> {
        fun accept(placementName: T, reward: V)
    }
}
