using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdminPanel: MonoBehaviour {
    [SerializeField] private GameObject emptyLabel;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TopMenuPanel topPanel;

    void Start() {
        cancelButton.onClick.AddListener(() => {
            topPanel.BackToProjects();
        });
    }
}
