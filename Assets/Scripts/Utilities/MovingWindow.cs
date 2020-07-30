using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWindow: MonoBehaviour {
    [SerializeField] private LongPressEventTrigger longPress;

    public InputHandler inputHandler;

    public bool IsMoving => isMoving;
    private bool isMoving = false;
    private Vector3 moveOffset;
    
    void Start() {
        inputHandler = FindObjectOfType<InputHandler>();
        longPress.onLongPressStart.AddListener(() => {
            isMoving = true;
            moveOffset = transform.position - inputHandler.inputPosition;
        });
        longPress.onLongPressEnd.AddListener(() => {
            isMoving = false;
        });    
    }

    void Update() {
        if (isMoving) {
            transform.position = inputHandler.inputPosition + moveOffset;
        }
    }
}
