using System.Collections;
using TMPro;
using UnityEngine;

public class WaveBannerUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text bannerText;
    [SerializeField] private float fadeDuration = 0.3f;

    private void Awake()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
    }

    public IEnumerator ShowBanner(string message, float displayDuration)
    {
        if (bannerText != null)
        {
            bannerText.text = message;
        }

        yield return StartCoroutine(Fade(0f, 1f));
        yield return new WaitForSeconds(displayDuration);
        yield return StartCoroutine(Fade(1f, 0f));
    }

    private IEnumerator Fade(float from, float to)
    {
        if (canvasGroup == null) yield break;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = to;
    }
}