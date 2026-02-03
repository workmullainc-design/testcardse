using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip cardClickClip;
    [SerializeField] private AudioClip matchClip;

    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AudioManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("AudioManager");
                    instance = obj.AddComponent<AudioManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlayCardClick()
    {
        if (cardClickClip != null)
        {
            sfxSource.PlayOneShot(cardClickClip);
        }
    }

    public void PlayMatch()
    {
        if (matchClip != null)
        {
            sfxSource.PlayOneShot(matchClip);
        }
    }
}
