using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    Ball ball;

    private void Start()
    {
        ball = FindObjectOfType<Ball>();
    }

    void FixedUpdate()
    {
        if (!ball.straining)
        {
            Vector3 targetPosition = new Vector3(transform.position.x, ball.transform.position.y + 2, -8);
            transform.position = Vector3.Lerp(transform.position, targetPosition, 1 / 5f);
        }
    }
}
