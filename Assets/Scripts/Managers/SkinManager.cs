using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinManager : MonoBehaviour
{
    public static SkinManager instance;

    public int selectedSkin;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

    }

    public void SetSkin(int skinId) => selectedSkin = skinId;

    public int GetSkin() => selectedSkin;
}
