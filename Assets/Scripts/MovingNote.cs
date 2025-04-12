using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum NoteType { Red, Yellow};

public class MovingNote : MonoBehaviour {
    [SerializeField] private NoteType noteType;
    [SerializeField] private float hitZoneThreshold = 0.055f;
    [SerializeField] private float speed = 2f;
    private Vector2 targetPosition;
    private Vector2 startPosition;
    private bool isInHitZone = false;
    private bool hasBeenHit = false;

    public NoteType GetNoteType() => noteType;
    public bool IsInHitZone() => isInHitZone;
    public void SetInHitZone(bool value) => isInHitZone = value;
    public void MarkAsHit() => hasBeenHit = true; // Indique que la note est frappée
    public bool HasBeenHit() => hasBeenHit; // Vérifie si la note a été frappée
    void Start() {
        startPosition = transform.position;
        targetPosition = new Vector2(0, transform.position.y); // Ciblage de la barre centrale

        Debug.Log($"Note est {noteType}");
    }

    void Update() {
        // Déplace les barres vers le centre de la barre de rythme
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPosition) < hitZoneThreshold && !hasBeenHit) {
            Debug.Log("Note a dépassé la hit zone sans être frappée !");
            ResetPosition();
        }
        Debug.DrawLine(transform.position, targetPosition, Color.red);
    }
    public void ResetPosition() {
        transform.position = startPosition;
        isInHitZone = false;
        hasBeenHit = false;
    }
}