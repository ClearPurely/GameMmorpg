﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Common.Data;


public class UIShopItem : MonoBehaviour, ISelectHandler
{
	public Image icon;
	public Text title;
	public Text price;
	public Text limitCLass;
	public Text count;

	public Image background;
	public Sprite normalBg;
	public Sprite selectedBg;
	private bool selected;
	public bool Selected
	{
		get { return selected; }
		set
		{
			selected = value;
			this.background.overrideSprite = selected ? selectedBg : normalBg;
		}
	}

	public int ShopItemID { get; set; }
	private UIShop shop;
	private ItemDefine item;
	private ShopItemDefine ShopItem { get; set; }

	void Start()
	{

	}

	public void SetShopItem(int id, ShopItemDefine shopItem, UIShop owner)
	{
		this.shop = owner;
		this.ShopItemID = id;
		this.ShopItem = shopItem;
		this.item = DataManager.Instance.Items[this.ShopItem.ItemID];

		this.title.text = this.item.Name;
		this.count.text = "x" + ShopItem.Count.ToString();
		this.price.text = ShopItem.Price.ToString();
		this.limitCLass.text = this.item.LimitClass.ToString();
		this.icon.overrideSprite = Resloader.Load<Sprite>(item.Icon);
	}

	public void OnSelect(BaseEventData eventData)
	{
		this.Selected = true;
		this.shop.SelectShopItem(this);
	}
}



