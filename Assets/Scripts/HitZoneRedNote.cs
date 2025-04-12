using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum ZoneType { Good, Perfect }
public class HitZoneRedNote : MonoBehaviour {
    private ZoneType zoneType;
    private KeyCode hitKey = KeyCode.Space; // Input de la barre espace

    private MovingNote currentNote = null;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.TryGetComponent(out MovingNote note) && note.GetNoteType() == NoteType.Red) {
            note.SetInHitZone(true);
            currentNote = note;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.TryGetComponent(out MovingNote note) && note == currentNote) {
            note.SetInHitZone(false);

            if (!note.HasBeenHit()) {
                Debug.Log("Note rouge ratée !");
                note.ResetPosition();
            }

            currentNote = null;
        }
    }

    private void Update() {
        if (Input.GetKeyDown(hitKey) && currentNote != null && !currentNote.HasBeenHit()) {
            switch (zoneType)
            {
                case ZoneType.Perfect:
                    Debug.Log("Parfait !");
                    break;
                case ZoneType.Good:
                    Debug.Log("Bien !");
                    break;
            }

            currentNote.MarkAsHit();
            currentNote.ResetPosition();
            currentNote = null;
        }
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, GetComponent<BoxCollider2D>().size);
    }
}