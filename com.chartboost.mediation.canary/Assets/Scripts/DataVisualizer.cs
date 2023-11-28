using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class DataVisualizer : SimpleSingleton<DataVisualizer>
{
    [SerializeField] private Text title;
    [SerializeField] private Text content;

    private void Awake()
    {
        Instance = this;
        CloseVisualizer();
    }

    public void UpdateContents(string newTitle, string contents)
    {
        title.text = newTitle;
        content.text = JsonPrettify(contents);
    }

    public void OpenVisualizer() => gameObject.SetActive(true);

    public void CloseVisualizer() => gameObject.SetActive(false);

    private static string JsonPrettify(string json)
    {
        using var stringReader = new StringReader(json);
        using var stringWriter = new StringWriter();
        var jsonReader = new JsonTextReader(stringReader);
        var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Formatting.Indented };
        jsonWriter.WriteToken(jsonReader);
        return stringWriter.ToString();
    }
}
