using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTriggerToResult : MonoBehaviour
{
    bool getResult = false;
    bool hasGetResult = false;
    private void OnCollisionEnter2D(Collision2D other) {
        getResult = true;
    }

    private void Update() {
        if (!hasGetResult && getResult)
        {
            StaticEventHandler.CallGetResult(GameManager.Instance.fakeCutScene);
            GameManager.Instance.levelIndex = -1;
            GameManager.Instance.gameState = GameState.Result;
            hasGetResult = true;
        }
    }
}
