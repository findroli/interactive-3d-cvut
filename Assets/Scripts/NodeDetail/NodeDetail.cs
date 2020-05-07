using UnityEngine;

public delegate void OnCancel();

public interface NodeDetail {
    void UpdateData(NodeDetailData data);
    void SetModelAnimator(Animator modelAnimator);
}
