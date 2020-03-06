using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddNodeDetailCell: MonoBehaviour {
    public Button addBtn;
    public Button createBtn;
    public Button cancelBtn;

    private bool _creationStarted = false;

    public bool creationStarted {
        get { return _creationStarted; }
        set {
            _creationStarted = value;
            if (_creationStarted) {
                addBtn.gameObject.SetActive(false);
                cancelBtn.gameObject.SetActive(true);
                createBtn.gameObject.SetActive(true);
            }
        }
    }
}
