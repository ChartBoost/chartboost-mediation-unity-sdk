using System;

/// <summary>
/// The Helium backend endpoint.
/// </summary>
public enum Endpoint
{
    Production,
    Staging
}

/// <summary>
/// The Helium backend that is defined by an endpoint.
/// </summary>
public struct Backend
{
    /// <summary>
    /// The production Helium endpoint.
    /// </summary>
    public readonly static Backend Production = new Backend("production");

    /// <summary>
    /// The staging Helium endpoint.
    /// </summary>
    public readonly static Backend Staging = new Backend("staging");

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="endpoint">The endpoint, as a string.</param>
    public Backend(string endpoint)
    {
        _endpoint = endpoint;
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="endpoint">The endpoint, as an enumeration.</param>
    public Backend(Endpoint endpoint)
    {
        _endpoint = endpoint switch
        {
            Endpoint.Production => "production",
            Endpoint.Staging => "staging",
            _ => "production",
        };
    }

    private string _endpoint;

    /// <summary>
    /// The endpoint enumeration value for the backend.
    /// </summary>
    public Endpoint endpoint
    {
        get
        {
            return _endpoint switch
            {
                "production" => Endpoint.Production,
                "staging" => Endpoint.Staging,
                _ => throw new ArgumentOutOfRangeException(nameof(_endpoint), $"Not expected endpoint type value: {_endpoint}")
            };
        }
    }

    /// <summary>
    /// Index representation of the endpoint.
    /// </summary>
    public int Index
    {
        get
        {
            return endpoint switch
            {
                Endpoint.Production => 0,
                Endpoint.Staging => 1,
                _ => 0,
            };
        }
    }

    /// <summary>
    /// String representation of the endpoint.
    /// </summary>
    public string StringRepresentation
    {
        get
        {
            return endpoint switch
            {
                Endpoint.Production => "production",
                Endpoint.Staging => "staging",
                _ => "production",
            };
        }
    }

    /// <summary>
    /// Prvoide the endpoint enumeration from an index value.
    /// </summary>
    /// <param name="index">The index value.</param>
    /// <returns>The endpoint. Returns production if index is out of range.</returns>
    public static Endpoint EndpointFromIndex(int index)
    {
        return index switch
        {
            1 => Endpoint.Staging,
            _ => Endpoint.Production
        };
    }

    /// <summary>
    /// The domain name for the SDK endpoint.
    /// </summary>
    public string SdkDomainName
    {
        get
        {
            switch (endpoint)
            {
                #if UNITY_IOS
                // https://github.com/ChartBoost/ios-helium-sdk/commit/265a9e6c21e104b0dc26125ae002e18b535038da
                case Endpoint.Production:
                default:
                    return "mediation-sdk.chartboost.com";
                case Endpoint.Staging:
                    return "mediation-sdk.chartboost.com";  // TODO: HB-6002, OPS-7562 - staging host is still WIP, use production host for now
                #else
                case Endpoint.Production:
                default:
                    return "https://helium-sdk.chartboost.com";
                case Endpoint.Staging:
                    return "https://helium-staging-sdk.chartboost.com";
                #endif
            }
        }
    }

    /// <summary>
    /// The domain name for the realtime bidding endpoint.
    /// </summary>
    public string RtbDomainName
    {
        get
        {
            switch (endpoint)
            {
                case Endpoint.Production:
                default:
                    return "https://helium-rtb.chartboost.com";
                case Endpoint.Staging:
                    return "https://helium-staging-rtb.chartboost.com";
            }
        }
    }
}
