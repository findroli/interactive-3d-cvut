using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeMenuManager : MonoBehaviour {
    public delegate void OnPlaceNode();
    public event OnPlaceNode OnNodePlaceTapped;

    [SerializeField] private Button createNodeButton;
    [SerializeField] private GameObject contentObject;

    private void Start() {
        createNodeButton.onClick.AddListener(() => { OnNodePlaceTapped?.Invoke(); });
    }
    
}
