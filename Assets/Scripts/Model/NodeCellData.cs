
using System;
using UnityEngine;

[Serializable]
public abstract class NodeCellData {
    protected abstract string getCellPrefabFileName();
    public GameObject GetCell() {
        var cellPrefab = Resources.Load(getCellPrefabFileName()) as GameObject;
        var cell = GameObject.Instantiate(cellPrefab);
        if(cell != null) cell.GetComponent<NodeDetailCell>().FillWithData(this);
        else Debug.Log("NodeCellData: Cell is null!");
        return cell;
    }
}

[Serializable]
public class NodeTextCellData: NodeCellData {
    protected override string getCellPrefabFileName() {
        return "NodeDetailTextCell";
    }

    public string text;
}

[Serializable]
public class NodeImageCellData: NodeCellData {
    protected override string getCellPrefabFileName() {
        return "NodeDetailImageCell";
    }

    public byte[] imageData;
}

[Serializable]
public class NodeVideoCellData: NodeCellData {
    protected override string getCellPrefabFileName() {
        return "NodeDetailVideoCell";
    }
    
    //private string text;
}