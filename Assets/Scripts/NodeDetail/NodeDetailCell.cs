using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NodeDetailCell: MonoBehaviour {
    public abstract void CreatingEnded();
    public abstract void FillWithData(NodeCellData data);
    public abstract NodeCellData GetData();
}
