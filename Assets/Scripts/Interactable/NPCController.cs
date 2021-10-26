using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, IInteractable
{
    [SerializeField] private Dialog dialog;
    [SerializeField] private bool isDog;

    public void Interact()
    {
        if (isDog) BattleHUD.isDog = true;
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog));
        DialogManager.Instance.SetName(gameObject.name);
    }
}
