using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Plant : Enemy
{
    [Header("Plant Details")]
    [SerializeField] private Enemy_Bullet bulletPrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float bulletSpeed = 7f;
    [SerializeField] private float attackCooldown = 1.5f;
    private float attackTimer;

    protected override void Update()
    {
        base.Update();

        if(attackTimer > 0)
            attackTimer -= Time.deltaTime;

        if (isPlayerDetected && attackTimer <= 0)
            Attack();
    }

    private void Attack()
    {
        attackTimer = attackCooldown;
        anim.SetTrigger("attack");
    }

    private void CreateBullet()
    {
        Enemy_Bullet newBullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        newBullet.SetVelocity(bulletSpeed * facingDir, 0);
    }

    protected override void HandleAnimation()
    {
        //Keep empty
    }


}
