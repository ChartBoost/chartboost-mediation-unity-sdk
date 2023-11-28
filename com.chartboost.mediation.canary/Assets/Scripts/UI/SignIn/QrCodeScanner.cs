#if UNITY_IOS
using System.Runtime.InteropServices;
#endif
using Chartboost.Events;
using UnityEngine;

public class QrCodeScanner
{
    public delegate void DidScanQrCodeEvent(string appId, string appSignature);

    public static event DidScanQrCodeEvent DidScanQrCode;

    public static void PresentQrCodeScanner()
    {
        #if UNITY_ANDROID
        using var qrCodeScanner = GetCodeScannerBridge();
        qrCodeScanner.CallStatic("presentQRCodeScanner", QrCodeScannerEventListener.Instance);
        #elif UNITY_IOS
        _presentQRCodeScanner(InternalDidScanQrCode);
        #endif
    }

    #if UNITY_ANDROID
    private static AndroidJavaClass GetCodeScannerBridge() =>
        new AndroidJavaClass("com.chartboost.mediation.canary.unity.QRCodeScannerBridge");

    internal class QrCodeScannerEventListener : AndroidJavaProxy
    {
        private QrCodeScannerEventListener() : base("com.chartboost.mediation.canary.unity.IQRCodeScannerListener")
        {
        }

        internal static readonly QrCodeScannerEventListener Instance = new QrCodeScannerEventListener();

        private void QrCodeScannerDidScan(string appId, string appSignature) 
            => EventProcessor.ProcessEvent(() => DidScanQrCode?.Invoke(appId, appSignature));
    }
    #elif UNITY_IOS
    [DllImport("__Internal")]
    private static extern void _presentQRCodeScanner(DidScanQrCodeCallback callback);
    
    delegate void DidScanQrCodeCallback(string appId, string appSignature);
    
    [AOT.MonoPInvokeCallback(typeof(DidScanQrCodeCallback))]
    static void InternalDidScanQrCode(string appId, string appSignature) 
        => EventProcessor.ProcessEvent(() => DidScanQrCode?.Invoke(appId, appSignature));
    #endif
}
