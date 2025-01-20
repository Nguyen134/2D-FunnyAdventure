using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject fruitDrop;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;
    private CapsuleCollider2D cd;
    
    private bool canBeControlled;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;
    private float defaultGravityScale;
    private bool canDoubleJump;

    [Header("Buffer & Coyote Jump")]
    [SerializeField] private float bufferJumpWindow = .25f;
    private float bufferJumpActivated = -1;
    [SerializeField] private float coyoteJumpWindow = .5f;
    private float coyoteJumpActivated = -1;

    [Header("Wall Interactions")]
    [SerializeField] private float wallJumpDuration;
    [SerializeField] private Vector2 wallJumpForce;
    private bool isWallJumping;
    private Coroutine wallJumpCoroutine;

    [Header("Knockback")]
    [SerializeField] private float knockbackDuration;
    [SerializeField] private Vector2 knockbackPower;
    [SerializeField] private Color transparent;
    [SerializeField] private float[] waitTime;
    private bool isKnocked = false;
    private bool canBeKnocked = true;


    [Header("Collision")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    [Space]
    [SerializeField] private Transform enemyCheck;
    [SerializeField] private float enemyCheckRadius;
    [SerializeField] private LayerMask whatIsEnemy;
    private bool isGrounded;
    private bool inAirborne;
    private bool isWallDetected;

    private float xInput;
    private float yInput;

    private bool isDoingPushUp;


    private bool facingRight = true;
    private int facingDir = 1;

    [Header("Player Visuals")]
    [SerializeField] private AnimatorOverrideController[] animators;
    [SerializeField] private GameObject deathVfx;
    [SerializeField] private ParticleSystem dustFx;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CapsuleCollider2D>();
        anim = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        defaultGravityScale = rb.gravityScale;
        RespawnFinished(false);

        UpdateSkin();
    }

    // Update is called once per frame
    void Update()
    {

        UpdateAirborneStatus();

        if (!canBeControlled)
        {
            HandleCollision();
            HandleAnimations();
            return;
        }

        if (isKnocked)
            return;

        HandleEnemyDetection();
        HandleInput();
        HandleWallSlide();
        HandleMovement();
        HandleFlip();
        HandleCollision();
        HandleAnimations();
    }

    public void Damage()
    {
        if (!canBeKnocked)
            return;

        if (GameManager.instance.FruitCollected() <= 0)
        {
            Die();
            GameManager.instance.RestartLevel();
        }
        else
        {
            ObjectCreator.instance.CreateObject(fruitDrop, transform, true);
            GameManager.instance.RemoveFruit();
        }

        return;
    }

    private void UpdateSkin()
    {
        SkinManager skinManager = SkinManager.instance;

        if (skinManager == null)
            return;

        anim.runtimeAnimatorController = animators[skinManager.GetSkin()];
    }

    private void HandleEnemyDetection()
    {
        if (rb.velocity.y >= 0)
            return;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemyCheck.position, enemyCheckRadius, whatIsEnemy);

        foreach(var enemy in colliders)
        {
            Enemy newEnemy = enemy.GetComponent<Enemy>();

            if(newEnemy != null)
            {
                AudioManager.instance.PlaySFX(1);

                newEnemy.Die();
                Jump();
            }
        }
    }

    public void RespawnFinished(bool finished)
    {
        if (finished)
        {
            rb.gravityScale = defaultGravityScale;
            canBeControlled = true;
            cd.enabled = true;

            AudioManager.instance.PlaySFX(11);
        }
        else
        {
            rb.gravityScale = 0;
            canBeControlled = false; 
            cd.enabled = false;
        }
    }

    public void Knockback(float sourceDamageXPosition)
    {
        float knockbackDir = 1;

        if(transform.position.x < sourceDamageXPosition)
            knockbackDir = -1;

        if (!canBeKnocked)
            return;

        if (isKnocked)
            return;

        AudioManager.instance.PlaySFX(9);

        CameraManager.instance.ScreenShake(knockbackDir);
        StartCoroutine(KnockbackRoutine());

        rb.velocity = new Vector2(knockbackPower.x * knockbackDir, knockbackPower.y);
    }

    private IEnumerator KnockbackRoutine()
    {
        isKnocked = true;
        canBeKnocked = false;
        anim.SetBool("isKnocked", true);

        yield return new WaitForSeconds(knockbackDuration);

        isKnocked = false;
        anim.SetBool("isKnocked", false);

        foreach(float seconds in waitTime)
        {
            ToggleColor(transparent);

            yield return new WaitForSeconds(seconds);

            ToggleColor(Color.white);

            yield return new WaitForSeconds(seconds); 
        }

        canBeKnocked = true ;
    }

    private void ToggleColor(Color newColor) => sr.color = newColor;

    public void Die()
    {
        AudioManager.instance.PlaySFX(0);

        GameObject newDeathVfx = Instantiate(deathVfx, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void Push(Vector2 direction, float duration = 0)
    {
        if(isDoingPushUp)
            return;

        StartCoroutine(PushCoroutine(direction, duration));
    }

    private IEnumerator PushCoroutine(Vector2 direction, float duration)
    {
        isDoingPushUp = true;
        canBeControlled = false;

        rb.velocity = Vector2.zero;
        rb.AddForce(direction, ForceMode2D.Impulse);

        yield return new WaitForSeconds(duration);

        canBeControlled = true;
        isDoingPushUp = false;
    }

    private void UpdateAirborneStatus()
    {
        if(isGrounded && inAirborne)
            HandleLanding();

        if(!isGrounded && !inAirborne)
            BecomeAirborne();
    }

    private void BecomeAirborne()
    {
        inAirborne = true;

        if(rb.velocity.y < 0)
            ActivateCoyoteJump();
    }

    private void HandleLanding()
    {
        dustFx.Play();

        inAirborne = false;
        canDoubleJump = true;

        AttemptBufferJump();
    }

    private void HandleInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpButton();
            RequestBufferJump();
        }
    }

    #region Buffer & Coyote Jump
    private void RequestBufferJump()
    {
        if(inAirborne)
            bufferJumpActivated = Time.time;
    }
    private void AttemptBufferJump()
    {
        if(Time.time < bufferJumpActivated + bufferJumpWindow)
        {
            bufferJumpActivated = Time.time - 1;
            Jump();
        }
    }

    private void ActivateCoyoteJump() => coyoteJumpActivated = Time.time;
    private void CancelCoyoteJump() => coyoteJumpActivated = Time.time - 1;

    #endregion

    private void HandleMovement()
    {
        if (isWallDetected)
            return;

        if (isWallJumping)
            return;

        rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
    }

    private void HandleWallSlide()
    {
        bool canWallSlide = isWallDetected && rb.velocity.y < 0;
        float yModifer = yInput < 0 ? 1 : .5f;

        if (!canWallSlide)
            return;

        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * yModifer);
    }

    private void JumpButton()
    {
        bool coyoteJumpAvailable = Time.time < coyoteJumpActivated + coyoteJumpWindow;

        if (isGrounded || coyoteJumpAvailable)
        {
            Jump();
        }
        else if (isWallDetected && !isGrounded)
        {
            WallJump();
        }
        else if (canDoubleJump)
        {
            DoubleJump();
        }

        CancelCoyoteJump();
    }

    private void Jump()
    {
        dustFx.Play();
        AudioManager.instance.PlaySFX(3);

        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private void DoubleJump()
    {
        AudioManager.instance.PlaySFX(3);

        isWallJumping = false;
        canDoubleJump = false;

        rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);

        anim.SetTrigger("doubleJump");
    }

    private void WallJump()
    {
        dustFx.Play();

        AudioManager.instance.PlaySFX(12);

        canDoubleJump = true;

        rb.velocity = new Vector2(wallJumpForce.x * -facingDir, wallJumpForce.y);

        Flip();

        if (wallJumpCoroutine != null)
            StopCoroutine(wallJumpCoroutine);

        wallJumpCoroutine = StartCoroutine(WallJumpRoutine());

        //StopAllCoroutines();
        //StartCoroutine(WallJumpRoutine());
    }

    private IEnumerator WallJumpRoutine()
    {
        isWallJumping = true;

        yield return new WaitForSeconds(wallJumpDuration);

        isWallJumping = false;
    }

    private void HandleCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        isWallDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
    }

    private void HandleAnimations()
    {
        anim.SetFloat("xVelocity", rb.velocity.x);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isWallDetected", isWallDetected);
    }

    private void HandleFlip()
    {
        if (isWallJumping)
            return;

        if(xInput < 0 && facingRight || xInput > 0 && !facingRight)
            Flip();
    }

    private void Flip()
    {
        facingDir *= -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(enemyCheck.position, enemyCheckRadius);

        //Draw ground check
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));

        //Draw wall check
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (wallCheckDistance * facingDir), transform.position.y));

    }
}
