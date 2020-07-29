using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdminPanel: MonoBehaviour {
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private GameObject emptyLabel;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button reloadButton;
    [SerializeField] private TopMenuPanel topPanel;
    [SerializeField] private GameObject loadingView;
    [SerializeField] private GameObject scrollViewContent;

    void Start() {
        cancelButton.onClick.AddListener(() => {
            topPanel.BackToProjects();
        });
        reloadButton.onClick.AddListener(LoadUsers);
        LoadUsers();
    }

    private void Update() {
        emptyLabel.SetActive(scrollViewContent.transform.childCount == 0 && !loadingView.activeInHierarchy);
    }

    private void OnEnable() {
        AdminListCell.onAccept += OnAccept;
        AdminListCell.onDecline += OnDecline;
    }

    private void OnDisable() {
        AdminListCell.onAccept -= OnAccept;
        AdminListCell.onDecline -= OnDecline;
    }

    void LoadUsers() {
        for(var i = 0; i < scrollViewContent.transform.childCount; i++) {
            Destroy(scrollViewContent.transform.GetChild(i).gameObject);
        }
        
        loadingView.SetActive(true);
        DBManager.shared().LoadPendingUsers((success, users) => {
            loadingView.SetActive(false);
            if (!success) {
                Helper.CreateMessagePopup("Oops! Loading users failed. Try refreshing.");
                return;
            }
            foreach (var user in users) {
                var cell = Instantiate(cellPrefab, scrollViewContent.transform).GetComponent<AdminListCell>();
                cell.SetName(user.name);
                cell.SetId(user.id);
            }
        });
    }

    void OnAccept(string id, AdminListCell cell) {
        DBManager.shared().AcceptUser(id, success => {
            if (success) {
                Destroy(cell.gameObject);
            }
        });
    }
    
    void OnDecline(string id, AdminListCell cell) {
        DBManager.shared().DeclineUser(id, success => {
            if (success) {
                Destroy(cell.gameObject);
            }
        });
    }
}
