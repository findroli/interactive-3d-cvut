using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour {
    [SerializeField] private Button loginBtn;
    [SerializeField] private InputField nameField;
    [SerializeField] private InputField passwordField;
    
    void Start() {
        loginBtn.onClick.AddListener(Login);
    }

    void Login() {
        var username = nameField.text;
        var password = passwordField.text;

        if (DBManager.shared().Login(username, password)) {
            SceneManager.LoadScene("MainMenuScene");
        }
    }
}
