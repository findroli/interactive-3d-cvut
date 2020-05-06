using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VersionsPopupPanel: MonoBehaviour {
    public delegate void OnCreateVersion();
    public event OnCreateVersion onCreateVersion;

    public delegate void OnVersionSelect(string name);
    public event OnVersionSelect onVersionSelect;

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
            cell.cellName = version;
            cell.onCellTap += cellName => { onVersionSelect?.Invoke(cellName); };
        }
        createVersionButton.transform.SetAsLastSibling();
    }
}
