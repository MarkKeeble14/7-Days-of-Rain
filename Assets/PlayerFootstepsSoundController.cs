using UnityEngine;

public class PlayerFootstepsSoundController : MonoBehaviour
{
    [SerializeField] private SerializableDictionary<int, AudioClip[]> surfaceFootStepsMap = new SerializableDictionary<int, AudioClip[]>();
    [SerializeField] private Vector2 minMaxPitch;
    [SerializeField] private Vector2 minMaxVolume;
    [SerializeField] private AudioSource source;
    private AudioClip lastPlayedClip;
    private AudioClip[] clips;

    [SerializeField] private AudioClip[] fallbackClips;


    private void Update()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.down, out hit);
        if (hit.transform != null)
        {
            int layer = hit.transform.gameObject.layer;
            if (surfaceFootStepsMap.ContainsKey(layer))
            {
                clips = surfaceFootStepsMap[layer];
            }
        }
        else
        {
            clips = fallbackClips;
        }
    }

    public void PlayStep()
    {
        AudioClip clip = GetClip();
        source.pitch = RandomHelper.RandomFloat(minMaxPitch);
        source.volume = RandomHelper.RandomFloat(minMaxVolume);
        source.PlayOneShot(clip);
    }

    private AudioClip GetClip()
    {
        AudioClip clip = RandomHelper.GetRandomFromArray(clips);
        if (clip == lastPlayedClip)
        {
            return GetClip();
        }
        lastPlayedClip = clip;
        return clip;
    }
}