using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    [SerializeField] private TMP_Text progressText;
    private readonly HashSet<CargoCollector> remaining = new HashSet<CargoCollector>();
    public int TotalCargo { get; private set; }
    public int DeliveredCargo { get; private set; }
    public bool IsPrepared { get; private set; }
    public bool IsComplete { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    // Called explicitly after spawning; independent of MonoBehaviour Start order.
    public void PrepareLevel(IEnumerable<CargoCollector> cargo)
    {
        if (IsPrepared) return;
        foreach (CargoCollector item in cargo)
            if (item != null) remaining.Add(item);
        TotalCargo = remaining.Count;
        IsPrepared = true;
        UpdateUI();
    }

    public bool CanDeliver(CargoCollector cargo)
    {
        return IsPrepared && !IsComplete && cargo != null && remaining.Contains(cargo);
    }

    public bool TryDeliver(CargoCollector cargo)
    {
        if (!IsPrepared || IsComplete || cargo == null || !remaining.Remove(cargo))
            return false;
        DeliveredCargo++;
        if (DeliveredCargo == TotalCargo)
        {
            IsComplete = true;
            Debug.Log("LEVEL COMPLETE!");
        }
        UpdateUI();
        return true;
    }

    private void UpdateUI()
    {
        if (progressText == null) return;
        progressText.text = "Рассортируй весь груз\n" + DeliveredCargo + " / " + TotalCargo;
        if (IsComplete) progressText.text += "\nУровень завершён!";
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
