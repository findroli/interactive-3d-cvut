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
    private float scrollViewWidth;
    private GameObject canvas;
    
    void Start() {
        importBtn.onClick.AddListener(ImportModel);
        SetupModelCells();
        canvas = GameObject.Find("Canvas");
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
            cell.onClick += newCell => {
                if(versionPanel != null) DeleteVersionsPopup();
                if(selectedCell != null) selectedCell.SetHighlight(false);
                if (selectedCell == newCell) {
                    selectedModel = null;
                    selectedCell = null;
                    return;
                }
                newCell.SetHighlight(true);
                selectedModel = prefabName;
                selectedCell = newCell;
                CreateVersionsPopup(newCell.GetVersionsStartPosition());
            };
        }
    }

    private void CreateVersionsPopup(Vector3 pos) {
        var popup = Instantiate(versionsListPrefab, canvas.transform);
        popup.transform.position = pos;
        versionPanel = popup.GetComponent<VersionsPopupPanel>();
        var versions = IOManager.LoadModelVersionNames(selectedModel).Select(v => v.Split('/').Last());
        versionPanel.FillWithVersions(versions.ToArray());
        versionPanel.onCreateVersion += () => {
            AppState.shared().modelName = selectedModel;
            AppState.shared().modelVersionName = null;
            SceneManager.LoadScene("CreatorScene");
        };
        versionPanel.onVersionSelect += (versionName) => {
            AppState.shared().modelName = selectedModel;
            AppState.shared().modelVersionName = versionName;
            SceneManager.LoadScene("CreatorScene");
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
