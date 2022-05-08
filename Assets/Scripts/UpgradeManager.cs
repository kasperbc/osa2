using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    public enum WeaponUpgrade {Basic, Multishot, RapidFire, BigBullet, Pierce, Sniper};
    public enum StatUpgrade {Health, Damage, FireRate};
    public enum DiamondUpgrade {EnemySlow, Heal, Thorns};

    public GameObject upgradePanel;
    public bool upgradePanelOpen;

    private PlayerControl controller;

    private Image[] upgradeIcons = new Image[3];

    private WeaponUpgrade upgrade1;
    private StatUpgrade upgrade2;
    private DiamondUpgrade upgrade3;

    [SerializeField]
    Sprite[] weaponUpgradeIcons;
    [SerializeField]
    Sprite[] statUpgradeIcons;
    [SerializeField]
    Sprite[] diamondUpgradeIcons;
    void Start()
    {
        upgradePanel.SetActive(false);
        upgradePanelOpen = false;

        upgradeIcons[0] = upgradePanel.transform.GetChild(0).GetComponent<Image>();
        upgradeIcons[1] = upgradePanel.transform.GetChild(1).GetComponent<Image>();
        upgradeIcons[2] = upgradePanel.transform.GetChild(2).GetComponent<Image>();

        controller = GetComponent<PlayerControl>();

        RandomizeUpgradeShop();
    }

    public void SelectUpgrade(int upgrade)
    {
        if (!upgradePanelOpen)
        {
            return;
        }

        switch (upgrade)
        {
            case 1:
                UpgradeWeapon();
                break;
            case 2:
                UpgradeStat();
                break;
            case 3:
                UpgradeDiamond();
                break;
            case -1:
                CloseUpgradeMenu();
                return;
        }

        CloseUpgradeMenu();
    }

    void UpgradeWeapon()
    {
        WeaponUpgrade upgrade = upgrade1;

        PlayerShoot shootComponent = GetComponent<PlayerShoot>();

        shootComponent.damageMultiplier = 1;
        shootComponent.reloadMultiplier = 1;
        shootComponent.bulletSpeedMultiplier = 1;
        shootComponent.pierceCount = 0;
        shootComponent.bulletCount = 1;
        shootComponent.bulletSize = 1;

        switch (upgrade)
        {
            case WeaponUpgrade.Multishot:
                shootComponent.damageMultiplier = 0.75f;
                shootComponent.reloadMultiplier = 1.25f;
                shootComponent.bulletCount = 3;
                break;
            case WeaponUpgrade.BigBullet:
                shootComponent.bulletSize = 1.05f;
                break;
            case WeaponUpgrade.RapidFire:
                shootComponent.damageMultiplier = 0.3f;
                shootComponent.reloadMultiplier = 0.3f;
                break;
            case WeaponUpgrade.Pierce:
                shootComponent.pierceCount = 3;
                break;
            case WeaponUpgrade.Sniper:
                shootComponent.damageMultiplier = 2;
                shootComponent.reloadMultiplier = 1.5f;
                shootComponent.bulletSpeedMultiplier = 2;
                break;
        }
    }

    void UpgradeStat()
    {
        StatUpgrade upgrade = upgrade2;

        PlayerShoot shootComponent = GetComponent<PlayerShoot>();
        Health healthComponent = GetComponent<Health>();

        switch (upgrade)
        {
            case StatUpgrade.Damage:
                shootComponent.damage += 10;
                break;
            case StatUpgrade.Health:
                healthComponent.AddMaxHealth(20);
                healthComponent.Heal(20);
                break;
            case StatUpgrade.FireRate:
                shootComponent.reloadTime *= 0.8f;
                break;
        }
    }

    void UpgradeDiamond()
    {
        DiamondUpgrade upgrade = upgrade3;

        switch (upgrade)
        {
            case DiamondUpgrade.EnemySlow:
                WaveManager.instance.enemySpeedModifier = 0.5f;
                break;
            case DiamondUpgrade.Thorns:
                WaveManager.instance.thorns = true;
                break;
            case DiamondUpgrade.Heal:
                GameObject.Find("Diamond").GetComponent<Health>().FullHeal();
                break;
        }
    }

    void RandomizeUpgradeShop()
    {
        upgrade1 = (WeaponUpgrade)Random.Range(1, 6);
        upgrade2 = (StatUpgrade)Random.Range(0, 2);
        upgrade3 = (DiamondUpgrade)Random.Range(0, 2);
    }

    void UpdateButtonPrompts()
    {
        if (controller.controlMethod == PlayerControl.ControlMethod.PS4)
        {
            upgradeIcons[0].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("ButtonPrompts/PS4_Square");
            upgradeIcons[1].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("ButtonPrompts/PS4_Triangle");
            upgradeIcons[2].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("ButtonPrompts/PS4_Circle");
        }
        if (controller.controlMethod == PlayerControl.ControlMethod.MouseAndKeyboard)
        {
            upgradeIcons[0].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("ButtonPrompts/1_Key_Dark");
            upgradeIcons[1].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("ButtonPrompts/2_Key_Dark");
            upgradeIcons[2].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("ButtonPrompts/3_Key_Dark");
        }
    }

    public void OpenUpgradeMenu()
    {
        upgradePanel.SetActive(true);
        upgradePanelOpen = true;

        RandomizeUpgradeShop();

        if (GameManager.instance.GetPlayerCount() == 1)
        {
            upgradePanel.GetComponent<RectTransform>().localScale = new Vector3(1.5f, 1.5f, 1);
        }

        UpdateButtonPrompts();

        UpdateIcons();
    }

    void UpdateIcons()
    {
        upgradeIcons[0].sprite = weaponUpgradeIcons[(int)upgrade1 - 1];
        upgradeIcons[1].sprite = statUpgradeIcons[(int)upgrade2];
        upgradeIcons[2].sprite = diamondUpgradeIcons[(int)upgrade3];

        upgradeIcons[0].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = upgrade1.ToString();
        upgradeIcons[1].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = upgrade2.ToString();
        upgradeIcons[2].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = upgrade3.ToString();

        print(upgrade1);
    }

    void CloseUpgradeMenu()
    {
        upgradePanel.SetActive(false);
        upgradePanelOpen = false;
    }
}
