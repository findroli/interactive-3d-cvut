using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Button = UnityEngine.UI.Button;

public class FullScreenViewer: MonoBehaviour {
    [SerializeField] private Button cancelBtn;
    [SerializeField] private Image image;

    [SerializeField] private GameObject videoObject;
    [SerializeField] private VideoPlayer player;
    [SerializeField] private Button playButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private RawImage rawImage;

    public void ViewImage(Sprite sprite) {
        transform.SetAsLastSibling();
        videoObject.SetActive(false);
        rawImage.gameObject.SetActive(false);
        image.gameObject.SetActive(true);
        image.sprite = sprite;
    }

    public void ShowVideo(string filePath) {
        transform.SetAsLastSibling();
        videoObject.SetActive(true);
        rawImage.gameObject.SetActive(true);
        playButton.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(false);
        image.gameObject.SetActive(false);
        player.url = "file://" + filePath;
        player.aspectRatio = VideoAspectRatio.FitInside;
    }

    void Start() {
        playButton.onClick.AddListener(Play);
        pauseButton.onClick.AddListener(Pause);
        image.preserveAspect = true;
        cancelBtn.onClick.AddListener(() => {
            gameObject.SetActive(false);
        });
    }

    private void Play() {
        StartCoroutine(PlayCoroutine());
        playButton.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(true);
    }

    private void Pause() {
        player.Pause();
        playButton.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(false);
    }

    private IEnumerator PlayCoroutine() {
        player.Prepare();
        var wait = new WaitForSeconds(1);
        while(!player.isPrepared) {
            yield return wait;
        }
        rawImage.texture = player.texture;
        player.Play();
    }
}
