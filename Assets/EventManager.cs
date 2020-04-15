using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour {
    public delegate void NodeCreateAction(InformationNode node);
    public static event NodeCreateAction OnNodeCreated;
    
    public delegate void NodeUpdateAction(InformationNode node);
    public static event NodeUpdateAction OnNodeUpdated;
    
    public delegate void NodeDeleteAction(string nodeId);
    public static event NodeDeleteAction OnNodeDeleted;

    public delegate void PlacingNodeChange(bool value);
    public static event PlacingNodeChange OnPlacingNodeChanged;

    public static void NodeCreated(InformationNode node) {
        OnNodeCreated?.Invoke(node);
    }
    
    public static void NodeUpdated(InformationNode node) {
        OnNodeUpdated?.Invoke(node);
    }

    public static void NodeDeleted(string id) {
        OnNodeDeleted?.Invoke(id);
    }

    public static void PlacingNodeChanged(bool value) {
        OnPlacingNodeChanged?.Invoke(value);
    }
}
