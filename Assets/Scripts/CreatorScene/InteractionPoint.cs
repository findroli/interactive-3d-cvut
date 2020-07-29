using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionPoint: MonoBehaviour {
    public delegate void OnInteractionDelegate(InteractionPoint point);
    public static event OnInteractionDelegate interactionDelegate;

    private CreatorManager creatorManager;
    private SpriteRenderer spriteRenderer;

    private void Start() {
        creatorManager = GameObject.Find("CreatorManager").GetComponent<CreatorManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update() {
        transform.LookAt(creatorManager.CurrentCamera.transform);
        //transparency just when behind object
        /*var direction = (creatorManager.CurrentCamera.transform.position - transform.position).normalized;
        Ray ray = new Ray(transform.position, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 500.0f)) {
            if (spriteRenderer.color.a > 0.95) {
                var newColor = spriteRenderer.color;
                newColor.a = 0.55f;
                spriteRenderer.color = newColor;
            }
            
        }
        else if (spriteRenderer.color.a < 0.6) {
            var newColor = spriteRenderer.color;
            newColor.a = 1;
            spriteRenderer.color = newColor;
        }*/
    }

    private void OnMouseDown() {
        if (interactionDelegate != null) {
            interactionDelegate(this);
        }
    }
}
