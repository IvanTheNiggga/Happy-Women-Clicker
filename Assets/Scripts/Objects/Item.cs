﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour, IPointerClickHandler
{
    private Clicker clicker;
    private GiveReward giveReward;
    private Inventory inventory;
    private StagesManager stagesManager;
    public Text text;
    public Image ico;

    private Text SaleDescription_Text;
    private Text SaleForCurrencyPrice_Text;
    private Text SaleForXpPrice_Text;

    public GameObject item;
    public GameObject SaleGrid;
    public GameObject InventoryGrid;

    public int stage;
    public int index;
    public string nameObject;
    public string itemName;
    public string investItemName;
    public string slotItemName;
    public string type;
    public double currencyPrice;
    public float xpPrice;
    public string useMethodName;
    public int count;

    private int taps;
    public bool clickable;

    bool Loaded;


    public void Start()
    {
        SaleDescription_Text = GameObject.Find("Sale Description").GetComponent<Text>();

        transform.localScale = new Vector3(1, 1, 1);
        taps = 0;
        clickable = true;
    }

    void CheckName()
    {
        if (name == itemName)
        {
            inventory.items.Add(this);
            gameObject.transform.SetParent(InventoryGrid.transform);
        }
        else if (name == investItemName)
        {
            inventory.investItems.Add(this);
            gameObject.transform.SetParent(SaleGrid.transform);
        }
    }

    public void AddGraphics()
    {
        if (!Loaded)
        {
            itemName = $"Item_S{stage}ID{index}";
            investItemName = $"InvestItem_S{stage}ID{index}";
            SaleGrid = GameObject.Find("SaleGrid");
            InventoryGrid = GameObject.Find("InventoryGrid");
            inventory = GameObject.Find("ClickerManager").GetComponent<Inventory>();
            clicker = GameObject.Find("ClickerManager").GetComponent<Clicker>();
            stagesManager = GameObject.Find("ClickerManager").GetComponent<StagesManager>();
            giveReward = GameObject.Find("ClickerManager").GetComponent<GiveReward>();

            ico.sprite = stagesManager.StagesDataBase[stagesManager.StageIndex].itemsDataBase[index].ico;
            nameObject = stagesManager.StagesDataBase[stagesManager.StageIndex].itemsDataBase[index].nameObject;
            type = stagesManager.StagesDataBase[stagesManager.StageIndex].itemsDataBase[index].type;
            currencyPrice = giveReward.KillReward * stagesManager.StagesDataBase[stagesManager.StageIndex].itemsDataBase[index].currencyPrice;
            xpPrice = stagesManager.StagesDataBase[stagesManager.StageIndex].itemsDataBase[index].xpPrice;
            useMethodName = stagesManager.StagesDataBase[stagesManager.StageIndex].itemsDataBase[index].useMethodName;

            CheckName();
            Loaded = true;
        }

        DestroyOnEmpty();
        if (name == itemName && count > 0)
        {
            string keyName = $"{itemName}Count";
            PlayerPrefs.SetInt(keyName, count);
        }
        text.text = FormatNumsHelper.FormatNumF0F1(count);
    }

    private void DestroyOnEmpty()
    {
        if (count <= 0 && Loaded)
        {
            if (name == investItemName)
            {
                inventory.investItems.Remove(this);
            }
            else
            {
                string keyName = $"{itemName}Count";
                PlayerPrefs.DeleteKey(keyName);
                inventory = GameObject.Find("ClickerManager").GetComponent<Inventory>();
                inventory.SelectedItem = null;
                inventory.items.Remove(this);
            }
            Destroy(gameObject);
            inventory.SortInventory();
        }
    }

    public void Use()
    {
        inventory.Invoke(useMethodName, 0f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickable == true)
        {
            Interface interfaceManager = GameObject.Find("INTERFACE").GetComponent<Interface>();

            if (interfaceManager.saleWindowOpened)
            {
                AddToInvestGrid();
                MultiSellAddgraphics();
            }
            else
            {
                inventory.SelectedItem = gameObject;

                interfaceManager.SwitchItemInfo(1);
                interfaceManager.SwitchMainInterface(0);
            }
        }
    }

    public void MultiSellAddgraphics()
    {
        SaleForCurrencyPrice_Text = GameObject.Find("SaleForCurrencyPrice").GetComponent<Text>();
        SaleForXpPrice_Text = GameObject.Find("SaleForXpPrice").GetComponent<Text>();

        int itemsCount = 0;
        float esPrice = 0;
        double pcPrice = 0;
        for (int i = 0; i < inventory.investItems.Count; i++)
        {
            Item item = SaleGrid.transform.GetChild(i).GetComponent<Item>();
            itemsCount += item.count;
            esPrice += item.count * item.xpPrice;
            pcPrice += item.count * item.currencyPrice;
        }
        if (itemsCount == 0)
        {
            SaleForCurrencyPrice_Text.text = "";
            SaleForXpPrice_Text.text = "";
            SaleDescription_Text.text = "Select items, you want to sell";
        }
        else
        {
            SaleForCurrencyPrice_Text.text = "+" + FormatNumsHelper.FormatNumF1(pcPrice);
            SaleForXpPrice_Text.text = "+" + FormatNumsHelper.FormatNumF1(esPrice);
            SaleDescription_Text.text = $"Sell {FormatNumsHelper.FormatNumF0(itemsCount)} items ?";
        }
    }


    public void AddToInvestGrid()
    {
        taps++;
        Invoke(nameof(TapEquals0), 0.2f);
        if (taps >= 2)
        {
            GameObject g = name == itemName ? GameObject.Find(itemName) : GameObject.Find(investItemName);

            inventory.MoveItem(this, count);

            g.GetComponent<Item>().AddGraphics();

            taps = 0;
        }
        else
        {
            inventory.MoveItem(this, 1);
        }
        AddGraphics();
        inventory.SortInventory();
    }

    public void TapEquals0()
    {
        taps = 0;
    }


    private void OnBecameInvisible()
    {
        clickable = false;
        GetComponent<Image>().raycastTarget = false;
    }
    private void OnBecameVisible()
    {
        clickable = true;
        GetComponent<Image>().raycastTarget = true;
    }

}

