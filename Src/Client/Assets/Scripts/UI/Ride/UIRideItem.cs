using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIRideItem : ListView.ListViewItem
{

	public Image icon;
	public Text title;
	public Text level;

	public Image background;
	public Sprite normalBg;
	public Sprite selectedBg;

	public override void onSelected(bool selected)
	{
		this.background.overrideSprite = selected ? selectedBg : normalBg;
	}

	// Use this for initialization
	void Start()
	{

	}

	public Item item;
	// Update is called once per frame
	void Update()
	{

	}

	public void setRideItem(Item item, UIRide ride)
	{

		this.item = item;
		if (this.title != null) this.title.text = this.item.Define.Name;
		if (this.level != null) this.level.text = this.item.Define.Level.ToString();
		if (this.icon != null) this.icon.overrideSprite = Resloader.Load<Sprite>(this.item.Define.Icon);
	}

}
