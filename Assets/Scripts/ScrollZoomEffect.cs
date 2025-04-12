using UnityEngine;
using UnityEngine.UI;

public class ScrollZoomEffect : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform viewport;
    [SerializeField] private RectTransform content;
    [SerializeField] private ScrollRect scrollRect;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomFactor = 1.2f;
    [SerializeField] private float zoomLerpSpeed = 10f;
    [SerializeField] private float maxZoomDistance = 200f;

    [Header("Snap Settings")]
    [SerializeField] private float scrollStopThreshold = 100f;
    [SerializeField] private float snapSpeed = 10f;

    private bool isSnapping = false;
    private Vector2 targetSnapPosition;

    void Update()
    {
        HandleZoomEffect();
        HandleSnapToNearest();
    }

    void HandleZoomEffect()
    {
        Vector3 centerWorldPos = viewport.TransformPoint(new Vector3(viewport.rect.width / 2, 0f, 0f));

        foreach (RectTransform child in content)
        {
            Vector3 childWorldPos = child.position;
            float distance = Mathf.Abs(centerWorldPos.x - childWorldPos.x);

            float t = Mathf.Clamp01(1 - (distance / maxZoomDistance));
            float targetScale = Mathf.Lerp(1f, zoomFactor, t);

            Vector3 scale = Vector3.Lerp(child.localScale, new Vector3(targetScale, targetScale, 1f), Time.deltaTime * zoomLerpSpeed);
            child.localScale = scale;
        }
    }

    void HandleSnapToNearest()
    {
        if (scrollRect.velocity.magnitude > scrollStopThreshold)
        {
            isSnapping = false;
            return;
        }

        if (!isSnapping)
        {
            isSnapping = true;

            float minDistance = float.MaxValue;
            RectTransform closest = null;

            Vector3 centerWorldPos = viewport.TransformPoint(new Vector3(viewport.rect.width / 2, 0f, 0f));

            foreach (Transform childTransform in content)
            {
                RectTransform child = childTransform as RectTransform;
                if (child != null)
                {
                    float distance = Mathf.Abs(centerWorldPos.x - child.position.x);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closest = child;
                    }
                }
            }

            if (closest != null)
            {
                Vector3 difference = viewport.TransformPoint(viewport.rect.center) - closest.position;
                targetSnapPosition = content.anchoredPosition + new Vector2(difference.x, 0f);
            }
        }

        content.anchoredPosition = Vector2.Lerp(content.anchoredPosition, targetSnapPosition, Time.deltaTime * snapSpeed);

        if (Vector2.Distance(content.anchoredPosition, targetSnapPosition) < 0.1f)
        {
            content.anchoredPosition = targetSnapPosition;
            isSnapping = false;
        }
    }
}
