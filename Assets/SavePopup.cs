using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavePopup : MonoBehaviour {
    public delegate void OnSave(string name);
    public event OnSave onSave;

    [SerializeField] private Button saveButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private InputField inputField;

    public void SetPlaceHolder(string newName) {
        inputField.text = newName;
    }
    
    void Start() {
        cancelButton.onClick.AddListener(Cancel);
        saveButton.onClick.AddListener(Save);
    }

    void Cancel() {
        Destroy(gameObject);
    }

    void Save() {
        Debug.Log("save tapped!");
        var text = inputField.GetComponentInChildren<Text>().text;
        if(string.IsNullOrEmpty(text)) {
            inputField.GetComponent<ObjectShaker>().Shake();
        } else {
            onSave?.Invoke(text);
            Destroy(gameObject);
        }
    }
}
