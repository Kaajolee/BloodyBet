using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CurrencyManager : MonoBehaviour
{
    [SerializeField]
    private int startingBalance = 1000;

    public int currentBet = 0;

    [SerializeField]
    public GameObject sliderObject;

    private Slider slider;

    [SerializeField]
    private GameObject value;

    [SerializeField]
    private GameObject type;

    private TextMeshProUGUI valueText;
    private TextMeshProUGUI typeText;



    public int Balance { get; private set; }

    public UnityEvent<int> OnBalanceChanged;

    void Start()
    {
        Balance = startingBalance;
        OnBalanceChanged?.Invoke(Balance);

        valueText = value.GetComponent<TextMeshProUGUI>();
        typeText = type.GetComponent<TextMeshProUGUI>();
        slider = sliderObject.GetComponent<Slider>();

        ChangeValuesOnHand();
    }

    public void AddMoney(int amount)
    {
        Balance += amount;
        OnBalanceChanged?.Invoke(Balance);

        ChangeValuesOnHand();
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

        ChangeValuesOnHand();
    }

    public void ChangeValuesOnHand()
    {
        currentBet = (int)(slider.value * Balance);
        valueText.text = currentBet.ToString();
        typeText.text = Balance.ToString();
        OnBalanceChanged?.Invoke(Balance);
    }
}
