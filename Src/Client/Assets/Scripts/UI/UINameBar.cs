using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UINameBar : MonoBehaviour 
{
    //public Image avatar;
    public Text characterName;

    public Character character;

    // Use this for initialization
    void Start () 
    {
        /*
		if(this.character!=null)
        {
            if (character.Info.Type == SkillBridge.Message.CharacterType.Monster)
                this.avatar.gameObject.SetActive(false);
            else
                this.avatar.gameObject.SetActive(true);
        }
        */
    }
	
	// Update is called once per frame
	void Update () {
        this.UpdateInfo();
	}

    void UpdateInfo()
    {
        if (this.character != null)
        {
            string name = this.character.Name + " Lv." + this.character.Info.Level;
            if(name != this.characterName.text)
            {
                this.characterName.text = name;
            }
        }
    }
}
