using UnityEngine;

public class CargoCollector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Container"))
            return;

        Debug.Log("Cargo delivered");

        Destroy(gameObject);
    }
}