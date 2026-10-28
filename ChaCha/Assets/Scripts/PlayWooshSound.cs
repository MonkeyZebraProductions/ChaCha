using UnityEngine;

public class PlayWooshSound : MonoBehaviour
{

    [SerializeField]
    private AudioSource m_AudioSource;

    private void OnTriggerEnter(Collider other)
    {
        if (m_AudioSource != null && !m_AudioSource.isPlaying)
        {
            m_AudioSource.Play();
            Debug.Log("PlaySound");
        }
    }
}
