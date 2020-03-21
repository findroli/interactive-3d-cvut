using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class NodeDetail: MonoBehaviour {
    public delegate void OnDone();
    public event OnDone onDone;
    public delegate void OnCancel();
    public event OnCancel onCancel;
    
    [SerializeField] private GameObject textCellPrefab;
    [SerializeField] private GameObject scrollViewContent;
    [SerializeField] private Button cancelBtn;
    [SerializeField] private Button doneBtn;
    [SerializeField] private Button addBtn;
    [SerializeField] private AddNodeDetailCell addCell;

    public InteractionPoint interactionPoint = null;
    private GameObject currentCreatingCell = null;

    public void UpdateData(NodeDetailItem[] items) {
        foreach (var item in items) {
            var cell = item.CreateCell(scrollViewContent.transform);
            cell.transform.SetAsFirstSibling();
            cell.GetComponent<NodeDetailTextCell>().CreatingEnded();
        }
    }

    public NodeDetailItem[] GetData() {
        var result = new List<NodeDetailItem>();
        for (int i = 0; i < scrollViewContent.transform.childCount; i++) {
            var cell = scrollViewContent.transform.GetChild(i).GetComponent<NodeDetailCell>();
            if (cell != null) {
                result.Add(cell.GetData());
            }
        }
        return result.ToArray();
    }
    
    private void Start() {
        cancelBtn.onClick.AddListener(Cancel);
        doneBtn.onClick.AddListener(Done);
        addBtn.onClick.AddListener(Add);
        addCell.cancelClickDelegate += () => {
            addCell.creationStarted = false;
            Destroy(currentCreatingCell);
            currentCreatingCell = null;
        };
        addCell.createClickDelegate += () => {
            addCell.creationStarted = false;
            currentCreatingCell.GetComponentInChildren<NodeDetailTextCell>().CreatingEnded();
            currentCreatingCell = null;
        };
    }

    private void Cancel() {
        Debug.Log("Cancel clicked!");
        onCancel?.Invoke();
    }

    private void Done() {
        Debug.Log("Done clicked!");
        onDone?.Invoke();
    }

    private void Add() {
        Debug.Log("Add clicked!");
        addCell.creationStarted = !addCell.creationStarted;
        currentCreatingCell = Instantiate(textCellPrefab, scrollViewContent.transform, false);
        var children = scrollViewContent.transform.childCount;
        if (children > 1) {
            currentCreatingCell.transform.SetSiblingIndex(children - 2);
        } else {
            currentCreatingCell.transform.SetSiblingIndex(0);   
        }
    }
    
}
