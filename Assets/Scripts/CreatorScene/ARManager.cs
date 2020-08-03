using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class ARManager: MonoBehaviour {
    [SerializeField] private ARPlaneManager arPlaneManager;
    [SerializeField] private GameObject tipObject;
    [SerializeField] private ARKitCoachingOverlay coachingOverlay;
    
    public delegate void OnPlacedObject();
    public static event OnPlacedObject onPlacedObject;

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
    
    ARRaycastManager m_RaycastManager;
    private CreatorManager m_CreatorManager;
    private bool m_HasPlacedObject = false;
    int m_NumberOfPlacedObjects = 0;

    void Awake() {
        m_RaycastManager = GetComponent<ARRaycastManager>();
        m_CreatorManager = GameObject.Find("CreatorManager").GetComponent<CreatorManager>();
        arPlaneManager.planesChanged += (changeEvents) => {
            if (changeEvents.added.Any()) {
                tipObject.GetComponentInChildren<Text>().text =
                    "After you found a plane you can place your model by tapping on it";
                tipObject.SetActive(true);
            }
        };
    }

    void Update() {
        if (Input.touchCount > 0 && !EventSystem.current.IsPointerOverGameObject() && !m_HasPlacedObject) {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) {
                if (m_RaycastManager.Raycast(touch.position, s_Hits, TrackableType.PlaneWithinPolygon)) {
                    Pose hitPose = s_Hits[0].pose;
                    m_CreatorManager.model.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);
                    m_HasPlacedObject = true;
                    tipObject.SetActive(false);
                    coachingOverlay.activatesAutomatically = false;
                    onPlacedObject?.Invoke();
                }
            }
        }
    }

    private void OnDisable() {
        m_HasPlacedObject = false;
    }
}
