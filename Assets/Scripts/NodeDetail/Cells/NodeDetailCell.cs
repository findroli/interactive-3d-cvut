using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NodeDetailCell: MonoBehaviour {
    public delegate void OnDelete(NodeDetailCell cell);
    public event OnDelete onDelete;
    protected void RaiseOnDelete(NodeDetailCell cell) {
        onDelete?.Invoke(cell);
    }
    
    public abstract void CreatingEnded();
    public abstract void FillWithData(NodeCellData data);
    public abstract NodeCellData GetData();
}
