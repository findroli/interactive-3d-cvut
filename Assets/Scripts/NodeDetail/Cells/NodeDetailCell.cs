using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NodeDetailCell: MonoBehaviour {
    
    public abstract NodeDetailItem GetData();
    public abstract void FillWithData(NodeDetailItem item);

}
