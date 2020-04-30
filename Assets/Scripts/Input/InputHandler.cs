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
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
        inputManager = gameObject.AddComponent<PCInputManager>();
#elif UNITY_IOS || UNITY_ANDROID
        inputManager = gameObject.AddComponent<MobileInputManager>();
#else
        Debug.Log("InputHandler.Start(): ERROR - Device is not supported!");
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
