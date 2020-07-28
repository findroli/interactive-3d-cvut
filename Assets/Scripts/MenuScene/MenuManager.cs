using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SFB;
using Dummiesman;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager: MonoBehaviour {
    [SerializeField] private GameObject projectCellPrefab;
    [SerializeField] private GameObject versionsListPrefab;

    [SerializeField] private Button importBtn;
    [SerializeField] private GameObject scrollView;
    [SerializeField] private GameObject projectsScrollViewContent;

    private ProjectCell selectedCell;
    private VersionsPopupPanel versionPanel;
    private string selectedModel;
    private GameObject canvas;
    
    void Start() {
        importBtn.onClick.AddListener(ImportModel);
        SetupModelCells();
        canvas = GameObject.Find("Canvas");
        UpdateUI();
    }

    private void OnEnable() {
        AppState.shared().onModeChange += UpdateUI;
    }
    
    private void OnDisable() {
        AppState.shared().onModeChange -= UpdateUI;
    }

    private void UpdateUI() {
        //importBtn.gameObject.SetActive(AppState.shared().Mode == AppMode.Edit);
        if(versionPanel != null) DeleteVersionsPopup();
        if(selectedCell != null) selectedCell.SetHighlight(false);
        selectedCell = null;
        selectedModel = null;
    }

    private void SetupModelCells() {
        var prefabs = Resources.LoadAll("Models");
        foreach (var prefab in prefabs) {
            var prefabName = prefab.name;
            var cellGameObject = Instantiate(projectCellPrefab, projectsScrollViewContent.transform);
            var cell = cellGameObject.GetComponent<ProjectCell>();
            Texture2D texture = null;
            var resPath = "ModelImages/" + prefabName;
            
#if UNITY_EDITOR
            texture = AssetPreview.GetAssetPreview(prefab);
            var texturePath = Application.dataPath + "/Textures/Resources/" + resPath + ".png";
            if(!File.Exists(texturePath)) SaveTextureAsPNG(texture, texturePath);
#endif

            texture = Resources.Load<Texture2D>(resPath);
            if(texture == null) Debug.Log("MenuManager.SetupModelCells(): Texture " + prefabName + " was not found!");
            cell.Setup(prefabName, texture);
            cell.onClick += OnModelSelect;
        }
    }

    private void OnModelSelect(ProjectCell newCell) {
        if(AppState.shared().Mode == AppMode.Presentation) {
            var versionName = IOManager.GetLastNameFromPath(IOManager.GetLatestVersionName(newCell.projectName));
            AppState.shared().ModelName = newCell.projectName;
            AppState.shared().ModelVersionName = versionName;
            SceneManager.LoadScene("CreatorScene");
            return;
        }
        if(versionPanel != null) DeleteVersionsPopup();
        if(selectedCell != null) selectedCell.SetHighlight(false);
        if (selectedCell == newCell) {
            selectedModel = null;
            selectedCell = null;
            return;
        }
        newCell.SetHighlight(true);
        selectedModel = newCell.projectName;
        selectedCell = newCell;
        CreateVersionsPopup(newCell.GetVersionsStartPosition());
    }

    private void CreateVersionsPopup(Vector3 pos) {
        var popup = Instantiate(versionsListPrefab, canvas.transform);
        popup.transform.position = pos;
        versionPanel = popup.GetComponent<VersionsPopupPanel>();
        var versions = IOManager.LoadModelVersionNames(selectedModel).Select(IOManager.GetLastNameFromPath);
        versionPanel.FillWithVersions(versions.ToArray());
        versionPanel.onCreateVersion += () => {
            AppState.shared().ModelName = selectedModel;
            AppState.shared().ModelVersionName = null;
            SceneManager.LoadScene("CreatorScene");
        };
        versionPanel.onVersionSelect += versionName => {
            AppState.shared().ModelName = selectedModel;
            AppState.shared().ModelVersionName = versionName;
            SceneManager.LoadScene("CreatorScene");
        };
        versionPanel.onVersionDelete += versionName => {
            IOManager.DeleteVersion(selectedModel, versionName);
            versionPanel.DeleteVersion(versionName);
        };
    }

    private void DeleteVersionsPopup() {
        Destroy(versionPanel.gameObject);
        versionPanel = null;
    }

    private void ImportModel() {
        Debug.Log("MenuManager.ImportModel(): Import not supported yet!");
    }
    
    private void SaveTextureAsPNG(Texture2D texture, string fullPath) {
        var bytes = texture.EncodeToPNG();
        File.WriteAllBytes(fullPath, bytes);
        Debug.Log("MenuManager.SaveTextureAsPNG(): " + bytes.Length/1024  + "kb was saved at:\n" + fullPath);
    }
}
