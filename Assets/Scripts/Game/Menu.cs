using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject objectToSetActive;

    [SerializeReference] public SceneEnum[] scene;

    public void ChangeScene(int sceneToLoad)
    {
        LevelManager.Instance.LoadScene((int) scene[sceneToLoad]);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void SetActiveGameObject(bool value)
    {
        objectToSetActive.SetActive(value);
    }

    public void HideSelf()
    {
        gameObject.SetActive(false);
    }

}
