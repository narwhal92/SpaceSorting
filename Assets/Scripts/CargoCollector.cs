using UnityEngine;

public class CargoCollector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        bool correct =
            (CompareTag("Energy") && other.CompareTag("EnergyContainer")) ||
            (CompareTag("Bio") && other.CompareTag("BioContainer")) ||
            (CompareTag("Tech") && other.CompareTag("TechContainer"));

        if (correct)
        {
            Debug.Log("Correct!");

            LevelManager.Instance.CargoDelivered();

            OrderManager.Instance.CargoDelivered(tag);

            Destroy(gameObject);
        }
        
        else
        {
            Debug.Log("Wrong container");
        }
    }
}