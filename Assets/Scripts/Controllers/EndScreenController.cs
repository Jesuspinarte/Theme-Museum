using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class EndScreenController : MonoBehaviour {
  [Header("Fade Settings")]
  [SerializeField] private float fadeDuration = 1f;

  private Image _uiImage;
  private Coroutine _fadeCoroutine;

  private void Awake() {
    _uiImage = GetComponent<Image>();
  }

  private void OnEnable() {
    if (_uiImage == null) _uiImage = GetComponent<Image>();
    if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
    _fadeCoroutine = StartCoroutine(FadeRoutine());
  }

  private IEnumerator FadeRoutine() {
    Color currentColor = _uiImage.color;
    float elapsedTime = 0f;

    currentColor.a = 0f;
    _uiImage.color = currentColor;

    while (elapsedTime < fadeDuration) {
      elapsedTime += Time.deltaTime;
      currentColor.a = Mathf.Clamp01(elapsedTime / fadeDuration);
      _uiImage.color = currentColor;

      yield return null;
    }

    currentColor.a = 1f;
    _uiImage.color = currentColor;
  }
}