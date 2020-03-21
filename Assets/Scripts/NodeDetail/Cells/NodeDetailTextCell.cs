using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NodeDetailTextCell: NodeDetailCell {
    [SerializeField] private InputField inputField;
    private RectTransform rectTransform;
    
    public void CreatingEnded() {
        inputField.enabled = false;
    }

    public override NodeDetailItem GetData() {
        return new NodeDetailTextItem(inputField.text);
    }

    public override void FillWithData(NodeDetailItem item) {
        var textItem = item as NodeDetailTextItem;
        if (textItem == null) { return; }
        inputField.text = textItem.text;
        OnValueChanged(textItem.text);
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
