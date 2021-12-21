using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactables : MonoBehaviour, IInteractable
{
    [SerializeField] private Dialog dialog;
    [SerializeField] private bool isDog;
    [SerializeField] private bool hasName;
    [SerializeField] private AudioClip sfx;

    [SerializeField] private Sprite upSprite;
    [SerializeField] private Sprite downSprite;
    [SerializeField] private Sprite leftSprite;
    [SerializeField] private Sprite rightSprite;

    private SpriteRenderer spriteRenderer;
    private GameObject firstTimeBubble;

    private bool isObtainedSword = false;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        try
        {
            firstTimeBubble = transform.Find("Bubble").gameObject;
        }
        catch
        {
            Debug.Log($"{gameObject.name} has no interact bubble.");
        } 
    }

    public void Interact(Vector3 playerDirection)
    {
        if (sfx != null) SoundManager.Instance.PlaySFX(sfx);
        if (firstTimeBubble != null && firstTimeBubble.activeInHierarchy) firstTimeBubble.SetActive(false);
        if (isObtainedSword && gameObject.name == "Shed") DeleteUpToEnd();
        if (gameObject.name == "Shed") isObtainedSword = true;

        if (isDog)
        {
            BattleHUD.isDog = true;
            SoundManager.Instance.StopBGM();
            GameManager.Instance.SetActiveOptionButton(false);
        }
        else
        {
            if (upSprite != null)
            TurnSprite(playerDirection);
        }
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, hasName));
        DialogManager.Instance.SetName(gameObject.name);
    }

    public void TurnSprite(Vector3 playerDirection)
    {
        if (playerDirection.x > 0) //right
            spriteRenderer.sprite = leftSprite;
        else if (playerDirection.x < 0) //left
            spriteRenderer.sprite = rightSprite;
        else if (playerDirection.y > 0) //up
            spriteRenderer.sprite = downSprite;
        else
            spriteRenderer.sprite = upSprite;

    }

    public void DeleteUpToEnd()
    {
        int range = 0;
        if (!dialog.Lines.Contains("/end/")) return;
        foreach (var line in dialog.Lines)
        {
            if (line != "/end/") range++;
            else
            {
                range++;
                break;
            }
        }
        dialog.Lines.RemoveRange(0, range);
    }
}

