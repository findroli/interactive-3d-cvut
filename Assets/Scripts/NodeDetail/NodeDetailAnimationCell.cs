using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeDetailAnimationCell: NodeDetailCell {
    public delegate void OnTriggerAnimation(string name);
    public event OnTriggerAnimation onTriggerAnimation;
    
    [SerializeField] private Button playBtn;
    [SerializeField] private Button deleteBtn;
    [SerializeField] private Text text;

    private string animId;
    
    void Start() {
        playBtn.onClick.AddListener(() => {
            onTriggerAnimation?.Invoke(animId);
        });
        deleteBtn.onClick.AddListener(() => { RaiseOnDelete(this); });
    }
    
    public override void CreatingEnded() { }

    public override void FillWithData(NodeCellData data) {
        var animData = data as NodeAnimationCellData;
        if (animData == null) return;
        text.text = animData.animName;
        animId = animData.animName;
        Debug.Log("NodeDetailAnimationCell.FillWithData(): " + animData.animName);
    }

    public override NodeCellData GetData() {
        return new NodeAnimationCellData {
            animName = animId
        };
    }
}
