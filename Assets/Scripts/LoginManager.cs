using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour {
    [SerializeField] private GameObject modePickPanel;

    [SerializeField] private Button presentBtn;
    [SerializeField] private Button editModeBtn;

    [SerializeField] private LoginCompoment loginComponent;
    [SerializeField] private RegisterComponent registerComponent;
    
    void Start() {
        loginComponent.onLogin += Login;
        loginComponent.toRegister += () => {
            loginComponent.gameObject.SetActive(false);
            registerComponent.gameObject.SetActive(true);
        };
        registerComponent.onRegister += Register;
        registerComponent.toLogin += () => {
            loginComponent.gameObject.SetActive(true);
            registerComponent.gameObject.SetActive(false);
        };
        presentBtn.onClick.AddListener(StartInPresentationMode);
        editModeBtn.onClick.AddListener(StartInEditMode);
    }

    void Login(string username, string password) {
        var user = DBManager.shared().Login(username, password);
        if (user != null) {
            AppState.shared().CurrentUser = user;
            if (user.Value.userType == User.UserType.presenter) {
                StartInPresentationMode();
            }
            else {
                loginComponent.gameObject.SetActive(false);
                modePickPanel.SetActive(true);
            }
        }
        else {
            loginComponent.LoginUnsuccesful();
        }
    }

    void Register(string username, string password, string companyCode) {
        var response = DBManager.shared().Register(username, password, companyCode);
        if (response) {
            registerComponent.gameObject.SetActive(false);
            loginComponent.gameObject.SetActive(true);
        }
        else {
            registerComponent.RegisterUnsuccessful();
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
