using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Saw : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer sr;

    [SerializeField] private bool sawExtended = true;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float idleTime;
    [SerializeField] private Transform[] wayPoint;

    //[SerializeField] private Vector3[] wayPointPosition; 

    public int wayPointIndex = 1;
    public int moveDir = 1;
    private bool canMove = true;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateWayPointInfo();

        transform.position = wayPoint[0].position;

        if (wayPoint[wayPointIndex].position.x > transform.position.x || wayPoint[wayPointIndex].position.y < transform.position.y)
            sr.flipX = !sr.flipX;
    }

    private void UpdateWayPointInfo()
    {
        Trap_SawWayPoint[] wayPointList = transform.parent.gameObject.GetComponentsInChildren<Trap_SawWayPoint>();

        wayPoint = new Transform[wayPointList.Length];

        for(int i = 0; i < wayPointList.Length; i++)
        {
            wayPoint[i] = wayPointList[i].transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!canMove)
            return;

        if (sawExtended)
            MoveExtended();
        else
            Move();
        
    }

    private void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position, wayPoint[wayPointIndex].position, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, wayPoint[wayPointIndex].position) < .1f)
        {
            wayPointIndex++;

            if (wayPointIndex >= wayPoint.Length)
                wayPointIndex = 0;
        }
    }

    private void MoveExtended()
    {
        transform.position = Vector2.MoveTowards(transform.position, wayPoint[wayPointIndex].position, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, wayPoint[wayPointIndex].position) < .1f)
        {
            if (wayPointIndex == wayPoint.Length - 1 || wayPointIndex == 0)
            {
                moveDir = moveDir * -1;
                StartCoroutine(StopMovement());
            }

            wayPointIndex = wayPointIndex + moveDir;
        }
    }

    private IEnumerator StopMovement()
    {
        canMove = false;
    
        yield return new WaitForSeconds(idleTime);

        canMove = true;
        sr.flipX = !sr.flipX;
    }

    [ContextMenu("Flip Sprite")]
    private void FlipSprite()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.flipX = !sr.flipX;
    }
}
