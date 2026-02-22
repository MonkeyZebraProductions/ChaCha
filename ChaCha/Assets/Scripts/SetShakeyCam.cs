using Cinemachine;
using UnityEngine;

public class SetShakeyCam : MonoBehaviour
{
    CinemachineBasicMultiChannelPerlin cBMCP;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cBMCP = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetShakey()
    {
        if (cBMCP != null)
        {
            cBMCP.m_AmplitudeGain = 2f;
            cBMCP.m_FrequencyGain = 0.25f;
        }
        else
        {
            Debug.Log("No Shakey");
        }
    }

    public void SetStill()
    {
        if (cBMCP != null)
        {
            cBMCP.m_AmplitudeGain = 0;
            cBMCP.m_FrequencyGain = 0;
        }
        else
        {
            Debug.Log("No Shakey");
        }
    }
}
