using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PasswordPopup: MonoBehaviour {
    public delegate void OnDone(bool success);
    public event OnDone onDone;
    
    [SerializeField] private Button doneBtn;
    [SerializeField] private Button cancelBtn;
    [SerializeField] private InputField passwordField;

    void Start() {
        doneBtn.onClick.AddListener(() => {
            var currentUser = AppState.shared().CurrentUser;
            if (currentUser != null && currentUser.Value.password == passwordField.text) {
                onDone?.Invoke(true);
                Destroy(gameObject);
            }
            else {
                doneBtn.GetComponent<ObjectShaker>().Shake();
            }
        });
        cancelBtn.onClick.AddListener(() => {
            onDone?.Invoke(false);
            Destroy(gameObject);
        });
    }
    
    
}
