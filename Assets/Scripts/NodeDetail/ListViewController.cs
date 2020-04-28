using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListViewController: MonoBehaviour {
    public delegate void OnFinish(string chosenCell);
    public event OnFinish onFinish;
    
    [SerializeField] private GameObject cellPrefab;
    
    [SerializeField] private GameObject content;
    [SerializeField] private Button cancelBtn;

    private void Start() {
        cancelBtn.onClick.AddListener(() => Finish(null));
    }

    public void FillWithData(string[] cellNames) {
        foreach (var cellName in cellNames) {
            var cell = Instantiate(cellPrefab, content.transform);
            cell.GetComponentInChildren<Text>().text = cellName;
            cell.GetComponent<Button>().onClick.AddListener(() => Finish(cellName));
        }
    }

    private void Finish(string cellName) {
        onFinish?.Invoke(cellName);
        Destroy(gameObject);
    }
}
