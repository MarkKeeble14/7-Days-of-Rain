using UnityEngine;

public class AnimalData : MonoBehaviour
{
    [SerializeField] private AnimalMovement movement;
    public AnimalMovement Movement => movement;
    [SerializeField] private AnimalRotation rotation;
    [SerializeField] private AnimalAudioSounds sounds;
    public AnimalRotation Rotation => rotation;

    [SerializeField] private Vector2 minMaxScale;

    [SerializeField] private Collider collider;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float deathAngleZ;
    [SerializeField] private Vector2 minMaxDeathAngleY;
    [SerializeField] private SimpleAudioClipContainer deathClip;

    [SerializeField] private MeshRenderer renderer;
    [SerializeField] private Material deadMat;

    private void Awake()
    {
        transform.localScale = RandomHelper.RandomFloat(minMaxScale) * Vector3.one;
    }

    [ContextMenu("Kill")]
    public void Kill()
    {
        StopAllCoroutines();
        sounds.enabled = false;
        movement.enabled = false;
        movement.Disabled = true;
        rotation.enabled = false;
        rotation.Disabled = true;
        collider.enabled = false;
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.None;
        transform.localEulerAngles = new Vector3(0, RandomHelper.RandomFloat(minMaxDeathAngleY), deathAngleZ);
        deathClip.PlayOneShot();
        renderer.material = deadMat;
    }
}
