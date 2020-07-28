using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ProjectCell: MonoBehaviour {
    private static Vector2 selectedSize = new Vector2(250, 302);
    private static Vector2 normalSize = new Vector2(204, 250);
    
    public delegate void OnClick(ProjectCell cell);
    public event OnClick onClick;

    [SerializeField] private Image underlineImage;
    [SerializeField] private Image image;
    [SerializeField] private Text text;
    [SerializeField] private Button button;

    public string projectName;

    private void Start() {
        image.preserveAspect = true;
        button.onClick.AddListener(() => {
            Debug.Log("Project button clicked!!");
            onClick?.Invoke(this);
        });
    }

    public void Setup(string projectName, Texture2D texture = null) {
        text.text = projectName;
        this.projectName = projectName;
        if (texture != null) {
            image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), 
                    new Vector2(texture.width/2f, texture.height/2f));
        }
        else {
            image.SetNativeSize();
        }
    }

    public void SetHighlight(bool value) {
        underlineImage.gameObject.SetActive(value);
        //var rt = GetComponent<RectTransform>();
        //rt.sizeDelta = value ? selectedSize : normalSize;
    }

    public Vector3 GetVersionsStartPosition() {
        var rt = GetComponent<RectTransform>();
        return transform.TransformPoint(new Vector3(0, - rt.sizeDelta.y / 2));
    }
}
