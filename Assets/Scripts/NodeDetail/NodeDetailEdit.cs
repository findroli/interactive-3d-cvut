using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class NodeDetail: MonoBehaviour {
    public delegate void OnDone();
    public event OnDone onDone;
    public delegate void OnCancel();
    public event OnCancel onCancel;
    
    [SerializeField] private GameObject textCellPrefab;
    [SerializeField] private GameObject imageCellPrefab;
    [SerializeField] private GameObject videoCellPrefab;
    [SerializeField] private GameObject animationCellPrefab;
    [SerializeField] private GameObject listViewPrefab;

    [SerializeField] private InputField titleInputField;
    [SerializeField] private GameObject scrollViewContent;
    [SerializeField] private Button cancelBtn;
    [SerializeField] private Button doneBtn;
    [SerializeField] private AddNodeDetailCell addCell;

    public InteractionPoint interactionPoint = null;
    public Animator modelAnimator;
    private GameObject currentCreatingCell = null;

    public void UpdateData(NodeDetailData data) {
        titleInputField.text = data.title;
        foreach (var cellData in data.cells) {
            var cellObj = cellData.GetCell();
            cellObj.transform.SetParent(scrollViewContent.transform);
            var cell = cellObj.GetComponent<NodeDetailCell>();
            cell.onDelete += OnDeleteCell;
            cell.CreatingEnded();
            var animCell = cell.GetComponent<NodeDetailAnimationCell>();
            if (animCell != null) animCell.onTriggerAnimation += OnAnimatorTrigger;
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
        addCell.AnimationCreateDelegate += SelectAnimation;
    }

    private void OnDeleteCell(NodeDetailCell cell) {
        if (cell.gameObject == currentCreatingCell) {
            currentCreatingCell = null;
        }
        Destroy(cell.gameObject);
    }

    private void SelectAnimation() {
        if (modelAnimator == null) {
            Debug.Log("Model has no animator!");
            return;
        }
        var listView = Instantiate(listViewPrefab, addCell.transform.position, Quaternion.identity, transform);
        var listViewController = listView.GetComponent<ListViewController>();
        var animNames = modelAnimator.runtimeAnimatorController.animationClips.Select(a => a.name).ToArray();
        listViewController.FillWithData(animNames);
        listViewController.onFinish += CreateAnimationCell;
    }

    private void CreateAnimationCell(string animationName) {
        if (animationName == null) return;
        CreateNewCell(animationCellPrefab);
        var cell = currentCreatingCell.GetComponent<NodeDetailAnimationCell>();
        cell.FillWithData(new NodeAnimationCellData {
            animName = animationName
        });
        cell.onTriggerAnimation += OnAnimatorTrigger;
    }

    private void OnAnimatorTrigger(string triggerName) {
        Debug.Log("Playing animation: " + triggerName);
        modelAnimator.SetTrigger(triggerName);
    }

    private void CreateVideo() {
        FileBrowseHandler.OpenFileBrowser(FileBrowseHandler.MediaType.video, path => {
            if(path == null) return;
            if (File.Exists(path)) {
                CreateNewCell(videoCellPrefab);
                currentCreatingCell.GetComponent<NodeDetailVideoCell>().FillWithData(new NodeVideoCellData {
                    videoFile = path
                });
            }
            else {
                Debug.Log("NodeDetail.CreateVideo(): Couldn't load the video!");
                addCell.CreationState = CreationState.add;
            }
        });
        
    }

    private void CreateImage() {
        FileBrowseHandler.OpenFileBrowser(FileBrowseHandler.MediaType.image, path => {
            if(path == null) return;
            if (File.Exists(path)) {
                byte[] fileData = File.ReadAllBytes(path);
                CreateNewCell(imageCellPrefab);
                currentCreatingCell.GetComponent<NodeDetailImageCell>().FillWithData(new NodeImageCellData {
                    imageData = fileData
                });
            }
            else {
                Debug.Log("NodeDetail.CreateImage(): Couldn't load the image!");
                addCell.CreationState = CreationState.add;
            }
        });
        
    }

    private void CreateNewCell(GameObject prefab) {
        var index = scrollViewContent.transform.childCount - 1;
        currentCreatingCell = Instantiate(prefab, scrollViewContent.transform, false);
        currentCreatingCell.transform.SetSiblingIndex(index);
        currentCreatingCell.GetComponent<NodeDetailCell>().onDelete += OnDeleteCell;
    }
    
}
