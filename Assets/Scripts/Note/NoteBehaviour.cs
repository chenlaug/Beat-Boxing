using UnityEngine;

public class NoteBehaviour : MonoBehaviour
{
    public enum NoteType { None, Red, Yellow }
    public enum HitResult { Miss, Good, Perfect }

    [SerializeField] private NoteType noteType;
    public NoteType Type => noteType;

    [HideInInspector] public bool inHitZone = false;
    [HideInInspector] public bool wasHit = false;
    [HideInInspector] public float hitZoneCenterX;
    [HideInInspector] public HitResult hitResult = HitResult.Miss;

    private bool holding = false;

    private void Update()
    {
        if (!inHitZone || wasHit)
            return;

        if (noteType == NoteType.Red)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                hitResult = EvaluateHit();
                wasHit = true;

                if (hitResult != HitResult.Miss)
                {
                    ScoreManager.Instance.AddScore(10);
                }

                Debug.Log($"Red Note Hit: {hitResult}");
                Destroy(gameObject);
            }
        }
        else if (noteType == NoteType.Yellow)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                holding = true;
                Debug.Log("Holding started");
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                if (holding)
                {
                    Debug.Log("Released too early while still in zone");
                    holding = false;
                    wasHit = true;
                    Destroy(gameObject);
                }
            }

            if (!Input.GetKey(KeyCode.Space) && holding)
            {
                Debug.Log("Key no longer held");
                holding = false;
                wasHit = true;
                Destroy(gameObject);
            }
        }
    }

    public HitResult EvaluateHit()
    {
        float distance = Mathf.Abs(transform.position.x - hitZoneCenterX);
        Debug.Log($"Distance to center: {distance}");

        if (distance < 0.1f)
            return HitResult.Perfect;
        else if (distance < 0.3f)
            return HitResult.Good;
        else
            return HitResult.Miss;
    }

    public bool IsHoldingCorrectly()
    {
        return noteType == NoteType.Yellow && Input.GetKey(KeyCode.Space);
    }

    public void CheckHoldEnd(float hitZoneRightX)
    {
        float noteRightX = GetRightEdgeX();
        float distance = Mathf.Abs(noteRightX - hitZoneRightX);

        HitResult result;
        if (distance < 0.1f)
            result = HitResult.Perfect;
        else if (distance < 0.3f)
            result = HitResult.Good;
        else
            result = HitResult.Miss;

        Debug.Log($"Yellow Hold End: {result}");

        if (result != HitResult.Miss)
        {
            ScoreManager.Instance.AddScore(10);
        }

        wasHit = true;
        Destroy(gameObject);
    }

    public float GetRightEdgeX()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        return transform.position.x + col.bounds.extents.x;
    }
}
