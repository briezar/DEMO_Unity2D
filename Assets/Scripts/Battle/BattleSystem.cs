using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy }
public class BattleSystem : MonoBehaviour
{
    [SerializeField] private BattleHUD playerHUD;
    [SerializeField] private EnemyUnit playerUnit;
    [SerializeField] private PlayerAnimator playerAnimator;
    [SerializeField] private BattleHUD enemyHUD;
    [SerializeField] private EnemyUnit enemyUnit;
    [SerializeField] private EnemyUnit enemyUnitDog;
    [SerializeField] private BattleDialogBox dialogBox;

    public event Action<bool> OnBattleOver;

    private BattleState state;
    private int currentAction;
    private int currentMove;
    private bool isFeedingDog = false;
    private bool isShout = false;
    private int numOfFeedDog = 0;

    private Coroutine coroutine;

    public void StartBattle()
    {
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Setup();
        StartCoroutine(playerHUD.SetPlayerData(playerUnit.Enemy));
        playerHUD.PlayEnterAnim();

        dialogBox.SetMoveNames(playerUnit.Enemy.MovesList);

        if (BattleHUD.isDog)
        {
            enemyUnit.gameObject.SetActive(false);
            enemyUnitDog.gameObject.SetActive(true);
            enemyUnitDog.Setup();
            StartCoroutine(enemyHUD.SetEnemyData(enemyUnitDog.Enemy));
            enemyHUD.PlayEnterAnim();
            StartCoroutine(enemyUnitDog.PlayDogAnim("idle"));

            dialogBox.SetMoveNames(playerUnit.Enemy.MovesList);
            yield return dialogBox.TypeDialog("You found the man's dog!");
        }
        else
        {
            enemyUnit.gameObject.SetActive(true);
            enemyUnitDog.gameObject.SetActive(false);
            enemyUnit.Setup();
            StartCoroutine(enemyHUD.SetEnemyData(enemyUnit.Enemy));
            enemyHUD.PlayEnterAnim();

            dialogBox.SetMoveNames(playerUnit.Enemy.MovesList);
            yield return dialogBox.TypeDialog($"A wild {enemyUnit.Enemy.EnemyBase.EnemyName} appeared!");
        }
        yield return new WaitForSeconds(1.5f);

        PlayerAction();
    }

    private void PlayerAction()
    {
        state = BattleState.PlayerAction;
        coroutine = StartCoroutine(dialogBox.TypeDialog("Choose an action"));
        dialogBox.EnableActionSelector(true);
    }

    private void PlayerMove()
    {
        state = BattleState.PlayerMove;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    private IEnumerator PerformPlayerMove()
    {
        state = BattleState.Busy;
        var move = playerUnit.Enemy.MovesList[currentMove];
        StopCoroutine(coroutine);
        yield return dialogBox.TypeDialog($"Player used {move.MoveBase.MoveName}");

        playerUnit.PlayDOTweenAttackAnim();
        if (move.MoveBase.MoveName == "Feed")
        {
            isFeedingDog = true;
            yield return playerAnimator.PlayThrowAnim("bone");
        }
        else if (move.MoveBase.MoveName == "Slash")
            yield return playerAnimator.PlayAttackAnim(false);
        else if (move.MoveBase.MoveName == "Throw")
            yield return playerAnimator.PlayThrowAnim("rock");
        else if (move.MoveBase.MoveName == "Shout")
            isShout = true;

        yield return new WaitForSeconds(0.5f);
        playerAnimator.PlayIdleAnim();
        bool isDefeated = false;

        if (UnityEngine.Random.Range(0, 100) < move.MoveBase.Accuracy)
        {
            if (!BattleHUD.isDog) enemyUnit.PlayHitAnim();
            isDefeated = (BattleHUD.isDog ? enemyUnitDog : enemyUnit).Enemy.TakeDamage(move, playerUnit.Enemy);
            yield return enemyHUD.UpdateHP(true);
        }
        else
        {
            yield return dialogBox.TypeDialog("You missed it.");
            yield return new WaitForSeconds(1f);
        }

        if (isDefeated)
        {
            if (BattleHUD.isDog)
                yield return dialogBox.TypeDialog("You gained the Dog's trust!");
            else
            {
                yield return dialogBox.TypeDialog($"You defeated the {enemyUnit.Enemy.EnemyBase.EnemyName}.");
                enemyUnit.PlayDefeatedAnim();
            }
            yield return new WaitForSeconds(2f);
            OnBattleOver(true);
            BattleHUD.isDog = false;

        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }

    private IEnumerator EnemyMove()
    {
        state = BattleState.EnemyMove;

        var move = (BattleHUD.isDog ? enemyUnitDog : enemyUnit).Enemy.GetRandomMove(0, 3);

        if (isShout)
        {
            if (UnityEngine.Random.Range(0, 10) < 6)
            {
                enemyUnitDog.Enemy.TakeDamage(40);
                yield return enemyHUD.UpdateHP(true);
                yield return dialogBox.TypeDialog("You succeeded, but the dog doesn't like it very much.");
            }
            else
            {
                enemyUnitDog.Enemy.TakeDamage(-20);
                yield return enemyHUD.UpdateHP(true);
                yield return dialogBox.TypeDialog("The dog hates you.");
            }
            isShout = false;
            move = enemyUnitDog.Enemy.GetRandomMove(0, 2);
            yield return new WaitForSeconds(1f);
        }

        else if (isFeedingDog)
        {
            if ((UnityEngine.Random.Range(0, 10) < 7) && numOfFeedDog < 3)
            {
                move = enemyUnitDog.Enemy.MovesList[3];
                isFeedingDog = false;
                numOfFeedDog++;
                enemyUnitDog.Enemy.TakeDamage(40);
                yield return enemyHUD.UpdateHP(true);
            }
            else
            {
                isFeedingDog = false;
                yield return dialogBox.TypeDialog("The dog does not want to eat.");
                yield return new WaitForSeconds(1f);
            }
        }

        yield return dialogBox.TypeDialog($"{(BattleHUD.isDog ? enemyUnitDog : enemyUnit).Enemy.EnemyBase.EnemyName} {move.MoveBase.MoveName}.");

        if (BattleHUD.isDog)
            if (move == enemyUnitDog.Enemy.MovesList[3])
            {
                yield return enemyUnitDog.PlayDogAnim("eat");
                isFeedingDog = false;
            }
            else if (move == enemyUnitDog.Enemy.MovesList[0])
                yield return enemyUnitDog.PlayDogAnim("sleep");

        if (!BattleHUD.isDog) enemyUnit.PlayDOTweenAttackAnim();

        yield return new WaitForSeconds(0.5f);

        if (BattleHUD.isDog) StartCoroutine(enemyUnitDog.PlayDogAnim("idle"));

        bool isDefeated = false;

        if (UnityEngine.Random.Range(0, 100) < move.MoveBase.Accuracy) // if hits
        {
            if (!BattleHUD.isDog) playerUnit.PlayHitAnim();
            isDefeated = playerUnit.Enemy.TakeDamage(move, (BattleHUD.isDog ? enemyUnitDog : enemyUnit).Enemy);
            yield return playerHUD.UpdateHP(false);
        }
        else
        {
            yield return dialogBox.TypeDialog("The attack missed.");
            yield return new WaitForSeconds(1f);
        }

        if (isDefeated)
        {
            if (!BattleHUD.isDog)
                yield return dialogBox.TypeDialog("You are defeated.");
            else
                yield return dialogBox.TypeDialog("You ran out of patience.");

            playerUnit.PlayDefeatedAnim();
            yield return new WaitForSeconds(2f);
            OnBattleOver(false);
            BattleHUD.isDog = false;
        }
        else
        {
            PlayerAction();
        }
    }

    public void HandleUpdate()
    {
        if (state == BattleState.PlayerAction)
        {
            ActionSelect();
        }
        else if (state == BattleState.PlayerMove)
        {
            MoveSelect();
        }
    }

    private void ActionSelect()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (currentAction < 1)
                currentAction++;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            if (currentAction > 0)
                currentAction--;
        }

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (currentAction == 0)
            {
                PlayerMove();
            }

            if (currentAction == 1)
            {
                //Run
            }
        }
    }

    private void MoveSelect()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (currentMove < playerUnit.Enemy.MovesList.Count - 1)
                currentMove++;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            if (currentMove > 0)
                currentMove--;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            if (currentMove < playerUnit.Enemy.MovesList.Count - 2)
                currentMove += 2;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            if (currentMove > 1)
                currentMove -= 2;
        }

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Enemy.MovesList[currentMove]);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PerformPlayerMove());
        }
    }
}

