using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager: MonoBehaviour {
    // Start is called before the first frame update

    [SerializeField] private Button importBtn;
    [SerializeField] private Button startBtn;
    [SerializeField] private Text modelNameText;
    
    void Start() {
        importBtn.onClick.AddListener(ImportModel);
        startBtn.onClick.AddListener(StartCreation);
    }

    // Update is called once per frame
    void Update() {
        
    }

    private void ImportModel() {
        Debug.Log("Import model - not functional yet!");
    }

    private void StartCreation() {
        Debug.Log("Start creation");
        SceneManager.LoadScene("CreatorScene");
    }
}
