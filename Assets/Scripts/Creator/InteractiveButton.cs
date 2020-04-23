using UnityEngine;
using UnityEngine.UI;

public class InteractiveButton : Button {
    private bool _selected;
    public bool selected {
        get { return _selected; }
        set { setSelection(value); }
    }
    
    private Color selectedColor;
    private Color unselectedColor;

    void Start() {
        unselectedColor = GetComponent<Image>().color;
        selectedColor = Color.black;
    }

    void setSelection(bool value) {
        _selected = value;
        GetComponent<Image>().color = _selected ? selectedColor : unselectedColor;
    }

}
