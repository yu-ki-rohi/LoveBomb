using UnityEngine;

public abstract class ProjectileBase : MonoBehaviour
{
    
    // ”ò‹——£
    private float flyingDistance;

    public virtual void Initialize()
    {
        flyingDistance = 0.0f;
    }

    public abstract void Deactivate();

    protected void GoStraight(float speed, float effectiveRange)
    {
        float distance = speed * Time.deltaTime;
        transform.Translate(Vector3.up * distance);
        flyingDistance += distance;

        if(flyingDistance < effectiveRange) { return; }

        Deactivate();
    }

}
