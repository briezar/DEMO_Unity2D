using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusic : MonoBehaviour
{
    [SerializeField] private AudioClip bgm;
    void Start()
    {
        if (bgm != null) SoundManager.Instance.PlayBGM(bgm);
    }

}
