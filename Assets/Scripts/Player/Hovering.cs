using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Hovering : MonoBehaviour
{
    private float originalPosY;
    private Sequence sequence;

    private void OnDestroy()
    {
        sequence.Kill();
    }
    void Start()
    {
        sequence = DOTween.Sequence();
        originalPosY = transform.position.y;
        sequence.SetLoops(-1);
        sequence.Append(transform.DOMoveY(originalPosY + 0.1f, 0.5f))
        .Append(transform.DOMoveY(originalPosY - 0.1f, 1f))
        .Append(transform.DOMoveY(originalPosY, 0.5f))
        .AppendInterval(1f);

    }
}
