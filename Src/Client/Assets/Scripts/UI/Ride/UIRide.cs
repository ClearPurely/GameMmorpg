using Managers;
using Models;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRide : UIWindow
{
	public Text descript;
	public GameObject itemPrefab;
	public ListView listMain;

	private UIRideItem selectedItem;

	// Use this for initialization
	void Start()
	{
		RefreshUI();
		this.listMain.onItemSelected += this.OnItemSelected;
	}

    private void OnItemSelected(ListView.ListViewItem item)
    {
		this.selectedItem = item as UIRideItem;
		this.descript.text = this.selectedItem.item.Define.Description;
	}

    private void OnDestory()
	{
		
	}
	void RefreshUI()
	{
		ClearList();
		InitItems();
	}

    //初始化坐骑信息
    void InitItems()
	{
		foreach (var kv in ItemManager.Instance.Items)
		{
			if (kv.Value.Define.Type == ItemType.Ride &&
				(kv.Value.Define.LimitClass == CharacterClass.None || kv.Value.Define.LimitClass == User.Instance.CurrentCharacter.Class))
			{
				//已经装备的先不显示
				if (EquipManager.Instance.Contains(kv.Key))
					continue;
				GameObject go = Instantiate(itemPrefab, this.listMain.transform);
				UIRideItem ui = go.GetComponent<UIRideItem>();
				ui.setRideItem(kv.Value, this);
				this.listMain.AddItem(ui);
			}
		}
	}

	private void ClearList()
	{
		this.listMain.RemoveAll();
	}

	public void DoRide()
    {
        if (this.selectedItem == null)
        {
			MessageBox.Show("请选择坐骑","提示");
			return;
        }
		User.Instance.Ride(this.selectedItem.item.Id);
    }
}
