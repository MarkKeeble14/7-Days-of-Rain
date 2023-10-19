using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueLineDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private CanvasGroup cv;
    [SerializeField] private float fadeSpeed;

    public void SetText(string text, float duration)
    {
        this.text.text = text;
        StartCoroutine(DestroyAfterTime(duration));
    }

    private IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        yield return StartCoroutine(Utils.ChangeCanvasGroupAlpha(cv, 0, Time.deltaTime * fadeSpeed));
        Destroy(gameObject);
    }
}
