using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;

public class CollisionTrigger : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other) {

        StartCoroutine(GameManager.Instance.Fade(0,1,2,Color.black));
        GameManager.Instance.gameState = GameState.Fail;
    }
}
