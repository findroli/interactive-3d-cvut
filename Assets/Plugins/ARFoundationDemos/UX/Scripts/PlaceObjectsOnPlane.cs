using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class PlaceObjectsOnPlane: MonoBehaviour {
    public static event Action onPlacedObject;
    
    public GameObject spawnedObject;

    ARRaycastManager m_RaycastManager;

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    private bool m_HasPlacedObject = false;

    int m_NumberOfPlacedObjects = 0;

    void Awake() {
        m_RaycastManager = GetComponent<ARRaycastManager>();
    }

    void Update() {
        if (Input.touchCount > 0 && !EventSystem.current.IsPointerOverGameObject() && !m_HasPlacedObject) {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) {
                if (m_RaycastManager.Raycast(touch.position, s_Hits, TrackableType.PlaneWithinPolygon)) {
                    Pose hitPose = s_Hits[0].pose;
                    spawnedObject.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);
                    m_HasPlacedObject = true;
                    onPlacedObject?.Invoke();
                }
            }
        }
    }
}
