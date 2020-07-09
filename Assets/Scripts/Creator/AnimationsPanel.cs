using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AnimationsPanel: MonoBehaviour {
    [SerializeField] private Button collapseButton;
    [SerializeField] private Image buttonImage;
    [SerializeField] private Sprite collapseImage;
    [SerializeField] private Sprite expandImage;
    [SerializeField] private GameObject scrollViewContent;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private GameObject emptyLabel;

    private float width;
    private bool collapsed = false;
    private Animator modelAnimator;
    
    void Start() {
        modelAnimator = FindObjectOfType<CreatorManager>().model.GetComponent<Animator>();
        width = GetComponent<RectTransform>().sizeDelta.x;
        FillWithData();
        collapseButton.onClick.AddListener(() => {
            if(collapsed) Expand();
            else Collapse();
        });
    }

    private void Collapse() {
        var tf = transform;
        tf.localPosition = new Vector2(tf.localPosition.x - width, 0f);
        buttonImage.sprite = expandImage;
        collapsed = true;
    }

    private void Expand() {
        var tf = transform;
        tf.localPosition = new Vector2(tf.localPosition.x + width, 0f);
        buttonImage.sprite = collapseImage;
        collapsed = false;
    }

    private void FillWithData() {
        if (modelAnimator == null) {
            emptyLabel.SetActive(true);
            return;
        }
        var animNames = modelAnimator.runtimeAnimatorController.animationClips.Select(a => a.name).ToArray();
        foreach (var animName in animNames) {
            var cell = Instantiate(cellPrefab, scrollViewContent.transform);
            cell.GetComponentInChildren<Text>().text = animName;
        }
    }
}
