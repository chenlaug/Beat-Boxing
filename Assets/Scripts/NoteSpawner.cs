using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [Header("Note Prefabs")]
    [SerializeField] private string _redNoteAddress;
    [SerializeField] private string _yellowNoteAddress;

    [Header("Settings")]
    [SerializeField] private Transform _noteSpawnPoint; // Point o� les notes vont appara�tre
    [SerializeField] private Transform _noteDespawnPoint; // Point o� les notes vont dispara�tre
    [SerializeField] private Transform _detectionCenter; // Point de destination des notes

    private GameObject _redNotePrefab;
    private GameObject _yellowNotePrefab;

    // M�thode pour charger les prefabs de notes
    public void LoadNotePrefabs()
    {
        AssetLoader.Instance.LoadAsset<GameObject>(_redNoteAddress, onSuccess: prefab => _redNotePrefab = prefab, onFailure: () => Debug.LogError("�chec du chargement de la RedNote"));
        AssetLoader.Instance.LoadAsset<GameObject>(_yellowNoteAddress, onSuccess: prefab => _yellowNotePrefab = prefab, onFailure: () => Debug.LogError("�chec du chargement de la YellowNote"));
    }

    // M�thode pour g�n�rer une note � un instant donn�
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
