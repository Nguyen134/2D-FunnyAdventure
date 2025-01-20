using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class MenuCharacter : MonoBehaviour
{
    private Animator anim;

    [SerializeField] private float speed;

    private Vector3 destination;

    private bool isMoving;
    private bool facingRight;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        anim.SetBool("isMoving", isMoving);

        if (isMoving)
            transform.position = Vector2.MoveTowards(transform.position, destination, speed * Time.deltaTime);

        if(Vector2.Distance(transform.position, destination) < .1f)
            isMoving = false;
    }

    public void MoveTo(Vector3 newDestination)
    {
        destination = newDestination;
        destination.y = transform.position.y;

        isMoving = true;
        HandleFlip(destination.x);
    }

    private void HandleFlip(float xValue)
    {
        if (xValue < transform.position.x && facingRight || xValue > transform.position.x && !facingRight)
            Flip();
    }

    private void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }
}
