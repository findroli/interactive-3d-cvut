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
    [SerializeField] private GameObject loadingView;
    
    void Start() {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available) {
                Debug.Log("Firebase ready to use!");
            } else {
                Debug.Log("Firebase init failed");
            }
        });
        loginComponent.onLogin += LoginFirebase;
        loginComponent.toRegister += () => {
            loginComponent.gameObject.SetActive(false);
            registerComponent.gameObject.SetActive(true);
        };
        registerComponent.onRegister += RegisterFirebase;
        registerComponent.toLogin += () => {
            loginComponent.gameObject.SetActive(true);
            registerComponent.gameObject.SetActive(false);
        };
        presentBtn.onClick.AddListener(StartInPresentationMode);
        editModeBtn.onClick.AddListener(StartInEditMode);
    }

    void LoginFirebase(string email, string password) {
        loadingView.SetActive(true);
        loginComponent.gameObject.SetActive(false);
        DBManager.shared().LoginFirebase(email, password, response => {
            loadingView.SetActive(false);
            if (response) {
                AppState.shared().CurrentUser = new User(email, password, User.UserType.admin);
                //if (user.Value.userType == User.UserType.presenter) {
                //    StartInPresentationMode();
                //}
                //else {
                    modePickPanel.SetActive(true);
                //}
            }
            else {
                loginComponent.gameObject.SetActive(true);
                loginComponent.LoginUnsuccesful();
            }
        });
    }

    void RegisterFirebase(string email, string password, string companyCode) {
        loadingView.SetActive(true);
        registerComponent.gameObject.SetActive(false);
        DBManager.shared().RegisterFirebase(email, password, HandleRegisterResponse);
    }

    void HandleRegisterResponse(bool response) {
        loadingView.SetActive(false);
        if (response) {
            loginComponent.gameObject.SetActive(true);
        }
        else {
            registerComponent.gameObject.SetActive(true);
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
