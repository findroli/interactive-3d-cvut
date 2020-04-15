using System.Collections;
using System.Collections.Generic;
using System.IO;
using SFB;
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
    [SerializeField] private GameObject imageCellPrefab;
    
    [SerializeField] private GameObject scrollViewContent;
    [SerializeField] private Button cancelBtn;
    [SerializeField] private Button doneBtn;
    [SerializeField] private AddNodeDetailCell addCell;

    public InteractionPoint interactionPoint = null;
    private GameObject currentCreatingCell = null;

    public void UpdateData(NodeCellData[] data) {
        foreach (var cellData in data) {
            var cell = cellData.GetCell();
            cell.transform.SetParent(scrollViewContent.transform);
            cell.GetComponent<NodeDetailCell>().CreatingEnded();
        }
        addCell.transform.SetAsLastSibling();
    }

    public NodeCellData[] GetData() {
        var result = new List<NodeCellData>();
        for (int i = 0; i < scrollViewContent.transform.childCount; i++) {
            var cell = scrollViewContent.transform.GetChild(i).GetComponent<NodeDetailCell>();
            if (cell != null) {
                result.Add(cell.GetData());
            }
        }
        return result.ToArray();
    }
    
    private void Start() {
        cancelBtn.onClick.AddListener(() => { onCancel?.Invoke(); });
        doneBtn.onClick.AddListener(() => { onDone?.Invoke(); });
        addCell.textCreateDelegate += () => {
            CreateNewCell(textCellPrefab);
        };
        addCell.imageCreateDelegate += CreateImage;
        addCell.confirmCreateDelegate += () => {
            if(currentCreatingCell == null) return;
            currentCreatingCell.GetComponentInChildren<NodeDetailCell>().CreatingEnded();
            currentCreatingCell = null;
        };
        addCell.cancelCreateDelegate += () => {
            Destroy(currentCreatingCell);
            currentCreatingCell = null;
        };
    }

    private void CreateImage() {
        var extensions = new [] {
            new ExtensionFilter("Image Files", "png", "jpg", "jpeg" )
        };
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
        if(paths.Length == 0) return;
        if (File.Exists(paths[0])) {
            byte[] fileData = File.ReadAllBytes(paths[0]);
            CreateNewCell(imageCellPrefab);
            currentCreatingCell.GetComponent<NodeDetailImageCell>().FillWithData(new NodeImageCellData {
                imageData = fileData
            });
        }
        else {
            Debug.Log("NodeDetail: Couldn't load the image!");
            addCell.CreationState = CreationState.add;
        }
    }

    private void CreateNewCell(GameObject prefab) {
        var index = scrollViewContent.transform.childCount - 1;
        currentCreatingCell = Instantiate(prefab, scrollViewContent.transform, false);
        currentCreatingCell.transform.SetSiblingIndex(index);
    }
    
}
