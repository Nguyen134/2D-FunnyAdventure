using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Credits : MonoBehaviour
{
    [SerializeField] private RectTransform rectT;
    [SerializeField] private float scrollSpeed = 200;
    [SerializeField] private float stopScrollPos = 0;
    [SerializeField] private string mainmenuSceneName = "MainMenu";
    private bool creditsSkipped;

    private void Update()
    {
        if (rectT.anchoredPosition.y < stopScrollPos)
            rectT.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
    }

    public void SkipCredits()
    {
        if (!creditsSkipped && rectT.anchoredPosition.y < stopScrollPos)
        {
            creditsSkipped = true;
            scrollSpeed *= 10;
        }
        else
        {
            MainMenu();
        }
    }

    private void MainMenu()
    {
        UI_FadeEffect.instance.ScreenFade(GoToMainMenu);
    }

    private void GoToMainMenu() => SceneManager.LoadScene(mainmenuSceneName);
}
