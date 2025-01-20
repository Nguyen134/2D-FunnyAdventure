using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct Skin
{
    public string skinName;
    public int skinPrice;
    public bool unlocked;
}

public class UI_SkinSelection : MonoBehaviour
{
    [SerializeField] private Skin[] skinList;

    [Header("UI Details")]
    [SerializeField] private int skinId;
    [SerializeField] private Animator anim;

    [SerializeField] private GameObject selectObj;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI bankText;

    private void Start()
    {
        LoadSkinUnlocked();
        UpdateSkinDisplay();
    }

    private void LoadSkinUnlocked()
    {

        for (int i = 0; i < skinList.Length; i++)
        {
            string skinName = skinList[i].skinName;
            bool skinUnlocked = PlayerPrefs.GetInt(skinName + "Unlocked", 0) == 1;

            if(skinUnlocked || i == 0)
                skinList[i].unlocked = true;
        }
    }

    public void SelectSKin()
    {
        SkinManager.instance.SetSkin(skinId);
         
        AudioManager.instance.PlaySFX(4);
    }

    public void BuySkin()
    {
        if (HaveEnoughFruits(skinList[skinId].skinPrice) == false)
        {
            AudioManager.instance.PlaySFX(6);

            //Debug.Log("Not enough fruits");
            return;
        }
       
        skinList[skinId].unlocked = true;

        string skinName = skinList[skinId].skinName;
        PlayerPrefs.SetInt(skinName + "Unlocked", 1);

        UpdateSkinDisplay();

        AudioManager.instance.PlaySFX(10);
    }

    public void NextSkin()
    {
        skinId++;

        if(skinId >= anim.layerCount)
            skinId = 0;

        UpdateSkinDisplay();

        AudioManager.instance.PlaySFX(4);
    }

    public void PreviousSkin()
    {
        skinId--;

        if(skinId < 0)
            skinId = anim.layerCount - 1;
    
        UpdateSkinDisplay();

        AudioManager.instance.PlaySFX(4);
    }

    private void UpdateSkinDisplay()
    {
        bankText.text = FruitsInBank().ToString();

        for(int i = 0; i < anim.layerCount; i++)
        {
            anim.SetLayerWeight(i, 0);
        }

        anim.SetLayerWeight(skinId, 1);

        if (skinList[skinId].unlocked)
        {
            priceText.transform.parent.gameObject.SetActive(false);
            selectObj.SetActive(true);
        }
        else
        {
            priceText.transform.parent.gameObject.SetActive(true);
            selectObj.SetActive(false);

            priceText.text = skinList[skinId].skinPrice.ToString();
        }
    }

    private int FruitsInBank() => PlayerPrefs.GetInt("TotalFruitsAmount");

    private bool HaveEnoughFruits(int _price)
    {
        if(FruitsInBank() >= _price)
        {
            PlayerPrefs.SetInt("TotalFruitsAmount", FruitsInBank() - _price);
            return true;
        }

        return false;
    }
}
