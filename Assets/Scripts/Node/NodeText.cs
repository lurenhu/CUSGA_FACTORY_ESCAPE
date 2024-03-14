// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;

// public class NodeText : MonoBehaviour
// {
//     public Node node;
//     private Text nodeText;

//     private void Awake() {
//         nodeText = GetComponent<Text>();
//     }

//     private void Update() {
//         if (node.isDragging || node.isPopping)
//         {
//             Debug.Log("In");
//             transform.position = HelperUtility.TranslateWorldToScreen(node.transform.position);
//         }
//     }

//     public void InitNode(Node node)
//     {
//         this.node = node;

//         nodeText.text = node.nodeProperty.nodeText;
//         transform.position = HelperUtility.TranslateWorldToScreen(node.transform.position);

//         node.nodeProperty.nodeTextInstance = this;

//         gameObject.SetActive(node.gameObject.activeSelf);
//     }
// }
