using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FlashOfLightDissapationMethod
{
    DESTROY,
    FADE
}

[RequireComponent(typeof(Light))]
public class FlashOfLight : MonoBehaviour
{
    private Light light;

    [SerializeField] private float maxIntensity;
    [SerializeField] private float scaleIntensityRate;
    [SerializeField] private float maxRange;
    [SerializeField] private float scaleRangeRate;
    [SerializeField] private float duration = 1f;
    [SerializeField] private MathHelper.CalculationMethod calcMethod;
    [SerializeField] private Color flashColor;
    [SerializeField] private float fadeIntensityRate;
    [SerializeField] private FlashOfLightDissapationMethod dissapationMethod;

    [SerializeField] private TemporaryAudioSourceSpawner audioSource;
    [SerializeField] private RandomClipAudioClipContainer possibleClips;

    private void Awake()
    {
        light = GetComponent<Light>();
    }

    [ContextMenu("Activate")]
    public void Activate(float delay)
    {
        StartCoroutine(Flash(delay));
    }

    private IEnumerator Flash(float delay)
    {
        light.color = flashColor;
        yield return new WaitForSeconds(delay);
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            switch (calcMethod)
            {
                case MathHelper.CalculationMethod.LERP:
                    light.intensity = Mathf.Lerp(light.intensity, maxIntensity, Time.deltaTime * scaleIntensityRate);
                    light.range = Mathf.Lerp(light.range, maxRange, Time.deltaTime * scaleRangeRate);
                    break;
                case MathHelper.CalculationMethod.MOVE_TOWARDS:
                    light.intensity = Mathf.MoveTowards(light.intensity, maxIntensity, Time.deltaTime * scaleIntensityRate);
                    light.range = Mathf.MoveTowards(light.range, maxRange, Time.deltaTime * scaleRangeRate);
                    break;
            }
            yield return null;
        }

        // Audio
        audioSource.PlayOneShot(possibleClips);

        switch (dissapationMethod)
        {
            case FlashOfLightDissapationMethod.DESTROY:
                Destroy(gameObject);
                break;
            case FlashOfLightDissapationMethod.FADE:
                while (light.intensity > 1)
                {
                    switch (calcMethod)
                    {
                        case MathHelper.CalculationMethod.LERP:
                            light.intensity = Mathf.Lerp(light.intensity, 0, Time.deltaTime * fadeIntensityRate);
                            break;
                        case MathHelper.CalculationMethod.MOVE_TOWARDS:
                            light.intensity = Mathf.MoveTowards(light.intensity, 0, Time.deltaTime * fadeIntensityRate);
                            break;
                    }
                    yield return null;
                }
                Destroy(gameObject);
                break;
            default:
                throw new UnhandledSwitchCaseException();
        }
    }
}
