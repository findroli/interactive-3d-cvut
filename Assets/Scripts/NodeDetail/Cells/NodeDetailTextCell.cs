using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class NodeDetailTextCell: NodeDetailCell {
    [SerializeField] private Button deleteBtn;
    public InputField inputField;
    private RectTransform rectTransform;
    private float scaleFactor;
    
    void Start() {
        scaleFactor = GameObject.Find("Canvas").GetComponent<CanvasScaler>().scaleFactor;
        Debug.Log("=== scale factor = " + scaleFactor);
        inputField = gameObject.GetComponent<InputField>();
        inputField.onValueChanged.AddListener(OnValueChanged);
        rectTransform = gameObject.GetComponent<RectTransform>();
        deleteBtn.onClick.AddListener(() => { RaiseOnDelete(this); });
    }
    
    public override void CreatingEnded() {
        inputField.enabled = false;
        deleteBtn.gameObject.SetActive(false);
    }

    public override void FillWithData(NodeCellData data) {
        var textData = data as NodeTextCellData;
        if(textData == null) return;
        Debug.Log("Filling text cell with: " + textData.text);
        if (textData.text != "") {
            inputField.text = textData.text;
        }
    }

    public override NodeCellData GetData() {
        return new NodeTextCellData {
            text = inputField.text
        };
    }

    void OnValueChanged(string newText) {
        GetComponent<ContentSizeFitter>().SetLayoutVertical();
    }
}
