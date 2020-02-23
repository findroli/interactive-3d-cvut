using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatorManager: MonoBehaviour
{
    [SerializeField] private GameObject testPrefab;
    [SerializeField] private InputManager inputManager;
    public GameObject model;

    private void OnEnable()
    {
        InputManager.onRotateModel += RotateModel;
    }

    private void OnDisable()
    {
        InputManager.onRotateModel -= RotateModel;
    }

    void Start() {
        model = GameObject.Instantiate(testPrefab, Vector3.zero, Quaternion.identity);
    }

    void Update() {
        
    }

    void RotateModel(Vector3 rotation)
    {
        model.transform.Rotate(rotation, 1f);
    }
}
