using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionPoint: MonoBehaviour {
    public delegate void OnInteractionDelegate(InteractionPoint point);
    public static event OnInteractionDelegate interactionDelegate;
    
    void Update() {
        transform.LookAt(Camera.main.transform);
    }

    private void OnMouseDown() {
        if (interactionDelegate != null) {
            interactionDelegate(this);
        }
    }
}
