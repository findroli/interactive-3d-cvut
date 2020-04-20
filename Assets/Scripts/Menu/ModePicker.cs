using System;
using UnityEngine;
using UnityEngine.UI;

public enum AppMode {
    Presentation, Edit
}

public class ModePicker: MonoBehaviour {
    [SerializeField] private Button presentationBtn;
    [SerializeField] private Button editModeBtn;
    [SerializeField] private GameObject presentationUnderline;
    [SerializeField] private GameObject editModeUnderline;

    private AppMode appMode = AppMode.Edit;
    public AppMode CurrentAppMode => appMode;

    void Start() {
        ModeChanged();
        presentationBtn.onClick.AddListener(() => {
            appMode = AppMode.Presentation;
            ModeChanged();
        });
        editModeBtn.onClick.AddListener(() => {
            appMode = AppMode.Edit;
            ModeChanged();
        });
    }

    private void ModeChanged() {
        presentationUnderline.SetActive(appMode == AppMode.Presentation);
        editModeUnderline.SetActive(appMode == AppMode.Edit);
        presentationBtn.interactable = appMode != AppMode.Presentation;
        editModeBtn.interactable = appMode != AppMode.Edit;
    }
}
