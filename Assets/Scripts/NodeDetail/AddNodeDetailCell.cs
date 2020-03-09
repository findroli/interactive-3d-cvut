 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddNodeDetailCell: MonoBehaviour {
    public delegate void OnCreateClickDelegate();
    public event OnCreateClickDelegate createClickDelegate;
    public delegate void OnCancelClickDelegate();
    public event OnCancelClickDelegate cancelClickDelegate;
    
    public Button addBtn;
    public Button createBtn;
    public Button cancelBtn;

    private bool _creationStarted = false;

    public bool creationStarted {
        get => _creationStarted;
        set {
            _creationStarted = value;
            addBtn.gameObject.SetActive(!_creationStarted);
            cancelBtn.gameObject.SetActive(_creationStarted);
            createBtn.gameObject.SetActive(_creationStarted);
        }
    }

    private void Start() {
        createBtn.onClick.AddListener(CreateTapped);
        cancelBtn.onClick.AddListener(CancelTapped);
    }

    private void CreateTapped() {
        createClickDelegate();
    }

    private void CancelTapped() {
        cancelClickDelegate();
    }
}
