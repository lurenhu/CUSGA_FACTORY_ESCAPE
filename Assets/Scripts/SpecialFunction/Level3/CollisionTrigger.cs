using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTrigger : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other) {
        GameManager.Instance.levelIndex++;
        GameManager.Instance.gameState = GameState.Generating;
    }
}
