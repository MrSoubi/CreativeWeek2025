using UnityEngine;

public class PickupDestroyer : MonoBehaviour
{
    public void DestroyAfterAnimation()
    {
        Destroy(transform.parent.gameObject);
    }
}
