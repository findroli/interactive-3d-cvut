using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VersionsPopupPanel: MonoBehaviour {
    public delegate void OnCreateVersion();
    public event OnCreateVersion onCreateVersion;

    public delegate void OnVersionSelect(string name);
    public event OnVersionSelect onVersionSelect;

    public delegate void OnVersionDelete(string name);
    public event OnVersionDelete onVersionDelete;

    [SerializeField] private GameObject versionCellPrefab;
    
    [SerializeField] private Button createVersionButton;
    [SerializeField] private GameObject emptyLabelObj;

    void Start() {
        createVersionButton.onClick.AddListener(() => { onCreateVersion?.Invoke(); });
    }

    public void FillWithVersions(string[] versions) {
        emptyLabelObj.SetActive(versions.Length == 0);
        foreach (var version in versions) {
            var cell = Instantiate(versionCellPrefab, transform).GetComponent<ModelVersionCell>();
            cell.FillWithData(version);
            cell.onCellTap += OnCellTap;
            cell.onDelete += OnDelete;
        }
        createVersionButton.transform.SetAsLastSibling();
    }

    public void DeleteVersion(string cellName) {
        for (int i = 0; i < transform.childCount; i++) {
            var cell = transform.GetChild(i).GetComponent<ModelVersionCell>();
            if (cell != null && cell.GetCellName() == cellName) {
                cell.onCellTap -= OnCellTap;
                cell.onDelete -= OnDelete;
                Destroy(cell.gameObject);
            }
        }
    }

    private void OnDelete(string cellName) {
        onVersionDelete?.Invoke(cellName);
    }

    private void OnCellTap(string cellName) {
        onVersionSelect?.Invoke(cellName);
    } 
}
