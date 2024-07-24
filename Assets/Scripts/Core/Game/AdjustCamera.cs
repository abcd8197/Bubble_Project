using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustCamera : MonoBehaviour
{
    private void Start()
    {
        // 0.5625
        float basicRatio = 1080f / 1920f;
        float currentRatio = Screen.width / (float)Screen.height;

        // 가로가 더 길다
        if (basicRatio > currentRatio + float.Epsilon)
            AdjusCameraResolution();
    }

    private void AdjusCameraResolution()
    {
        // 기본 카메라 사이즈
        float basicCameraSize = Camera.main.orthographicSize;
        // 0.5625
        float basicRatio = 1080f / 1920f;
        // ex) 1080x2500 --> 0.432
        float currentRatio = Screen.width / (float)Screen.height;

        // 0.5625 / 0.432 --> 1.3020833....
        float cameraSizeDestination = basicRatio / currentRatio;
        // 6.510416
        Camera.main.orthographicSize = cameraSizeDestination * basicCameraSize;

        // 사이즈 변경된 만큼 Y축을 위로 올려 조작부가 화면 하단에 올 수 있게 한다.
        Camera.main.transform.position = new Vector3(0, Camera.main.orthographicSize - basicCameraSize, -Camera.main.farClipPlane);
    }
}
