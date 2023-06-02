package com.chartboost.mediation.unity

import android.util.Log
import com.chartboost.heliumsdk.domain.ChartboostMediationAdException

@Deprecated("EventProcessor utilizes deprecated APIs and will be removed in the future")
object EventProcessor {
    private val TAG = EventProcessor::class.java.simpleName

    private fun getAuctionData(key: String, data: Map<String, String>?, defaultValue: String = ""): String {
        return data?.get(key) ?: defaultValue
    }

    @JvmStatic
    fun serializeEvent(placementName: String, eventConsumer: EventConsumer<String>)
        = eventConsumer.accept(placementName)

    @JvmStatic
    fun serializeEventWithException(placementName: String, error: ChartboostMediationAdException?, eventConsumer: EventWithErrorConsumer<String, String>)
        = eventConsumer.accept(placementName,error?.toString() ?: "")

    @JvmStatic
    fun serializeLoadEvent(placementName: String, loadId: String, data: Map<String, String>, error: ChartboostMediationAdException?,
                           loadConsumer: LoadEventConsumer<String, String, String, String, Double, String, String>) {
        val errorMessage = error?.toString() ?: ""

        val partnerId =  getAuctionData("partner_id", data)
        val auctionId = getAuctionData("auction-id", data)
        val lineItemId = getAuctionData("line_item_id", data)

        val price = try {
            data["price"]?.toDouble() ?: 0.0
        } catch (e: NumberFormatException) {
            Log.d(TAG, "HeliumBidEvent failed to serialize price, defaulting to 0.0", e)
            0.0
        }

        loadConsumer.accept(placementName, loadId, auctionId, partnerId, price, lineItemId, errorMessage)
    }

    fun interface EventConsumer<PlacementName>{
        fun accept(placementName: PlacementName)
    }

    fun interface EventWithErrorConsumer<PlacementName, ErrorMessage> {
        fun accept(placementName: PlacementName, errorMessage: ErrorMessage)
    }

    fun interface LoadEventConsumer<PlacementName, LoadId, AuctionId, PartnerId, Price, LineItemId, Error> {
        fun accept(placementName: PlacementName, loadId: LoadId, auctionId: AuctionId, partnerId: PartnerId, price: Price, lineItemId: LineItemId, error: Error)
    }
}
