using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelVersionCell: MonoBehaviour {
    public delegate void OnCellTap(string cellName);
    public event OnCellTap onCellTap;
    
    [SerializeField] private Text text;
    [SerializeField] private Button button;

    public string cellName;
    
    void Start() {
        button.onClick.AddListener(() => {
            onCellTap?.Invoke(cellName);
        });
    }
}
