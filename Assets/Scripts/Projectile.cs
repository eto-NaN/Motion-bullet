using UnityEngine;

public class Projectile : MonoBehaviour
{
    const float gy = -9.81f;
    Vector2 g = new Vector2(0f, gy);     // gravity vector

    Vector2 p0, u;
    float sm, t, ti;
    bool flying;

    public void Launch(Vector2 p0, Vector2 u, float sm, Vector2 target)
    {
        this.p0 = p0;
        this.u = u;
        this.sm = sm;
        t = 0f;

        // Equation 2
        float uy = u.y * sm;
        float disc = uy * uy - 2f * gy * (p0.y - target.y);
        if (disc < 0f)
        {
            Destroy(gameObject);
            return;
        }
        float root = Mathf.Sqrt(disc);
        float t1 = (-uy + root) / gy;
        float t2 = (-uy - root) / gy;

        // keep the positive time for which the horizontal position matches the target
        float err1 = t1 > 0f ? Mathf.Abs(p0.x + u.x * sm * t1 - target.x) : float.MaxValue;
        float err2 = t2 > 0f ? Mathf.Abs(p0.x + u.x * sm * t2 - target.x) : float.MaxValue;
        ti = err1 < err2 ? t1 : t2;

        // check the horizontal displacement at ti
        float xAtTi = p0.x + u.x * sm * ti;
        Debug.Log("ti = " + ti + " s, x error = " + Mathf.Abs(xAtTi - target.x));

        transform.position = p0;
        flying = true;
    }

    void Update()
    {
        if (!flying) return;

        t += Time.deltaTime;
        if (t >= ti)
        {
            t = ti;
            flying = false;
            Destroy(gameObject, 1f);
        }

        // Equation 1
        transform.position = p0 + u * sm * t + g * (t * t) / 2f;
    }
}