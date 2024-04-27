using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTriggerToResult : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other) {
        GameManager.Instance.gameState = GameState.Fail;
    }
}
