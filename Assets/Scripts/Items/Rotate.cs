using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 2f;

    void Update()
    {
        transform.Rotate(0f, 0f, 360 * rotationSpeed * Time.deltaTime);

    }
}
