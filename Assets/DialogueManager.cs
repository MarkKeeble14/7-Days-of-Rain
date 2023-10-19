using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Dialogue
{
    public string Text;
    public float WaitTime;
    public float DestroyAfter;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager _Instance { get; private set; }

    [SerializeField] private DialogueLineDisplay textPrefab;
    [SerializeField] private Transform spawnOn;

    private void Awake()
    {
        _Instance = this;
    }

    public IEnumerator ExecuteDialogue(Dialogue[] dialogues)
    {
        for (int i = 0; i < dialogues.Length; i++)
        {
            Dialogue d = dialogues[i];

            DialogueLineDisplay text = Instantiate(textPrefab, spawnOn);
            text.SetText(d.Text, d.DestroyAfter);

            yield return new WaitForSeconds(d.WaitTime);
        }
    }
}
