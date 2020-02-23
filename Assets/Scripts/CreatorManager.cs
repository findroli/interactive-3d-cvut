using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatorManager: MonoBehaviour
{
    [SerializeField] private GameObject testPrefab;
    [SerializeField] private GameObject nodePrefab;
    
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Button interactCreationBtn;
    
    public GameObject model;
    public GameObject node;

    private bool interactCreationMode = false;

    void Start() {
        model = GameObject.Instantiate(testPrefab, Vector3.zero, Quaternion.identity);
        interactCreationBtn.onClick.AddListener(ToggleInteractionPointCreation);
    }

    void Update() {
        if (interactCreationMode)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (!node.activeInHierarchy)
                {
                    node.SetActive(true);
                }
                node.transform.position = hit.point;
                
            }
            else if (node.activeInHierarchy)
            {
                node.SetActive(false);
            }
            if (Input.GetMouseButtonDown(0))
            {
                SetInteractionPointCreation(false);
            }
        }
    }

    void ToggleInteractionPointCreation()
    {
        SetInteractionPointCreation(!interactCreationMode);
    }

    private void SetInteractionPointCreation(bool value)
    {
        interactCreationMode = !interactCreationMode;
        interactCreationBtn.GetComponent<Image>().color = interactCreationMode ? Color.yellow : Color.white;
        if (interactCreationMode)
        {
            node = Instantiate(nodePrefab);
            node.transform.SetParent(model.transform);
            node.SetActive(false);
        }
        else
        {
            if (!node.activeInHierarchy)
            {
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
