using System;
using System.Collections.Generic;
using System.Linq;
using Dummiesman;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreatorManager: MonoBehaviour {
    [SerializeField] private GameObject nodePrefab;
    
    [SerializeField] private InputManager inputManager;
    [SerializeField] private MobileInputManager mobileInputManager;
    [SerializeField] private InteractiveButton interactCreationBtn;
    [SerializeField] private Button exitBtn;
    [SerializeField] private Button saveProjectBtn;
    [SerializeField] private InputField projectName;
    [SerializeField] private ViewModePicker viewModePicker;

    [SerializeField] private GameObject viewCamera;
    [SerializeField] private GameObject arObject;

    public GameObject model;
    public GameObject node;

    private string modelName;
    private GameObject canvas;
    private bool interactCreationMode = false;
    private List<InteractionPoint> interactNodes = new List<InteractionPoint>();
    private Dictionary<InteractionPoint, NodeDetailData> nodesData = new Dictionary<InteractionPoint, NodeDetailData>();
    private NodeDetail currentDetail = null;

    void Start() {
#if UNITY_EDITOR
        mobileInputManager.enabled = false;
#endif
#if !UNITY_EDITOR && UNITY_IOS
        inputManager.enabled = false;
#endif
        
        canvas = GameObject.Find("Canvas");
        LoadModel();
        saveProjectBtn.onClick.AddListener(Save);
        interactCreationBtn.onClick.AddListener(ToggleInteractionPointCreation);
        exitBtn.onClick.AddListener(() => { SceneManager.LoadScene("MainMenuScene"); });
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
        var loadInfo = FindObjectOfType<LoadInfo>();
        var prefab = Resources.Load("Models/" + loadInfo.ModelName);
        modelName = loadInfo.ModelName;
        model = Instantiate(prefab) as GameObject;
        model.layer = 8;
        if (loadInfo.VersionName != null) {
            var fileContent = IOManager.LoadProjectJson(loadInfo.ModelName, loadInfo.VersionName);
            var projectData = JsonUtility.FromJson<ProjectData>(fileContent);
            LoadPointsFromData(projectData.ToOriginal());
            projectName.text = loadInfo.VersionName;
            Debug.Log("CreatorManager.LoadModel(): loaded project model:\n" + modelName);
        }
        if(loadInfo.AppMode == AppMode.Edit) {
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
    
    private void Save() {
        var versionName = projectName.text ?? "untitled";
        var data = new ProjectData(nodesData.Values.ToArray());
        var json = JsonUtility.ToJson(data, true);
        IOManager.SaveCurrentProject(modelName, versionName, json);
        var imagePath = IOManager.CurrentProjectVersionImagePath(modelName, versionName);
        if(imagePath != null) ScreenCapture.CaptureScreenshot(imagePath);
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
        var nodeDetailPrefab = Resources.Load("NodeDetail") as GameObject;
        var nodeGO = Instantiate(nodeDetailPrefab, canvas.transform);
        nodeGO.transform.position = Camera.main.WorldToScreenPoint(point.transform.position);
        var nodeDetail = nodeGO.GetComponent<NodeDetail>();
        nodeDetail.interactionPoint = point;
        nodeDetail.modelAnimator = model.GetComponent<Animator>();
        nodeDetail.onDone += OnDetailDone;
        nodeDetail.onCancel += OnDetailCancel;
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
        inputManager.onRotateModel += RotateModel;
        inputManager.onZoomModel += ZoomModel;
        mobileInputManager.onRotateModel += RotateModel;
        mobileInputManager.onZoomModel += ZoomModel;
        InteractionPoint.interactionDelegate += OnInteractionPointSelect;
        viewModePicker.onViewModeChanged += OnViewModeChanged;
    }

    private void OnDisable() {
        inputManager.onRotateModel -= RotateModel;
        inputManager.onZoomModel -= ZoomModel;
        mobileInputManager.onRotateModel -= RotateModel;
        mobileInputManager.onZoomModel -= ZoomModel;
        InteractionPoint.interactionDelegate -= OnInteractionPointSelect;
        viewModePicker.onViewModeChanged -= OnViewModeChanged;
    }

    private void OnDetailDone() {
        if (nodesData.ContainsKey(currentDetail.interactionPoint)) {
            nodesData[currentDetail.interactionPoint] = currentDetail.GetData();
        } else {
            nodesData.Add(currentDetail.interactionPoint, currentDetail.GetData());
        }
        currentDetail.onDone -= OnDetailDone;
        currentDetail.onCancel -= OnDetailCancel;
        Destroy(currentDetail.gameObject);
        currentDetail = null;
    }

    private void OnDetailCancel() {
        currentDetail.onDone -= OnDetailDone;
        currentDetail.onCancel -= OnDetailCancel;
        Destroy(currentDetail.gameObject);
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
}
