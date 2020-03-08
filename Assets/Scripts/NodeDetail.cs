using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class NodeDetail: MonoBehaviour {
    [SerializeField] private GameObject textCellPrefab;
    
    [SerializeField] private GameObject scrollViewContent;
    [SerializeField] private Button cancelBtn;
    [SerializeField] private Button doneBtn;
    [SerializeField] private Button addBtn;
    [SerializeField] private AddNodeDetailCell addCell;

    private GameObject currentCreatingCell = null;

    private void Start() {
        cancelBtn.onClick.AddListener(Cancel);
        doneBtn.onClick.AddListener(Done);
        addBtn.onClick.AddListener(Add);
        AddNodeDetailCell.cancelClickDelegate += () => {
            addCell.creationStarted = false;
            Destroy(currentCreatingCell);
            currentCreatingCell = null;
        };
        AddNodeDetailCell.createClickDelegate += () => {
            addCell.creationStarted = false;
            currentCreatingCell.GetComponentInChildren<NodeDetailTextCell>().CreatingEnded();
            currentCreatingCell = null;
        };
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
        currentCreatingCell = Instantiate(textCellPrefab, scrollViewContent.transform, false);
    }
    
    
}
