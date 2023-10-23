using UnityEngine;

public class CinematicBars : MonoBehaviour
{
    public static CinematicBars _Instance { get; private set; }
    private void Awake()
    {
        _Instance = this;
    }

    public bool Active { get; set; }

    [SerializeField] private RectTransform topBlackBar;
    [SerializeField] private RectTransform bottomBlackBar;
    [SerializeField] private Vector3 topGoalPos;
    [SerializeField] private Vector3 topRestPos;
    [SerializeField] private Vector3 botGoalPos;
    [SerializeField] private Vector3 botRestPos;
    [SerializeField] private float speed;

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    private void Update()
    {
        if (Active)
        {
            topBlackBar.anchoredPosition = Vector3.MoveTowards(topBlackBar.anchoredPosition, topGoalPos, Time.deltaTime * speed);
            bottomBlackBar.anchoredPosition = Vector3.MoveTowards(bottomBlackBar.anchoredPosition, botGoalPos, Time.deltaTime * speed);
        }
        else
        {
            topBlackBar.anchoredPosition = Vector3.MoveTowards(topBlackBar.anchoredPosition, topRestPos, Time.deltaTime * speed);
            bottomBlackBar.anchoredPosition = Vector3.MoveTowards(bottomBlackBar.anchoredPosition, botRestPos, Time.deltaTime * speed);
        }
    }
}
