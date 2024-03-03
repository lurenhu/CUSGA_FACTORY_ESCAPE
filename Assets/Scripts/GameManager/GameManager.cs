using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonobehaviour<GameManager>
{
    public List<NodeGraphSO> nodeGraphSOs;

    private void Start() {
        NodeMapBuilder.Instance.GenerateNodeMap(nodeGraphSOs[0]);
    }
}
