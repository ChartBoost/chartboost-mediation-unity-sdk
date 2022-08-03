package com.chartboost.heliumsdk.unity;

import android.util.Log;
import com.chartboost.heliumsdk.ad.HeliumAdError;
import org.json.JSONException;
import org.json.JSONObject;
import java.util.HashMap;

public class HeliumEventProcessor {
    private static final String TAG = "HeliumEventProcessor";
    private static final String EMPTY_STRING = "";

    @FunctionalInterface
    public interface HeliumEventConsumer<T, V, S> {
        void accept(T placementName, V errorCode, S errorDescription);
    }

    public static void serializeHeliumEvent(String placementName, HeliumAdError heliumAdError, HeliumEventConsumer<String, Integer, String> eventConsumer){
        if (placementName == null)
            placementName = EMPTY_STRING;

        int errorCode = -1;
        String errorDescription = EMPTY_STRING;

        if (heliumAdError != null) {
            int tempCode = heliumAdError.getCode();
            errorDescription = heliumAdError.getMessage();

            // we do this in order to bypass lint issues
            if (errorCode != tempCode)
                errorCode = tempCode;
        }

        eventConsumer.accept(placementName, errorCode, errorDescription);
    }

    @FunctionalInterface
    public interface HeliumBidEventConsumer<T, V, S, X>
    {
        void accept(T placementName, V auctionId, S partnerId, X price);
    }

    public static void serializeHeliumBidEvent(String placementName, HashMap<String, String> dataMap, HeliumBidEventConsumer<String, String, String, Double> eventConsumer)
    {
        try {
            if (placementName == null)
                placementName = EMPTY_STRING;

            String partnerId = dataMap.get("partner_id");
            partnerId = partnerId == null ? EMPTY_STRING : partnerId;
            String auctionId = dataMap.get("auction-id");
            auctionId = auctionId == null ? EMPTY_STRING : auctionId;
            String priceAsString = dataMap.get("price");
            priceAsString = priceAsString == null ? "0" : priceAsString;
            double price = Double.parseDouble(priceAsString);
            eventConsumer.accept(placementName, auctionId, partnerId, price);
        }
        catch (Exception e) {
            Log.d(TAG, "bidFetchingInformationError", e);
        }
    }

    @FunctionalInterface
    public interface HeliumRewardEventConsumer<T, V>
    {
        void accept(T placementName, V reward);
    }

    public static void serializeHeliumRewardEvent(String placementName, String reward, HeliumRewardEventConsumer<String, Integer> eventConsumer) {
        if (placementName == null)
            placementName = EMPTY_STRING;

        int rewardAsInt = 0;
        if (reward != null)
            rewardAsInt = Integer.parseInt(reward);

        eventConsumer.accept(placementName, rewardAsInt);
    }

    public static String serializePlacementILRDData(String placementName, JSONObject ilrdInfo) {
        JSONObject serializedString = new JSONObject();
        try {
            serializedString.put("placementName", placementName);
            serializedString.put("ilrd", ilrdInfo);
        } catch (JSONException e) {
            Log.d(TAG, "serializeError", e);
        }
        return serializedString.toString();
    }
}
