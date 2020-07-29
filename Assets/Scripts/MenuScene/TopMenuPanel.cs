using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TopMenuPanel: MonoBehaviour {
    [SerializeField] private Text username;
    [SerializeField] private Text modeText;
    [SerializeField] private Button profileButton;
    
    [SerializeField] private GameObject menuObject;
    [SerializeField] private Button switchModeButton;
    [SerializeField] private Button logoutButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button adminPanelButton;
    [SerializeField] private Text adminButtonText;

    [SerializeField] private GameObject adminPanel;

    public void BackToProjects() {
        adminPanel.SetActive(false);
        adminButtonText.text = "Users panel";
    }
    
    private void Start() {
        menuObject.SetActive(false);
        UpdateUI();
        adminPanelButton.onClick.AddListener(() => {
            menuObject.SetActive(false);
            if (adminPanel.activeInHierarchy) {
                BackToProjects();
            }
            else {
                adminPanel.SetActive(true);
                adminButtonText.text = "Products";
            }
        });
        profileButton.onClick.AddListener(() => {
            menuObject.SetActive(!menuObject.activeInHierarchy);
        });
        exitButton.onClick.AddListener(() => {
            DBManager.shared().LogoutCurrent();
            Application.Quit();
        });
        logoutButton.onClick.AddListener(() => {
            AppState.shared().CurrentUser = null;
            DBManager.shared().LogoutCurrent();
            SceneManager.LoadScene("LoginScene");
        });
        if (AppState.shared().CurrentUser?.userType == User.UserType.presenter) {
            switchModeButton.gameObject.SetActive(false);
        }
        else {
            switchModeButton.onClick.AddListener(() => {
                if (AppState.shared().Mode == AppMode.Edit) {
                    ChangeMode(AppMode.Presentation);
                    return;
                }
                Helper.CreatePasswordPopup(success => {
                    if (success) ChangeMode(AppMode.Edit);
                });
            });
        }
    }

    private void ChangeMode(AppMode mode) {
        AppState.shared().Mode = mode;
        menuObject.SetActive(false);
    }
    
    private void OnEnable() {
        AppState.shared().onModeChange += UpdateUI;
    }
    
    private void OnDisable() {
        AppState.shared().onModeChange -= UpdateUI;
    }

    public void UpdateUI() {
        var appState = AppState.shared();
        username.text = appState.CurrentUser.HasValue ? appState.CurrentUser.Value.name : "";
        modeText.text = appState.Mode == AppMode.Edit ? "Edit mode" : "Presentation mode";
    }
}
