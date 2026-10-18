using UnityEngine;

public class CannonController : MonoBehaviour
{
    public GameObject cannonBallPrefab;
    public GameObject targetMarkerPrefab;
    public float launchAngleDeg = 45f; // firing constraint: fixed launch angle

    const float gy = -9.81f; // gravity, y component (m/s^2)

    Camera cam;
    GameObject marker;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        // target position in world space (1 unit=1 m)
        Vector2 target = cam.ScreenToWorldPoint(Input.mousePosition);
        if (target.y < 0f) return; // ignore clicks below the ground

        Vector2 p0 = transform.position;

        float dx = target.x - p0.x;
        float dy = target.y - p0.y;
        if (Mathf.Abs(dx) < 0.1f) return;

        // Firing solution
        // from Equation 1, for the target (dx, dy) at time t:
        //   dx = ux * sm * t
        //   dy = uy * sm * t + gy * t^2 / 2
        // eliminating t, with ux = cos(theta) and uy = sin(theta):
        //   sm^2 = -gy * dx^2 / ( 2 * cos^2(theta) * (|dx| * tan(theta) - dy) )
        float theta = launchAngleDeg * Mathf.Deg2Rad;
        float cos = Mathf.Cos(theta);
        float tan = Mathf.Tan(theta);

        float denom = 2f * cos * cos * (Mathf.Abs(dx) * tan - dy);
        if (denom <= 0f)
        {
            Debug.Log("Target out of range for this angle");
            return;
        }
        float sm = Mathf.Sqrt(-gy * dx * dx / denom);

        // launch direction
        Vector2 u = new Vector2(Mathf.Sign(dx) * cos, Mathf.Sin(theta));

        // rotate the cannon along u
        float angle = Mathf.Atan2(u.y, u.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        if (marker != null) Destroy(marker);
        marker = Instantiate(targetMarkerPrefab, target, Quaternion.identity);

        GameObject ball = Instantiate(cannonBallPrefab, p0, Quaternion.identity);
        ball.GetComponent<Projectile>().Launch(p0, u, sm, target);
    }
}