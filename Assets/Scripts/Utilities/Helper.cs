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

    public static void FullScreenViewer(Sprite sprite) {
        var creatorManager = GameObject.FindObjectOfType<CreatorManager>();
        creatorManager.ViewFullScreenImage(sprite);
    }

    public static void CreatePasswordPopup(PasswordPopup.OnDone callback) {
        var canvas = GameObject.Find("Canvas");
        var popup = GameObject.Instantiate(Resources.Load("PasswordPopup"), canvas.transform) as GameObject;
        popup.GetComponent<PasswordPopup>().onDone += callback;
    }
}