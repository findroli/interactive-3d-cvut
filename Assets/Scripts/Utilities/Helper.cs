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

    public static void CreateMessagePopup(string message) {
        var canvas = GameObject.Find("Canvas");
        var popup = GameObject.Instantiate(Resources.Load("MessagePopup"), canvas.transform) as GameObject;
        popup.GetComponent<MessagePopup>().SetText(message);
    }

    public static void CreateConfirmPopup(string message, string confirmText, ConfirmPopup.OnConfirm callback) {
        var canvas = GameObject.Find("Canvas");
        var popup = GameObject.Instantiate(Resources.Load("ConfirmPopup"), canvas.transform) as GameObject;
        var component = popup.GetComponent<ConfirmPopup>();
        component.SetText(message);
        component.SetConfirmText(confirmText);
        component.onConfirm += callback;
    }

    public static void CreateSavePopup(string placeholder, SavePopup.OnSave callback) {
        var canvas = GameObject.Find("Canvas");
        var popup = GameObject.Instantiate(Resources.Load("SavePopup"), canvas.transform) as GameObject;
        var component = popup.GetComponent<SavePopup>();
        component.SetPlaceHolder(placeholder);
        component.onSave += callback;
    }
}