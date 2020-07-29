using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdminListCell: MonoBehaviour {
    public delegate void OnAccept(string userId, AdminListCell cell);
    public static event OnAccept onAccept;
    
    public delegate void OnDecline(string userId, AdminListCell cell);
    public static event OnDecline onDecline;
    
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button declineButton;
    [SerializeField] private Text nameText;

    private string userId = "";

    public void SetName(string name) {
        nameText.text = name;
    } 
    
    public void SetId(string id) {
        userId = id;
    } 
    
    void Start() {
        acceptButton.onClick.AddListener(() => { onAccept?.Invoke(userId, this); });
        declineButton.onClick.AddListener(() => { onDecline?.Invoke(userId, this); });
    }
}
