using Models;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISetting : UIWindow {

	public void ClickOnExitToCharSelect() 
	{
		SceneManager.Instance.LoadScene("CharSelect");
		SoundManager.Instance.PlayMusic(SoundDefine.Music_Select);
		UserService.Instance.SendGameLeave();
	}

	public void ClickOnSystemConfig()
	{
		//UIPopCharMenu menu = UIManager.Instance.Show<UIPopCharMenu>();
		UIManager.Instance.Show<UISystemConfig>();
		this.Close();
	}

	public void ClickOnExitGame()
	{
		UserService.Instance.SendGameLeave(true);
	}
}
