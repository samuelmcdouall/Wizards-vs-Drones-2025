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
    [SerializeField]
    TMP_Text _batteriesText;
    WVDSoundManager _soundManager;
    [SerializeField]
    Image[] _boughtUpgradeIcons;
    int _totalBoughtUpgrades;

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
    [SerializeField]
    Sprite _threeArcSprite;

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
    [SerializeField]
    Sprite _stunSprite;

    [Header("Drop Rate Increase")]
    [SerializeField]
    int _dropRateIncreaseBasePrice;
    [SerializeField]
    int _dropRateIncreaseFinalPrice;
    [SerializeField]
    Button _dropRateIncreasePurchaseButton;
    [SerializeField]
    TMP_Text _dropRateIncreasePriceText;
    [SerializeField]
    GameObject _dropRateIncreaseDisabledImage;
    [SerializeField]
    GameObject _dropRateIncreasePurchasedImage;
    [SerializeField]
    Sprite _dropRateIncreasedSprite;

    [Header("Slow")]
    [SerializeField]
    int _slowBasePrice;
    [SerializeField]
    int _slowFinalPrice;
    [SerializeField]
    Button _slowPurchaseButton;
    [SerializeField]
    TMP_Text _slowPriceText;
    [SerializeField]
    GameObject _slowDisabledImage;
    [SerializeField]
    GameObject _slowPurchasedImage;
    [SerializeField]
    Sprite _slowSprite;

    [Header("DOT")]
    [SerializeField]
    int _dotBasePrice;
    [SerializeField]
    int _dotFinalPrice;
    [SerializeField]
    Button _dotPurchaseButton;
    [SerializeField]
    TMP_Text _dotPriceText;
    [SerializeField]
    GameObject _dotDisabledImage;
    [SerializeField]
    GameObject _dotPurchasedImage;
    [SerializeField]
    Sprite _dotSprite;

    [Header("Pierce")]
    [SerializeField]
    int _pierceBasePrice;
    [SerializeField]
    int _pierceFinalPrice;
    [SerializeField]
    Button _piercePurchaseButton;
    [SerializeField]
    TMP_Text _piercePriceText;
    [SerializeField]
    GameObject _pierceDisabledImage;
    [SerializeField]
    GameObject _piercePurchasedImage;
    [SerializeField]
    Sprite _pierceSprite;

    [Header("Explode On Death")]
    [SerializeField]
    int _explodeOnDeathBasePrice;
    [SerializeField]
    int _explodeOnDeathFinalPrice;
    [SerializeField]
    Button _explodeOnDeathPurchaseButton;
    [SerializeField]
    TMP_Text _explodeOnDeathPriceText;
    [SerializeField]
    GameObject _explodeOnDeathDisabledImage;
    [SerializeField]
    GameObject _explodeOnDeathPurchasedImage;
    [SerializeField]
    Sprite _explodeOnDeathSprite;

    [Header("Critical")]
    [SerializeField]
    int _criticalBasePrice;
    [SerializeField]
    int _criticalFinalPrice;
    [SerializeField]
    Button _criticalPurchaseButton;
    [SerializeField]
    TMP_Text _criticalPriceText;
    [SerializeField]
    GameObject _criticalDisabledImage;
    [SerializeField]
    GameObject _criticalPurchasedImage;
    [SerializeField]
    Sprite _criticalSprite;

    [Header("Health Increase")]
    [SerializeField]
    int _healthIncreaseBasePrice;
    [SerializeField]
    int _healthIncreaseFinalPrice;
    [SerializeField]
    Button _healthIncreasePurchaseButton;
    [SerializeField]
    TMP_Text _healthIncreasePriceText;
    [SerializeField]
    GameObject _healthIncreaseDisabledImage;
    [SerializeField]
    GameObject _healthIncreasePurchasedImage;
    [SerializeField]
    Sprite _healthIncreasedSprite;

    [Header("Attack Speed")]
    [SerializeField]
    int _attackSpeedBasePrice;
    [SerializeField]
    int _attackSpeedFinalPrice;
    [SerializeField]
    Button _attackSpeedPurchaseButton;
    [SerializeField]
    TMP_Text _attackSpeedPriceText;
    [SerializeField]
    GameObject _attackSpeedDisabledImage;
    [SerializeField]
    GameObject _attackSpeedPurchasedImage;
    [SerializeField]
    Sprite _attackSpeedSprite;

    [Header("Dash Recharge")]
    [SerializeField]
    int _dashRechargeBasePrice;
    [SerializeField]
    int _dashRechargeFinalPrice;
    [SerializeField]
    Button _dashRechargePurchaseButton;
    [SerializeField]
    TMP_Text _dashRechargePriceText;
    [SerializeField]
    GameObject _dashRechargeDisabledImage;
    [SerializeField]
    GameObject _dashRechargePurchasedImage;
    [SerializeField]
    Sprite _dashRechargeSprite;

    [Header("Low Health Damage")]
    [SerializeField]
    int _lowHealthDamageBasePrice;
    [SerializeField]
    int _lowHealthDamageFinalPrice;
    [SerializeField]
    Button _lowHealthDamagePurchaseButton;
    [SerializeField]
    TMP_Text _lowHealthDamagePriceText;
    [SerializeField]
    GameObject _lowHealthDamageDisabledImage;
    [SerializeField]
    GameObject _lowHealthDamagePurchasedImage;
    [SerializeField]
    Sprite _lowHealthDamageSprite;

    void Awake()
    {
        SetDiscountPrice(_threeArcBasePrice, ref _threeArcFinalPrice, _threeArcPriceText);
        SetDiscountPrice(_stunBasePrice, ref _stunFinalPrice, _stunPriceText);
        SetDiscountPrice(_dropRateIncreaseBasePrice, ref _dropRateIncreaseFinalPrice, _dropRateIncreasePriceText);
        SetDiscountPrice(_slowBasePrice, ref _slowFinalPrice, _slowPriceText);
        SetDiscountPrice(_dotBasePrice, ref _dotFinalPrice, _dotPriceText);
        SetDiscountPrice(_pierceBasePrice, ref _pierceFinalPrice, _piercePriceText);
        SetDiscountPrice(_explodeOnDeathBasePrice, ref _explodeOnDeathFinalPrice, _explodeOnDeathPriceText);
        SetDiscountPrice(_criticalBasePrice, ref _criticalFinalPrice, _criticalPriceText);
        SetDiscountPrice(_healthIncreaseBasePrice, ref _healthIncreaseFinalPrice, _healthIncreasePriceText);
        SetDiscountPrice(_attackSpeedBasePrice, ref _attackSpeedFinalPrice, _attackSpeedPriceText);
        SetDiscountPrice(_dashRechargeBasePrice, ref _dashRechargeFinalPrice, _dashRechargePriceText);
        SetDiscountPrice(_lowHealthDamageBasePrice, ref _lowHealthDamageFinalPrice, _lowHealthDamagePriceText);
        _soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<WVDSoundManager>();
    }
    void UpdateBuyableItems()
    {
        print("Checking prices");
        CheckPlayerCanBuyUpgrade(_threeArcFinalPrice, _threeArcPurchasedImage, _threeArcPurchaseButton, _threeArcDisabledImage);
        CheckPlayerCanBuyUpgrade(_stunFinalPrice, _stunPurchasedImage, _stunPurchaseButton, _stunDisabledImage);
        CheckPlayerCanBuyUpgrade(_dropRateIncreaseFinalPrice, _dropRateIncreasePurchasedImage, _dropRateIncreasePurchaseButton, _dropRateIncreaseDisabledImage);
        CheckPlayerCanBuyUpgrade(_slowFinalPrice, _slowPurchasedImage, _slowPurchaseButton, _slowDisabledImage);
        CheckPlayerCanBuyUpgrade(_dotFinalPrice, _dotPurchasedImage, _dotPurchaseButton, _dotDisabledImage);
        CheckPlayerCanBuyUpgrade(_pierceFinalPrice, _piercePurchasedImage, _piercePurchaseButton, _pierceDisabledImage);
        CheckPlayerCanBuyUpgrade(_explodeOnDeathFinalPrice, _explodeOnDeathPurchasedImage, _explodeOnDeathPurchaseButton, _explodeOnDeathDisabledImage);
        CheckPlayerCanBuyUpgrade(_criticalFinalPrice, _criticalPurchasedImage, _criticalPurchaseButton, _criticalDisabledImage);
        CheckPlayerCanBuyUpgrade(_healthIncreaseFinalPrice, _healthIncreasePurchasedImage, _healthIncreasePurchaseButton, _healthIncreaseDisabledImage);
        CheckPlayerCanBuyUpgrade(_attackSpeedFinalPrice, _attackSpeedPurchasedImage, _attackSpeedPurchaseButton, _attackSpeedDisabledImage);
        CheckPlayerCanBuyUpgrade(_dashRechargeFinalPrice, _dashRechargePurchasedImage, _dashRechargePurchaseButton, _dashRechargeDisabledImage);
        CheckPlayerCanBuyUpgrade(_lowHealthDamageFinalPrice, _lowHealthDamagePurchasedImage, _lowHealthDamagePurchaseButton, _lowHealthDamageDisabledImage);
        _batteriesText.text = "" + _playerScript.BatteryCount;
    }
    public void PurchaseThreeArc()
    {
        _playerScript.BatteryCount -= _threeArcFinalPrice;
        _playerScript.PurchasedUpgrades.ShootThreeArc = true;
        _threeArcPurchaseButton.enabled = false;
        _threeArcPurchasedImage.SetActive(true);
        UpdateBuyableItems();
        AddSpriteToPurchasedIcons(_threeArcSprite);
        _soundManager.PlaySFXAtPlayer(_soundManager.BuyButtonSFX);

    }

    private void AddSpriteToPurchasedIcons(Sprite sprite)
    {
        _boughtUpgradeIcons[_totalBoughtUpgrades].gameObject.SetActive(true);
        _boughtUpgradeIcons[_totalBoughtUpgrades].sprite = sprite;
        if (sprite == _threeArcSprite)
        {
            _boughtUpgradeIcons[_totalBoughtUpgrades].color = new Color(0.7f, 0.0f, 0.0f);
        }
        else if (sprite == _stunSprite)
        {
            _boughtUpgradeIcons[_totalBoughtUpgrades].color = new Color(0.0f, 0.2f, 0.45f);
        }
        _totalBoughtUpgrades++;
    }

    public void PurchaseStun()
    {
        _playerScript.BatteryCount -= _stunFinalPrice;
        _playerScript.PurchasedUpgrades.StunAttacks = true;
        _playerScript.PurchasedUpgrades.StunAttackDuration = 0.5f;
        _stunPurchaseButton.enabled = false;
        _stunPurchasedImage.SetActive(true);
        AddSpriteToPurchasedIcons(_stunSprite);
        UpdateBuyableItems();
        _soundManager.PlaySFXAtPlayer(_soundManager.BuyButtonSFX);
    }

    public void PurchaseDropRateIncrease()
    {
        _playerScript.BatteryCount -= _dropRateIncreaseFinalPrice;
        _playerScript.PurchasedUpgrades.DropRateIncrease = 0.25f;
        _dropRateIncreasePurchaseButton.enabled = false;
        _dropRateIncreasePurchasedImage.SetActive(true);
        AddSpriteToPurchasedIcons(_dropRateIncreasedSprite);
        UpdateBuyableItems();
        _soundManager.PlaySFXAtPlayer(_soundManager.BuyButtonSFX);
    }

    public void PurchaseSlow()
    {
        _playerScript.BatteryCount -= _slowFinalPrice;
        _playerScript.PurchasedUpgrades.SlowAttacks = true;
        _playerScript.PurchasedUpgrades.SlowAttackPercentage = 0.5f;
        _playerScript.PurchasedUpgrades.SlowAttackDuration = 1.0f;
        _slowPurchaseButton.enabled = false;
        _slowPurchasedImage.SetActive(true);
        AddSpriteToPurchasedIcons(_slowSprite);
        UpdateBuyableItems();
        _soundManager.PlaySFXAtPlayer(_soundManager.BuyButtonSFX);
    }

    public void PurchaseDOT()
    {
        _playerScript.BatteryCount -= _dotFinalPrice;
        _playerScript.PurchasedUpgrades.DOTAttacks = true;
        _playerScript.PurchasedUpgrades.DOTAttackDuration = 3.0f;
        _playerScript.PurchasedUpgrades.DOTAttackInterval = 1.0f;
        _playerScript.PurchasedUpgrades.DOTAttackDamage = 1;
        _dotPurchaseButton.enabled = false;
        _dotPurchasedImage.SetActive(true);
        AddSpriteToPurchasedIcons(_dotSprite);
        UpdateBuyableItems();
        _soundManager.PlaySFXAtPlayer(_soundManager.BuyButtonSFX);
    }

    public void PurchasePierce()
    {
        _playerScript.BatteryCount -= _pierceFinalPrice;
        _playerScript.PurchasedUpgrades.Pierce = true;
        _piercePurchaseButton.enabled = false;
        _piercePurchasedImage.SetActive(true);
        AddSpriteToPurchasedIcons(_pierceSprite);
        UpdateBuyableItems();
        _soundManager.PlaySFXAtPlayer(_soundManager.BuyButtonSFX);
    }

    public void PurchaseExplodeOnDeath()
    {
        _playerScript.BatteryCount -= _explodeOnDeathFinalPrice;
        _playerScript.PurchasedUpgrades.ExplodeOnDeathChance = 0.25f;
        _explodeOnDeathPurchaseButton.enabled = false;
        _explodeOnDeathPurchasedImage.SetActive(true);
        AddSpriteToPurchasedIcons(_explodeOnDeathSprite);
        UpdateBuyableItems();
        _soundManager.PlaySFXAtPlayer(_soundManager.BuyButtonSFX);
    }

    public void PurchaseCritical()
    {
        _playerScript.BatteryCount -= _criticalFinalPrice;
        _playerScript.PurchasedUpgrades.CriticalChance = 0.25f;
        _criticalPurchaseButton.enabled = false;
        _criticalPurchasedImage.SetActive(true);
        AddSpriteToPurchasedIcons(_criticalSprite);
        UpdateBuyableItems();
        _soundManager.PlaySFXAtPlayer(_soundManager.BuyButtonSFX);
    }

    public void PurchaseHealthIncrease()
    {
        _playerScript.BatteryCount -= _healthIncreaseFinalPrice;
        _playerScript.PurchasedUpgrades.HealthIncrease = true;
        _playerScript.MaxHealth += 3;
        _playerScript.CurrentHealth = _playerScript.MaxHealth;
        _healthIncreasePurchaseButton.enabled = false;
        _healthIncreasePurchasedImage.SetActive(true);
        AddSpriteToPurchasedIcons(_healthIncreasedSprite);
        UpdateBuyableItems();
        _soundManager.PlaySFXAtPlayer(_soundManager.BuyButtonSFX);
    }

    public void PurchaseAttackSpeed()
    {
        _playerScript.BatteryCount -= _attackSpeedFinalPrice;
        _playerScript.PurchasedUpgrades.AttackSpeedModifier = 1.5f;
        _playerScript.UpgradeAttackSpeedAnimation();
        _attackSpeedPurchaseButton.enabled = false;
        _attackSpeedPurchasedImage.SetActive(true);
        AddSpriteToPurchasedIcons(_attackSpeedSprite);
        UpdateBuyableItems();
        _soundManager.PlaySFXAtPlayer(_soundManager.BuyButtonSFX);
    }

    public void PurchaseDashRecharge()
    {
        _playerScript.BatteryCount -= _dashRechargeFinalPrice;
        _playerScript.PurchasedUpgrades.DashRechargeModifier = 1.25f;
        _dashRechargePurchaseButton.enabled = false;
        _dashRechargePurchasedImage.SetActive(true);
        AddSpriteToPurchasedIcons(_dashRechargeSprite);
        UpdateBuyableItems();
        _soundManager.PlaySFXAtPlayer(_soundManager.BuyButtonSFX);
    }

    public void PurchaseLowHealthDamage()
    {
        _playerScript.BatteryCount -= _lowHealthDamageFinalPrice;
        _playerScript.PurchasedUpgrades.LowHealthDamageBonus = 2;
        _lowHealthDamagePurchaseButton.enabled = false;
        _lowHealthDamagePurchasedImage.SetActive(true);
        AddSpriteToPurchasedIcons(_lowHealthDamageSprite);
        UpdateBuyableItems();
        _soundManager.PlaySFXAtPlayer(_soundManager.BuyButtonSFX);
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
        WVDFunctionsCheck.InShopMenu = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    private void OnDisable()
    {
        WVDFunctionsCheck.InShopMenu = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
