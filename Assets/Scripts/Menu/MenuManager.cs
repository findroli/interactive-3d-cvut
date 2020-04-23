using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SFB;
using Dummiesman;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager: MonoBehaviour {
    [SerializeField] private GameObject projectCellPrefab;
    [SerializeField] private GameObject versionCellPrefab;

    [SerializeField] private Button importBtn;
    [SerializeField] private Button startBtn;
    [SerializeField] private Button newVersionBtn;
    [SerializeField] private ModePicker modePicker;
    [SerializeField] private GameObject projectsScrollViewContent;
    [SerializeField] private GameObject versionsScrollViewContent;

    private LoadInfo loadInfo;
    private string selectedModel;
    
    void Start() {
        loadInfo = FindObjectOfType<LoadInfo>();
        if (loadInfo == null) {
            var loadInfoGameObject = new GameObject();
            loadInfo = loadInfoGameObject.AddComponent<LoadInfo>();
            loadInfoGameObject.name = "LoadInfo";
            DontDestroyOnLoad(loadInfoGameObject);
        }
        importBtn.onClick.AddListener(ImportModel);
        startBtn.onClick.AddListener(StartCreation);
        newVersionBtn.onClick.AddListener(NewVersion);
        SetupModelCells();
    }

    private void SetupModelCells() {
        var prefabNames = Resources.LoadAll("Models").Select(p => p.name);
        foreach (var prefabName in prefabNames) {
            var cellGameObject = Instantiate(projectCellPrefab, projectsScrollViewContent.transform);
            var cell = cellGameObject.GetComponent<ProjectCell>();
            Texture2D texture = null;
            //if (File.Exists(projectPath + "/image.png")) {
            // byte[] fileData = File.ReadAllBytes(projectPath + "/image.png");
            // texture = new Texture2D(2, 2);
            // texture.LoadImage(fileData);
            //}
            cell.Setup(prefabName, texture);
            cell.onClick += () => {
                selectedModel = prefabName;
                UpdateVersions();
            };
        }
    }

    private void UpdateVersions() {
        for (int i = 0; i < versionsScrollViewContent.transform.childCount; i++) {
            Destroy(versionsScrollViewContent.transform.GetChild(i).gameObject);
        }
        
        var versions = IOManager.LoadModelVersionNames(selectedModel).Select(v => v.Split('/').Last());
        foreach (var version in versions) {
            var cellGameObject = Instantiate(versionCellPrefab, versionsScrollViewContent.transform);
            var cell = cellGameObject.GetComponent<ProjectCell>();
            cell.Setup(version);
            cell.onClick += () => {
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

    private void StartCreation() {
        loadInfo.SetAppMode(modePicker.CurrentAppMode);
        SceneManager.LoadScene("CreatorScene");
    }

    private void ImportModel() {
        Debug.Log("Import not supported yet!");
    }
}
