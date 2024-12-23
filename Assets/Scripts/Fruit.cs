using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    [SerializeField] private GameObject pickupVfx;

    protected Animator anim;
    protected SpriteRenderer sr;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        SetRandomFruitIfNeeded();
    }

    private void SetRandomFruitIfNeeded()
    {
        if (!GameManager.instance.FruitHaveRandomLook())
            return;

        int randomIndex = Random.Range(0, 8);
        anim.SetFloat("fruitIndex", randomIndex);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
       Player player = collision.GetComponent<Player>();

       if(player != null)
       {
            GameManager.instance.AddFruit();
            AudioManager.instance.PlaySFX(8);

            Destroy(gameObject);
            GameObject newVfx = Instantiate(pickupVfx, transform.position, Quaternion.identity);
       }
    }
}
