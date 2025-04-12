using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [Header("Note Prefabs")]
    [SerializeField] private string _redNoteAddress;
    [SerializeField] private string _yellowNoteAddress;

    [Header("Settings")]
    [SerializeField] private Transform _noteSpawnPoint; // Point où les notes vont apparaître
    [SerializeField] private Transform _noteDespawnPoint; // Point où les notes vont disparaître
    [SerializeField] private Transform _detectionCenter; // Point de destination des notes

    private GameObject _redNotePrefab;
    private GameObject _yellowNotePrefab;

    // Méthode pour charger les prefabs de notes
    public void LoadNotePrefabs()
    {
        AssetLoader.Instance.LoadAsset<GameObject>(_redNoteAddress, onSuccess: prefab => _redNotePrefab = prefab, onFailure: () => Debug.LogError("Échec du chargement de la RedNote"));
        AssetLoader.Instance.LoadAsset<GameObject>(_yellowNoteAddress, onSuccess: prefab => _yellowNotePrefab = prefab, onFailure: () => Debug.LogError("Échec du chargement de la YellowNote"));
    }

    // Méthode pour générer une note à un instant donné
    public void SpawnNoteAtTime(string noteType, float duration, float noteTravelTime)
    {
        GameObject notePrefab = null;

        if (noteType == "RedNote" && _redNotePrefab != null)
        {
            notePrefab = _redNotePrefab;
        }
        else if (noteType == "YellowNote" && _yellowNotePrefab != null)
        {
            notePrefab = _yellowNotePrefab;
        }

        if (notePrefab != null)
        {
            GameObject noteInstance = Instantiate(notePrefab, _noteSpawnPoint.position, Quaternion.identity, _noteSpawnPoint);
            noteInstance.GetComponent<NoteMover>().Initialize(noteTravelTime, _detectionCenter.position, _noteDespawnPoint.position, noteInstance);
            if (noteType == "YellowNote")
            {
                Vector3 newScale = noteInstance.transform.localScale;
                newScale.x += duration;
                noteInstance.transform.localScale = newScale;
            }
        }
    }
}
