using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PCInputManager: MonoBehaviour, IAnyInputManager {
    public event OnSwipe onSwipe;
    public event OnPinch onPinch;

    private bool swiping = false;
    private bool pinching = false;
    private Vector3 touchPos;
    private Vector3 screenCenter;
    
    public Vector3 GetInputPosition() {
        return Input.mousePosition;
    }
    
    void Start() {
        screenCenter = new Vector3(Screen.width/2f, Screen.height/2f, 0); 
    }

    void Update() {
        if (swiping) {
            var diff = Input.mousePosition - touchPos;
            if (diff.magnitude > 0.2) {
                var directions = new Vector3(-diff.y, -diff.x, diff.z);
                onSwipe?.Invoke(directions);
                touchPos = Input.mousePosition;
            }
        } else if (pinching) {
            var diff = Input.mousePosition - touchPos;
            if (diff.magnitude > 0.2) {
                if ((touchPos - screenCenter).magnitude > (Input.mousePosition - screenCenter).magnitude) {
                    onPinch?.Invoke(diff.magnitude);
                } else {
                    onPinch?.Invoke(-diff.magnitude);
                }
                touchPos = Input.mousePosition;
            }
        }

        if (!swiping && !pinching && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
            if (Input.GetKey(KeyCode.LeftShift)) pinching = true;
            else swiping = true;
            touchPos = Input.mousePosition;
        }
        if ((swiping || pinching) && Input.GetMouseButtonUp(0)) {
            swiping = false;
            pinching = false;
        }
    }
}