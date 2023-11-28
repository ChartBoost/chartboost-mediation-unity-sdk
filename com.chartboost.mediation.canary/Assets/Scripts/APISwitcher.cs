using System;
using UnityEngine;

public class APISwitcher : SimpleSingleton<APISwitcher>
{
    public SwitcherTabs bannerTabs;
    public SwitcherTabs fullscreenTabs;

    /// <summary>
    /// Standard Unity Awake call
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void Awake()
    {
        UseNewBannerAPI(Environment.Shared.UseNewBannerAPI);
        UseNewFullscreenAPI(Environment.Shared.UseNewFullscreenAPI);
    }

    /// <summary>
    /// Whether or not to use new Fullscreen API
    /// </summary>
    /// <param name="useNewFullscreenAPI"></param>
    public void UseNewFullscreenAPI(bool useNewFullscreenAPI)
    {
        UseNewAPI(useNewFullscreenAPI, fullscreenTabs);
    }
        
    /// <summary>
    /// Whether or not to use new Banner API
    /// </summary>
    /// <param name="useNewBannerAPI"></param>
    public void UseNewBannerAPI(bool useNewBannerAPI)
    {
        UseNewAPI(useNewBannerAPI, bannerTabs);
    }

    private void UseNewAPI(bool useNewAPI, SwitcherTabs tabs)
    {
        foreach (var tab in tabs.newAPITabs)
        {
            tab.SetActive(useNewAPI);
        }
        foreach (var tab in tabs.oldAPITabs)
        {
            tab.SetActive(!useNewAPI);
        }
    }
}

[Serializable]
public class SwitcherTabs
{
    /// <summary>
    /// Tabs that are visible when new API is in use
    /// </summary>
    public GameObject[] oldAPITabs;
    
    /// <summary>
    /// Tabs that are visible when old API is in use
    /// </summary>
    public GameObject[] newAPITabs;
}
