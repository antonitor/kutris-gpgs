using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera vCam;
    
    private FichaController ficha;
    private Spawner.Face lastFace;
    [HideInInspector]
    public bool endgame = false;
    float circularEndgmae = 0f;

    CinemachineBasicMultiChannelPerlin virtualCameraNoise;
    float ShakeElapsedTime = 0f;

    public float ShakeDuration = 0.3f;
    public float ShakeAmplitude = 1.2f;
    public float ShakeFrequency = 2.0f;

    void Start()
    {
        if (vCam != null)
        {
            virtualCameraNoise = vCam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
        }
        MoveCamera(Spawner.Face.FRONT);
    }

    private void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time);

        if (endgame)
        {
            circularEndgmae += Time.deltaTime / 14;
            vCam.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = circularEndgmae;
        }
        else if (!ficha.tetraToSpawnFace().Equals(lastFace))
        {
            lastFace = ficha.tetraToSpawnFace();
            MoveCamera(lastFace);
        } 

        if (virtualCameraNoise != null)
        {
            if (ShakeElapsedTime > 0)
            {
                virtualCameraNoise.m_AmplitudeGain = ShakeAmplitude;
                virtualCameraNoise.m_FrequencyGain = ShakeFrequency;

                ShakeElapsedTime -= Time.deltaTime;
            }
            else
            {
                virtualCameraNoise.m_AmplitudeGain = 0f;
                ShakeElapsedTime = 0f;
            }
        }
        
    }

    public void MoveCamera(Spawner.Face face)
    {
        switch (face)
        {
            case Spawner.Face.FRONT:
                vCam.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = 0f;
                break;
            case Spawner.Face.LEFT:
                vCam.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = 1;
                break;
            case Spawner.Face.BACK:
                vCam.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = 2;
                break;
            case Spawner.Face.RIGHT:
                vCam.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = 3;
                break;
        }
    }

    public void FollowFicha(FichaController ficha)
    {
        this.ficha = ficha;
    }

    public void Shake()
    {
        ShakeDuration = 0.1f;
        ShakeFrequency = 1f;
        ShakeElapsedTime = ShakeDuration;
    }
    
    public void Stomp()
    {
        ShakeDuration = 0.5f;
        ShakeFrequency = 3f;
        ShakeElapsedTime = ShakeDuration;

    }

}
