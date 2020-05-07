using UnityEngine;
using UnityEngine.UI;

public class NodeDetailPresentation: MonoBehaviour {
    [SerializeField] private GameObject textCellPrefab;
    [SerializeField] private GameObject imageCellPrefab;
    [SerializeField] private GameObject videoCellPrefab;
    [SerializeField] private GameObject animationCellPrefab;
    
    [SerializeField] private Text titleText;
    [SerializeField] private GameObject scrollViewContent;
    [SerializeField] private Button cancelBtn;
    
    public Animator modelAnimator;
    
    public void UpdateData(NodeDetailData data) {
        titleText.text = data.title;
        foreach (var cellData in data.cells) {
            var cellObj = cellData.GetCell();
            cellObj.transform.SetParent(scrollViewContent.transform);
            var cell = cellObj.GetComponent<NodeDetailCell>();
            cell.CreatingEnded();
            var animCell = cell.GetComponent<NodeDetailAnimationCell>();
            if (animCell != null) animCell.onTriggerAnimation += OnAnimatorTrigger;
        }
    }

    private void Start() {
        cancelBtn.onClick.AddListener(() => { Destroy(gameObject); });
    }
    
    private void OnAnimatorTrigger(string triggerName) {
        Debug.Log("Playing animation: " + triggerName);
        modelAnimator.SetTrigger(triggerName);
    }
    
}