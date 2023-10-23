using UnityEngine;

public class AnimalAudioSounds : MonoBehaviour
{
    [SerializeField] private RandomClipAudioClipContainer availableSounds;

    [SerializeField] private Vector2 minMaxTimeBetweenSounds;
    [SerializeField] private Vector2 chanceToEmit;

    private float timer;

    private void Awake()
    {
        SetTimer();

    }

    private void SetTimer()
    {
        timer = RandomHelper.RandomFloat(minMaxTimeBetweenSounds);
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            if (RandomHelper.EvaluateChanceTo(chanceToEmit))
            {
                availableSounds.PlayOneShot();
            }
            SetTimer();
        }
    }
}
