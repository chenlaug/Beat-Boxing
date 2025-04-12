using UnityEngine;

public class NoteExportJson : MonoBehaviour
{

    public void ExportChartToJson(string filePath)
    {
        string json = JsonUtility.ToJson(this, true);  // Serialize la liste des notes
        System.IO.File.WriteAllText(filePath, json);
        Debug.Log($"Chart export� vers {filePath}");
    }

    public void LoadChartFromJson(string filePath)
    {
        if (System.IO.File.Exists(filePath))
        {
            string json = System.IO.File.ReadAllText(filePath);
            JsonUtility.FromJsonOverwrite(json, this);
            Debug.Log($"Chart charg� depuis {filePath}");
        }
        else
        {
            Debug.LogError("Fichier chart non trouv� !");
        }
    }
}
