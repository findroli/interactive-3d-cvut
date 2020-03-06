using UnityEngine;

public class CreatorManager: MonoBehaviour
{
    [SerializeField] private GameObject testPrefab;
    [SerializeField] private GameObject nodePrefab;
    
    [SerializeField] private InputManager inputManager;
    [SerializeField] private InteractiveButton interactCreationBtn;
    
    public GameObject model;
    public GameObject node;

    private bool interactCreationMode = false;

    void Start() {
        model = Instantiate(testPrefab, Vector3.zero, Quaternion.identity);
        interactCreationBtn.onClick.AddListener(ToggleInteractionPointCreation);
    }

    void Update() {
        if (interactCreationBtn.selected) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
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
        else{ 
            if (!node.activeInHierarchy) {
                Destroy(node);
            }
            node = null;
        }
    }
    
    private void OnEnable() {
        InputManager.onRotateModel += RotateModel;
    }

    private void OnDisable() {
        InputManager.onRotateModel -= RotateModel;
    }

    private void RotateModel(Vector3 rotation) {
        model.transform.Rotate(rotation, 1f);
    }
}
