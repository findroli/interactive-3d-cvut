using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadInfo: MonoBehaviour {
    private string importObjectPath = null;
    public string ImportObjectPath => importObjectPath;

    private AppMode appMode = AppMode.Edit;
    public AppMode AppMode => appMode;

    private string loadProjectName = null;
    public string LoadProjectName => loadProjectName;


    public void SetPath(string path) {
        importObjectPath = path;
    }

    public void SetAppMode(AppMode mode) {
        appMode = mode;
    }

    public void SetLoadProjectName(string projectName) {
        loadProjectName = projectName;
    }
}
