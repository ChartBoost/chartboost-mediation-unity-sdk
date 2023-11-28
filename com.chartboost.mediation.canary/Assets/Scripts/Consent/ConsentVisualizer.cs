using Chartboost;
using UnityEngine;
using UnityEngine.UI;

public class ConsentVisualizer : SimpleSingleton<ConsentVisualizer>
{
    [SerializeField] private Text title;
    [SerializeField] private VerticalLayoutGroup content;

    [SerializeField] private PartnerConsentHandler partnerConsentHandlerPrefab;

    private readonly string[] _allPartners = {
        Partners.AdColony,
        Partners.AdMob,
        Partners.AmazonPublisherServices,
        Partners.AppLovin,
        Partners.DigitalTurbineExchange,
        Partners.GoogleBidding,
        Partners.HyprMX,
        Partners.InMobi,
        Partners.IronSource,
        Partners.MetaAudienceNetwork,
        Partners.Mintegral,
        Partners.MobileFuse,
        Partners.Pangle,
        Partners.Tapjoy,
        Partners.Unity,
        Partners.Verve,
        Partners.Vungle,
        Partners.Yahoo,
    };

    public void OpenVisualizer()
    {
        gameObject.SetActive(true);
    }

    public void CloseVisualizer()
    {
        gameObject.SetActive(false);
    }

    private void Awake()
    {
        Instance = this;
        ConfigureConsentHandlers();
        CloseVisualizer();
    }   

    private void ConfigureConsentHandlers()
    {
        title.text = "Partner Consents";
        foreach (var partnerId in _allPartners)
        {
            var instance = Instantiate(partnerConsentHandlerPrefab, content.transform);
            instance.Initialize(partnerId);
        }
    }
}
