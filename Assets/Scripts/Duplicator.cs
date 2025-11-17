using UnityEngine;

public class Duplicator : MonoBehaviour
{
    [SerializeField] private float offset = 2.142857f;
    [SerializeField] private GameObject objectToDuplicate;
    [SerializeField] private Transform target;
    
    // Update is called once per frame
    void Update()
    {
        if (transform.position.x + offset * 2 < target.position.x)
        {
            Instantiate(objectToDuplicate, new Vector3(transform.position.x + offset * 2, transform.position.y, transform.position.z), Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
