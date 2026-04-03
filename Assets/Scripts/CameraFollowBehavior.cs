using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollowBehavior : MonoBehaviour
{
    [SerializeField]
    private float speed = 0.3f;

    [SerializeField]
    private Transform targetPos;

    private Vector3 velocity = Vector3.zero;

    private Vector3 target;

    [SerializeField]
    private float panOffset = 3f;
    private float downOffset;
    [SerializeField]
    private float normalOffset = 1.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetPos = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {

        target = targetPos.position + new Vector3(0f, normalOffset + downOffset, -10f);

        transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, speed);
    }

    public void SetPanAngle(float panMultiplier)
    {
        downOffset = panOffset * panMultiplier;
    }
}
