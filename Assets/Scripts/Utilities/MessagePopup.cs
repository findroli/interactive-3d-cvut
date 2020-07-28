using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessagePopup: MonoBehaviour {
    [SerializeField] private Button doneButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Text messageText;

    public void SetText(string text) {
        messageText.text = text;
    }
    
    void Start() {
        doneButton.onClick.AddListener(Exit);
        cancelButton.onClick.AddListener(Exit);
    }

    void Exit() {
        Destroy(gameObject);
    }
}
