
/// <summary>
/// UI_ScrollerInput is a Unity MonoBehaviour script that manages the automatic scrolling of UI content within a viewport.

/// This class handles the automatic scrolling of a UI content within a viewport. 
/// It allows for setting a scrolling speed and gradually slows down the scrolling as it approaches the end of the content. 
/// The class also provides a method to reset the content position and speed to their initial values.
/// </summary>


using UnityEngine;


public class UIScrollingSpeed : MonoBehaviour
{
    // ==========================================
    // 1. References
    // ==========================================
    [Header("References")]
    public RectTransform viewport;                                                                                      // Viewport RectTransform (the visible area through which the content is viewed)
    public RectTransform content;                                                                                       // Content RectTransform (the scrollable content that moves within the viewport)

    // ==========================================
    // 2. Parameters
    // ==========================================
    [Header("Parameters")]
    public float speed = 50f;                                                                                           // Scrolling speed
    public float slowDownDistance = 200f;                                                                               // Distance from the end of the content at which to begin slowing down

    private Vector3 startPosition;
    private float startSpeed;

    // ==========================================
    // 3. Unity Methods
    // ==========================================
    void Start()
    {
        InitializeScroller();
    }

    void Update()
    {
        HandleScrolling();
    }

    // ==========================================
    // 4. Custom Methods
    // ==========================================

    // 1. Initializes the scroller by storing the starting position and speed
    private void InitializeScroller()
    {
        startSpeed = speed;
        if (content != null)
        {
            startPosition = content.localPosition;
        }
    }

    // 2. Handles the scrolling logic, including slowing down as it approaches the end of the content
    private void HandleScrolling()
    {
        if (viewport == null || content == null)
            return;

        // Calculate how far the content needs to travel
        float length = Mathf.Max(0, content.sizeDelta.y - viewport.rect.height);
        float distanceTraveled = Vector3.Distance(content.localPosition, startPosition);

        // Calculate how much distance is left to travel
        float distanceLeft = length - distanceTraveled;

        if (speed < 0)
        {
            distanceLeft = distanceTraveled;
        }

        // Automatically begin slowing down as we get closer to the specified slowDownDistance
        float currentSpeed = Mathf.Lerp(speed, 0, Mathf.InverseLerp(slowDownDistance, 0, distanceLeft));

        // Only scroll if there is still distance to travel
        if (length > 0 && distanceLeft > 0)
        {
            Vector3 delta = Vector3.up * currentSpeed * Time.deltaTime;
            content.localPosition += delta;
        }
    }

    // ==========================================
    // 5. Reset Method
    // ==========================================
    public void ResetPosition()
    {
        if (content != null)
        {
            content.localPosition = startPosition;
        }
        speed = startSpeed;
    }
}
