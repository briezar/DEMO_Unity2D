using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] private BattleUnitBase[] battleUnitBase;
    [SerializeField] private bool isPlayerUnit;
    [SerializeField] private Transform platform;

    public BattleUnitData BattleUnitData { get; set; }

    private Image image;
    private Vector3 originalPos;
    private Vector3 platformOriginalPos;
    private Color originalColor;

    private DogAnimator dogAnimator;

    private void Awake()
    {
        image = GetComponent<Image>();
        originalPos = image.transform.localPosition;
        platformOriginalPos = platform.localPosition;
        if (TryGetComponent<DogAnimator>(out var dogAnimator)) this.dogAnimator = dogAnimator;
        originalColor = image.color;
    }

    public void Setup()
    {
        //Pick a random enemy from battleUnitBase[]; If isDog then use new set of skill for player
        BattleUnitData = new BattleUnitData(battleUnitBase[Random.Range(0, battleUnitBase.Length)], (BattleHUD.isDog ? 1 : 0));

        if (!isPlayerUnit)
        {
            image.sprite = BattleUnitData.BattleUnitBase.FrontSprite;
            image.SetNativeSize();
        }
        image.color = originalColor;
        PlayEnterAnimation();
        BattleUnitData.PlayEntrySound();
    }

    public void PlayEnterAnimation()
    {
        image.transform.localPosition = new Vector3(originalPos.x + (isPlayerUnit ? -500f : 500f), originalPos.y);
        image.transform.DOLocalMoveX(originalPos.x, 1.5f);

        platform.localPosition = new Vector3(platformOriginalPos.x + (isPlayerUnit ? -500f : 500f), platformOriginalPos.y);
        platform.DOLocalMoveX(platformOriginalPos.x, 1.25f);
    }

    public void PlayDOTweenAttackAnim()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveX(originalPos.x + (isPlayerUnit ? 50f : -50f), 0.25f));
        sequence.Append(image.transform.DOLocalMoveX(originalPos.x, 0.25f));
    }

    public void PlayHitAnim()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.gray, 0.1f));
        sequence.Append(image.DOColor(originalColor, 0.1f));
        sequence.Append(image.DOColor(Color.gray, 0.1f));
        sequence.Append(image.DOColor(originalColor, 0.1f));
    }

    public void PlayDefeatedAnim()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
        sequence.Join(image.DOFade(0f, 0.5f));
        BattleUnitData.PlayExitSound();

    }

    public IEnumerator PlayDogAnim(string anim)
    {
        switch (anim)
        {
            case "sleep":
                dogAnimator.PlaySleepAnim(true);
                yield return new WaitForSeconds(4f);
                break;

            case "eat":
                dogAnimator.PlayEatAnim(true);
                yield return new WaitForSeconds(3f);
                break;

            case "idle":
                dogAnimator.PlayIdleAnim(true);
                yield return null;
                break;

            default:
                dogAnimator.StopAnim();
                yield return null;
                break;
        }
    }


}

