using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonobehaviour<GameManager>
{
    public List<NodeGraphSO> nodeGraphSOs;

    public Transform graphTransform;

    private void Start() {
        NodeMapBuilder.Instance.GenerateNodeMap(nodeGraphSOs[0]);
    }

    public void CloseGraph()
    {
        graphTransform.gameObject.SetActive(false);
    }
}
