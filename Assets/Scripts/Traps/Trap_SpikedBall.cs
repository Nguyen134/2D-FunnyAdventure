using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_SpikedBall : MonoBehaviour
{
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float angle;
    private float timer;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime * rotateSpeed;
        float newAngle = Mathf.Sin(timer) * angle;
        transform.rotation = Quaternion.Euler(0,0, newAngle);

    }
}
