using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{

    class UIElement
    {
        public string ResourcesPath;
        public bool Cache;
        public GameObject Instance;
    }

    private Dictionary<Type, UIElement> UIResources = new Dictionary<Type, UIElement>();

    public UIManager()
    {
        //初始化UI资源
        this.UIResources.Add(typeof(UISetting), new UIElement() { ResourcesPath = "UI/UISetting", Cache = true });

        this.UIResources.Add(typeof(UITest), new UIElement() { ResourcesPath = "UI/UITest", Cache = true });
        this.UIResources.Add(typeof(UIBag), new UIElement() { ResourcesPath = "UI/UIBag", Cache = false });
        this.UIResources.Add(typeof(UIShop), new UIElement() { ResourcesPath = "UI/UIShop", Cache = false });
        this.UIResources.Add(typeof(UICharEquip), new UIElement() { ResourcesPath = "UI/UICharEquip", Cache = false });
        this.UIResources.Add(typeof(UIQuestSystem), new UIElement() { ResourcesPath = "UI/UIQuestSystem", Cache = false });
        this.UIResources.Add(typeof(UIQuestDialog), new UIElement() { ResourcesPath = "UI/UIQuestDialog", Cache = false });
        this.UIResources.Add(typeof(UIFriends), new UIElement() { ResourcesPath = "UI/UIFriends", Cache = false });

        this.UIResources.Add(typeof(UIGuild), new UIElement() { ResourcesPath = "UI/Guild/UIGuild", Cache = false });
        this.UIResources.Add(typeof(UIGuildList), new UIElement() { ResourcesPath = "UI/Guild/UIGuildList", Cache = false });
        this.UIResources.Add(typeof(UIGuidPopNoGuid), new UIElement() { ResourcesPath = "UI/Guild/UIGuidPopNoGuid", Cache = false });
        this.UIResources.Add(typeof(UIGuildPopCreate), new UIElement() { ResourcesPath = "UI/Guild/UIGuidCreate", Cache = false });
        this.UIResources.Add(typeof(UIGuildApplyList), new UIElement() { ResourcesPath = "UI/Guild/UIGuildApplyList", Cache = false });

        this.UIResources.Add(typeof(UIPopCharMenu), new UIElement() { ResourcesPath = "UI/UIPopCharMenu", Cache = false });
        this.UIResources.Add(typeof(UIRide), new UIElement() { ResourcesPath = "UI/UIRide", Cache = false });
        this.UIResources.Add(typeof(UISystemConfig), new UIElement() { ResourcesPath = "UI/UISystemConfig", Cache = false });
    }

    ~UIManager()
    {
    }

    /// <summary>
    /// Show UI
    /// </summary>
    public T Show<T>()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Win_Open);
        Type type = typeof(T);
        if(this.UIResources.ContainsKey(type))
        {
            UIElement info = this.UIResources[type];
            if(info.Instance != null)
            {
                info.Instance.SetActive(true);
            }
            else
            {
                UnityEngine.Object prefab = Resources.Load(info.ResourcesPath);
                if(prefab == null)
                {
                    return default(T);
                }
                info.Instance = (GameObject)GameObject.Instantiate(prefab);
            }
            return info.Instance.GetComponent<T>();
        }
        return default(T);
    }

    public void Close(Type type)
    {
        if (this.UIResources.ContainsKey(type))
        {
            UIElement info = this.UIResources[type];
            if(info.Cache)
            {
                info.Instance.SetActive(false);
            }
            else
            {
                GameObject.Destroy(info.Instance);
                info.Instance = null;
            }
        }
    }

    public void Close<T>()
    {
        this.Close(typeof(T));
    }
}

