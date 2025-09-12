
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    private GameController gameController;

    public void Init(GameController controller)
    {
        gameController = controller;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gameController.AddPoint();
        }
    }
}
