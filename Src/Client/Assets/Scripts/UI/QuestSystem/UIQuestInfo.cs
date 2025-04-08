using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using Managers;

public class UIQuestInfo : MonoBehaviour {
	public Text title;
	public Text[] targets;
	public Text description;
	public Text overview;
	public UIIconItem rewardItems;
	public Text rewardMoney;
	public Text rewardExp;

	public Button navButton;
	private int npc = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	public void SetQuestInfo(Quest quest)
    {
		this.title.text = string.Format("[{0}]{1}", quest.Define.Type, quest.Define.Name);
		if (this.overview != null) this.overview.text = quest.Define.Overview;

		if (this.description != null)
		{
			if (quest.Info == null)
			{
				this.description.text = quest.Define.Dialog;
			}
			else
			{
				if (quest.Info.Status == SkillBridge.Message.QuestStatus.Complated)
				{
					this.description.text = quest.Define.DialogFinish;
				}
			}
		}

		this.rewardMoney.text = quest.Define.RewardGold.ToString();
		this.rewardExp.text = quest.Define.RewardExp.ToString();

		if(quest.Info == null)
        {
			this.npc = quest.Define.AcceptNPC;
        }
		else if(quest.Info.Status == SkillBridge.Message.QuestStatus.Complated)
        {
			this.npc = quest.Define.SubmitNPC;
        }
		this.navButton.gameObject.SetActive(this.npc > 0);

		//强制布局一次，刷新布局组件
		foreach (var fitter in this.GetComponentsInChildren<ContentSizeFitter>())
		{
			fitter.SetLayoutVertical();
        }
	}

	void Update()
    {
    }

	public void OnClickAbandon()
    {
    }

	public void OnClickNav()
    {
		Vector3 pos = NPCManager.Instance.GetNpcPosition(this.npc);
		User.Instance.CurrentCharacterObject.StartNav(pos);
		UIManager.Instance.Close<UIQuestSystem>();
    }
}
