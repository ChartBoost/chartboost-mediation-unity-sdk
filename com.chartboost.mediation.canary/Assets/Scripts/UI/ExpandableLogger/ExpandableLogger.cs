using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Logging Panel that allows adding logs that can be expanded and collapsed
/// for more detailed logging info
/// </summary>
public class ExpandableLogger : MonoBehaviour
{
    [SerializeField]
    private LogItem logItemPrefab;
    [SerializeField]
    private Transform contents;
    [SerializeField] 
    private ScrollRect scrollRect;
    [SerializeField]
    private List<LogItem> logItems = new List<LogItem>();

    /// <summary>
    /// Adds a log entry to this logging panel
    /// </summary>
    /// <param name="log"></param>
    /// <param name="detailedInfo">Extra detailed log that can be viewed by expanding this log entry</param>
    /// <param name="logType"> Type of log that will determine the color of log entry </param>
    public void Log(string log, string detailedInfo = null, LogType logType = LogType.Log)
    {
        var logItem = Instantiate(logItemPrefab, contents);
        logItem.SetData(log, detailedInfo, logType);
        logItems.Add(logItem);
    }

    public void Clear()
    {
        foreach (var item in logItems)
            Destroy(item.gameObject);
        logItems.Clear();
        scrollRect.normalizedPosition = new Vector2(0, 1);
    }
}
