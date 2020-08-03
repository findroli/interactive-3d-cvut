using System;
using System.Collections.Generic;
using System.Linq;
using Dummiesman;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CreatorManager : MonoBehaviour {
    private static string ModelLayerName = "Interaction";
    private static string PointsLayerName = "Points";
    
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
    public Camera CurrentCamera => currentCamera;
    
    private Camera currentCamera;
    private ViewMode currentViewMode = ViewMode.view3D;
    private string modelName;
    private string versionName;
    private GameObject canvas;
    private bool interactCreationMode = false;
    private List<InteractionPoint> interactNodes = new List<InteractionPoint>();
    private Dictionary<InteractionPoint, NodeDetailData> nodesData = new Dictionary<InteractionPoint, NodeDetailData>();
    private NodeDetail currentDetail = null;
    private List<MovingWindow> moveWindows;
    private Vector3 last3Dscale = Vector3.one;

    public void ViewFullScreenImage(Sprite sprite) {
        fullScreenViewer.gameObject.SetActive(true);
        fullScreenViewer.ViewImage(sprite);
    }

    public void AddMovingWindow(MovingWindow mw) {
        moveWindows.Add(mw);
    }

    public void RemoveMovingWindow(MovingWindow mw) {
        moveWindows.Remove(mw);
    }

    private void Awake() {
        canvas = GameObject.Find("Canvas");
        LoadModel();
    }

    void Start() {
        moveWindows = new List<MovingWindow>();
        currentCamera = viewCamera.GetComponent<Camera>();
        SetupUI();
        saveProjectBtn.onClick.AddListener(Save);
        interactCreationBtn.onClick.AddListener(ToggleInteractionPointCreation);
        exitBtn.onClick.AddListener(Exit);
    }

    void Update() {
        if(inputHandler.Tapped()) {
            if (interactCreationBtn.selected) {
                HandlePointCreationInput();
            }
            else {
                CheckForInteractionPointSelection();
            }
        }
    }

    void HandlePointCreationInput() {
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        var mask = LayerMask.GetMask(ModelLayerName);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) {
            EndInteractionPointCreation(true, hit.point);
        }
        else {
            EndInteractionPointCreation(false, Vector3.zero);
        }
        
    }

    void CheckForInteractionPointSelection() {
        Debug.Log("Checking for interaction... " + inputHandler.inputPosition);
        Ray ray = currentCamera.ScreenPointToRay(inputHandler.inputPosition);
        ray.origin -= ray.direction * 10f;
        RaycastHit hit;
        var mask = LayerMask.GetMask(PointsLayerName);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) {
            Debug.Log("Hit something!" + hit.collider.gameObject.name);
            var point = hit.collider.gameObject.GetComponent<InteractionPoint>();
            if (point != null) {
                Debug.Log("Its interaction point");
                point.UserTapped();
            }
        }
    }

    void LoadModel() {
        Debug.Log("CreatorManager.LoadModel(): started!");
        modelName = AppState.shared().ModelName;
        var prefab = Resources.Load("Models/" + modelName);
        model = Instantiate(prefab) as GameObject;
        var children = model.GetComponentsInChildren<Transform>();
        foreach (var child in children) {
            child.gameObject.layer = LayerMask.NameToLayer(ModelLayerName);
        }
        versionName = AppState.shared().ModelVersionName;
        if (versionName != null) {
            var fileContent = IOManager.LoadProjectJson(modelName, versionName);
            var projectData = JsonUtility.FromJson<ProjectData>(fileContent);
            LoadPointsFromData(projectData.ToOriginal());
            Debug.Log("CreatorManager.LoadModel(): loaded project model:\n" + modelName);
        }
        if (AppState.shared().Mode == AppMode.Edit) {
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
        if (!interactCreationBtn.selected) {
            interactCreationBtn.selected = true;
            creationTip.SetActive(true);
            creationTip.GetComponentInChildren<Text>().text = "To place interaction point by clicking on 3D model";
        }
        else {
            EndInteractionPointCreation(false, Vector3.zero);
        }
    }

    private void EndInteractionPointCreation(bool shouldCreate, Vector3 position) {
        interactCreationBtn.selected = false;
        creationTip.SetActive(false);
        if (shouldCreate) {
            node = Instantiate(nodePrefab, model.transform, false);
            node.transform.position = position;
            var interactPoint = node.GetComponent<InteractionPoint>();
            if (interactPoint != null) interactNodes.Add(interactPoint);
        }
        else {
            Destroy(node);
        }
        node = null;
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
            if (imagePath != null) ScreenCapture.CaptureScreenshot(imagePath);
        });
    }

    private void Exit() {
        Helper.CreateConfirmPopup("Are you sure you want to exit presentation? Unsaved changes will be lost!", "Exit",
            () => { SceneManager.LoadScene("MainMenuScene"); });
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
        if (currentDetail != null) return;
        if (AppState.shared().Mode == AppMode.Presentation) {
            var prefab = Resources.Load("NodeDetailPresentation") as GameObject;
            var obj = Instantiate(prefab, canvas.transform, false);
            obj.transform.position = currentCamera.WorldToScreenPoint(point.transform.position);
            var detail = obj.GetComponent<NodeDetailPresentation>();
            detail.onCancel += OnDetailCancel;
            detail.modelAnimator = model.GetComponent<Animator>();
            currentDetail = detail;
            moveWindows.Add(detail.GetComponent<MovingWindow>());;
            if (nodesData.ContainsKey(point)) {
                detail.UpdateData(nodesData[point]);
            }
            return;
        }

        var nodeDetailPrefab = Resources.Load("NodeDetailEdit") as GameObject;
        var nodeGO = Instantiate(nodeDetailPrefab, canvas.transform, false);
        nodeGO.transform.position = currentCamera.WorldToScreenPoint(point.transform.position);
        var nodeDetail = nodeGO.GetComponent<NodeDetailEdit>();
        nodeDetail.interactionPoint = point;
        nodeDetail.modelAnimator = model.GetComponent<Animator>();
        nodeDetail.onDone += OnDetailDone;
        nodeDetail.onCancel += OnDetailCancel;
        nodeDetail.onDelete += OnDetailDelete;
        currentDetail = nodeDetail;
        moveWindows.Add(nodeDetail.GetComponent<MovingWindow>());;
        if (nodesData.ContainsKey(point)) {
            nodeDetail.UpdateData(nodesData[point]);
        }
    }

    private void OnViewModeChanged(ViewMode viewMode) {
        currentViewMode = viewMode;
        arObject.SetActive(viewMode == ViewMode.viewAR);
        viewCamera.SetActive(viewMode == ViewMode.view3D);
        viewChange.gameObject.SetActive(viewMode == ViewMode.view3D);
        creationTip.SetActive(false);
        OnDetailCancel();
        if (viewMode == ViewMode.viewAR) {
            last3Dscale = model.transform.localScale;
            model.transform.localScale = model.transform.localScale / 8;
            currentCamera = arObject.GetComponentInChildren<Camera>();
        }
        else {
            model.transform.position = Vector3.zero;
            model.transform.localScale = last3Dscale;
            currentCamera = viewCamera.GetComponent<Camera>();
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
        moveWindows.Remove(detail.GetComponent<MovingWindow>());
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
        moveWindows.Remove(editDetail.GetComponent<MovingWindow>());
    }

    private void OnDetailCancel() {
        var presDetail = currentDetail as NodeDetailPresentation;
        if (presDetail != null) {
            presDetail.onCancel -= OnDetailCancel;
            moveWindows.Remove(presDetail.GetComponent<MovingWindow>());
            Destroy(presDetail.gameObject);
        }
        var editDetail = currentDetail as NodeDetailEdit;
        if (editDetail != null) {
            editDetail.onDone -= OnDetailDone;
            editDetail.onCancel -= OnDetailCancel;
            editDetail.onDelete -= OnDetailDelete;
            moveWindows.Remove(editDetail.GetComponent<MovingWindow>());
            Destroy(editDetail.gameObject);
        }
        currentDetail = null;
    }

    private void RotateModel(Vector3 rotation) {
        if(IsAnyWindowMoving()) return;
        if (currentViewMode == ViewMode.viewAR) {
            rotation.x = 0f;
            rotation.z = 0f;
        }
        model.transform.Rotate(rotation/10f, Space.World);
    }

    private void ZoomModel(float value) {
        if(IsAnyWindowMoving()) return;
        var scale = model.transform.localScale;
        if (scale.x + value / 100f <= 0f) {
            model.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            return;
        }
        model.transform.localScale = new Vector3(scale.x + value / 1000f, scale.y + value / 1000f, scale.z + value / 1000f);
        Debug.Log("Scale: " + model.transform.localScale);
    }

    private bool IsAnyWindowMoving() {
        return moveWindows.Count > 0 && moveWindows.Any(mw => mw.IsMoving);
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
