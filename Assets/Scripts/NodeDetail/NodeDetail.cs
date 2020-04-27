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
    [SerializeField] private GameObject videoCellPrefab;

    [SerializeField] private InputField titleInputField;
    [SerializeField] private GameObject scrollViewContent;
    [SerializeField] private Button cancelBtn;
    [SerializeField] private Button doneBtn;
    [SerializeField] private AddNodeDetailCell addCell;

    public InteractionPoint interactionPoint = null;
    private GameObject currentCreatingCell = null;

    public void UpdateData(NodeDetailData data) {
        titleInputField.text = data.title;
        foreach (var cellData in data.cells) {
            var cellObj = cellData.GetCell();
            cellObj.transform.SetParent(scrollViewContent.transform);
            var cell = cellObj.GetComponent<NodeDetailCell>();
            cell.onDelete += OnDeleteCell;
            cell.CreatingEnded();
        }
        addCell.transform.SetAsLastSibling();
    }

    public NodeDetailData GetData() {
        var result = new List<NodeCellData>();
        for (int i = 0; i < scrollViewContent.transform.childCount; i++) {
            var cell = scrollViewContent.transform.GetChild(i).GetComponent<NodeDetailCell>();
            if (cell != null) {
                result.Add(cell.GetData());
            }
        }
        return new NodeDetailData {
            title = titleInputField.text ?? "",
            position = interactionPoint.transform.localPosition,
            cells = result.ToArray()
        };
    }
    
    private void Start() {
        cancelBtn.onClick.AddListener(() => { onCancel?.Invoke(); });
        doneBtn.onClick.AddListener(() => { onDone?.Invoke(); });
        addCell.TextCreateDelegate += () => { CreateNewCell(textCellPrefab); };
        addCell.ImageCreateDelegate += CreateImage;
        addCell.VideoCreateDelegate += CreateVideo;
        addCell.AnimationCreateDelegate += () => { Debug.Log("Animation not implemented yet!"); };
    }

    private void OnDeleteCell(NodeDetailCell cell) {
        if (cell.gameObject == currentCreatingCell) {
            currentCreatingCell = null;
        }
        Destroy(cell.gameObject);
    }

    private void CreateVideo() {
        var extensions = new [] {
            new ExtensionFilter("Video Files", "mp4")
        };
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
        if(paths.Length == 0) return;
        if (File.Exists(paths[0])) {
            CreateNewCell(videoCellPrefab);
            currentCreatingCell.GetComponent<NodeDetailVideoCell>().FillWithData(new NodeVideoCellData {
                videoFile = paths[0]
            });
        }
        else {
            Debug.Log("NodeDetail.CreateVideo(): Couldn't load the video!");
            addCell.CreationState = CreationState.add;
        }
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
            Debug.Log("NodeDetail.CreateImage(): Couldn't load the image!");
            addCell.CreationState = CreationState.add;
        }
    }

    private void CreateNewCell(GameObject prefab) {
        var index = scrollViewContent.transform.childCount - 1;
        currentCreatingCell = Instantiate(prefab, scrollViewContent.transform, false);
        currentCreatingCell.transform.SetSiblingIndex(index);
        currentCreatingCell.GetComponent<NodeDetailCell>().onDelete += OnDeleteCell;
    }
    
}
