using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Trunk : Enemy
{
    [Header("Trunk Details")]
    [SerializeField] private Enemy_Bullet bulletPrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float bulletSpeed = 7f;
    [SerializeField] private float attackCooldown = 1.5f;
    private float attackTimer;

    protected override void Update()
    {
        base.Update();

        if (isDead)
            return;

        if(attackTimer > 0)
            attackTimer -= Time.deltaTime;

        if (isPlayerDetected && attackTimer <= 0)
            Attack();

        HandleMovement();

        if (isGrounded)
            HandleTurnAround();
    }

    private void HandleTurnAround()
    {
        if (!isGroundInfrontDetected || isWallDetected)
        {
            Flip();
            idleTimer = idleDuration;
            rb.velocity = Vector2.zero;
        }
    }

    private void HandleMovement()
    {
        if (idleTimer > 0)
            return;

        rb.velocity = new Vector2(moveSpeed * facingDir, rb.velocity.y);
    }

    private void Attack()
    {
        attackTimer = attackCooldown;
        idleTimer = idleDuration + attackCooldown;
        anim.SetTrigger("attack");
    }

    private void CreateBullet()
    {
        Enemy_Bullet newBullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);

        if (facingRight)
            newBullet.transform.Rotate(0, 180, 0);

        newBullet.SetVelocity(bulletSpeed * facingDir, 0);
    }
}
