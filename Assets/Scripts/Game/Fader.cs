using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class Fader : MonoBehaviour
{
    public static Fader Instance { get; private set; }
    private Image image;
    private Canvas faderCanvas;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
        image = GetComponent<Image>();
        faderCanvas = transform.GetComponentInParent<Canvas>();
    }

    private void Update()
    {
        image.raycastTarget = (image.color.a != 0);
    }

    public IEnumerator FadeIn(float time)
    {
        yield return image.DOFade(1f, time).WaitForCompletion();
    }

    public IEnumerator FadeOut(float time)
    {
        yield return image.DOFade(0f, time).WaitForCompletion();
    }

    public void SetCanvasSortOrder(int order)
    {
        faderCanvas.sortingOrder = order;
    }
}
