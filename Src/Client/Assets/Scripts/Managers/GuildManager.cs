using Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using UnityEngine;

namespace Managers
{
    public class GuildManager : Singleton<GuildManager>
    {
        public NGuildInfo guildInfo;

        public NGuildMemberInfo mymemberInfo;
        public bool HasGuild
        { get { return this.guildInfo != null; } }

        public void Init(NGuildInfo guild)
        {
            this.guildInfo = guild;
            if (guild == null)
            {
                this.mymemberInfo = null;
                return;
            }
            foreach (var item in guild.Members)
            {
                if (item.characterId == User.Instance.CurrentCharacter.Id)
                {
                    this.mymemberInfo = item;
                    break;
                }
            }
        }

        public void ShowGuild()
        {
            if (this.HasGuild)
            {
                UIManager.Instance.Show<UIGuild>();
            }
            else
            {
                var win = UIManager.Instance.Show<UIGuidPopNoGuid>();
                win.OnClose += PopNoGuildOnClose;
            }
        }

        void PopNoGuildOnClose(UIWindow sender, UIWindow.WindowResult result)
        {
            if (result == UIWindow.WindowResult.Yes)
            {
                UIManager.Instance.Show<UIGuildPopCreate>();
            }
            else if (result == UIWindow.WindowResult.No)
            {
                Debug.LogError("点击");
                UIManager.Instance.Show<UIGuildList>();
            }
        }
    }
}
