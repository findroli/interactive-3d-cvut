using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NodeDetailImageCell: NodeDetailCell {
    [SerializeField] private Image image;
    [SerializeField] private Button deleteButton;

    private byte[] textureData;

    private void Start() {
        deleteButton.onClick.AddListener(() => { RaiseOnDelete(this); });
    }

    public override void CreatingEnded() { }

    public override void FillWithData(NodeCellData data) {
        var imageCellData = data as NodeImageCellData;
        if(imageCellData == null) return;
        textureData = imageCellData.imageData;
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(imageCellData.imageData); //..this will auto-resize the texture dimensions.
        image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width/2f, tex.height/2f));
        image.preserveAspect = true;
    }

    public override NodeCellData GetData() {
        return new NodeImageCellData {
            imageData = textureData
        };
    }
}
