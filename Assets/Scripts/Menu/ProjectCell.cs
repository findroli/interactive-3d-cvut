using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ProjectCell: MonoBehaviour {
    public delegate void OnClick();
    public event OnClick onClick;
    
    [SerializeField] private Image image;
    [SerializeField] private Text text;
    [SerializeField] private Button button;
    
    private void Start() {
        button.onClick.AddListener(() => {
            Debug.Log("Project button clicked!!");
            onClick?.Invoke();
        });
    }

    public void Setup(string projectName, Texture texture = null) {
        text.text = projectName;
    }
}
