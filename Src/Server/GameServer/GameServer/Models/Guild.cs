using Common;
using Common.Utils;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace GameServer.Models
{
    class Guild
    {
        public int Id { get { return this.Data.Id; } }

        public string Name { get{ return this.Data.Name; } }
        public double timestamp;
        public TGuild Data;

        public Guild(TGuild guild)
        {
            this.Data = guild;
        }


        public bool JoinApply(NGuildApplyInfo apply)
        {
            var oldApply = this.Data.Applies.FirstOrDefault(v => v.CharacterId == apply.characterId);
            if (oldApply != null)
            {
                return false;
            }

            var dbApply = DBService.Instance.Entities.GuildApplies.Create();
            dbApply.GuildId = apply.GuildId;
            dbApply.CharacterId = apply.characterId;
            dbApply.Name = apply.Name;
            dbApply.Class = apply.Class;
            dbApply.Level = apply.Level;
            dbApply.ApplyTime = DateTime.Now;

            DBService.Instance.Entities.GuildApplies.Add(dbApply);
            this.Data.Applies.Add(dbApply);
            DBService.Instance.Save();

            this.timestamp = TimeUtil.timestamp;
            return true;
        }

        public bool JoinApprove(NGuildApplyInfo apply)
        {
            var oldApply = this.Data.Applies.FirstOrDefault(v => v.CharacterId == apply.characterId && v.Result == 0);
            if (oldApply == null)
            {
                return false;
            }
            oldApply.Result = (int)apply.Result;
            if (apply.Result == ApplyResult.Accept)
            {
                this.AddMember(apply.characterId, apply.Name, apply.Class, apply.Level, GuildTitle.None);
            }
            DBService.Instance.Save();

            this.timestamp = TimeUtil.timestamp;
            return true;
        }

        public void AddMember(int characterId, string name, int @class, int level, GuildTitle title)
        {
            DateTime now = DateTime.Now;
            var memberItem = new TGuildMember()
            {
                CharacterId = characterId,
                Name = name,
                Class = @class,
                Level = level,
                Title = (int)title,
                JoinTime = now,
                LastTime = now
            };
            this.Data.Members.Add(memberItem);
            var character = CharacterManager.Instance.GetCharacter(characterId);

            if (character !=null)
            {
                character.Data.GuildId = this.Id;
            }
            else
            {
                //DBService.Instance.Entities.Database.ExecuteSqlCommand("UPDATE Characters SET GuildId = @p0 WHERE CharacterId = @p1", this.Id, characterId);
                TCharacter dbchar = DBService.Instance.Entities.Characters.FirstOrDefault(c=> c.ID == characterId);
                dbchar.GuildId = this.Id;
            }
            timestamp = TimeUtil.timestamp;
        }


        public void Leave(int characterId)
        {

            var member = GetDbMember(characterId);
            Log.InfoFormat(" Leave Guild : {0}:{1}", member.Id, member.Name);
            RemoveMember(member);
        }


        public void PostProcess(Character from, NetMessageResponse message)
        {
            if (message.Guild == null)
            {
                message.Guild = new GuildResponse();
                message.Guild.Result = Result.Success;
                message.Guild.guildInfo = this.GuildInfo(from);
            }
        }
        public NGuildInfo GuildInfo(Character from)
        {
            NGuildInfo info = new NGuildInfo()
            {
                Id = this.Id,
                GuildName = this.Name,
                Notice = this.Data.Notice,
                leaderId = this.Data.LeaderID,
                leaderName = this.Data.LeaderName,
                createTime = (long)TimeUtil.GetTimestamp(this.Data.CreateTime),
                memberCount = this.Data.Members.Count
            };

            if (from !=null)
            {
                info.Members.AddRange(GetMemberInfos());
                if (from.Id == this.Data.LeaderID)
                {
                    info.Applies.AddRange(GetApplyInfos());
                }
            }

            return info;
        }

        List<NGuildMemberInfo> GetMemberInfos()
        {
            List<NGuildMemberInfo> members = new List<NGuildMemberInfo>();
            foreach (var member in this.Data.Members)
            {
                var memberInfo = new NGuildMemberInfo()
                {
                    Id = member.Id,
                    characterId = member.CharacterId,
                    Title = (GuildTitle) member.Title,
                    joinTime = (long)TimeUtil.GetTimestamp(member.JoinTime),
                    lastTime = (long)TimeUtil.GetTimestamp(member.LastTime)
                };

                var character = CharacterManager.Instance.GetCharacter(member.CharacterId);
                if (character !=null)
                {
                    memberInfo.Info = character.GetBasicInfo();
                    memberInfo.Status = 1;
                    member.Level = character.Data.Level;
                    member.Name = character.Data.Name;
                    member.LastTime = DateTime.Now;
                }
                else
                {
                    memberInfo.Info = this.GetMemberInfos(member);
                    memberInfo.Status = 0;
                }
                members.Add(memberInfo);
            }
            return members;
        }

        public NCharacterInfo GetMemberInfos(TGuildMember member)
        {
            return new NCharacterInfo() {
                 Id = member.CharacterId,
                 Name = member.Name,
                 Class = (CharacterClass) member.Class,
                 Level = member.Level
            };
        }

        public  List<NGuildApplyInfo> GetApplyInfos()
        {
            List<NGuildApplyInfo> applies = new List<NGuildApplyInfo>();
            foreach (var apply in this.Data.Applies)
            {
                if (apply.Result != (int)ApplyResult.None)
                    continue;
                applies.Add(new NGuildApplyInfo()
                {
                    characterId = apply.CharacterId,
                    GuildId = apply.GuildId,
                    Name = apply.Name,
                    Level = apply.Level,
                    Class = apply.Class,
                    Result =(ApplyResult)apply.Result
                });
            }
            return applies;
        }


        TGuildMember GetDbMember(int characterId)
        {
            foreach (var member in this.Data.Members)
            {
                if (member.CharacterId == characterId)
                {
                    return member;
                }
            }
            return null;

        }

        TGuildApply GetDbApply(int characterId)
        {
            foreach (var apply in this.Data.Applies)
            {
                if (apply.CharacterId== characterId)
                {
                    return apply;
                }
            }
            return null;

        }

        public void ExcuteCommand(GuildAdminCommand command, int sourceId, int targetId)
        {
            var Source = GetDbMember(sourceId);
            var Target = GetDbMember(targetId);

            switch (command)
            {
                case GuildAdminCommand.Promote:
                    Target.Title = (int)GuildTitle.VicePresident;
                    break;
                case GuildAdminCommand.Depost:
                    Target.Title = (int)GuildTitle.None;
                    break;
                case GuildAdminCommand.Transfer:
                    Target.Title = (int)GuildTitle.President;
                    Source.Title = (int)GuildTitle.None;
                    break;
                case GuildAdminCommand.Kickout:
                    RemoveMember(Target);
                    break;
            }
            DBService.Instance.Save();
            timestamp = TimeUtil.timestamp;
        }

        public void RemoveMember(TGuildMember member)
        {
            var apply = GetDbApply(member.CharacterId);
            DBService.Instance.Entities.GuildMembers.Remove(member);
            DBService.Instance.Entities.GuildApplies.Remove(apply);

            var character = CharacterManager.Instance.GetCharacter(member.CharacterId);
            if (character !=null)
            {
                character.Data.GuildId = 0;
                character.Guild = null;
            }
            else
            {
                TCharacter dbchar = DBService.Instance.Entities.Characters.FirstOrDefault(c => c.ID==member.CharacterId);
                dbchar.GuildId = 0;
            }
        }
    }
}
