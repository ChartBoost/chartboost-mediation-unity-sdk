using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionPanel : MonoBehaviour
{
    public Toggle queueToggle;
    public Toggle reuseToggle;
    
    /// <summary>
    /// Getter for getting state of mode in selection panel
    /// </summary>
    public AdLoadType LoadType => _loadType;

    [SerializeField][GrayOut]
    private AdLoadType _loadType;

    /// <summary>
    /// Handler to set state of `LoadType` in selection panel
    /// </summary>
    /// <param name="loadType"></param>
    public void SetLoadType(AdLoadType loadType)
    {
        _loadType = loadType;
    }

    /// <summary>
    /// Set the ad load type as Reuse.
    /// </summary>
    public void SetAsReuse()
    {
        _loadType = AdLoadType.Reuse;
    }

    /// <summary>
    /// Set the ad load type as Queue.
    /// </summary>
    public void SetAsQueue()
    {
        _loadType = AdLoadType.Queue;
    }

    /// <summary>
    /// Set the ad load type as Replace.
    /// </summary>
    public void SetAsReplace()
    {
        _loadType = AdLoadType.Replace;
    }

    public void ToggleView(bool active)
    {
        gameObject.SetActive(active);
    }

    public void ConfigureForPlacementType(PlacementType placementType)
    {
        switch (placementType)
        {
            case PlacementType.Banner:
                ToggleView(false);
                break;
            case PlacementType.Interstitial:
            case PlacementType.Rewarded:
                ToggleView(true);
                queueToggle.gameObject.SetActive(false);
                reuseToggle.gameObject.SetActive(true);
                break;
            case PlacementType.Fullscreen:
                ToggleView(true);
                queueToggle.gameObject.SetActive(true);
                reuseToggle.gameObject.SetActive(false);
                break;        
        }
    }
}
