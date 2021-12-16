using System;
using System.Collections;
using UnityEngine;

public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy }
public class BattleSystem : MonoBehaviour
{
    [SerializeField] private BattleHUD playerHUD;
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private PlayerAnimator playerAnimator;
    [SerializeField] private BattleHUD enemyHUD;
    [SerializeField] private BattleUnit enemyUnit;
    [SerializeField] private BattleUnit enemyUnitDog;
    [SerializeField] private GameObject dogGameObbject;
    [SerializeField] private BattleDialogBox dialogBox;

    [SerializeField] private AudioClip dogTheme;
    [SerializeField] private AudioClip selectingSFX;
    [SerializeField] private AudioClip acceptSFX;
    [SerializeField] private AudioClip hitSFX;


    public event Action<bool> OnBattleOver;

    private BattleState state;
    private bool canSelect = false;
    private int currentAction;
    private int currentMove;
    private bool isFeedingDog = false;
    private bool isShout = false;
    private int numOfFeedDog = 0;

    private Coroutine coroutine;


    public void StartBattle()
    {
        canSelect = false;
        if (BattleHUD.isDog) SoundManager.Instance.PlayBGM(dogTheme);
        StartCoroutine(SetupBattle());
        currentAction = 0;
        dialogBox.UpdateActionSelection(currentAction);
        currentMove = 0;
        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Enemy.MovesList[currentMove]);
    }

    public IEnumerator SetupBattle()
    {
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableMoveSelector(false);

        playerUnit.Setup();
        StartCoroutine(playerHUD.SetPlayerData(playerUnit.Enemy));
        playerHUD.PlayEnterAnim();

        dialogBox.SetMoveNames(playerUnit.Enemy.MovesList);

        enemyUnit.gameObject.SetActive(!BattleHUD.isDog);
        enemyUnitDog.gameObject.SetActive(BattleHUD.isDog);
        dialogBox.SetMoveNames(playerUnit.Enemy.MovesList);

        if (BattleHUD.isDog)
        {
            enemyUnitDog.Setup();
            StartCoroutine(enemyHUD.SetEnemyData(enemyUnitDog.Enemy));
            enemyHUD.PlayEnterAnim();
            StartCoroutine(enemyUnitDog.PlayDogAnim("idle"));

            numOfFeedDog = 0;
            isFeedingDog = false;

            yield return dialogBox.TypeDialog("You found the man's dog!");
        }
        else
        {
            enemyUnit.Setup();
            StartCoroutine(enemyHUD.SetEnemyData(enemyUnit.Enemy));
            enemyHUD.PlayEnterAnim();

            yield return dialogBox.TypeDialog($"A wild {enemyUnit.Enemy.BattleUnitBase.EnemyName} appeared!");
        }
        yield return new WaitForSeconds(1.5f);
        canSelect = true;
        PlayerAction();
    }

    private void PlayerAction()
    {
        coroutine = StartCoroutine(dialogBox.TypeDialog("Choose an action"));
        state = BattleState.PlayerAction;
        dialogBox.EnableMoveSelector(false);
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
        yield return dialogBox.TypeDialog($"Player used {move.MoveBase.MoveName}");

        playerUnit.PlayDOTweenAttackAnim();
        move.PlayMoveSFX();
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
            if (!BattleHUD.isDog)
            {
                enemyUnit.PlayHitAnim();
                SoundManager.Instance.PlaySFX(hitSFX);
            }
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
            {
                dogGameObbject.SetActive(false);
                yield return dialogBox.TypeDialog("You gained the Dog's trust!");
                yield return new WaitForSeconds(1f);
                yield return dialogBox.TypeDialog("Now return the dog to its owner.");

                GameObject.Find("Man").GetComponent<Interactables>().DeleteUpToEnd();
            }
            else
            {
                yield return dialogBox.TypeDialog($"You defeated the {enemyUnit.Enemy.BattleUnitBase.EnemyName}.");
                enemyUnit.PlayDefeatedAnim();
            }

            StartCoroutine(EndBattle(true));

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
            isFeedingDog = false;

            if ((UnityEngine.Random.Range(0, 10) < 7) && numOfFeedDog < 3)
            {
                move = enemyUnitDog.Enemy.MovesList[3];
                numOfFeedDog++;
                enemyUnitDog.Enemy.TakeDamage(40);
                yield return enemyHUD.UpdateHP(true);
            }
            else
            {
                yield return dialogBox.TypeDialog("The dog does not want to eat.");
                yield return new WaitForSeconds(1f);
            }
        }

        yield return dialogBox.TypeDialog($"{(BattleHUD.isDog ? enemyUnitDog : enemyUnit).Enemy.BattleUnitBase.EnemyName} {move.MoveBase.MoveName}.");
        move.PlayMoveSFX();

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
            if (!BattleHUD.isDog)
            {
                playerUnit.PlayHitAnim();
                SoundManager.Instance.PlaySFX(hitSFX);
            }
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

            StartCoroutine(EndBattle(false));
        }
        else
        {
            PlayerAction();
        }
    }

    private IEnumerator EndBattle(bool isWin)
    {
        BattleHUD.isDog = false;
        if (!isWin) playerUnit.PlayDefeatedAnim();
        SoundManager.Instance.StopBGM();
        yield return new WaitForSeconds(2f);
        yield return Fader.Instance.FadeIn(0.3f);
        StartCoroutine(Fader.Instance.FadeOut(0.3f));
        OnBattleOver(isWin);
    }
    private IEnumerator RunAway()
    {
        canSelect = false;
        yield return dialogBox.TypeDialog("You ran away.");
        yield return EndBattle(false);
        canSelect = true;
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
        if (!canSelect) return;
        if (Input.GetKeyDown(KeyCode.S))
        {
            SoundManager.Instance.PlaySFX(selectingSFX);
            if (currentAction < 1)
                currentAction++;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            SoundManager.Instance.PlaySFX(selectingSFX);
            if (currentAction > 0)
                currentAction--;
        }


        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            SoundManager.Instance.PlaySFX(acceptSFX);
            if (currentAction == 0)
            {
                StopCoroutine(coroutine);
                PlayerMove();
            }

            if (currentAction == 1)
            {
                StopCoroutine(coroutine);
                StartCoroutine(RunAway());
            }
        }
    }


    private void MoveSelect()
    {
        if (!canSelect) return;

        if (Input.GetKeyDown(KeyCode.D))
        {
            SoundManager.Instance.PlaySFX(selectingSFX);
            if (currentMove < playerUnit.Enemy.MovesList.Count - 1)
                currentMove++;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            SoundManager.Instance.PlaySFX(selectingSFX);
            if (currentMove > 0)
                currentMove--;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            SoundManager.Instance.PlaySFX(selectingSFX);
            if (currentMove < playerUnit.Enemy.MovesList.Count - 2)
                currentMove += 2;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            SoundManager.Instance.PlaySFX(selectingSFX);
            if (currentMove > 1)
                currentMove -= 2;
        }

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Enemy.MovesList[currentMove]);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            SoundManager.Instance.PlaySFX(acceptSFX);
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PerformPlayerMove());
        }
    }
}

