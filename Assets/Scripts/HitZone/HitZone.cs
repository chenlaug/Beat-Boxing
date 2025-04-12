using UnityEngine;

public class HitZone : MonoBehaviour
{
    [Header("Feedback Particles")]
    public GameObject goodEffect;
    public GameObject missEffect;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<NoteBehaviour>(out var note))
        {
            note.inHitZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<NoteBehaviour>(out var note))
        {
            note.inHitZone = false;

            if (!note.wasHit)
            {
                Debug.Log("MISS!");
                PlayEffect(missEffect, note.transform.position);
                Destroy(note.gameObject);
            }
            else
            {
                PlayEffect(goodEffect, note.transform.position);
            }
        }
    }

    private void PlayEffect(GameObject prefab, Vector3 position)
    {
        if (prefab == null) return;
        GameObject fx = Instantiate(prefab, position, Quaternion.identity);
        Destroy(fx, 0.5f);
    }
}