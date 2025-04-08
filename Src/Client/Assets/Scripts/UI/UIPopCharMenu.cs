using Managers;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPopCharMenu : UIWindow,IDeselectHandler {

	public int targetId;
	public string targetName;

	public void OnDeselect(BaseEventData eventData)
    {
        //只能通过调试来得知eventData的类型，BaseEventData是父类
        PointerEventData pointerEvent = eventData as PointerEventData;
        if (pointerEvent.hovered.Contains(this.gameObject))
        {
            return;
        }

        this.Close(WindowResult.None);
    }

	public void OnEnable()
    {
        this.GetComponent<Selectable>().Select();
        this.Root.transform.position = Input.mousePosition + new Vector3(40, 0, 0);
        this.Root.transform.SetAsLastSibling();
    }
		
	public void OnChat()
    {
        ChatManager.Instance.startPrivateChat(targetId, targetName);
        this.Close(WindowResult.No);
    }

    public void OnAddFriend()
    {
        FriendService.Instance.SendFriendAddRequest(targetId, targetName);
        this.Close(WindowResult.No);
    }

    public void OnInviteTeam()
    {
        TeamService.Instance.SendTeamInviteRequest(targetId, targetName);
        this.Close(WindowResult.No);
    }
}
