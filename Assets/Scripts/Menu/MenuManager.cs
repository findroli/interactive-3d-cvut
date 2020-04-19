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
    [SerializeField] private Button importBtn;
    [SerializeField] private Button startBtn;
    [SerializeField] private Text modelNameText;
    [SerializeField] private ModePicker modePicker;

    private LoadInfo loadInfo;
    
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
        UpdateImportModelName();
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

    private void UpdateImportModelName() {
        if (loadInfo.ImportObjectPath == null) {
            modelNameText.text = "";
            return;
        }
        modelNameText.text = loadInfo.ImportObjectPath.Split('/').Last();
    }
}
