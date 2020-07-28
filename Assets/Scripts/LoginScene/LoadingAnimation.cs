using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingAnimation : MonoBehaviour
{
    [SerializeField] private RectTransform movingDotRectTransform;
    private float rotateSpeed = 200f;
    
    void Update() {
         movingDotRectTransform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }
}
