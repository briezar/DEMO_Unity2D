using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portals : MonoBehaviour
{
    [SerializeField] private Transform endPos;
    [SerializeField] private AudioClip bgmToPlay;
    [SerializeField] private AudioClip sfxToPlay;

    public Transform EndPos => endPos;

    public void PlayEnterBGM()
    {
        if (bgmToPlay == null) return;
        SoundManager.Instance.PlayBGM(bgmToPlay);
    }

    public void PlayEnterSFX()
    {
        if (sfxToPlay != null) SoundManager.Instance.PlaySFX(sfxToPlay);
    }
}
