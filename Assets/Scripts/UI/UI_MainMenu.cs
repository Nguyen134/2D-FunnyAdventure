using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_MainMenu : MonoBehaviour
{
    
    [SerializeField] private GameObject[] uiElements;

    [SerializeField] private GameObject continueButton;

    [Header("Interactive Camera")]
    [SerializeField] private MenuCharacter menuCharacter;
    [SerializeField] private CinemachineCamera cinemachine;
    [SerializeField] private Transform mainMenuPoint;
    [SerializeField] private Transform skinSelectionPoint;

    public string sceneName;

    private void Start()
    {
        if (HasLevelProgression())
            continueButton.SetActive(true);
    }

    public void SwitchUI(GameObject uiToEnable)
    {
        foreach (GameObject uiElement in uiElements)
        {
            uiElement.SetActive(false);
        }

        uiToEnable.SetActive(true);

        AudioManager.instance.PlaySFX(4);
    }

    public void Play()
    {
        UI_FadeEffect.instance.ScreenFade(LoadLevel);
    }

    private void LoadLevel() => SceneManager.LoadScene(sceneName);

    public void Continue()
    {
        int levelToLoad = PlayerPrefs.GetInt("ContinueLevel", 0);
        int lastUsedSkin = PlayerPrefs.GetInt("LastUsedSkin");

        SkinManager.instance.SetSkin(lastUsedSkin);

        SceneManager.LoadScene("Level_" + levelToLoad);

        AudioManager.instance.PlaySFX(4);
    }

    public void Exit()
    {
        Application.Quit();
    }

    private bool HasLevelProgression() => PlayerPrefs.GetInt("ContinueLevel", 0) > 0;

    public void MoveCameraToMainMenu()
    {
        cinemachine.Follow = mainMenuPoint;
        menuCharacter.MoveTo(mainMenuPoint.position);
    }

    public void MoveCameraToSkinMenu()
    {
        cinemachine.Follow = skinSelectionPoint;
        menuCharacter.MoveTo(skinSelectionPoint.position);
    }
}
