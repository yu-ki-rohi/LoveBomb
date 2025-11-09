using UnityEngine;
public interface IEnemyManaged
{
    // –ß‚è’l‚Æ‚µ‚ÄAUpdate‚ª³íI—¹‚µ‚½‚©‚Ç‚¤‚©‚ğ•Ô‚·
    public bool ManagedUpdate();

    public bool ManagedFixedUpdate();
}
