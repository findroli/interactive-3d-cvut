using UnityEngine;

public delegate void OnSwipe(Vector3 offset);
public delegate void OnPinch(float distance);

public interface IAnyInputManager {
    event OnSwipe onSwipe;
    event OnPinch onPinch;
    Vector3 GetInputPosition();
}