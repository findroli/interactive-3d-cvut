using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager: MonoBehaviour {
    public delegate void OnRotateModel(Vector3 directions);
    public event OnRotateModel onRotateModel;

    public delegate void OnZoomModel(float value);
    public event OnZoomModel onZoomModel;

    private bool swiping = false;
    private bool pinching = false;
    private Vector3 touchPos;
    private Vector3 screenCenter;
    
    void Start() {
        screenCenter = new Vector3(Screen.width/2f, Screen.height/2f, 0); 
    }

    void Update() {
        if (swiping) {
            var diff = Input.mousePosition - touchPos;
            if (diff.magnitude > 0.2) {
                var directions = new Vector3(diff.y, -diff.x, diff.z);
                onRotateModel?.Invoke(directions);
                Debug.Log("Rotation value: " + diff);
                touchPos = Input.mousePosition;
            }
        } else if (pinching) {
            var diff = Input.mousePosition - touchPos;
            if (diff.magnitude > 0.2) {
                if ((touchPos - screenCenter).magnitude > (Input.mousePosition - screenCenter).magnitude) {
                    onZoomModel?.Invoke(diff.magnitude);
                } else {
                    onZoomModel?.Invoke(-diff.magnitude);
                }
                Debug.Log("Zoom value: " + diff.magnitude);
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
