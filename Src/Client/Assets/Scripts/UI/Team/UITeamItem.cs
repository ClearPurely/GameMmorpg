﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using Models;
using Services;
using UnityEngine.UI;
using SkillBridge.Message;

public class UITeamItem : ListView.ListViewItem
{
    public Text nickname;
    public Image classIcon;
    public Image leaderIcon;
    public Image background;
    public override void onSelected(bool selected)
    {
        this.background.enabled = selected ? true : false;
    }

    public int idx;

    public NCharacterInfo Info;

    void Start()
    {
        this.background.enabled = false;
    }

    public void SetMemberInfo(int idx, NCharacterInfo item, bool isLeader)
    {
        this.idx = idx;
        this.Info = item;
        if (this.nickname != null) this.nickname.text = this.Info.Level.ToString().PadRight(4) + this.Info.Name;
        if (this.classIcon != null) this.classIcon.overrideSprite = SpriteManager.Instance.classIcons[(int)this.Info.Class - 1];
        if (this.leaderIcon != null) this.leaderIcon.gameObject.SetActive(isLeader);

    }

}
