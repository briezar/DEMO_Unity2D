using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

[Serializable]
public enum SceneEnum
{
    TITLE_SCREEN,
    GAME
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject continueText;
    [SerializeField] private TMP_Text loadingText;

    private float target;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (loadingScreen.activeInHierarchy && slider.value < target)
        {
            slider.value = Mathf.MoveTowards(slider.value, target, Time.deltaTime);
        }

    }

    public async void LoadScene(int sceneEnum)
    {
        continueText.SetActive(false);
        target = 0;
        slider.value = 0;

        loadingScreen.SetActive(true);

        var scene = SceneManager.LoadSceneAsync(sceneEnum);
        scene.allowSceneActivation = false;

        do
        {
            target = Mathf.Clamp01(scene.progress / 0.9f);
        } while (scene.progress < 0.9f);

        target = slider.maxValue;
        while (slider.value != slider.maxValue) await Task.Yield();

        loadingText.text = "LOADING COMPLETE";
        continueText.SetActive(true);

        while (!Input.anyKeyDown) await Task.Yield();
        
        continueText.SetActive(false);
        scene.allowSceneActivation = true;
        await Task.Delay(200);
        //if (sceneEnum == (int)SceneEnum.GAME) { Cursor.visible = false; }
        loadingScreen.SetActive(false);
        loadingText.text = "LOADING...";


    }
}
