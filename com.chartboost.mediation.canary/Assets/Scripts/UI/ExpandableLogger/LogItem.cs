using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A log entry on logging panel
/// </summary>
public class LogItem : MonoBehaviour
{
    public Text TitleText;
    public GameObject DetailsPanel;
    public Text DetailsText;

    [Header("Expand/collapse")]
    public GameObject ExpandCollapsePanel;
    public Toggle ExpandCollapseToggle;
    public GameObject ExpandCollapseToggleIcon;



    private void Awake()
    {
        ExpandCollapseToggle.onValueChanged.AddListener(OnExpandCollapseToggle);
    }

    /// <summary>
    /// Sets log entry's UI fields with the details provided
    /// </summary>
    /// <param name="log"></param>
    /// <param name="detailedInfo"></param>
    /// <param name="logType"></param>
    public void SetData(string log, string detailedInfo = null, LogType logType = LogType.Log)
    {
        TitleText.text = log;
        if (!string.IsNullOrEmpty(detailedInfo))
        {
            ExpandCollapsePanel.gameObject.SetActive(true);
            DetailsText.text = detailedInfo;
        }
        else
        {
            DetailsPanel.SetActive(false);
        }

        var color = logType switch
        {
            LogType.Log => Color.black,
            LogType.Error => Color.red,
            LogType.Warning => new Color(0.509434f, 0.3914657f,0,1),    // brown
            _ => Color.black,
        };
        TitleText.color = color;
        DetailsText.color = color;
    }

    private void OnExpandCollapseToggle(bool isOn)
    {
        ExpandCollapseToggleIcon.transform.eulerAngles = new Vector3(0, 0, isOn ? -90 : 0);
        DetailsPanel.SetActive(isOn);
    }
}
