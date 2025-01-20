using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class Enemy : MonoBehaviour
{
    private SpriteRenderer sr => GetComponent<SpriteRenderer>();
    protected Animator anim;
    protected Rigidbody2D rb;

    [SerializeField] protected Collider2D[] colliders;
    [SerializeField] protected Transform player;

    [Space]

    [SerializeField] protected float moveSpeed = 2f;
    protected bool canMove;
    [SerializeField] protected float idleDuration = 1.5f;

    [Header("Death Details")]
    [SerializeField] protected float deathImpact = 5f;
    [SerializeField] protected float deathRotationSpeed = 150f;
    protected int deathRotationDir = 1;
    protected bool isDead;

    [Header("Basics Collision")]
    [SerializeField] protected float groundCheckDistance = 1.1f;
    [SerializeField] protected float wallCheckDistance = .7f;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected float playerCheckDistance = 15f;
    [SerializeField] protected LayerMask whatIsPlayer;
    protected bool isPlayerDetected;
    protected bool isGrounded;
    protected bool isGroundInfrontDetected;
    protected bool isWallDetected;

    protected int facingDir = -1;
    protected bool facingRight = false;
    protected float idleTimer;


    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        colliders = GetComponentsInChildren<Collider2D>();
    }

    protected virtual void Start()
    {
        if(sr.flipX && !facingRight)
        {
            sr.flipX = false;
            Flip();
        }

        PlayerManager.OnPlayerRespawn += UpdatePlayerReference;
    }

    private void UpdatePlayerReference()
    {
        if(player == null)
            player = PlayerManager.instance.player.transform;
    }

    protected virtual void Update()
    {
        HandleCollision();
        HandleAnimation();

        idleTimer -= Time.deltaTime;

        if(isDead)
            HandleDeathRotation();
    }

    public virtual void Die()
    {
        foreach(var collider in colliders)
        {
            collider.enabled = false;
        }

        isDead = true;
        anim.SetTrigger("hit");
       

        rb.velocity = new Vector2(rb.velocity.x, deathImpact);


        if (Random.Range(0, 100) < 50)
            deathRotationDir = deathRotationDir * -1;

        PlayerManager.OnPlayerRespawn -= UpdatePlayerReference;
        Destroy(gameObject, 10f);
    }

    protected void HandleDeathRotation()
    {
        transform.Rotate(0, 0, (deathRotationSpeed * deathRotationDir) * Time.deltaTime);
    }

    protected virtual void HandleFlip(float xValue)
    {
        if (xValue < transform.position.x && facingRight || xValue > transform.position.x && !facingRight)
            Flip();
    }

    protected virtual void Flip()
    {
        facingDir *= -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    [ContextMenu("Change Facing Direction")]
    public void FlipDefaultFacingDir()
    {
        sr.flipX = !sr.flipX;
    }

    protected virtual void HandleAnimation()
    {
        anim.SetFloat("xVelocity", rb.velocity.x);
    }

    protected virtual void HandleCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        isGroundInfrontDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        isWallDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
        isPlayerDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDir, playerCheckDistance, whatIsPlayer);
    }

    protected virtual void OnDrawGizmos()
    {
        //Draw ground check
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));

        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));

        //Draw wall check
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (wallCheckDistance * facingDir), transform.position.y));

        //Draw player check
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (playerCheckDistance * facingDir), transform.position.y));
    }
}
