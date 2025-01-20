using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_InGame : MonoBehaviour
{
    public static UI_InGame instance;

    [SerializeField] private TextMeshProUGUI fruitText;
    //[SerializeField] private TextMeshProUGUI timerText;

    [SerializeField] private GameObject pausePanel;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            Pause();
    }

    public void Pause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void Resume()
    {
        pausePanel.SetActive(false);
        AudioManager.instance.PlaySFX(4);

        Time.timeScale = 1;
    }

    public void Restart()
    {
        GameManager.instance.RestartLevel();
        AudioManager.instance.PlaySFX(4);

        Time.timeScale = 1;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
        AudioManager.instance.PlaySFX(4);

        Time.timeScale = 1;
    }

    public void UpdateFruitsUI(int _fruitCollected, int _totalFruits)
    {
        fruitText.text = _fruitCollected + " / " + _totalFruits;
    }

    /*public void UpdateTimerUI(float _timer)
    {
        timerText.text = _timer.ToString("00") + " s";
    }*/
}
