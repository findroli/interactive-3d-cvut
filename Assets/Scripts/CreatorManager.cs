using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dummiesman;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreatorManager: MonoBehaviour
{
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject nodeDetailPrefab;
    
    [SerializeField] private InputManager inputManager;
    [SerializeField] private InteractiveButton interactCreationBtn;
    [SerializeField] private Button saveBtn;
    [SerializeField] private Button loadBtn;
    [SerializeField] private Button exitBtn;
    [SerializeField] private Button saveProjectBtn;
    [SerializeField] private InputField projectName;

    public GameObject model;
    public GameObject node;

    private GameObject canvas;
    private bool interactCreationMode = false;
    private List<InteractionPoint> interactNodes = new List<InteractionPoint>();
    private Dictionary<InteractionPoint, NodeDetailData> nodesData = new Dictionary<InteractionPoint, NodeDetailData>();
    private NodeDetail currentDetail = null;

    void Start() {
        canvas = GameObject.Find("Canvas");
        LoadModel();
        saveBtn.onClick.AddListener(Save);
        loadBtn.onClick.AddListener(Load);
        interactCreationBtn.onClick.AddListener(ToggleInteractionPointCreation);
        InteractionPoint.interactionDelegate += OnInteractionPointSelect;
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
        var loadInfo = FindObjectOfType<LoadInfo>();
        model = new OBJLoader().Load(loadInfo.ImportObjectPath);
        model.layer = 8;
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
        var data = new DetailsArrayJsonWrapper(nodesData.Values.ToArray());
        var json = JsonUtility.ToJson(data, true);
        Debug.Log("CreatorManager.Save(): Created JSON:\n" + json);
        var path = Application.dataPath + "/data.json";
        if (File.Exists(path)) {
            File.Delete(path);
            Debug.Log("CreatorManager.Save(): File already exists - was deleted!");
        }
        var sr = File.CreateText(path);
        sr.Write(json);
        sr.Close();
        Debug.Log("CreatorManager.Save(): JSON was saved in path:\n" + path);
    }

    private void Load() {
        var path = Application.dataPath + "/data.json";
        if (!File.Exists(path)) {
            Debug.Log("CreatorManager.Load(): File with JSON was not found!");
            return;
        }
        var sr = new StreamReader(path);
        var fileContent = sr.ReadToEnd();
        sr.Close();
        LoadFromData(JsonUtility.FromJson<DetailsArrayJsonWrapper>(fileContent).ToOriginal());
    }

    private void LoadFromData(NodeDetailData[] data) {
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
    
    private void OnEnable() {
        InputManager.onRotateModel += RotateModel;
    }

    private void OnDisable() {
        InputManager.onRotateModel -= RotateModel;
    }

    private void OnDetailDone() {
        if (nodesData.ContainsKey(currentDetail.interactionPoint)) {
            nodesData[currentDetail.interactionPoint] = currentDetail.GetData();
        } else {
            nodesData.Add(currentDetail.interactionPoint, currentDetail.GetData());
        }
        Destroy(currentDetail.gameObject);
        currentDetail = null;
    }

    private void OnDetailCancel() {
        Destroy(currentDetail.gameObject);
        currentDetail = null;
    }

    private void RotateModel(Vector3 rotation) {
        model.transform.Rotate(rotation, 1f);
    }
}
