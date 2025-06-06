﻿using Models;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildPopCreate : UIWindow {
	public InputField GuildName;
	public InputField GuildNotice;
	public Text gold;
	// Use this for initialization
	void Start () {
		GuildServcie.Instance.OnGuildCreateResult = OnGuildCreate;
		gold.text = "5000";
	}

	private void onDestroy()
	{
		GuildServcie.Instance.OnGuildCreateResult = null;
	}


    public override void OnYesClick()
    {
		int goldnum = 0;
        if (string.IsNullOrEmpty(GuildName.text))
        {
			MessageBox.Show("请输入公会名称","错误",MessageBoxType.Error);
			return;
        }

		if (GuildName.text.Length < 2 || GuildName.text.Length >10)
		{
			MessageBox.Show("公会名字请在2和10个字符之间", "错误", MessageBoxType.Error);
			return;
		}

		if (string.IsNullOrEmpty(GuildNotice.text))
		{
			MessageBox.Show("请输入公会宣言", "错误", MessageBoxType.Error);
			return;
		}

		if (GuildNotice.text.Length < 3 || GuildNotice.text.Length > 50)
		{
			MessageBox.Show("公会宣言请在2和50个字符之间", "错误", MessageBoxType.Error);
			return;
		}

		if (User.Instance.CurrentCharacter.Gold < 5000)
		{
			MessageBox.Show("创建公会需要5000金币", "错误", MessageBoxType.Error);
			return;
		}
		int.TryParse(gold.text, out goldnum);
		GuildServcie.Instance.sendGuildCreate(this.GuildName.text, this.GuildNotice.text, goldnum);

	}
    // Update is called once per frame
    void Update () {
		
	}

	void OnGuildCreate(bool result)
    {
        if (result)
        {
			this.Close(WindowResult.Yes);
        }
    }
}
