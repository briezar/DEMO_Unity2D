using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DialogManager : MonoBehaviour
{
    [SerializeField] private GameObject dialogBox;
    [SerializeField] private GameObject nameBox;

    [SerializeField] private TMP_Text dialogText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private float typeSpeed;
    [SerializeField] private AudioClip newLineSFX;
    [SerializeField] private AudioSource typeSFX;

    public event Action OnMeetDog;

    public static DialogManager Instance { get; private set; }

    public event Action OnShowDialog;
    public event Action OnCloseDialog;

    private Dialog dialog;
    private int currentLine = 0;
    private bool isTyping;
    private bool isSkip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (currentLine < dialog.Lines.Count && dialog.Lines[currentLine] != "/end/")
            {
                if (!isTyping)
                {
                    StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
                    if (newLineSFX != null && currentLine != 0) SoundManager.Instance.PlaySFX(newLineSFX);
                }
                else isSkip = true;

            }
            else
            {
                if (isTyping)
                { isSkip = true; return; }
                currentLine = 0;
                dialogBox.SetActive(false);
                OnCloseDialog?.Invoke();
                if (BattleHUD.isDog)
                {
                    OnMeetDog();
                }
            }

        }
    }

    public IEnumerator ShowDialog(Dialog dialog, bool hasName)
    {
        yield return new WaitForEndOfFrame();
        OnShowDialog?.Invoke();
        this.dialog = dialog;

        dialogBox.SetActive(true);
        nameBox.SetActive(hasName);
        StartCoroutine(TypeDialog(dialog.Lines[0]));
    }

    public IEnumerator TypeDialog(string line)
    {
        isTyping = true;
        dialogText.text = "";
        typeSFX.Play();

        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            if (isSkip) continue;
            else
            {
                yield return new WaitForSeconds(typeSpeed);
            }
        }
        typeSFX.Stop();

        isTyping = false;
        isSkip = false;
        currentLine++;

    }

    public void SetName(string line)
    {
        nameText.text = line;
    }


}
