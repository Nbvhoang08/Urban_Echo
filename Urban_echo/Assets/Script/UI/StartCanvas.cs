using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCanvas : CanvasUI
{
    public ThirdPCamera cam;
    private void Awake()
    {
        if (cam == null)
        {
            cam = FindAnyObjectByType<ThirdPCamera>();
        }
    }

    private void Update()
    {
        if (cam == null)
        {
            cam = FindAnyObjectByType<ThirdPCamera>();
        }
    }
    public void TapToPlay()
    {
        SoundManager.Instance.PlayClickSound();
        cam.StartTransition();
        StartCoroutine(ReadytoStart());
    }
    IEnumerator ReadytoStart()
    {
        yield return 1;
        UIManager.Instance.CloseUI<StartCanvas>(1f);
        UIManager.Instance.OpenUI<GamePlayCanvas>();
    }

}
