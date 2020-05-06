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
    [SerializeField] private Button logoutButton;
    [SerializeField] private Button exitButton;

    private void Start() {
        menuObject.SetActive(false);
        UpdateUI();
        profileButton.onClick.AddListener(() => {
            menuObject.SetActive(!menuObject.activeInHierarchy);
        });
        exitButton.onClick.AddListener(() => {
            Application.Quit();
        });
        logoutButton.onClick.AddListener(() => {
            AppState.shared().currentUser = null;
            SceneManager.LoadScene("LoginScene");
        });
    }

    public void UpdateUI() {
        username.text = AppState.shared().currentUser.Value.username;
        modeText.text = AppState.shared().mode == AppMode.Edit ? "Edit mode" : "Presentation mode";
    }
}
