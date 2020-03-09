using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct NodeDetailModel {
    public NodeDetailItem[] items;
}

public interface NodeDetailItem { }

[Serializable]
public struct NodeDetailText: NodeDetailItem {
    public string text;
}

[Serializable]
public struct NodeDetailImage: NodeDetailItem {
    
}


