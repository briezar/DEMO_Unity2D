using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text barText;
    [SerializeField] private HPBar hpBar;
    [SerializeField] private bool isPlayer;

    public static bool isDog;

    private BattleUnitData enemy;
    private BattleUnitData player;

    private Vector3 originalPos;

    private void Awake()
    {
        originalPos = transform.localPosition;
    }

    public IEnumerator SetEnemyData(BattleUnitData enemy)
    {
        this.enemy = enemy;
        nameText.text = enemy.BattleUnitBase.EnemyName;

        if (isDog)
        {
            hpBar.SetHP(1f);
            barText.text = "Trust";
            StartCoroutine(hpBar.SetHPSmooth(0f));
        }
        else
        {
            barText.text = "HP";
            yield return new WaitForSeconds(.75f);
            StartCoroutine(hpBar.SetHPSmooth(1f));
        }
    }

    public IEnumerator SetPlayerData(BattleUnitData player)
    {
        this.player = player;
        if (isDog) barText.text = "Patience";
        else barText.text = "HP";
        yield return new WaitForSeconds(0.75f);
        StartCoroutine(hpBar.SetHPSmooth(1f));
    }

    public IEnumerator UpdateHP(bool isPlayerTurn)
    {
        if (isPlayerTurn)
            if (isDog)
                yield return hpBar.SetHPSmooth((float)(enemy.MaxHp - enemy.HP) / enemy.MaxHp);
            else
                yield return hpBar.SetHPSmooth((float)enemy.HP / enemy.MaxHp);
        else
            yield return hpBar.SetHPSmooth((float)player.HP / player.MaxHp);
    }

    public void PlayEnterAnim()
    {
        transform.localPosition = new Vector3(originalPos.x + (isPlayer ? -800f : 800f), originalPos.y);
        transform.DOLocalMoveX(originalPos.x, 1.5f).SetEase(Ease.OutBack);
    }
}
