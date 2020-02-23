using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public delegate void OnRotateModel(Vector3 directions);
    public static event OnRotateModel onRotateModel;
    
    void Start() {
        
    }

    void Update() {
        if (onRotateModel != null) {
            var rotation = Vector3.zero;
            if (Input.GetKey(KeyCode.LeftArrow)) {
                rotation.y -= 1f;
            }
            if (Input.GetKey(KeyCode.RightArrow)) {
                rotation.y += 1f;
            }
            if (Input.GetKey(KeyCode.UpArrow)) {
                rotation.x += 1f;
            }
            if (Input.GetKey(KeyCode.DownArrow)) {
                rotation.x -= 1f;
            }
            onRotateModel(rotation);
        }
        
    }
}
