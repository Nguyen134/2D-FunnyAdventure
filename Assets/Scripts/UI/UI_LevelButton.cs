using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_LevelButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelNumberText;
    [SerializeField] private TextMeshProUGUI fruitText;


    private string sceneName;
    private int levelIndex;

    public void SetupButton(int _levelIndex)
    {
        levelIndex = _levelIndex;
        levelNumberText.text = "Level " + levelIndex;
        sceneName = "Level_" + levelIndex;

        fruitText.text = FruitsInfoText();
    }

    public void LoadLevel()
    {
        
        bool levelUnlocked = PlayerPrefs.GetInt("Level" + levelIndex + "Unlocked", 0) == 1;

        if (levelIndex > 1 && !levelUnlocked)
        {
            AudioManager.instance.PlaySFX(13, false);
            return;
        }

        SceneManager.LoadScene(sceneName);

        AudioManager.instance.PlaySFX(4);
    }

    private string FruitsInfoText()
    {
        int totalFruits = PlayerPrefs.GetInt("Level" + levelIndex + "TotalFruits");
        string totalFruitsText = totalFruits == 0 ? "?" : totalFruits.ToString();

        int fruitsCollected = PlayerPrefs.GetInt("Level" + levelIndex + "FruitsCollected");

        return "Fruits: " + fruitsCollected + " / " + totalFruitsText;
    }
}
