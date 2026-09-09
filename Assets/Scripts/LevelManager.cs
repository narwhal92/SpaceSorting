using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    private int remainingCargo;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        remainingCargo = GameObject.FindObjectsByType<CargoCollector>(
            FindObjectsSortMode.None
        ).Length;

        Debug.Log("Remaining Cargo: " + remainingCargo);
    }

    public void CargoDelivered()
    {
        remainingCargo--;

        Debug.Log("Remaining Cargo: " + remainingCargo);

        if (remainingCargo <= 0)
        {
            Debug.Log("LEVEL COMPLETE!");
        }
    }
}