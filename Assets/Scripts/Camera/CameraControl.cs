using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float minSize          = 6.5f;
    public float screenEdgeBuffer = 5.0f;
    public float dampTime         = 0.25f;
    [HideInInspector] public Transform[] targets;

    private float zoomSpeed;

    private new Camera camera;
    private Vector3 moveVelocity;
    private Vector3 desiredPosition;


    private void Awake()
    {
        camera = GetComponentInChildren<Camera>();
    }

    private void FixedUpdate()
    {
        Move();
        Zoom();
    }

    private void Move()
    {
        FindAveragePosition();
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref moveVelocity, dampTime);
    }

    private void FindAveragePosition()
    {
        int numTargets = 0;
        Vector3 averagePos = new Vector3();

        for (int i = 0; i < targets.Length; i++)
        {
            if (!targets[i].gameObject.activeSelf)
            {
                continue;
            }

            averagePos += targets[i].position;
            numTargets++;
        }

        if (numTargets > 0)
        {
            averagePos /= numTargets;
        }

        averagePos.y = transform.position.y;
        desiredPosition = averagePos;
    }

    private void Zoom()
    {
        float requiredSize = FindRequiredSize();
        camera.orthographicSize = Mathf.SmoothDamp(camera.orthographicSize, requiredSize, ref zoomSpeed, dampTime);
    }

    private float FindRequiredSize()
    {
        float size = 0f;
        Vector3 desiredLocalPos = transform.InverseTransformPoint(desiredPosition);

        for (int i = 0; i < targets.Length; i++)
        {
            if (!targets[i].gameObject.activeSelf)
            {
                continue;
            }

            Vector3 targetLocalPos = transform.InverseTransformPoint(targets[i].position);
            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / camera.aspect);
        }
        
        size += screenEdgeBuffer;
        size = Mathf.Max(size, minSize);

        return size;
    }

    public void SetStartPositionAndSize()
    {
        FindAveragePosition();
        transform.position = desiredPosition;
        camera.orthographicSize = FindRequiredSize();
    }
}