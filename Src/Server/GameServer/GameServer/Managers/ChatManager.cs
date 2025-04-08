using Common;
using Common.Utils;
using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
     class ChatManager:Singleton<ChatManager>
    {
        public List<ChatMessage> System = new List<ChatMessage>();//系统
        public List<ChatMessage> World = new List<ChatMessage>();//世界
        public Dictionary<int, List<ChatMessage>> Local = new Dictionary<int, List<ChatMessage>>();//本地消息
        public Dictionary<int, List<ChatMessage>> Team = new Dictionary<int, List<ChatMessage>>();//队伍消息
        public Dictionary<int, List<ChatMessage>> Guild = new Dictionary<int, List<ChatMessage>>();//公会消息

        public double TimeChatTs;

        public void Init()
        {
            TimeChatTs = TimeUtil.timestamp;
        }

        public void ClearMessage()
        {
            Local.Clear();
            System.Clear();
            World.Clear();
            Team.Clear();
            Guild.Clear();
            TimeChatTs = TimeUtil.timestamp;
        }

        public void AddMessage(Character from, ChatMessage message)
        {
            message.FromId = from.Id;
            message.FromName = from.Name;
            message.Time = TimeUtil.timestamp;

            switch (message.Channel)
            {
                case ChatChannel.Local:
                    this.AddLocalMessage(from.Info.mapId, message);
                    break;
                case ChatChannel.World:
                    this.AddWorldMessage(message);
                    break;
                case ChatChannel.System:
                    this.AddSystemMessage(message);
                    break;
                case ChatChannel.Team:
                    this.AddTeamMessage(from.Team.Id, message);
                    break;
                case ChatChannel.Guild:
                    this.AddGuildMessage(from.Guild.Id, message);
                    break;
            }
        }

      
        public void AddLocalMessage(int mapId, ChatMessage message)
        {
            if (!Local.TryGetValue(mapId, out List<ChatMessage> messages))
            {
                messages = new List<ChatMessage>();
                this.Local[mapId] = messages;
            }
            messages.Add(message);
        }

        public void AddSystemMessage(ChatMessage message)
        {
            this.System.Add(message);
        }

        public void AddWorldMessage(ChatMessage message)
        {
            this.World.Add(message);
        }


        public void AddGuildMessage(int guildId, ChatMessage message)
        {
            if (!Guild.TryGetValue(guildId, out List<ChatMessage> messages))
            {
                messages = new List<ChatMessage>();
                this.Guild[guildId] = messages;
            }
            messages.Add(message);
        }

        public void AddTeamMessage(int teamId, ChatMessage message)
        {
            if (!Team.TryGetValue(teamId, out List<ChatMessage> messages))
            {
                messages = new List<ChatMessage>();
                this.Team[teamId] = messages;
            }
            messages.Add(message);
        }

        internal int GetLocalMessages(int mapId, int localIdx, List<ChatMessage> result)
        {
            if (!Local.TryGetValue(mapId, out List<ChatMessage> messages))
            {
                return 0;
            }
            return GetNewMessage(localIdx, result, messages);
        }

        internal int GetWorldMessages(int worldIdx, List<ChatMessage> result)
        {
            return GetNewMessage(worldIdx, result, this.World);
        }

        internal int GetSystemMessages(int systemIdx, List<ChatMessage> result)
        {
            return GetNewMessage(systemIdx, result, this.System);
        }

        internal int GetTeamMessages(int teamId ,int teamIdx, List<ChatMessage> result)
        {
            if (!Team.TryGetValue(teamId, out List<ChatMessage> messages))
            {
                return 0;
            }
            return GetNewMessage(teamIdx, result, messages);
        }

        internal int GetGuildMessages(int guildId, int guildIdx, List<ChatMessage> result)
        {
            if (!Guild.TryGetValue(guildId, out List<ChatMessage> messages))
            {
                return 0;
            }
            return GetNewMessage(guildIdx, result, messages);
        }

        public int GetNewMessage(int idx, List<ChatMessage> result, List<ChatMessage> messages)
        {
            if (idx != 0 && messages.Count == 0)
            {
                return 0;
            }

            if (idx == 0)
            {
                if (messages.Count > GameDefine.MaxChatRecordNums)
                {
                    idx = messages.Count - GameDefine.MaxChatRecordNums;
                }
            }

            for (; idx < messages.Count; idx++)
            {
                result.Add(messages[idx]);
            }
                        
            return idx;
        }
    }
}
