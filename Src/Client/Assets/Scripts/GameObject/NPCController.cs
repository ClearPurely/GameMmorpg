﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.Data;
using Managers;
using Models;

public class NPCController : MonoBehaviour {
	public int npcID;

	SkinnedMeshRenderer renderer;
	Animator anim;
	Color orignColor;

	private bool inInteractive = false;

	NpcDefine npc;

	NpcQuestStatus questStatus;

	// Use this for initialization
	void Start () {
		renderer = this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
		anim = this.gameObject.GetComponentInChildren<Animator>();
		orignColor = renderer.sharedMaterial.color;
		npc = NPCManager.Instance.GetNpcDefine(npcID);
		NPCManager.Instance.UpdateNpcPosition(this.npcID, this.transform.position);

		this.StartCoroutine(Actions());     //随机动作，待机时会休息
		RefreshNpcStatus();
		QuestManager.Instance.onQuestStatusChanged += OnQuestStatusChanged;
	}

	void OnQuestStatusChanged(Quest quest)
    {
		this.RefreshNpcStatus();
    }
	void RefreshNpcStatus()
    {
		questStatus = QuestManager.Instance.GetQuestStatusByNpc(this.npcID);
		UIWorldElementManager.Instance.AddNpcQueststatus(this.transform, questStatus);
	}

	void OnDestroy()
    {
		QuestManager.Instance.onQuestStatusChanged -= OnQuestStatusChanged;
		if (UIWorldElementManager.Instance != null)
			UIWorldElementManager.Instance.RemoveNpcQuestStatus(this.transform);

	}


	IEnumerator Actions()
    {
		while(true)
        {
			if (inInteractive)
				yield return new WaitForSeconds(2f);
			else
				yield return new WaitForSeconds(Random.Range(5f, 10f));
			this.Relax();
        }
    }

	
	// Update is called once per frame
	void Update () {
		
	}

	void Relax()
    {
		anim.SetTrigger("Relax");

    }

	void Interactive()
    {
		if(!inInteractive)
        {
			inInteractive = true;
			StartCoroutine(DoInteractive());	//启动一个协程
        }
    }

	IEnumerator DoInteractive()
    {
		yield return FaceToPlayer();	//在协程中调用一个函数
		if(NPCManager.Instance.Interactive(npc))
        {
			anim.SetTrigger("Talk");
        }
		yield return new WaitForSeconds(3f);	//3秒之内无法重复点击
		inInteractive = false;
    }
	IEnumerator FaceToPlayer()
    {
		Vector3 faceTo = (User.Instance.CurrentCharacterObject.transform.position - this.transform.position).normalized;
		while(Mathf.Abs(Vector3.Angle(this.gameObject.transform.forward, faceTo)) > 5)
        {
			this.gameObject.transform.forward = Vector3.Lerp(this.gameObject.transform.forward, faceTo, Time.deltaTime * 5f);
			yield return null;
		}
	}


	void OnMouseDown()
	{
		//点击npc，如果距离过远，会自动寻路过去
		if (Vector3.Distance(this.transform.position, User.Instance.CurrentCharacterObject.transform.position) > 2f)
		{
			User.Instance.CurrentCharacterObject.StartNav(this.transform.position);
		}
		Interactive();
    }

	private void OnMouseOver()
    {
		Highlight(true);
    }

	private void OnMouseEnter()
	{
		Highlight(true);
	}

	private void  OnMouseExit()
    {
		Highlight(false);
	}

	void Highlight(bool highlight)
    {
		if(highlight)
        {
			if (renderer.sharedMaterial.color != Color.white)
				renderer.sharedMaterial.color = Color.white;
        }
		else
        {
			if (renderer.sharedMaterial.color != orignColor)
				renderer.sharedMaterial.color = orignColor;
        }


    }


}
