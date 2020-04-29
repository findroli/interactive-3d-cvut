using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputHandler: MonoBehaviour {
    public delegate void OnRotateModel(Vector3 directions);
    public event OnRotateModel onRotateModel;

    public delegate void OnZoomModel(float value);
    public event OnZoomModel onZoomModel;
    
    public Vector3 inputPosition => inputManager.GetInputPosition();

    private IAnyInputManager inputManager;
    
    private void Start() {
#if UNITY_EDITOR
        inputManager = gameObject.AddComponent<PCInputManager>();
#endif
#if !UNITY_EDITOR && UNITY_IOS
        inputManager = gameObject.AddComponent<MobileInputManager>();
#endif
        if (inputManager != null) {
            inputManager.onPinch += HandlePinchGesture;
            inputManager.onSwipe += HandleSwipeGesture;
        }
    }

    private void HandleSwipeGesture(Vector3 moveOffset) {
        onRotateModel?.Invoke(moveOffset);
    }

    private void HandlePinchGesture(float pointsDistance) {
        onZoomModel?.Invoke(pointsDistance);
    }
}
