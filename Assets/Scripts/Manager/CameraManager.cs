using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CinemachineVirtualCamera cinemachineVirtualCam;
    public static CinemachineTransposer transposer;
    public static CinemachineBasicMultiChannelPerlin channelPerlin;

    private void Start()
    {
        cinemachineVirtualCam = GetComponent<CinemachineVirtualCamera>();
        channelPerlin = cinemachineVirtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        transposer = cinemachineVirtualCam.GetCinemachineComponent<CinemachineTransposer>();
    }

    public static IEnumerator ShakeCamera(float intensity, float time)
    {
        if (channelPerlin.m_AmplitudeGain < 2)
            channelPerlin.m_AmplitudeGain += intensity;
        yield return new WaitForSeconds(time);
        channelPerlin.m_AmplitudeGain -= intensity;
    }
}
