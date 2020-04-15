using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatorManager: MonoBehaviour
{
    [SerializeField] private GameObject testPrefab;
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject nodeDetailPrefab;
    
    [SerializeField] private InputManager inputManager;
    [SerializeField] private InteractiveButton interactCreationBtn;
    
    public GameObject model;
    public GameObject node;

    private GameObject canvas;
    private bool interactCreationMode = false;
    private Dictionary<InteractionPoint, NodeCellData[]> nodesData = new Dictionary<InteractionPoint, NodeCellData[]>();
    private NodeDetail currentDetail = null;

    void Start() {
        canvas = GameObject.Find("Canvas");
        model = Instantiate(testPrefab, Vector3.zero, Quaternion.identity);
        model.layer = 8;
        interactCreationBtn.onClick.AddListener(ToggleInteractionPointCreation);
        InteractionPoint.interactionDelegate += point => {
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
        };
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

    void ToggleInteractionPointCreation() {
        SetInteractionPointCreation(!interactCreationBtn.selected);
    }

    private void SetInteractionPointCreation(bool value) {
        interactCreationBtn.selected = value;
        if (value) {
            node = Instantiate(nodePrefab);
            node.transform.SetParent(model.transform);
            node.SetActive(false);
        }
        else { 
            if (!node.activeInHierarchy) { Destroy(node); }
            node = null;
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
