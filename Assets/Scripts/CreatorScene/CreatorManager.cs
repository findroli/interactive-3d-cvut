using System;
using System.Collections.Generic;
using System.Linq;
using Dummiesman;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CreatorManager: MonoBehaviour {
    [SerializeField] private GameObject nodePrefab;
    
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private CameraViewChange viewChange;
    [SerializeField] private InteractiveButton interactCreationBtn;
    [SerializeField] private GameObject creationTip;
    [SerializeField] private Button exitBtn;
    [SerializeField] private Button saveProjectBtn;
    [SerializeField] private ViewModePicker viewModePicker;
    [SerializeField] private FullScreenViewer fullScreenViewer;

    [SerializeField] private GameObject viewCamera;
    [SerializeField] private GameObject arObject;

    public GameObject model;
    public GameObject node;

    private string modelName;
    private string versionName;
    private GameObject canvas;
    private bool interactCreationMode = false;
    private List<InteractionPoint> interactNodes = new List<InteractionPoint>();
    private Dictionary<InteractionPoint, NodeDetailData> nodesData = new Dictionary<InteractionPoint, NodeDetailData>();
    private NodeDetail currentDetail = null;

    public void ViewFullScreenImage(Sprite sprite) {
        fullScreenViewer.gameObject.SetActive(true);
        fullScreenViewer.ViewImage(sprite);
    }

    private void Awake() {
        canvas = GameObject.Find("Canvas");
        LoadModel();
    }

    void Start() {
        SetupUI();
        saveProjectBtn.onClick.AddListener(Save);
        interactCreationBtn.onClick.AddListener(ToggleInteractionPointCreation);
        exitBtn.onClick.AddListener(Exit);
    }

    void Update() {
        if (interactCreationBtn.selected) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            var mask = LayerMask.GetMask("Interaction");
            if (Physics.Raycast(ray, out hit, 500.0f, ~mask)) {
                if (!node.activeInHierarchy) {
                    node.SetActive(true);
                }
                node.transform.position = hit.point;
            }
            else if (node.activeInHierarchy) {
                node.SetActive(false);
            }
            if (Input.GetMouseButtonDown(0)) {
                SetInteractionPointCreation(false);
            }
        }
    }

    void LoadModel() {
        Debug.Log("CreatorManager.LoadModel(): started!");
        modelName = AppState.shared().ModelName;
        var prefab = Resources.Load("Models/" + modelName);
        model = Instantiate(prefab) as GameObject;
        model.layer = 8;
        versionName = AppState.shared().ModelVersionName;
        if (versionName != null) {
            var fileContent = IOManager.LoadProjectJson(modelName, versionName);
            var projectData = JsonUtility.FromJson<ProjectData>(fileContent);
            LoadPointsFromData(projectData.ToOriginal());
            Debug.Log("CreatorManager.LoadModel(): loaded project model:\n" + modelName);
        }
        if(AppState.shared().Mode == AppMode.Edit) {
            CreateMeshColliderRecursively(model.gameObject);
        }
    }

    void CreateMeshColliderRecursively(GameObject gameObject) {
        for (int i = 0; i < gameObject.transform.childCount; i++) {
            var child = gameObject.transform.GetChild(i);
            var meshFilter = child.gameObject.GetComponent<MeshFilter>();
            if (meshFilter != null) {
                var childCollider = child.gameObject.AddComponent<MeshCollider>();
                childCollider.sharedMesh = meshFilter.mesh;
            }
            if (child.childCount > 0) {
                CreateMeshColliderRecursively(child.gameObject);
            }
        }
    }

    void ToggleInteractionPointCreation() {
        SetInteractionPointCreation(!interactCreationBtn.selected);
    }

    private void SetInteractionPointCreation(bool value) {
        interactCreationBtn.selected = value;
        creationTip.SetActive(value);
        if (value) {
            node = Instantiate(nodePrefab, model.transform, false);
            node.SetActive(false);
        }
        else { 
            if (!node.activeInHierarchy) Destroy(node);
            else {
                var interactPoint = node.GetComponent<InteractionPoint>();
                if(interactPoint != null) interactNodes.Add(interactPoint);
            }
            node = null;
        }
    }

    private void SetupUI() {
        var mode = AppState.shared().Mode;
        interactCreationBtn.gameObject.SetActive(mode == AppMode.Edit);
        saveProjectBtn.gameObject.SetActive(mode == AppMode.Edit);
    }
    
    private void Save() {
        Helper.CreateSavePopup(string.IsNullOrEmpty(versionName) ? "untitled" : versionName, saveName => {
            versionName = saveName;
            var data = new ProjectData(nodesData.Values.ToArray());
            var json = JsonUtility.ToJson(data, true);
            IOManager.SaveCurrentProject(modelName, saveName, json);
            var imagePath = IOManager.CurrentProjectVersionImagePath(modelName, versionName);
            if(imagePath != null) ScreenCapture.CaptureScreenshot(imagePath);
        });
    }

    private void Exit() {
        Helper.CreateConfirmPopup("Are you sure you want to exit presentation?", "Exit", () => {
            SceneManager.LoadScene("MainMenuScene");
        });
    }

    private void LoadPointsFromData(NodeDetailData[] data) {
        foreach (var detailData in data) {
            node = Instantiate(nodePrefab, model.transform, false);
            node.transform.localPosition = detailData.position;
            var interactPoint = node.GetComponent<InteractionPoint>();
            if (interactPoint != null) {
                interactNodes.Add(interactPoint);
                nodesData.Add(interactPoint, detailData);
            }
        }
    }

    private void OnInteractionPointSelect(InteractionPoint point) {
        if(currentDetail != null) return;
        if (AppState.shared().Mode == AppMode.Presentation) {
            var prefab = Resources.Load("NodeDetailPresentation") as GameObject;
            var obj = Instantiate(prefab, canvas.transform, false);
            obj.transform.position = Camera.main.WorldToScreenPoint(point.transform.position);
            var detail = obj.GetComponent<NodeDetailPresentation>();
            detail.onCancel += OnDetailCancel;
            detail.modelAnimator = model.GetComponent<Animator>();
            currentDetail = detail;
            if (nodesData.ContainsKey(point)) {
                detail.UpdateData(nodesData[point]);
            }
            return;
        }
        var nodeDetailPrefab = Resources.Load("NodeDetailEdit") as GameObject;
        var nodeGO = Instantiate(nodeDetailPrefab, canvas.transform, false);
        nodeGO.transform.position = Camera.main.WorldToScreenPoint(point.transform.position);
        var nodeDetail = nodeGO.GetComponent<NodeDetailEdit>();
        nodeDetail.interactionPoint = point;
        nodeDetail.modelAnimator = model.GetComponent<Animator>();
        nodeDetail.onDone += OnDetailDone;
        nodeDetail.onCancel += OnDetailCancel;
        nodeDetail.onDelete += OnDetailDelete;
        currentDetail = nodeDetail;
        if (nodesData.ContainsKey(point)) {
            nodeDetail.UpdateData(nodesData[point]);
        }
    }

    private void OnViewModeChanged(ViewMode viewMode) {
        arObject.SetActive(viewMode == ViewMode.viewAR);
        viewCamera.SetActive(viewMode == ViewMode.view3D);
        if (viewMode == ViewMode.viewAR) {
            arObject.GetComponentInChildren<PlaceObjectsOnPlane>().spawnedObject = model;
        } else {
            model.transform.position = Vector3.zero;
        }
    }
    
    private void OnEnable() {
        inputHandler.onRotateModel += RotateModel;
        inputHandler.onZoomModel += ZoomModel;
        viewChange.onViewChange += ChangeView;
        InteractionPoint.interactionDelegate += OnInteractionPointSelect;
        viewModePicker.onViewModeChanged += OnViewModeChanged;
    }

    private void OnDisable() {
        inputHandler.onRotateModel -= RotateModel;
        inputHandler.onZoomModel -= ZoomModel;
        viewChange.onViewChange -= ChangeView;
        InteractionPoint.interactionDelegate -= OnInteractionPointSelect;
        viewModePicker.onViewModeChanged -= OnViewModeChanged;
    }

    private void OnDetailDone() {
        var detail = currentDetail as NodeDetailEdit;
        if (nodesData.ContainsKey(detail.interactionPoint)) {
            nodesData[detail.interactionPoint] = detail.GetData();
        } else {
            nodesData.Add(detail.interactionPoint, detail.GetData());
        }
        detail.onDone -= OnDetailDone;
        detail.onCancel -= OnDetailCancel;
        detail.onDelete -= OnDetailDelete;
        Destroy(detail.gameObject);
        currentDetail = null;
    }

    private void OnDetailDelete() {
        var editDetail = currentDetail as NodeDetailEdit;
        if (editDetail == null) return;
        editDetail.onDone -= OnDetailDone;
        editDetail.onCancel -= OnDetailCancel;
        editDetail.onDelete -= OnDetailDelete;
        if (nodesData.ContainsKey(editDetail.interactionPoint)) {
            nodesData.Remove(editDetail.interactionPoint);
        }
        interactNodes.Remove(editDetail.interactionPoint);
        Destroy(editDetail.gameObject);
        Destroy(editDetail.interactionPoint.gameObject);
        currentDetail = null;
    }

    private void OnDetailCancel() {
        var presDetail = currentDetail as NodeDetailPresentation;
        if (presDetail != null) {
            presDetail.onCancel -= OnDetailCancel;
            Destroy(presDetail.gameObject);
        }
        var editDetail = currentDetail as NodeDetailEdit;
        if (editDetail != null) {
            editDetail.onDone -= OnDetailDone;
            editDetail.onCancel -= OnDetailCancel;
            editDetail.onDelete -= OnDetailDelete;
            Destroy(editDetail.gameObject);
        }
        currentDetail = null;
    }

    private void RotateModel(Vector3 rotation) {
        model.transform.Rotate(rotation/10f, Space.World);
    }

    private void ZoomModel(float value) {
        var scale = model.transform.localScale;
        if (scale.x + value / 100f <= 0f) {
            model.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            return;
        }
        model.transform.localScale = new Vector3(scale.x + value / 100f, scale.y + value / 100f, scale.z + value / 100f);
        Debug.Log("Scale: " + model.transform.localScale);
    }

    private void ChangeView(CameraViewChange.CameraDefaultView view) {
        viewChange.SetButtonsHidden(true);
        switch (view) {
            case CameraViewChange.CameraDefaultView.front:
                model.transform.rotation = Quaternion.identity;
                break;
            case CameraViewChange.CameraDefaultView.side:
                model.transform.rotation = Quaternion.LookRotation(Vector3.left);
                break;
            case CameraViewChange.CameraDefaultView.top:
                model.transform.rotation = Quaternion.LookRotation(Vector3.down);
                break;
        }
    }
}
