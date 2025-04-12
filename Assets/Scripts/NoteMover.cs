using System.Collections.Generic;
using UnityEngine;

public class NoteMover : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private float spawnTime;
    private Vector3 _noteDespawnPoint;
    private List<GameObject> _activeNotes = new List<GameObject>();

    public void Initialize(float timeToReachTarget, Vector3 endPoint, Vector3 noteDespawnPosition, GameObject note)
    {
        _noteDespawnPoint = noteDespawnPosition;
        spawnTime = Time.time;
        _activeNotes.Add(note);

        // Calcule de la direction et de la vitesse pour un mouvement constant
        direction = (endPoint - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, endPoint);
        speed = distance / timeToReachTarget;
    }

    void Update()
    {
        if (_activeNotes.Count > 0)
        {
            Movement();
        }
    }

    public void Movement()
    {
        // Déplace chaque note dans la direction calculée à une vitesse constante
        for (int i = _activeNotes.Count - 1; i >= 0; i--)
        {
            GameObject note = _activeNotes[i];

            if (note != null)
            {
                note.transform.position += direction * speed * Time.deltaTime;

                if (note.transform.position.x <= _noteDespawnPoint.x)
                {
                    Destroy(note);
                    _activeNotes.RemoveAt(i);
                }
            }
        }
    }
}
