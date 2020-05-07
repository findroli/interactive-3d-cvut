using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class IOManager {
    public static string GetLastNameFromPath(string path) {
        return path?.Split('/').Last();
    }
    
    public static bool SaveCurrentProject(string modelName, string versionName, string json, bool overwrite = true) {
        var versionFolder = GetVersionFolder(modelName, versionName, true);
        var jsonPath = versionFolder + "/interactionPoints.json";
        if (File.Exists(jsonPath)) {
            if (!overwrite) return false; 
            File.Delete(jsonPath);
            Debug.Log("IOManager.SaveCurrentProject(): File already exists - was deleted!");
        }
        var sr = File.CreateText(jsonPath);
        sr.Write(json);
        sr.Close();
        Debug.Log("IOManager.SaveCurrentProject(): JSON was saved in path:\n" + jsonPath);
        return true;
    }

    public static string CurrentProjectVersionImagePath(string modelName, string versionName) {
        var path = GetVersionFolder(modelName, versionName);
        return path == null ? null : path + "/image.png";
    }

    public static string[] LoadSavedProjects() {
        return Directory.GetDirectories(GetAllModelsFolder());
    }

    public static string[] LoadModelVersionNames(string modelName) {
        var folder = GetModelFolder(modelName);
        if (folder == null) return new string[] {};
        return Directory.GetDirectories(folder);
    }

    public static string LoadProjectJson(string modelName, string versionName) {
        var folder = GetVersionFolder(modelName, versionName);
        if (folder == null) {
            Debug.Log("IOManager.LoadProjectJson(): Model version directory was not found!\n" + 
                      "Model: " + modelName + " Version: " + versionName);
            return null;
        }
        var jsonPath = folder + "/interactionPoints.json";
        if (!File.Exists(jsonPath)) {
            Debug.Log("IOManager.LoadProjectJson(): File with JSON was not found!");
            return null;
        }
        var sr = new StreamReader(jsonPath);
        var fileContent = sr.ReadToEnd();
        sr.Close();
        return fileContent;
    }

    public static string GetLatestVersionName(string modelName) {
        var versions = LoadModelVersionNames(modelName);
        if(!versions.Any()) return null;
        return versions.OrderBy(Directory.GetCreationTime).First();
    }

    private static string GetAllModelsFolder() {
        var folderPath = Application.persistentDataPath + "/Models";
        if (!Directory.Exists(folderPath)) {
            Directory.CreateDirectory(folderPath);
        }
        return folderPath;
    }

    private static string GetModelFolder(string name, bool createIfNonExisting = false) {
        var folderPath = GetAllModelsFolder() + "/" + name;
        if (!Directory.Exists(folderPath)) {
            if (createIfNonExisting) Directory.CreateDirectory(folderPath);
            else return null;
        }
        return folderPath;
    }

    private static string GetVersionFolder(string modelName, string versionName, bool createIfNonExisting = false) {
        var modelFolderPath = GetModelFolder(modelName, createIfNonExisting);
        if (!Directory.Exists(modelFolderPath)) return null;
        var folderPath = modelFolderPath + "/" + versionName;
        if (!Directory.Exists(folderPath)) {
            if (createIfNonExisting) Directory.CreateDirectory(folderPath);
            else return null;
        }
        return folderPath;
    }
}
