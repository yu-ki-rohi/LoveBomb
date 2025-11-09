using UnityEngine;

public interface IContactable
{
    public void OnCollisionEnter(Collider other);
    public void OnCollisionStay(Collider other);

    public void OnCollisionExit(Collider other);
    public void OnTriggerEnter(Collider other);
    public void OnTriggerStay(Collider other);
    public void OnTriggerExit(Collider other);
}
