 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddNodeDetailCell: MonoBehaviour {
    public delegate void OnTextCreateDelegate();
    public event OnTextCreateDelegate textCreateDelegate;
    public delegate void OnImageCreateDelegate();
    public event OnImageCreateDelegate imageCreateDelegate;
    public delegate void OnVideoCreateDelegate();
    public event OnVideoCreateDelegate videoCreateDelegate;
    public delegate void OnConfirmCreateDelegate();
    public event OnConfirmCreateDelegate confirmCreateDelegate;
    public delegate void OnCancelCreateDelegate();
    public event OnCancelCreateDelegate cancelCreateDelegate;
    
    public Button addBtn;
    public Button textBtn;
    public Button imageBtn;
    public Button videoBtn;
    public Button confirmBtn;
    public Button cancelBtn;

    private CreationState _creationState = CreationState.add;
    
    public CreationState CreationState {
        get => _creationState;
        set {
            _creationState = value;
            addBtn.gameObject.SetActive(_creationState == CreationState.add);
            textBtn.gameObject.SetActive(_creationState == CreationState.chooseType);
            imageBtn.gameObject.SetActive(_creationState == CreationState.chooseType);
            videoBtn.gameObject.SetActive(_creationState == CreationState.chooseType);
            confirmBtn.gameObject.SetActive(_creationState == CreationState.confirm);
            cancelBtn.gameObject.SetActive(_creationState == CreationState.confirm);
        }
    }

    private void Start() {
        addBtn.onClick.AddListener(() => { CreationState = CreationState.chooseType; });
        textBtn.onClick.AddListener(() => {
            CreationState = CreationState.confirm;
            textCreateDelegate?.Invoke();
        });
        imageBtn.onClick.AddListener(() => {
            CreationState = CreationState.confirm;
            imageCreateDelegate?.Invoke();
        });
        videoBtn.onClick.AddListener(() => {
            CreationState = CreationState.confirm;
            videoCreateDelegate?.Invoke();
        });
        confirmBtn.onClick.AddListener(() => {
            CreationState = CreationState.add;
            confirmCreateDelegate?.Invoke();
        });
        cancelBtn.onClick.AddListener(() => {
            CreationState = CreationState.add;
            cancelCreateDelegate?.Invoke();
        });
    }
}

 public enum CreationState {
     add,
     chooseType,
     confirm
 }
