using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogAnimator : ImageAnimator
{
    [SerializeField] private Sprite[] idleAnim;
    [SerializeField] private Sprite[] eatAnim;
    [SerializeField] private Sprite[] sleepAnim;

    private Sprite[] chosenAnim;


    public void PlayIdleAnim(bool isLoop)
    {
        chosenAnim = idleAnim;
        PlayAnimation(chosenAnim, 6, isLoop);
        //yield return chosenAnim[chosenAnim.Length - 1];
    }

    public void PlayEatAnim(bool isLoop)
    {
        chosenAnim = eatAnim;
        PlayAnimation(chosenAnim, 6, isLoop);
        //yield return chosenAnim[chosenAnim.Length - 1];
    }

    public void PlaySleepAnim(bool isLoop)
    {
        chosenAnim = sleepAnim;
        PlayAnimation(chosenAnim, 3, isLoop);
        //yield return chosenAnim[chosenAnim.Length - 1];
    }


    public void StopAnim()
    {
        StopPlaying();
    }
}
