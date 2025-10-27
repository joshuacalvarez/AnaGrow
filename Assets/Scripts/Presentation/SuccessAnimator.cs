using UnityEngine;
using TMPro;
using System.Collections;

public class SuccessAnimator : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] RectTransform logo;
    [SerializeField] RectTransform targetCenter;
    [SerializeField] CanvasGroup successGroup;
    [SerializeField] TMP_Text successText;

    [Header("Timing")]
    [SerializeField] float moveScaleDuration = 0.6f;
    [SerializeField] float successFadeDuration = 0.35f;

    [Header("Style")]
    [SerializeField] Vector3 targetScale = new Vector3(2f, 2f, 1f);
    [SerializeField] AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] string successMessage = "SUCCESS";

    Vector2 startPos;
    Vector3 startScale;

    void Awake()
    {
        if (!logo || !targetCenter)
        {
            Debug.LogWarning("SuccessAnimator: assign logo and targetCenter in Inspector");
            enabled = false;
            return;
        }

        startPos = logo.anchoredPosition;
        startScale = logo.localScale;

        if (successGroup)
        {
            successGroup.alpha = 0f;
            successGroup.gameObject.SetActive(false);
        }
    }

    public void Play()
    {
        StopAllCoroutines();
        StartCoroutine(PlayRoutine());
    }

    IEnumerator PlayRoutine()
    {
        if (successText) successText.text = successMessage;

        // Move & scale logo to center
        Vector2 fromPos = logo.anchoredPosition;
        Vector2 toPos = targetCenter.anchoredPosition;
        Vector3 fromScale = logo.localScale;
        Vector3 toScale = targetScale;

        float t = 0f;
        while (t < moveScaleDuration)
        {
            float k = ease.Evaluate(t / moveScaleDuration);
            logo.anchoredPosition = Vector2.LerpUnclamped(fromPos, toPos, k);
            logo.localScale = Vector3.LerpUnclamped(fromScale, toScale, EaseOutBack(k)); // subtle pop
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        logo.anchoredPosition = toPos;
        logo.localScale = toScale;

        if (successGroup)
        {
            successGroup.gameObject.SetActive(true);
            yield return StartCoroutine(FadeCanvasGroup(successGroup, 0f, 1f, successFadeDuration));
        }
    }

    public void ResetToStart()
    {
        StopAllCoroutines();
        logo.anchoredPosition = startPos;
        logo.localScale = startScale;
        if (successGroup)
        {
            successGroup.alpha = 0f;
            successGroup.gameObject.SetActive(false);
        }
    }

    Vector2 WorldToAnchored(RectTransform target)
    {
        RectTransform parent = logo.parent as RectTransform;
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parent,
            RectTransformUtility.WorldToScreenPoint(null, target.position),
            null,
            out localPoint
        );
        return localPoint;
    }

    float EaseOutBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float a, float b, float d)
    {
        float t = 0f;
        cg.blocksRaycasts = false;
        cg.interactable = false;
        while (t < d)
        {
            cg.alpha = Mathf.Lerp(a, b, t / d);
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        cg.alpha = b;
        cg.blocksRaycasts = true;
        cg.interactable = true;
    }
}
