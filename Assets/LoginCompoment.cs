using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginCompoment : MonoBehaviour
{
    public delegate void OnLogin(string username, string password);
    public event OnLogin onLogin;
    public delegate void ToRegister();
    public event ToRegister toRegister;

    [SerializeField] private Button loginBtn;
    [SerializeField] private InputField nameField;
    [SerializeField] private InputField passwordField;
    [SerializeField] private Button toRegisterBtn;

    public void LoginUnsuccesful() {
        loginBtn.GetComponent<ObjectShaker>().Shake();
    }

    void Start() {
        loginBtn.onClick.AddListener(OnLoginTapped);
        toRegisterBtn.onClick.AddListener(() => { toRegister?.Invoke(); });
    }

    private void OnLoginTapped() {
        var username = nameField.text;
        var password = passwordField.text;
        if (username == "" || password == "") {
            LoginUnsuccesful();
            return;
        }
        onLogin?.Invoke(username, password);
    }
}
