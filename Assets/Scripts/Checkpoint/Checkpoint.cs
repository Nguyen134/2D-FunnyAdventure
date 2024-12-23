using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Animator anim;

    private bool active;

    [SerializeField] private bool canBeReActivated;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        canBeReActivated = GameManager.instance.canReActivate;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(active && !canBeReActivated)
            return;

        Player player = collision.GetComponent<Player>();

        if (player != null)
            ActivateCheckpoint();

    }

    private void ActivateCheckpoint()
    {
        active = true;
        PlayerManager.instance.UpdateRespawnPosition(transform);
        anim.SetTrigger("activate");
    }
}
