using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullScreenViewer: MonoBehaviour {
    [SerializeField] private Button cancelBtn;
    [SerializeField] private Image image;

    public void ViewImage(Sprite sprite) {
        transform.SetAsLastSibling();
        image.sprite = sprite;
    }

    public void ShowVideo() {
        
    }
    
    void Start() {
        image.preserveAspect = true;
        cancelBtn.onClick.AddListener(() => gameObject.SetActive(false));
    }
}
