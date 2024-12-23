using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private UI_InGame inGameUI;

    [Header("Level Management")]
    //[SerializeField] private float levelTimer;
    [SerializeField] private int currentLevel;
    private int nextLevel;

    [Header("Fruits Management")]
    public bool fruitHaveRandomLook;
    public int fruitsCollected;
    public int totalFruits;
    public Transform fruitParent;

    [Header("Checkpoint")]
    public bool canReActivate;

    [Header("Managers")]
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private SkinManager skinManager;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private ObjectCreator objectCreator;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        currentLevel = SceneManager.GetActiveScene().buildIndex;
        nextLevel = currentLevel + 1;

        inGameUI = UI_InGame.instance;

        CollectFruitsInfo();
        CreateManagersIfNeeded();
    }

    private void Update()
    {
        /*levelTimer += Time.deltaTime;
        inGameUI.UpdateTimerUI(levelTimer); */
    }

    private void CreateManagersIfNeeded()
    {
        if (PlayerManager.instance == null)
            Instantiate(playerManager);

        if(SkinManager.instance == null)
            Instantiate(skinManager);

        if (AudioManager.instance == null)
            Instantiate(audioManager);

        if(ObjectCreator.instance == null)
            Instantiate(objectCreator);
    }

    private void CollectFruitsInfo()
    {
        Fruit[] allFruits = FindObjectsOfType<Fruit>();
        totalFruits = allFruits.Length;

        inGameUI.UpdateFruitsUI(fruitsCollected, totalFruits);

        PlayerPrefs.SetInt("Level" + currentLevel + "TotalFruits", totalFruits);
    }

    [ContextMenu("Parent All Fruits")]
    private void ParentAllFruits()
    {
        if (fruitParent == null)
            return;

        Fruit[] allFruits = FindObjectsOfType<Fruit>();

        foreach(Fruit fruit in allFruits)
        {
            fruit.transform.parent = fruitParent;
        }
    }

    public void AddFruit()
    {
        fruitsCollected++;
        inGameUI.UpdateFruitsUI(fruitsCollected, totalFruits);
    }

    public void RemoveFruit()
    {
        fruitsCollected--;
        inGameUI.UpdateFruitsUI(fruitsCollected, totalFruits);
    }

    public int FruitCollected() => fruitsCollected;

    public bool FruitHaveRandomLook() => fruitHaveRandomLook;

    public void LevelComplete()
    {
        SaveLevelProgression();
        SaveFruitsInfo();

        LoadNextScene();
    }

    
    private void SaveFruitsInfo()
    {
        int fruitsCollectedBefore = PlayerPrefs.GetInt("Level" + currentLevel + "FruitsCollected", 0);

        if(fruitsCollected > fruitsCollectedBefore)
            PlayerPrefs.SetInt("Level" + currentLevel + "FruitsCollected", fruitsCollected);

        int totalFruitsInBank = PlayerPrefs.GetInt("TotalFruitsAmount");
        PlayerPrefs.SetInt("TotalFruitsAmount", totalFruitsInBank + fruitsCollected);
    }

    private void SaveLevelProgression()
    {
        if (NoMoreLevels() == false)
        {
            PlayerPrefs.SetInt("Level" + nextLevel + "Unlocked", 1);

            PlayerPrefs.SetInt("ContinueLevel", nextLevel);

            if(SkinManager.instance != null)
                PlayerPrefs.SetInt("LastUsedSkin", SkinManager.instance.GetSkin());
        }
    }

    public void RestartLevel()
    {
        UI_FadeEffect.instance.ScreenFade(LoadCurrentScene);
    }

    private void LoadCurrentScene() => SceneManager.LoadScene("Level_" + currentLevel);
    private void LoadTheEndScene() => SceneManager.LoadScene("TheEnd");

    private void LoadNextLevel()
    {
        SceneManager.LoadScene("Level_" +  nextLevel);
    }

    private void LoadNextScene()
    {
        if (NoMoreLevels())
            UI_FadeEffect.instance.ScreenFade(LoadTheEndScene);
        else
            UI_FadeEffect.instance.ScreenFade(LoadNextLevel);
    }

    private bool NoMoreLevels()
    {
        int lastLevel = SceneManager.sceneCountInBuildSettings - 2;
        bool noMoreLevels = currentLevel == lastLevel;

        return noMoreLevels;
    }
}
