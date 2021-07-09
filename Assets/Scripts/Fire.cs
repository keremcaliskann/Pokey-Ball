using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public Transform ball;

    void Update()
    {
        transform.position = ball.position;
    }
}
