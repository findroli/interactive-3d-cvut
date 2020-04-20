using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ViewMode {
    viewAR, view3D
}

public class ViewModePicker : MonoBehaviour {
    public delegate void OnViewModeChanged(ViewMode viewMode);
    public event OnViewModeChanged onViewModeChanged;
    
    [SerializeField] private Button arBtn;
    [SerializeField] private Button normalViewBtn;

    private ViewMode viewMode = ViewMode.view3D;
    public ViewMode CurrentViewMode => viewMode;

    private Color tintColor;
    private Color otherColor = Color.white;
    private Color notSelectedColor = Color.white;
    
    void Start() {
        tintColor = arBtn.GetComponent<Image>().color;
        notSelectedColor.a = 0.2f;
        ModeChanged();
        
        arBtn.onClick.AddListener(() => {
            viewMode = ViewMode.viewAR;
            ModeChanged();
        });
        normalViewBtn.onClick.AddListener(() => {
            viewMode = ViewMode.view3D;
            ModeChanged();
        });
    }

    void ModeChanged() {
        onViewModeChanged?.Invoke(viewMode);
        arBtn.GetComponent<Image>().color = viewMode == ViewMode.viewAR ? tintColor : notSelectedColor;
        normalViewBtn.GetComponent<Image>().color = viewMode == ViewMode.view3D ? tintColor : notSelectedColor;
        arBtn.GetComponentInChildren<Text>().color = viewMode == ViewMode.viewAR ? otherColor : tintColor;
        normalViewBtn.GetComponentInChildren<Text>().color = viewMode == ViewMode.view3D ? otherColor : tintColor;
    }
}
