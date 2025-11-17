using UnityEngine;

public class Follower : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float verticalOffset = 0f;

    void Start()
    {
        transform.position = new Vector3(target.position.x, target.position.y + verticalOffset, transform.position.z);
    }

    void LateUpdate()
    {
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y + verticalOffset, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, followSpeed * Time.deltaTime);
        transform.position = new Vector3(targetPosition.x, transform.position.y, transform.position.z);
    }
}
