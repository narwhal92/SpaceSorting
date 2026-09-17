using TMPro;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance;

    [SerializeField]
    private TMP_Text orderText;

    [SerializeField]
    private int totalOrders = 3;

    private int currentOrder = 1;

    private int energyRequired;
    private int bioRequired;

    private int energyDelivered;
    private int bioDelivered;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GenerateOrder();
    }

    public void CargoDelivered(string cargoTag)
    {
        if (cargoTag == "Energy")
        {
            energyDelivered++;
        }

        if (cargoTag == "Bio")
        {
            bioDelivered++;
        }

        UpdateUI();

        CheckOrderComplete();
    }

    private void GenerateOrder()
    {
        energyRequired = Random.Range(1, 4);
        bioRequired = Random.Range(1, 3);

        energyDelivered = 0;
        bioDelivered = 0;

        UpdateUI();
    }

    private void UpdateUI()
    {
        orderText.text =
            "ORDER " + currentOrder + "/" + totalOrders +
            "\n\n" +
            "Energy: " +
            energyDelivered +
            "/" +
            energyRequired +
            "\n" +
            "Bio: " +
            bioDelivered +
            "/" +
            bioRequired;
    }

    private void CheckOrderComplete()
    {
        if (
            energyDelivered >= energyRequired &&
            bioDelivered >= bioRequired
        )
        {
            if (currentOrder >= totalOrders)
            {
                orderText.text =
                    "LEVEL COMPLETE!";
            }
            else
            {
                currentOrder++;
                GenerateOrder();
            }
        }
    }
}