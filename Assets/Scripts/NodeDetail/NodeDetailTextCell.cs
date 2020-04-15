using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NodeDetailTextCell: NodeDetailCell {
    public InputField inputField;
    private RectTransform rectTransform;
    
    public override void CreatingEnded() {
        inputField.enabled = false;
    }

    public override void FillWithData(NodeCellData data) {
        var textData = data as NodeTextCellData;
        if(textData == null) return;
        if (textData.text != "") {
            inputField.text = textData.text;
        }
        Debug.Log("Filled text cell with data: " + textData.text);
    }

    public override NodeCellData GetData() {
        return new NodeTextCellData {
            text = inputField.text
        };
    }

    void Start() {
        inputField = gameObject.GetComponent<InputField>();
        inputField.onValueChanged.AddListener(OnValueChanged);
        rectTransform = gameObject.GetComponent<RectTransform>();
    }

    void OnValueChanged(string newText) {
        //TODO: not properly counting the height of the text
        var newLineCount = inputField.text.Count(c => c == '\n');
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, newLineCount * 16 + 30);
    }
}
