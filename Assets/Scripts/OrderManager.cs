using TMPro;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance;

    [SerializeField]
    private TMP_Text orderText;

    private int energyRequired = 2;
    private int bioRequired = 1;

    private int energyDelivered;
    private int bioDelivered;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();
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

    private void UpdateUI()
    {
        orderText.text =
            "ORDER\n\n" +
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
            orderText.text =
                "ORDER COMPLETE!";
        }
    }
}