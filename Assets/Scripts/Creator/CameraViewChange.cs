using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CameraViewChange: MonoBehaviour {
    public enum CameraDefaultView {
        top, side, front
    }
    
    public delegate void OnViewChange(CameraDefaultView view);
    public event OnViewChange onViewChange;
    
    [SerializeField] private Button changeBtn;
    [SerializeField] private Button topBtn;
    [SerializeField] private Button sideBtn;
    [SerializeField] private Button frontBtn;
    [SerializeField] private GameObject buttonsObject;

    private bool buttonsHidden = true;
    private Coroutine[] currentCoroutines = new Coroutine[] {};

    public void SetButtonsHidden(bool value) {
        buttonsHidden = value;
        UpdateButtons();
    }
    
    void Start() {
        topBtn.onClick.AddListener(() => onViewChange?.Invoke(CameraDefaultView.top));
        sideBtn.onClick.AddListener(() => onViewChange?.Invoke(CameraDefaultView.side));
        frontBtn.onClick.AddListener(() => onViewChange?.Invoke(CameraDefaultView.front));
        changeBtn.onClick.AddListener(() => SetButtonsHidden(!buttonsHidden));
    }

    void UpdateButtons() {
        foreach(var coroutine in currentCoroutines) {
            StopCoroutine(coroutine);
        }
        
        buttonsObject.SetActive(!buttonsHidden);
        var basePos = changeBtn.transform.position;
        var topPos = buttonsHidden ? basePos : new Vector3(basePos.x, basePos.y + 80, basePos.z);
        var sidePos = buttonsHidden ? basePos : new Vector3(basePos.x + 60, basePos.y + 60, basePos.z);
        var frontPos = buttonsHidden ? basePos : new Vector3(basePos.x + 80, basePos.y, basePos.z);

        currentCoroutines = new Coroutine[] {
            StartCoroutine(Helper.AnimateMovement(topBtn.transform, 10f, topPos)),
            StartCoroutine(Helper.AnimateMovement(sideBtn.transform, 10f, sidePos)),
            StartCoroutine(Helper.AnimateMovement(frontBtn.transform, 10f, frontPos))
        };
    }
}
