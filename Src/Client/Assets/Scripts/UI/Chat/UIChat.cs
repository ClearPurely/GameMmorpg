using Candlelight.UI;
using Managers;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UIChat : MonoBehaviour
{

	public HyperText textArea;

	public TabView cannelTab;
	public InputField chatText;
	public Text charTarget;

	public Dropdown ChannelSelect;
	// Use this for initialization
	void Start()
	{
		this.cannelTab.OnTabSelect += OnDisplayChannelSelected;
		ChatManager.Instance.OnChat += RefreshUI;
	}

	private void OnDestroy()
	{
		ChatManager.Instance.OnChat -= RefreshUI;
	}

	// Update is called once per frame
	void Update()
	{
		//在PlayerInputController中FixedUpdate中更新判断是否在输入状态中
		InputManager.Instance.isInputMode = chatText.isFocused;
	}

	void OnDisplayChannelSelected(int idx)
	{
		ChatManager.Instance.displayChannel = (ChatManager.LocalChannel)idx;
		RefreshUI();
	}

	private void RefreshUI()
	{
		this.textArea.text = ChatManager.Instance.GetCurrentMessage();
		this.ChannelSelect.value = (int)ChatManager.Instance.sendChannel - 1;
		if (ChatManager.Instance.SendChannel == ChatChannel.Private)
		{
			this.charTarget.gameObject.SetActive(true);
			if (ChatManager.Instance.PrivateID != 0)
			{
				this.charTarget.text = ChatManager.Instance.PrivateName + ": ";
			}
			else
				this.charTarget.text = "无玩家:";
		}
		else
		{
			this.charTarget.gameObject.SetActive(false);
		}
	}



	public void OnClickChatLink(HyperText text, HyperText.LinkInfo linkInfo)
	{
		if (string.IsNullOrEmpty(linkInfo.Name))
		{
			return;
		}
		//<a  name = "c:1001:Name" class="player">Name</a>  人物c
		//<a  name = "i:1002:Name" class="player">Name</a> 道具i

		if (linkInfo.Name.StartsWith("c:"))
		{
			string[] strs = linkInfo.Name.Split(":".ToCharArray());
			UIPopCharMenu menu = UIManager.Instance.Show<UIPopCharMenu>();
			menu.targetId = int.Parse(strs[1]);
			menu.targetName = strs[2];
		}
	}

	public void OnClickSend()
	{
		this.OnEndInput();
	}

	public void OnEndInput()
	{
		if (!string.IsNullOrEmpty(this.chatText.text.Trim()))
		{
			this.SendChat(this.chatText.text);
		}
		this.chatText.text = "";
	}


	private void SendChat(string content)
	{
		ChatManager.Instance.SendChat(content, ChatManager.Instance.PrivateID, ChatManager.Instance.PrivateName);
	}


	public void OnSendChannelChanged()
	{
		if (ChatManager.Instance.sendChannel == (ChatManager.LocalChannel)(this.ChannelSelect.value + 1))
			return;

		if (!ChatManager.Instance.setSendChannel((ChatManager.LocalChannel)this.ChannelSelect.value + 1))
		{
			this.ChannelSelect.value = (int)ChatManager.Instance.sendChannel - 1;
		}
		else
		{
			this.RefreshUI();
		}
	}

}
