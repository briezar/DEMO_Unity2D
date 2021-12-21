using System.Collections;
using UnityEngine;

public class GameInit : MonoBehaviour
{
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private GameObject closeInfoButton;
    [SerializeField] private GameObject dialogBox;
    [SerializeField] private Dialog dialog;

    private void Start()
    {
        StartCoroutine(Fader.Instance.FadeIn(float.MinValue));
        GameManager.Instance.PauseGame();
        GameManager.Instance.SetActiveOptionButton(false);
        infoPanel.SetActive(true);
        closeInfoButton.SetActive(false);
        StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        yield return new WaitForSecondsRealtime(1f);
        yield return new WaitUntil(()=> Input.anyKeyDown);
        GameManager.Instance.ResumeGame();
        PlayerController.canMove = false;
        infoPanel.SetActive(false);
        closeInfoButton.SetActive(true);
        yield return Fader.Instance.FadeOut(1f);
        yield return DialogManager.Instance.ShowDialog(dialog, false);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
        GameManager.Instance.SetActiveOptionButton(true);
        PlayerController.canMove = true;

        Fader.Instance.SetCanvasSortOrder(11);  //Make Fader canvas overlay options menu
        gameObject.SetActive(false);
    }
}
