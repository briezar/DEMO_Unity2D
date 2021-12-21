using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState { FreeRoam, Battle, Dialog }
public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] private Button[] optionButton;

    private GameState state;
    public bool GameIsPaused { get; private set; }

    public static GameManager Instance { get; private set; }

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
        if (battleSystem.gameObject.activeInHierarchy) battleSystem.gameObject.SetActive(false);
    }
    private void Start()
    {
        playerController.OnEncountered += StartBattle;
        DialogManager.Instance.OnMeetDog += StartBattle;
        battleSystem.OnBattleOver += EndBattle;

        DialogManager.Instance.OnShowDialog += () => state = GameState.Dialog;
        DialogManager.Instance.OnCloseDialog += () => { if (state == GameState.Dialog) state = GameState.FreeRoam; };
    }

    private void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        battleSystem.StartBattle();
    }

    private void EndBattle(bool playerWon)
    {
        state = GameState.FreeRoam;
        SoundManager.Instance.PlayLastBGM();
        battleSystem.gameObject.SetActive(false);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            foreach (var button in optionButton)
            {
                if (button.gameObject.activeInHierarchy)
                {
                    button.onClick.Invoke();
                    break;
                }

            }
        }
        if (GameIsPaused) return;
        switch (state)
        {
            case GameState.FreeRoam:
                playerController.HandleUpdate();
                break;
            case GameState.Battle:
                battleSystem.HandleUpdate();
                break;
            case GameState.Dialog:
                DialogManager.Instance.HandleUpdate();
                break;
            default:
                break;
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        GameIsPaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        GameIsPaused = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetActiveOptionButton(bool value)
    {
        optionButton[0].gameObject.SetActive(value);
    }
}
