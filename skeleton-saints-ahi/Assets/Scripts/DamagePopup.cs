using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] private TextMeshPro _textMeshPro;
    [SerializeField] private float _destroyDelay;

    private void Awake()
    {
        _textMeshPro = GetComponent<TextMeshPro>();
    }

    public void Setup(float damage)
    {
        _textMeshPro.SetText(damage.ToString());
        StartCoroutine(FadeCoroutine());
        Destroy(gameObject, 1f);
    }

    public void Setup(string text)
    {
        _textMeshPro.SetText(text);
        StartCoroutine(FadeCoroutine());
        Destroy(gameObject, 1f);
    }

    private IEnumerator FadeCoroutine()
    {
        float waitTime = 0;
        while (waitTime < 1)
        {
            _textMeshPro.fontMaterial.SetColor("_FaceColor", Color.Lerp(_textMeshPro.color, Color.clear, waitTime));
            yield return null;
            waitTime += Time.deltaTime / 1f;

        }

    }
}
