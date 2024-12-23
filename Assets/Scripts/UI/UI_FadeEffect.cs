using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_FadeEffect : MonoBehaviour
{
    public static UI_FadeEffect instance;

    public Animator anim;

    private void Awake()
    {
        if(instance == null)
            instance = this;

        anim = GetComponent<Animator>();
    }

    public void ScreenFade(System.Action onComplete = null)
    {
        StartCoroutine(FadeCoroutine(onComplete));
    }

    private IEnumerator FadeCoroutine(System.Action onComplete)
    {
        anim.SetTrigger("fadeOut");
        yield return new WaitForSeconds(1f);
        onComplete?.Invoke();
        anim.SetTrigger("fadeIn");
    }
}
