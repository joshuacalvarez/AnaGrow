using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TodaysSetButtonClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [Header("Scales")]
    public Vector3 normal = Vector3.one;
    public Vector3 pressed = new Vector3(0.9f, 0.9f, 1f);
    public float tweenTime = 0.1f;

    [Header("Scene Name (optional)")]
    public string sceneToLoad;

    void Awake()
    {
        transform.localScale = normal;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(TweenScale(pressed));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(TweenScale(normal));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    System.Collections.IEnumerator TweenScale(Vector3 target)
    {
        Vector3 start = transform.localScale;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / tweenTime;
            transform.localScale = Vector3.Lerp(start, target, t);
            yield return null;
        }
        transform.localScale = target;
    }
}
