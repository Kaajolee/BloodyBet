using UnityEngine;
using UnityEngine.Events;

public class CurrencyManager : MonoBehaviour
{
    [SerializeField]
    private int startingBalance = 1000;

    public int Balance { get; private set; }

    public UnityEvent<int> OnBalanceChanged;

    void Start()
    {
        Balance = startingBalance;
        OnBalanceChanged?.Invoke(Balance);
    }

    public void AddMoney(int amount)
    {
        Balance += amount;
        OnBalanceChanged?.Invoke(Balance);
    }

    public void RemoveMoney(int amount)
    {
        if (Balance < amount)
        {
            Balance = 0;
            return;
        }

        Balance -= amount;
        OnBalanceChanged?.Invoke(Balance);
    }
}
