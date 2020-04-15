using UnityEngine;

public class InformationNode {
    string id;
    Vector3 position;
    NodeDetailItem[] nodeDetailItems;

    InformationNode(string id, Vector3 position, NodeDetailItem[] nodeDetailItems) {
        this.id = id;
        this.position = position;
        this.nodeDetailItems = nodeDetailItems;
    }
}