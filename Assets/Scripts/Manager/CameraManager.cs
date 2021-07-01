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

    public static IEnumerator ShakeCamera(float intensity, float time, bool isBoss = false)
    {

        if (channelPerlin.m_AmplitudeGain < 2)
            channelPerlin.m_AmplitudeGain += intensity;

        yield return new WaitForSeconds(time);
        channelPerlin.m_AmplitudeGain -= intensity;
    }

    public static void SetCameraTarget(Transform target)
    {
        cinemachineVirtualCam.Follow = target;
        cinemachineVirtualCam.LookAt = target;
    }
}
