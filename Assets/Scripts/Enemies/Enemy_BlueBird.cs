using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_BlueBird : Enemy
{
    [Header("BlueBird Details")]
    [SerializeField] private float travelDistance = 8f;
    [SerializeField] private float flyForce = 1.5f;

    private Vector3[] wayPoints;
    private int wayIndex;

    protected override void Start()
    {
        base.Start();

        wayPoints = new Vector3[2];

        wayPoints[0] = new Vector3(transform.position.x - travelDistance / 2, transform.position.y);
        wayPoints[1] = new Vector3(transform.position.x + travelDistance / 2, transform.position.y);

        wayIndex = Random.Range(0, wayPoints.Length);
    }

    protected override void Update()
    {
        base.Update();

        HandleMovement();
    }

    private void FlyUp() => rb.velocity = new Vector2(rb.velocity.x, flyForce);

    private void HandleMovement()
    {
        transform.position = Vector2.MoveTowards(transform.position, wayPoints[wayIndex], moveSpeed * Time.deltaTime);
        HandleFlip(wayPoints[wayIndex].x);

        if(Vector2.Distance(transform.position, wayPoints[wayIndex]) < .1f)
        {
            wayIndex++;

            if(wayIndex >= wayPoints.Length)
                wayIndex = 0;
        }
    }

    protected override void HandleAnimation()
    {
        //Keep empty
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Vector3 leftPos = new Vector3(transform.position.x - travelDistance / 2, transform.position.y);
        Vector3 rightPos = new Vector3(transform.position.x + travelDistance / 2, transform.position.y);

        Gizmos.DrawWireSphere(leftPos, .5f);
        Gizmos.DrawWireSphere(rightPos, .5f);
    }
}
