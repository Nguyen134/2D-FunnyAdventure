using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit_DroppedByPlayer : Fruit
{
    [SerializeField] private Vector2 velocity;
    [SerializeField] private Color transperentColor;
    [SerializeField] private float[] waitTime;
    private bool canPickup;

    protected override void Start()
    {
        base.Start();

        StartCoroutine(BlinkCoroutine());
    }

    private void Update()
    {
        transform.position += new Vector3(velocity.x, velocity.y) * Time.deltaTime;
    }

    private IEnumerator BlinkCoroutine()
    {
        anim.speed = 0;

        foreach(float seconds in waitTime)
        {
            ToggleSpeedAndColor(transperentColor);

            yield return new WaitForSeconds(seconds);

            ToggleSpeedAndColor(Color.white);

            yield return new WaitForSeconds(seconds);
        }

        velocity.x = 0;
        anim.speed = 1;
        canPickup = true;
    }

    private void ToggleSpeedAndColor(Color color)
    {
        velocity.x *= -1;
        sr.color = color;
    }


    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (canPickup == false)
            return;

        base.OnTriggerEnter2D(collision);
    }
}
