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

    private float buttonSpacing = 40f;
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
        StartCoroutine(AnimateMovement(textBtn.gameObject, animationSpeed, targetPosition));
        StartCoroutine(AnimateMovement(animBtn.gameObject, animationSpeed, targetPosition));
        StartCoroutine(AnimateMovement(imageBtn.gameObject, animationSpeed, targetPosition));
        StartCoroutine(AnimateMovement(videoBtn.gameObject, animationSpeed, targetPosition));
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
        StartCoroutine(AnimateMovement(textBtn.gameObject, animationSpeed, textPos));
        StartCoroutine(AnimateMovement(animBtn.gameObject, animationSpeed, animPos));
        StartCoroutine(AnimateMovement(imageBtn.gameObject, animationSpeed, imagePos));
        StartCoroutine(AnimateMovement(videoBtn.gameObject, animationSpeed, videoPos));
    }

    private IEnumerator AnimateMovement(GameObject obj, float speed, Vector3 toPos) {
        while ((toPos - obj.transform.position).magnitude > speed) {
            var direction = (toPos - obj.transform.position).normalized;
            obj.transform.position += direction * speed;
            yield return null;
        }
        obj.transform.position = toPos;
    }
}

 public enum CreationState {
     add,
     chooseType
 }
