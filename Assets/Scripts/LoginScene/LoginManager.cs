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
        DBManager.shared();
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
                ShowMessage("Sign in failed! Check your credentials or try later.");
            }
        });
    }

    void HandleAfterLoginFirebase(string id, string email, string password) {
        DBManager.shared().GetUser(id, email, password, user => {
            loadingView.SetActive(false);
            if (user.approved != User.Approval.yes) {
                if (user.approved == User.Approval.pending) {
                    ShowMessage("Your account has not yet been approved. Wait or contact your supervisor for more information.");
                } else if (user.approved == User.Approval.declined) {
                    ShowMessage("Your registration has been declined. Contact your supervisor for more information.");
                }
                loginComponent.gameObject.SetActive(true);
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

    void RegisterFirebase(string username, string email, string password) {
        loadingView.SetActive(true);
        registerComponent.gameObject.SetActive(false);
        DBManager.shared().RegisterFirebase(username, email, password, HandleRegisterResponse);
    }

    void HandleRegisterResponse(bool response) {
        loadingView.SetActive(false);
        if (response) {
            ShowMessage("Successfully registered! After your approval, you can sign in.");
            loginComponent.gameObject.SetActive(true);
        }
        else {
            registerComponent.gameObject.SetActive(true);
            registerComponent.RegisterUnsuccessful();
            ShowMessage("Registration failed! Make sure your email is in valid format and your password is at least 6 characters long.");
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
