using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeDetail: MonoBehaviour {
    [SerializeField] private GameObject scrollViewContent;
    [SerializeField] private Button cancelBtn;
    [SerializeField] private Button doneBtn;
    [SerializeField] private Button addBtn;
    [SerializeField] private AddNodeDetailCell addCell;

    private void Start() {
        cancelBtn.onClick.AddListener(Cancel);
        doneBtn.onClick.AddListener(Done);
        addBtn.onClick.AddListener(Add);
    }

    private void Cancel() {
        Debug.Log("Cancel clicked!");
    }

    private void Done() {
        Debug.Log("Done clicked!");
    }

    private void Add() {
        Debug.Log("Add clicked!");
        addCell.creationStarted = !addCell.creationStarted;
    }
    
    
}
