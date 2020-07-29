 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddNodeDetailCell: MonoBehaviour {
    public delegate void OnTextCreateDelegate();
    public event OnTextCreateDelegate TextCreateDelegate;
    public delegate void OnImageCreateDelegate();
    public event OnImageCreateDelegate ImageCreateDelegate;
    public delegate void OnVideoCreateDelegate();
    public event OnVideoCreateDelegate VideoCreateDelegate;
    public delegate void OnAnimationCreate();
    public event OnAnimationCreate AnimationCreateDelegate;
    
    public Button addBtn;
    public Button textBtn;
    public Button imageBtn;
    public Button videoBtn;
    public Button animBtn;
    public Button cancelBtn;

    private float buttonSpacing = 70f;
    private float animationSpeed = 10f;
    
    private CreationState _creationState = CreationState.add;
    public CreationState CreationState {
        get => _creationState;
        set {
            _creationState = value;
            addBtn.gameObject.SetActive(_creationState == CreationState.add);
            cancelBtn.gameObject.SetActive(_creationState == CreationState.chooseType);
            textBtn.gameObject.SetActive(_creationState == CreationState.chooseType);
            imageBtn.gameObject.SetActive(_creationState == CreationState.chooseType);
            videoBtn.gameObject.SetActive(_creationState == CreationState.chooseType);
            animBtn.gameObject.SetActive(_creationState == CreationState.chooseType);
            if(value == CreationState.add) TransitionCollapse();
            else if(value == CreationState.chooseType) TransitionExpand();
        }
    }

    private void Start() {
        //var rectTransform = GetComponent<RectTransform>();
        buttonSpacing *= GameObject.Find("Canvas").GetComponent<Canvas>().scaleFactor; //rectTransform.sizeDelta.x / 10;
        addBtn.onClick.AddListener(() => { CreationState = CreationState.chooseType; });
        cancelBtn.onClick.AddListener(() => { CreationState = CreationState.add; });
        textBtn.onClick.AddListener(() => {
            CreationState = CreationState.add;
            TextCreateDelegate?.Invoke();
        });
        imageBtn.onClick.AddListener(() => {
            CreationState = CreationState.add;
            ImageCreateDelegate?.Invoke();
        });
        videoBtn.onClick.AddListener(() => {
            CreationState = CreationState.add;
            VideoCreateDelegate?.Invoke();
        });
        animBtn.onClick.AddListener(() => {
            CreationState = CreationState.add;
            AnimationCreateDelegate?.Invoke();
        });
    }

    private void TransitionCollapse() {
        var targetPosition = addBtn.transform.position;
        StartCoroutine(Helper.AnimateMovement(textBtn.transform, animationSpeed, targetPosition));
        StartCoroutine(Helper.AnimateMovement(animBtn.transform, animationSpeed, targetPosition));
        StartCoroutine(Helper.AnimateMovement(imageBtn.transform, animationSpeed, targetPosition));
        StartCoroutine(Helper.AnimateMovement(videoBtn.transform, animationSpeed, targetPosition));
    }

    private void TransitionExpand() {
        var pos = addBtn.transform.position;
        textBtn.transform.position = pos;
        animBtn.transform.position = pos;
        imageBtn.transform.position = pos;
        videoBtn.transform.position = pos;
        var textPos = new Vector3(pos.x - buttonSpacing * 2, pos.y, pos.z);
        var animPos = new Vector3(pos.x - buttonSpacing, pos.y, pos.z);
        var imagePos = new Vector3(pos.x + buttonSpacing, pos.y, pos.z);
        var videoPos = new Vector3(pos.x + buttonSpacing * 2, pos.y, pos.z);
        StartCoroutine(Helper.AnimateMovement(textBtn.transform, animationSpeed, textPos));
        StartCoroutine(Helper.AnimateMovement(animBtn.transform, animationSpeed, animPos));
        StartCoroutine(Helper.AnimateMovement(imageBtn.transform, animationSpeed, imagePos));
        StartCoroutine(Helper.AnimateMovement(videoBtn.transform, animationSpeed, videoPos));
    }
}

public enum CreationState {
    add, chooseType
}
