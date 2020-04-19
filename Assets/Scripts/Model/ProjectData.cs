
using System;
using System.Linq;

[Serializable]
public class ProjectData {
    public string name;
    public string modelPath;
    public DetailDataJsonWrapper[] detailsData;

    public ProjectData(string name, string modelPath, NodeDetailData[] detailsArray) {
        detailsData = detailsArray.Select(d => new DetailDataJsonWrapper(d)).ToArray();
        this.name = name;
        this.modelPath = modelPath;
    }

    public NodeDetailData[] ToOriginal() {
        return detailsData.Select(d => d.ToOriginal()).ToArray();
    }
}