using System;
using System.Collections.Generic;
using System.Linq;
using Dummiesman;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreatorManager: MonoBehaviour {
    [SerializeField] private GameObject testCarPrefab;
    [SerializeField] private GameObject nodePrefab;
    
    [SerializeField] private InputManager inputManager;
    [SerializeField] private InteractiveButton interactCreationBtn;
    [SerializeField] private Button exitBtn;
    [SerializeField] private Button saveProjectBtn;
    [SerializeField] private InputField projectName;
    [SerializeField] private ViewModePicker viewModePicker;

    [SerializeField] private GameObject viewCamera;
    [SerializeField] private GameObject arObject;

    public GameObject model;
    public GameObject node;

    private string loadedModelPath;
    private GameObject canvas;
    private bool interactCreationMode = false;
    private List<InteractionPoint> interactNodes = new List<InteractionPoint>();
    private Dictionary<InteractionPoint, NodeDetailData> nodesData = new Dictionary<InteractionPoint, NodeDetailData>();
    private NodeDetail currentDetail = null;

    void Start() {
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
        var isDemoRun = false;
        var loadInfo = FindObjectOfType<LoadInfo>();
        var modelPath = loadInfo == null ? "" : loadInfo.ImportObjectPath;
        if (loadInfo == null || loadInfo.ImportObjectPath == null && loadInfo.LoadProjectName == null) {
            model = Instantiate(testCarPrefab);
            modelPath = "test";
            isDemoRun = true;
        }
        else if (loadInfo.LoadProjectName != null) {
            var fileContent = IOManager.LoadProjectJson(loadInfo.LoadProjectName);
            var projectData = JsonUtility.FromJson<ProjectData>(fileContent);
            modelPath = projectData.modelPath;
            if (modelPath == "test") {
                model = Instantiate(testCarPrefab);
                isDemoRun = true;
            }
            else {
                model = new OBJLoader().Load(modelPath);
            }
            LoadPointsFromData(projectData.ToOriginal());
            projectName.text = projectData.name;
            Debug.Log("CreatorManager.LoadModel(): loaded project model from path:\n" + modelPath);
        }
        else {
            model = new OBJLoader().Load(modelPath);
        }
        model.layer = 8;
        loadedModelPath = modelPath;
        if(loadInfo != null && loadInfo.AppMode == AppMode.Edit && !isDemoRun) {
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
        var name = projectName.text ?? "untitled";
        var path = loadedModelPath;
        var data = new ProjectData(name, path, nodesData.Values.ToArray());
        var json = JsonUtility.ToJson(data, true);
        IOManager.SaveCurrentProject(name, json);
        ScreenCapture.CaptureScreenshot(IOManager.CurrentProjectImagePath(name));
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
        model.SetActive(viewMode == ViewMode.view3D);
        if (viewMode == ViewMode.viewAR) {
            //arObject.GetComponentInChildren<PlaceObjectsOnPlane>().placedPrefab = model;
        }
    }
    
    private void OnEnable() {
        InputManager.onRotateModel += RotateModel;
        InteractionPoint.interactionDelegate += OnInteractionPointSelect;
        viewModePicker.onViewModeChanged += OnViewModeChanged;
    }

    private void OnDisable() {
        InputManager.onRotateModel -= RotateModel;
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
        model.transform.Rotate(rotation, 1f);
    }
}
