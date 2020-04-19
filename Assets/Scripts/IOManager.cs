using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class IOManager {
    public static bool SaveCurrentProject(string projectName, string json, bool overwrite = true) {
        var projectFolder = GetProjectFolder(projectName, true);
        var jsonPath = projectFolder + "/interactionPoints.json";
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

    public static string[] LoadSavedProjecs() {
        return Directory.GetDirectories(GetAllProjectsFolder());
    }

    public static string LoadProjectJson(string projectName) {
        var folder = GetProjectFolder(projectName);
        if (folder == null) {
            Debug.Log("IOManager.LoadProject(): Project directory was not found!");
            return null;
        }
        var jsonPath = folder + "/interactionPoints.json";
        if (!File.Exists(jsonPath)) {
            Debug.Log("IOManager.LoadProject(): File with JSON was not found!");
            return null;
        }
        var sr = new StreamReader(jsonPath);
        var fileContent = sr.ReadToEnd();
        sr.Close();
        return fileContent;
    }

    private static string GetAllProjectsFolder() {
        var folderPath = Application.persistentDataPath + "/Projects";
        if (!Directory.Exists(folderPath)) {
            Directory.CreateDirectory(folderPath);
        }
        return folderPath;
    }

    private static string GetProjectFolder(string name, bool createIfNonExisting = false) {
        var folderPath = GetAllProjectsFolder() + "/" + name;
        if (!Directory.Exists(folderPath)) {
            if (createIfNonExisting) {
                Directory.CreateDirectory(folderPath);
            }
            else {
                return null;
            }
        }
        return folderPath;
    }
}
