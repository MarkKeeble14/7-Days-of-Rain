using UnityEngine;
using TMPro;

public class ToolTip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI contentText;
    [SerializeField] private TextMeshProUGUI labelText;

    private bool hasLabel;

    public void Set(string label, string content)
    {
        hasLabel = label.Length > 0;

        labelText.text = label;
        contentText.text = content;
    }

    private void Update()
    {
        labelText.gameObject.SetActive(hasLabel);
    }
}
