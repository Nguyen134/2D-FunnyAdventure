using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_LevelSelection : MonoBehaviour
{
    [SerializeField] private UI_LevelButton buttonPrefab;
    [SerializeField] private Transform buttonParent;

    [SerializeField] private bool[] levelsUnlocked;

    private void Start()
    {
        LoadLevelsInfo();
        CreateLevelButtons();
    }

    private void CreateLevelButtons()
    {
        int levelsAmount = SceneManager.sceneCountInBuildSettings - 1;

        for (int i = 1; i < levelsAmount; i++)
        {
            UI_LevelButton newButton = Instantiate(buttonPrefab,buttonParent);
            newButton.SetupButton(i);
        }
    }

    private void LoadLevelsInfo()
    {
        int levelsAmount = SceneManager.sceneCountInBuildSettings - 1;
        levelsUnlocked = new bool[levelsAmount];

        for(int i = 1; i < levelsAmount; i++)
        {
            levelsUnlocked[i] = PlayerPrefs.GetInt("Level" + i + "Unlocked", 0) == 1;
        }

        levelsUnlocked[1] = true;
    }
}
