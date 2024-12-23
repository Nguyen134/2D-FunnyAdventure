using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_FallingPlatform : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private BoxCollider2D[] colliders;


    [Header("Platfrom Fall details")]
    [SerializeField] private float impactSpeed;
    [SerializeField] private float impactDuration;
    private bool impactHappend;
    private float impactTimer;
    [Space]
    [SerializeField] private float fallDelay = .5f;
    [SerializeField] private float cooldown;


    private Transform startPos;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        colliders = GetComponents<BoxCollider2D>();
    }

    private void Start()
    {
        startPos = transform;
    }

    private void Update()
    {
        HandleImpact();
    }

    private void HandleImpact()
    {
        if (impactTimer < 0)
            return;

        impactTimer -= Time.deltaTime;
        transform.position = 
            Vector2.MoveTowards(transform.position, transform.position + Vector3.down * 10, impactSpeed * Time.deltaTime);

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(impactHappend)
            return;

        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            Invoke(nameof(Falling), fallDelay);
            impactTimer = impactDuration;
            impactHappend = true;

            GameObject fallingPrefab = ObjectCreator.instance.fallingPrefab;
            ObjectCreator.instance.CreateObject(fallingPrefab, startPos, false, cooldown);
            Destroy(gameObject, 10f);
        }

    }

    private void Falling()
    {
        anim.SetTrigger("deactive");
        rb.isKinematic = false;

        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }
    }
}
