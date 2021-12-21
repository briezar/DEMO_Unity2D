using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Start()
    {
        SoundManager.Instance.ChangeMasterVolume(slider.value);
        slider.onValueChanged.AddListener(val => SoundManager.Instance.ChangeMasterVolume(val));
    }
}
