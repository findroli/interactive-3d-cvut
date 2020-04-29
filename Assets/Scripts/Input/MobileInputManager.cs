using UnityEngine;
using UnityEngine.EventSystems;

public class MobileInputManager: MonoBehaviour, IAnyInputManager {
    public event OnSwipe onSwipe;
    public event OnPinch onPinch;
    
    private bool swiping = false;
    private bool pinching = false;
    private Vector3 touchPos;
    private float touchDistance;
    private Vector3 screenCenter;

    public Vector3 GetInputPosition() {
        return Input.touchCount > 0 ? Input.touches[0].position : Vector2.zero;
    }
    
    void Start() {
        screenCenter = new Vector3(Screen.width/2f, Screen.height/2f, 0); 
    }

    void Update() {
        UpdateGestures(Input.touchCount);
        
        if (swiping) {
            Vector3 pos = Input.touches[0].position;
            var diff = pos - touchPos;
            if (diff.magnitude > 0.2) {
                var directions = new Vector3(diff.y, -diff.x, diff.z);
                onSwipe?.Invoke(directions);
                Debug.Log("Rotation value: " + diff);
                touchPos = Input.mousePosition;
            }
        }
        else if (pinching) {
            var diff = (Input.touches[0].position - Input.touches[1].position).magnitude - touchDistance;
            if (Mathf.Abs(diff) > 0.2) {
                onPinch?.Invoke(diff/2f);
                Debug.Log("Zoom value: " + diff/2);
                touchDistance = (Input.touches[0].position - Input.touches[1].position).magnitude;
            }
        }
    }

    private void UpdateGestures(int touchCount) {
        switch (touchCount) {
            case 2: {
                if (!pinching && !EventSystem.current.IsPointerOverGameObject()) {
                    swiping = false;
                    pinching = true;
                    touchDistance = (Input.touches[0].position - Input.touches[1].position).magnitude;
                }
                break;
            }
            case 1: {
                if (!swiping && !EventSystem.current.IsPointerOverGameObject()) {
                    pinching = false;
                    swiping = true;
                    touchPos = Input.touches[0].position;
                }
                break;
            }
            case 0: {
                swiping = false;
                pinching = false;
                break;
            }
        } 
    }
}