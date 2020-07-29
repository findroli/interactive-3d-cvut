using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Unity.Editor;
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
                Debug.Log("Firebase init failed!");
            }
        });
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://interactive3d-25d90.firebaseio.com/");
        SetUpLoginAndRegister();
        presentBtn.onClick.AddListener(StartInPresentationMode);
        editModeBtn.onClick.AddListener(StartInEditMode);
    }

    private void SetUpLoginAndRegister() {
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
    }

    void LoginFirebase(string email, string password) {
        loadingView.SetActive(true);
        loginComponent.gameObject.SetActive(false);
        DBManager.shared().LoginFirebase(email, password, (success, data) => {
            if (success) {
                HandleAfterLoginFirebase(data, email, password);
            }
            else {
                loadingView.SetActive(false);
                loginComponent.gameObject.SetActive(true);
                loginComponent.LoginUnsuccesful();
                ShowMessage("Login was not successful!");
            }
        });
    }

    void HandleAfterLoginFirebase(string id, string email, string password) {
        DBManager.shared().GetUser(id, email, password, user => {
            loadingView.SetActive(false);
            if (!user.approved) {
                ShowMessage("Your account has not yet been approved. Contact your supervisor for more information.");
                return;
            }
            AppState.shared().CurrentUser = user;
            if (user.userType == User.UserType.presenter) {
                StartInPresentationMode();
            }
            else {
                modePickPanel.SetActive(true);
            }
        });
    }

    void RegisterFirebase(string email, string password) {
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
            ShowMessage("Register was not successful!");
        }
    }

    void ShowMessage(string message) {
        Helper.CreateMessagePopup(message);
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
