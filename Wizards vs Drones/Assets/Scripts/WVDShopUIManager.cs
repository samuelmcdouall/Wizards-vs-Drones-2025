using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WVDShopUIManager : MonoBehaviour
{
    [Header("General")]
    [SerializeField]
    WVDPlayer _playerScript;
    [SerializeField]
    float _discountChance;
    [SerializeField]
    int[] _possibleDiscounts;

    [Header("Three Arc")]
    [SerializeField]
    int _threeArcBasePrice;
    [SerializeField]
    int _threeArcFinalPrice;
    [SerializeField]
    Button _threeArcPurchaseButton;
    [SerializeField]
    TMP_Text _threeArcPriceText;
    [SerializeField]
    GameObject _threeArcDisabledImage;
    [SerializeField]
    GameObject _threeArcPurchasedImage;

    [Header("Stun")]
    [SerializeField]
    int _stunBasePrice;
    [SerializeField]
    int _stunFinalPrice;
    [SerializeField]
    Button _stunPurchaseButton;
    [SerializeField]
    TMP_Text _stunPriceText;
    [SerializeField]
    GameObject _stunDisabledImage;
    [SerializeField]
    GameObject _stunPurchasedImage;


    void Awake()
    {
        SetDiscountPrice(_threeArcBasePrice, ref _threeArcFinalPrice, _threeArcPriceText);
        SetDiscountPrice(_stunBasePrice, ref _stunFinalPrice, _stunPriceText);
    }
    void UpdateBuyableItems()
    {
        print("Checking prices");
        CheckPlayerCanBuyUpgrade(_threeArcFinalPrice, _threeArcPurchasedImage, _threeArcPurchaseButton, _threeArcDisabledImage);
        CheckPlayerCanBuyUpgrade(_stunFinalPrice, _stunPurchasedImage, _stunPurchaseButton, _stunDisabledImage);

    }
    public void PurchaseThreeArc()
    {
        _playerScript.BatteryCount -= _threeArcFinalPrice;
        _playerScript.PurchasedUpgrades.ShootThreeArc = true;
        _threeArcPurchaseButton.enabled = false;
        _threeArcPurchasedImage.SetActive(true);
        UpdateBuyableItems();
    }

    public void PurchaseStun()
    {
        _playerScript.BatteryCount -= _stunFinalPrice;
        _playerScript.PurchasedUpgrades.StunAttacks = true;
        _playerScript.PurchasedUpgrades.StunAttackDuration = 0.5f;
        _stunPurchaseButton.enabled = false;
        _stunPurchasedImage.SetActive(true);
        UpdateBuyableItems();
    }

    






    void SetDiscountPrice(int basePrice, ref int finalPrice, TMP_Text priceText)
    {
        float rand = Random.Range(0.0f, 1.0f);
        if (rand < _discountChance)
        {
            int chosenDiscount = _possibleDiscounts[Random.Range(0, _possibleDiscounts.Length)];
            float priceModifier = (100 - chosenDiscount) / 100.0f;
            int discountedPrice = (int)(Mathf.Round(basePrice * priceModifier));
            priceText.text = $"Price: {discountedPrice} ({chosenDiscount}% Off!)";
            finalPrice = discountedPrice;
        }
        else
        {
            priceText.text = $"Price: {basePrice}";
            finalPrice = basePrice;
        }
    }

    void CheckPlayerCanBuyUpgrade(int finalPrice, GameObject purchasedImage, Button purchaseButton, GameObject disabledImage)
    {
        if (!purchasedImage.activeSelf) // If haven't bought the item
        {
            if (_playerScript.BatteryCount < finalPrice) // Don't have enough
            {
                purchaseButton.enabled = false;
                disabledImage.SetActive(true);
            }
            else // Have enough
            {
                purchaseButton.enabled = true;
                disabledImage.SetActive(false);
            }
        }
        else // If have bought the item
        {
            purchaseButton.enabled = false;
            disabledImage.SetActive(false);
        }
    }



    private void OnEnable()
    {
        UpdateBuyableItems();
    }
}
