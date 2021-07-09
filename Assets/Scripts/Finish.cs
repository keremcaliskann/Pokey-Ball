using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour
{
    GameController gameController;
    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }
    private void OnTriggerExit(Collider collider)
    {
        if (collider.transform.CompareTag("Ball"))
        {
            if (collider.transform.position.y > transform.position.y)
            {
                gameController.Finish();
            }
        }
    }
}
