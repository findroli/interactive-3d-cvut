﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterComponent : MonoBehaviour
{
    public delegate void OnRegister(string username, string email, string password);
    public event OnRegister onRegister;
    public delegate void ToLogin();
    public event ToLogin toLogin;

    [SerializeField] private Button registerBtn;
    [SerializeField] private InputField nameField;
    [SerializeField] private InputField emailField;
    [SerializeField] private InputField passwordField;
    [SerializeField] private InputField password2Field;
    [SerializeField] private Button toLoginBtn;

    public void RegisterUnsuccessful() {
        registerBtn.GetComponent<ObjectShaker>().Shake();
    }
    
    void Start() {
        registerBtn.onClick.AddListener(Register);
        toLoginBtn.onClick.AddListener(() => { toLogin?.Invoke(); });
    }

    void Register() {
        var username = nameField.text;
        var email = emailField.text;
        var password = passwordField.text;
        var password2 = password2Field.text;

        if (email == "" || username == "" || password == "" || password2 == "" || password != password2) {
            RegisterUnsuccessful();
            return;
        }
        
        onRegister?.Invoke(username, email, password);
    }
}
