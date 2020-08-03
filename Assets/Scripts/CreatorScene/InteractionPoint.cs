using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionPoint: MonoBehaviour {
    public delegate void OnInteractionDelegate(InteractionPoint point);
    public static event OnInteractionDelegate interactionDelegate;

    private CreatorManager creatorManager;

    public void UserTapped() {
        if (interactionDelegate != null) {
            interactionDelegate(this);
        }
    }

    private void Start() {
        creatorManager = GameObject.Find("CreatorManager").GetComponent<CreatorManager>();
    }

    void Update() {
        transform.LookAt(creatorManager.CurrentCamera.transform);
    }
}
