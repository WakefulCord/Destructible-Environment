using UnityEngine;

public class SoundManager : MonoBehaviour
{
    #region Class References
    private static SoundManager _instance;

    [SerializeField] private AudioSource audioSource;
    #endregion

    #region Private Fields

    #endregion

    #region Properties
    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<SoundManager>();
                if (_instance == null)
                {
                    Debug.LogError("SoundManager has not been assigned");
                }
            }
            return _instance;
        }
    }


    #endregion

    #region Start Up
    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }
    #endregion

    #region Class Methods
    public void PlayAudioClip(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("SoundManager: AudioClip is null");
            return;
        }

        audioSource.PlayOneShot(clip);
    }

    public void PlayClipAtPoint(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("SoundManager: AudioClip is null");
            return;
        }

        AudioSource.PlayClipAtPoint(clip, position, volume);
    }
    #endregion

}
