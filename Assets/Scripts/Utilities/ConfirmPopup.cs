using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmPopup: MonoBehaviour {
    public delegate void OnConfirm();
    public event OnConfirm onConfirm;
    
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button xButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Text popupText;

    public void SetText(string text) {
        popupText.text = text;
    }

    public void SetConfirmText(string text) {
        confirmButton.GetComponentInChildren<Text>().text = text;
    }
    
    void Start() {
        confirmButton.onClick.AddListener(Confirm);
        xButton.onClick.AddListener(Cancel);
        cancelButton.onClick.AddListener(Cancel);
    }

    void Confirm() {
        onConfirm?.Invoke();
        Destroy(gameObject);
    }

    void Cancel() {
        Destroy(gameObject);
    }
}
