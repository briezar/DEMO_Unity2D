using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ImageAnimator : MonoBehaviour
{
    private Sprite[] frameArray;
    private int fps = 6;
    private bool isLooping = true;

    private bool isPlaying = true;
    private int currentFrame;
    private float timer;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void Update()
    {
        image.SetNativeSize();
        if (timer < 10f) timer += Time.deltaTime;
        float framerate = 1 / (float)fps;

        if (!isPlaying || frameArray == null) return;

        if (timer >= framerate)
        {
            timer = 0f;
            currentFrame = (currentFrame + 1) % frameArray.Length;

            if (!isLooping && currentFrame == 0)
            {
                isPlaying = false;
            }
            else
            {
                image.sprite = frameArray[currentFrame];
            }

        }
    }

    public void PlayAnimation(Sprite[] animation, int fps, bool isLooping)
    {
        frameArray = animation;
        this.fps = fps;
        this.isLooping = isLooping;
        isPlaying = true;

        currentFrame = 0;
        timer = 0f;
        image.sprite = animation[0];
    }

    public void StopPlaying()
    {
        isPlaying = false;
    }
}
