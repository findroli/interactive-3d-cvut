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
    [SerializeField] private GameObject versionCellPrefab;

    [SerializeField] private Button importBtn;
    [SerializeField] private Button newVersionBtn;
    [SerializeField] private ModePicker modePicker;
    [SerializeField] private GameObject scrollView;
    [SerializeField] private GameObject projectsScrollViewContent;
    [SerializeField] private GameObject versionsScrollViewContent;
    [SerializeField] private GameObject emptyVersionsLabel;

    private LoadInfo loadInfo;
    private ProjectCell selectedCell;
    private string selectedModel;
    private float scrollViewWidth;
    
    void Start() {
        loadInfo = FindObjectOfType<LoadInfo>();
        if (loadInfo == null) {
            var loadInfoGameObject = new GameObject();
            loadInfo = loadInfoGameObject.AddComponent<LoadInfo>();
            loadInfoGameObject.name = "LoadInfo";
            DontDestroyOnLoad(loadInfoGameObject);
        }
        importBtn.onClick.AddListener(ImportModel);
        newVersionBtn.onClick.AddListener(NewVersion);
        SetupModelCells();
        
        scrollViewWidth = Screen.width + scrollView.GetComponent<RectTransform>().sizeDelta.x;
    }

    //TODO: not used - smaller cells when further from the center
    private void UpdateCellSizes() {
        for(int i = 0; i < projectsScrollViewContent.transform.childCount; i++) {
            var cell = projectsScrollViewContent.transform.GetChild(i).GetComponent<RectTransform>();
            var dist = (scrollView.transform.position - cell.position).magnitude;
            var scale = 1 - dist / (scrollViewWidth / 2);
            Debug.Log("Dist: " + dist + ", Width: " + scrollViewWidth + ", Scale: " + scale);
            if (scale < 0) scale = 0;
            cell.sizeDelta = new Vector2(200 * scale, cell.sizeDelta.y);
        }
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
            cell.onClick += (newCell) => {
                if(selectedCell != null) selectedCell.SetHighlight(false);
                newCell.SetHighlight(true);
                selectedModel = prefabName;
                selectedCell = newCell;
                UpdateVersions();
            };
        }
    }

    private void UpdateVersions() {
        for (int i = 0; i < versionsScrollViewContent.transform.childCount; i++) {
            Destroy(versionsScrollViewContent.transform.GetChild(i).gameObject);
        }
        var versions = IOManager.LoadModelVersionNames(selectedModel).Select(v => v.Split('/').Last());
        emptyVersionsLabel.SetActive(!versions.Any());
        foreach (var version in versions) {
            var cellGameObject = Instantiate(versionCellPrefab, versionsScrollViewContent.transform);
            var cell = cellGameObject.GetComponent<ProjectCell>();
            cell.Setup(version);
            cell.onClick += (clickedCell) => {
                loadInfo.SetAppMode(modePicker.CurrentAppMode);
                loadInfo.SetModelName(selectedModel);
                loadInfo.SetVersion(version);
                SceneManager.LoadScene("CreatorScene");
            };
        }
    }

    private void NewVersion() {
        if (selectedModel == null) return;
        loadInfo.SetAppMode(modePicker.CurrentAppMode);
        loadInfo.SetModelName(selectedModel);
        loadInfo.SetVersion(null);
        SceneManager.LoadScene("CreatorScene");
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
