using UnityEngine;

public class TracerEffect : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Vector3 startPos;
    [SerializeField] private Vector3 endPos;

    [SerializeField] private float maxAliveTime = 5f;
    private float lifeTimer = 0.0f;
    private float progress = 0.0f;
    private float totalDistance;

    public void OnInit(float s, Vector3 start, Vector3 end)
    {
        speed = s;
        startPos = start;
        endPos = end;
        transform.position = startPos;
        lifeTimer = 0.0f;
        progress = 0.0f;
        totalDistance = Vector3.Distance(startPos, endPos);
    }

    public bool UpdateTracer(float dt)
    {
        lifeTimer += dt;
        if (totalDistance > 0f)
            progress += (speed * dt) / totalDistance;
        else
            progress = 1f;

        transform.position = Vector3.Lerp(startPos, endPos, progress);

        if (progress >= 1f || lifeTimer >= maxAliveTime)
            return true;

        return false;
    }
}