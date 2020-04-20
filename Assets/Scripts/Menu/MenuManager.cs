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

    [SerializeField] private Button demoBtn;
    [SerializeField] private Button importBtn;
    [SerializeField] private Button startBtn;
    [SerializeField] private Text modelNameText;
    [SerializeField] private ModePicker modePicker;
    [SerializeField] private GameObject projectsScrollViewContent;

    private LoadInfo loadInfo;
    
    void Start() {
        loadInfo = FindObjectOfType<LoadInfo>();
        if (loadInfo == null) {
            var loadInfoGameObject = new GameObject();
            loadInfo = loadInfoGameObject.AddComponent<LoadInfo>();
            loadInfoGameObject.name = "LoadInfo";
            DontDestroyOnLoad(loadInfoGameObject);
        }
        
        demoBtn.onClick.AddListener(Demo);
        importBtn.onClick.AddListener(ImportModel);
        startBtn.onClick.AddListener(StartCreation);
        UpdateImportModelName();
        
        var projects = IOManager.LoadSavedProjecs();
        foreach (var projectPath in projects) {
            Debug.Log("Found project in path:\n" + projectPath);
            var cellGameObject = Instantiate(projectCellPrefab, projectsScrollViewContent.transform);
            var cell = cellGameObject.GetComponent<ProjectCell>();
            var projectName = projectPath.Split('/').Last();
            Texture2D texture = null;
            if (File.Exists(projectPath + "/image.png")) {
                byte[] fileData = File.ReadAllBytes(projectPath + "/image.png");
                texture = new Texture2D(2, 2);
                texture.LoadImage(fileData);
            }
            cell.Setup(projectName, texture);
            cell.onClick += () => {
                loadInfo.SetAppMode(modePicker.CurrentAppMode);
                loadInfo.SetLoadProjectName(projectName);
                SceneManager.LoadScene("CreatorScene");
            };
        }
    }

    private void ImportModel() {
        var extensions = new [] {
            new ExtensionFilter("3D model files", "obj", "mtl")
        };
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
        if(paths.Length == 0) return;
        if (File.Exists(paths[0])) {
            loadInfo.SetPath(paths[0]);
            UpdateImportModelName();
        }
    }

    private void StartCreation() {
        loadInfo.SetAppMode(modePicker.CurrentAppMode);
        SceneManager.LoadScene("CreatorScene");
    }

    private void Demo() {
        loadInfo.SetAppMode(modePicker.CurrentAppMode);
        loadInfo.SetPath(null);
        loadInfo.SetLoadProjectName(null);
        SceneManager.LoadScene("CreatorScene");
    }

    private void UpdateImportModelName() {
        if (loadInfo.ImportObjectPath == null) {
            modelNameText.text = "";
            return;
        }
        modelNameText.text = loadInfo.ImportObjectPath.Split('/').Last();
    }
}
