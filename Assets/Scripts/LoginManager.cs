using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour {
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject modePickPanel;

    [SerializeField] private Button loginBtn;
    [SerializeField] private InputField nameField;
    [SerializeField] private InputField passwordField;

    [SerializeField] private Button presentBtn;
    [SerializeField] private Button editModeBtn;
    
    void Start() {
        loginBtn.onClick.AddListener(Login);
        presentBtn.onClick.AddListener(StartInPresentationMode);
        editModeBtn.onClick.AddListener(StartInEditMode);
    }

    void Login() {
        var username = nameField.text;
        var password = passwordField.text;

        var user = DBManager.shared().Login(username, password);
        if (user != null) {
            AppState.shared().CurrentUser = user;
            if (user.Value.userType == User.UserType.presenter) {
                StartInPresentationMode();
            }
            else {
                loginPanel.SetActive(false);
                modePickPanel.SetActive(true);
            }
        }
        else {
            loginBtn.GetComponent<ObjectShaker>().Shake();
        }
    }

    void StartInPresentationMode() {
        AppState.shared().Mode = AppMode.Presentation;
        SceneManager.LoadScene("MainMenuScene");
    }

    void StartInEditMode() {
        AppState.shared().Mode = AppMode.Edit;
        SceneManager.LoadScene("MainMenuScene");
    }
}
