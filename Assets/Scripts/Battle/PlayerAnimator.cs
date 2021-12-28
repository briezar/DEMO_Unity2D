using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerAnimator : ImageAnimator
{
    [SerializeField] private Sprite[] attackAnim;
    [SerializeField] private Sprite[] throwAnim;
    [SerializeField] private Sprite[] idleAnim;

    [SerializeField] private GameObject throwObjectParent;
    [SerializeField] private GameObject throwObject;
    [SerializeField] private Transform throwPosition;
    [SerializeField] private Transform throwDestination;

    [SerializeField] private Sprite[] throwObjectSprites;

    private Image throwObjectImage;

    private Sprite[] chosenAnim;

    protected override void OnAwake()
    {
        throwObjectImage = throwObject.GetComponent<Image>();
    }

    public IEnumerator PlayAttackAnim(bool isLoop)
    {
        chosenAnim = attackAnim;
        PlayAnimation(chosenAnim, 6, isLoop);
        yield return chosenAnim[chosenAnim.Length - 1];
    }

    public IEnumerator PlayThrowAnim(string objectToThrow)
    {
        throwObjectParent.SetActive(true);
        throwObject.transform.localPosition = throwPosition.localPosition;

        chosenAnim = throwAnim;
        PlayAnimation(chosenAnim, 6, false);
        yield return chosenAnim[chosenAnim.Length - 1];
        if (objectToThrow == "rock")
        {
            throwObjectImage.sprite = throwObjectSprites[0];
        }
        if (objectToThrow == "bone")
        {
            throwObjectImage.sprite = throwObjectSprites[1];
        }
        throwObjectImage.SetNativeSize();
        yield return throwObject.transform.DOLocalMove(throwDestination.localPosition, 1f).WaitForCompletion();
        throwObjectParent.SetActive(false);
    }

    public void PlayIdleAnim()
    {
        chosenAnim = idleAnim;
        PlayAnimation(chosenAnim, 1, false);
    }

    public void StopAnim()
    {
        StopPlaying();
    }
}
