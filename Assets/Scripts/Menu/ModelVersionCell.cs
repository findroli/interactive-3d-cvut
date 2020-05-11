using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelVersionCell: MonoBehaviour {
    public delegate void OnCellTap(string cellName);
    public event OnCellTap onCellTap;
    public delegate void OnDelete(string cellName);
    public event OnDelete onDelete;
    
    [SerializeField] private Text text;
    [SerializeField] private Button button;
    [SerializeField] private Button deleteButton;

    private string cellName;

    public void FillWithData(string cellName) {
        this.cellName = cellName;
        text.text = cellName;
    }

    public string GetCellName() {
        return cellName; 
    }
    
    void Start() {
        button.onClick.AddListener(() => {
            onCellTap?.Invoke(cellName);
        });
        deleteButton.onClick.AddListener(() => {
            onDelete?.Invoke(cellName);
        });
    }
}
