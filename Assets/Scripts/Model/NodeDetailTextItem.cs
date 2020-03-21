
using System;
using UnityEngine;

[Serializable]
public class NodeDetailTextItem: NodeDetailItem {

    public string text;

    public NodeDetailTextItem(string text) {
        this.text = text;
    }

    public GameObject CreateCell(Transform parent) {
        var cell = GameObject.Instantiate(Resources.Load("NodeDetailTextCell"), parent) as GameObject;
        cell.GetComponent<NodeDetailTextCell>().FillWithData(this);
        return cell;
    }

}