using UnityEngine;

public class PlayWooshSound : MonoBehaviour
{

    [SerializeField]
    private AudioSource m_AudioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_AudioSource != null && !m_AudioSource.isPlaying)
        {
            m_AudioSource.Play();
            Debug.Log("PlaySound");
        }
    }
}
