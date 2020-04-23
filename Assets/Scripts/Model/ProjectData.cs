
using System;
using System.Linq;

[Serializable]
public class ProjectData {
    public DetailDataJsonWrapper[] detailsData;

    public ProjectData(NodeDetailData[] detailsArray) {
        detailsData = detailsArray.Select(d => new DetailDataJsonWrapper(d)).ToArray();
    }

    public NodeDetailData[] ToOriginal() {
        return detailsData.Select(d => d.ToOriginal()).ToArray();
    }
}