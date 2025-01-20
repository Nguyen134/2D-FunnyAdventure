using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BackgroundType {Blue, Brown, Gray, Green, Pink, Purple, Yellow}
public class AnimatedBackground : MonoBehaviour
{
    [SerializeField] private Vector2 moveDirection;

    [Header("Color")]
    [SerializeField] private BackgroundType backgroundType;
    [SerializeField] private Texture2D[] textures;

    private MeshRenderer mr;

    private void Awake()
    {
        UpdateBackgroundTexture();
    }

    // Update is called once per frame
    void Update()
    {
        mr.material.mainTextureOffset += moveDirection * Time.deltaTime;
    }

    [ContextMenu("Update Background")]
    private void UpdateBackgroundTexture()
    {
        if(mr == null)
            mr = GetComponent<MeshRenderer>();

        mr.material.mainTexture = textures[(int)backgroundType];
    }
}
