using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShapeDragHandler : MonoBehaviour
{
    public float dragSpeed = 10f;
    public float cellSize = 1.28f;
    public Vector2 gridMin = new Vector2(0f, 0f);
    public Vector2 gridMax = new Vector2(7f, 8f);

    private Rigidbody rb;
    private Camera cam;
    private bool isDragging = false;
    private Vector3 dragOffset;
    private Plane dragPlane;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;

        // Freeze everything by default
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    private void OnMouseDown()
    {
        isDragging = true;

        // Unfreeze X and Y movement for dragging
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;

        dragPlane = new Plane(Vector3.back, transform.position);
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            dragOffset = transform.position - hitPoint;
        }
    }

    private void OnMouseDrag()
    {
        if (!isDragging) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 target = ray.GetPoint(enter) + dragOffset;

            // Clamp to grid area
            target.x = Mathf.Clamp(target.x, gridMin.x * cellSize, gridMax.x * cellSize);
            target.y = Mathf.Clamp(target.y, gridMin.y * cellSize, gridMax.y * cellSize);
            target.z = transform.position.z;

            rb.linearVelocity = (target - transform.position) * dragSpeed;
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;

        // Stop and freeze the tray
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeAll;

        // Snap to grid
        Vector3 pos = transform.position;
        float snappedX = Mathf.Round(pos.x / cellSize) * cellSize;
        float snappedY = Mathf.Round(pos.y / cellSize) * cellSize;
        Vector3 snappedPos = new Vector3(snappedX, snappedY, pos.z);

        rb.MovePosition(snappedPos);
    }
}
