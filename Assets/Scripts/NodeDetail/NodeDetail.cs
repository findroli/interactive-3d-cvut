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

    public void UpdateData(string[] texts) {
        foreach (var text in texts) {
            var cell = Instantiate(textCellPrefab, scrollViewContent.transform, false);
            cell.GetComponent<NodeDetailTextCell>().inputField.text = text;
            cell.transform.SetAsFirstSibling();
            cell.GetComponent<NodeDetailTextCell>().CreatingEnded();
        }
    }

    public string[] GetData() {
        var result = new List<string>();
        for (int i = 0; i < scrollViewContent.transform.childCount; i++) {
            var cell = scrollViewContent.transform.GetChild(i).GetComponent<NodeDetailTextCell>();
            if (cell != null) {
                result.Add(cell.inputField.text);
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
        currentCreatingCell.transform.SetAsFirstSibling();
    }
    
}
