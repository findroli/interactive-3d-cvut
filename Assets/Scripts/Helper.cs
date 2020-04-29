using System.Collections;
using UnityEngine;

public class Helper {
    public static IEnumerator AnimateMovement(Transform obj, float speed, Vector3 toPos) {
        while ((toPos - obj.position).magnitude > speed) {
            var direction = (toPos - obj.position).normalized;
            obj.position += direction * speed;
            yield return null;
        }
        obj.transform.position = toPos;
    }
}