using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ScreenFader : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;
    public TMP_Text blindText;

    private void Start()
    {
        fadeImage.color = new Color(0, 0, 0, 0);
    }

    public void BlackOut()
    {
        StartCoroutine(FadeIn());
        blindText.text = "Player is blind";
    }
    public void UnBlackOut()
    {
        StartCoroutine(FadeOut());
        blindText.text = "Player can see";
    }

    public IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, Mathf.Clamp01(elapsedTime / fadeDuration));
            yield return null;
        }
    }

    public IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, 1 - Mathf.Clamp01(elapsedTime / fadeDuration));
            yield return null;
        }
    }
}
