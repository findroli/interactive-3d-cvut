using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadInfo: MonoBehaviour {
    private AppMode appMode = AppMode.Edit;
    public AppMode AppMode => appMode;

    private string modelName = null;
    public string ModelName => modelName;
    
    private string versionName = null;
    public string VersionName => versionName;

    public void SetVersion(string version) {
        versionName = version;
    }

    public void SetAppMode(AppMode mode) {
        appMode = mode;
    }

    public void SetModelName(string model) {
        modelName = model;
    }
}
