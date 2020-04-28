using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class DetailDataJsonWrapper {
    public Vector3 position;
    public string title;
    public CellDataJsonWrapper[] cells;

    public DetailDataJsonWrapper(NodeDetailData detailData) {
        position = detailData.position;
        title = detailData.title;
        cells = detailData.cells.Select(c => new CellDataJsonWrapper(c)).ToArray();
    }

    public NodeDetailData ToOriginal() {
        var origCells = cells.Select(c => c.ToOriginal()).ToArray();
        return new NodeDetailData {
            position = this.position,
            title = this.title,
            cells = origCells
        };
    }
}

[Serializable]
public class CellDataJsonWrapper {
    public string type;
    public string textData;
    public byte[] imageData;

    public CellDataJsonWrapper(NodeCellData cellData) {
        var cellType = cellData.GetType();
        if (cellType == typeof(NodeTextCellData)) {
            type = "NodeTextCellData";
            textData = (cellData as NodeTextCellData).text;
        } 
        else if (cellType == typeof(NodeImageCellData)) {
            type = "NodeImageCellData";
            imageData = (cellData as NodeImageCellData).imageData;
        }
        else if (cellType == typeof(NodeVideoCellData)) {
            type = "NodeVideoCellData";
            textData = (cellData as NodeVideoCellData).videoFile;
        }
        else if (cellType == typeof(NodeAnimationCellData)) {
            type = "NodeAnimationCellData";
            textData = (cellData as NodeAnimationCellData).animName;
        }
    }

    public NodeCellData ToOriginal() {
        switch (type) {
            case "NodeTextCellData":
                return new NodeTextCellData { text = textData };
            case "NodeImageCellData":
                return new NodeImageCellData { imageData = imageData };
            case "NodeVideoCellData":
                return new NodeVideoCellData() { videoFile = textData };
            case "NodeAnimationCellData":
                return new NodeAnimationCellData() { animName = textData };
        }
        return new NodeTextCellData { text = "" };
    }
}
