﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class NodeDetailVideoCell: NodeDetailCell {
    [SerializeField] private RawImage image;
    [SerializeField] private VideoPlayer player;
    [SerializeField] private Button deleteButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button fullScreenButton;
    
    private string filename;

    private void Start() {
        deleteButton.onClick.AddListener(() => { RaiseOnDelete(this); });
        playButton.onClick.AddListener(Play);
        pauseButton.onClick.AddListener(Pause);
        fullScreenButton.onClick.AddListener(() => {
            player.Pause();
            Helper.FullScreenViewer(filename);
        });
    }

    public override void CreatingEnded() {
        deleteButton.gameObject.SetActive(false);
    }

    public override void FillWithData(NodeCellData data) {
        var videoCellData = data as NodeVideoCellData;
        if(videoCellData == null) return;
        filename = videoCellData.videoFile;
        player.url = "file://" + filename;
    }

    public override NodeCellData GetData() {
        return new NodeVideoCellData {
            videoFile = filename
        };
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
        image.texture = player.texture;
        player.Play();
    }
}
