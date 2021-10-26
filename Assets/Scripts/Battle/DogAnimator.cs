using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogAnimator : MonoBehaviour
{
    [SerializeField] private Sprite[] idleAnim;
    [SerializeField] private Sprite[] eatAnim;
    [SerializeField] private Sprite[] sleepAnim;

    private Sprite[] chosenAnim;
    private ImageAnimator imageAnimator;

    private void Awake()
    {
        imageAnimator = GetComponent<ImageAnimator>();
    }

    public void PlayIdleAnim(bool isLoop)
    {
        chosenAnim = idleAnim;
        imageAnimator.PlayAnimation(chosenAnim, 6, isLoop);
        //yield return chosenAnim[chosenAnim.Length - 1];
    }

    public void PlayEatAnim(bool isLoop)
    {
        chosenAnim = eatAnim;
        imageAnimator.PlayAnimation(chosenAnim, 6, isLoop);
        //yield return chosenAnim[chosenAnim.Length - 1];
    }

    public void PlaySleepAnim(bool isLoop)
    {
        chosenAnim = sleepAnim;
        imageAnimator.PlayAnimation(chosenAnim, 3, isLoop);
        //yield return chosenAnim[chosenAnim.Length - 1];
    }


    public void StopAnim()
    {
        imageAnimator.StopPlaying();
    }
}
