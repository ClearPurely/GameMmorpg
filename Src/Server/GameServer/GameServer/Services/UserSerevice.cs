﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Network;
using SkillBridge.Message;
using GameServer.Entities;
using GameServer.Managers;

namespace GameServer.Services
{
    class UserService : Singleton<UserService>
    {

        public UserService()
        {
            //消息分发器（MessageDistributer）进行订阅
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserLoginRequest>(this.OnLogin);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserRegisterRequest>(this.OnRegister);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserCreateCharacterRequest>(this.OnCreateCharacter);

            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameEnterRequest>(this.OnGameEnter);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameLeaveRequest>(this.OnGameLeave);
        }


        public void Init()
        {

        }

        //角色登录
        void OnLogin(NetConnection<NetSession> sender, UserLoginRequest request)
        {
            Log.InfoFormat("UserLoginRequest: User:{0}  Pass:{1}", request.User, request.Passward);

            //NetMessage message = new NetMessage();
            //message.Response = new NetMessageResponse();
            //message.Response.userLogin = new UserLoginResponse();
            sender.Session.Response.userLogin = new UserLoginResponse();


            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user == null)
            {
                //message.Response.userLogin.Result = Result.Failed;
                //message.Response.userLogin.Errormsg = "用户不存在";
                sender.Session.Response.userLogin.Result = Result.Failed;
                sender.Session.Response.userLogin.Errormsg = "用户不存在";
            }
            else if (user.Password != request.Passward)
            {
                //message.Response.userLogin.Result = Result.Failed;
                //message.Response.userLogin.Errormsg = "密码错误";
                sender.Session.Response.userLogin.Result = Result.Failed;
                sender.Session.Response.userLogin.Errormsg = "密码错误";

            }
            else
            {
                sender.Session.User = user;     //将登录的用户信息保存在内存中

                //message.Response.userLogin.Result = Result.Success;
                //message.Response.userLogin.Errormsg = "None";
                //message.Response.userLogin.Userinfo = new NUserInfo();
                //message.Response.userLogin.Userinfo.Id = (int)user.ID;
                //message.Response.userLogin.Userinfo.Player = new NPlayerInfo();
                //message.Response.userLogin.Userinfo.Player.Id = user.Player.ID;
                sender.Session.Response.userLogin.Result = Result.Success;
                sender.Session.Response.userLogin.Errormsg = "None";
                sender.Session.Response.userLogin.Userinfo = new NUserInfo();
                sender.Session.Response.userLogin.Userinfo.Id = (int)user.ID;
                sender.Session.Response.userLogin.Userinfo.Player = new NPlayerInfo();
                sender.Session.Response.userLogin.Userinfo.Player.Id = user.Player.ID;
                foreach (var c in user.Player.Characters)
                {
                    NCharacterInfo info = new NCharacterInfo();
                    info.Id = c.ID;        //使用entity的id，这里还没有创建
                    info.Name = c.Name;
                    info.Type = CharacterType.Player;
                    info.Class = (CharacterClass)c.Class;
                    info.ConfigId = c.ID;        //保存DbId
                    //message.Response.userLogin.Userinfo.Player.Characters.Add(info);
                    sender.Session.Response.userLogin.Userinfo.Player.Characters.Add(info);
                }
            }
            //byte[] data = PackageHandler.PackMessage(message);
            //sender.SendData(data, 0, data.Length);
            sender.SendResponse();
        }

        void OnRegister(NetConnection<NetSession> conn, UserRegisterRequest request)
        {
            Log.InfoFormat("UserRegisterRequest: User:{0}  Pass:{1}", request.User, request.Passward);

            //NetMessage message = new NetMessage();
            //message.Response = new NetMessageResponse();
            //message.Response.userRegister = new UserRegisterResponse();
            conn.Session.Response.userRegister = new UserRegisterResponse();

            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user != null)
            {
                //message.Response.userRegister.Result = Result.Failed;
                //message.Response.userRegister.Errormsg = "用户已存在.";
                conn.Session.Response.userRegister.Result = Result.Failed;
                conn.Session.Response.userRegister.Errormsg = "用户已存在.";

            }
            else
            {
                TPlayer player = DBService.Instance.Entities.Players.Add(new TPlayer());
                DBService.Instance.Entities.Users.Add(new TUser() { Username = request.User, Password = request.Passward, Player = player });
                DBService.Instance.Entities.SaveChanges();
                //message.Response.userRegister.Result = Result.Success;
                //message.Response.userRegister.Errormsg = "None";
                conn.Session.Response.userRegister.Result = Result.Success;
                conn.Session.Response.userRegister.Errormsg = "None";
            }
            //byte[] data = PackageHandler.PackMessage(message);
            //conn.SendData(data, 0, data.Length);
            conn.SendResponse();
        }

        //角色创建
        private void OnCreateCharacter(NetConnection<NetSession> sender, UserCreateCharacterRequest request)
        {
            Log.InfoFormat("UserCreateCharacterRequest: Name:{0}  Class:{1}", request.Name, request.Class);

            TCharacter character = new TCharacter()
            {
                Name = request.Name,
                Class = (int)request.Class,
                TID = (int)request.Class,
                Level = 1,
                MapID = 1,
                MapPosX = 5000,
                MapPosY = 4000,
                MapPosZ = 820,
                Gold = 100000,
                Equips = new byte[28]
            };
            //初始化增加背包
            var bag = new TCharacterBag();
            bag.Owner = character;
            bag.Items = new byte[0];
            bag.Unlocked = 20;
            character.Bag = DBService.Instance.Entities.CharacterBags.Add(bag);

            //创建角色
            character = DBService.Instance.Entities.Characters.Add(character);
            character.Items.Add(new TCharacterItem()
            {
                Owner = character,
                ItemID = 1,
                ItemCount = 20,
            });
            character.Items.Add(new TCharacterItem()
            {
                Owner = character,
                ItemID = 2,
                ItemCount = 20
            });
            sender.Session.User.Player.Characters.Add(character);      //将角色信息保存到内存中
            DBService.Instance.Entities.SaveChanges();

            //NetMessage message = new NetMessage();
            //message.Response = new NetMessageResponse();
            //message.Response.createChar = new UserCreateCharacterResponse();
            //message.Response.createChar.Result = Result.Success;
            //message.Response.createChar.Errormsg = "None";      //填写错误提示
            sender.Session.Response.createChar = new UserCreateCharacterResponse();
            sender.Session.Response.createChar.Result = Result.Success;
            sender.Session.Response.createChar.Errormsg = "None";
            foreach (var c in sender.Session.User.Player.Characters)
            {
                NCharacterInfo info = new NCharacterInfo();
                info.Id = c.ID;        //使用entity的id，这里还没有创建
                info.Name = c.Name;
                info.Type = CharacterType.Player;
                info.Class = (CharacterClass)c.Class;
                info.ConfigId = c.TID;        //保存DbId
                //message.Response.createChar.Characters.Add(info);
                sender.Session.Response.createChar.Characters.Add(info);
            }
            //byte[] data = PackageHandler.PackMessage(message);
            //sender.SendData(data, 0, data.Length);
            sender.SendResponse();
        }

        
        void OnGameEnter(NetConnection<NetSession> sender, UserGameEnterRequest request)
        {
            TCharacter dbchar = sender.Session.User.Player.Characters.ElementAt(request.characterIdx);
            Log.InfoFormat("UserGameEnterRequest: characterID:{0}:{1} Map:{2}", dbchar.ID, dbchar.Name, dbchar.MapID);
            Character character = CharacterManager.Instance.AddCharacter(dbchar);

            //NetMessage message = new NetMessage();
            //message.Response = new NetMessageResponse();
            //message.Response.gameEnter = new UserGameEnterResponse();
            //message.Response.gameEnter.Result = Result.Success;
            //message.Response.gameEnter.Errormsg = "None";
            //message.Response.gameEnter.Character = character.Info;      //进入成功，发送初始角色信息
            //byte[] data = PackageHandler.PackMessage(message);
            //sender.SendData(data, 0, data.Length);
            SessionManager.Instance.AddSession(character.Id, sender);
            sender.Session.Response.gameEnter = new UserGameEnterResponse();
            sender.Session.Response.gameEnter.Result = Result.Success;
            sender.Session.Response.gameEnter.Errormsg = "None";

            //道具系统测试
            //int itemId = 2;
            //bool hasItem = character.ItemManager.HasItem(itemId);
            //Log.InfoFormat("HasItem: [{0}]{1}", itemId, hasItem);
            //if (hasItem)
            //{
            //    //character.ItemManager.RemoveItem(itemId, 1);

            //}
            //else
            //{
            //    character.ItemManager.AddItem(1, 200);
            //    character.ItemManager.AddItem(2, 100);
            //    character.ItemManager.AddItem(3, 30);
            //    character.ItemManager.AddItem(4, 120);
            //}
            //Models.Item item = character.ItemManager.GetItem(itemId);
            //Log.InfoFormat("Item:[{0}][{1}]", itemId, item);
            //DBService.Instance.Save();

            sender.Session.Character = character;
            sender.Session.PostResponser = character;

            sender.Session.Response.gameEnter.Character = character.Info;        // 进入成功，发送初始角色信息
            sender.SendResponse();

            MapManager.Instance[dbchar.MapID].CharacterEnter(sender, character);

        }

        void OnGameLeave(NetConnection<NetSession> sender, UserGameLeaveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("UserGameLeaveRequest: characterID:{0}:{1} Map:{2}", character.Id, character.Info.Name, character.Info.mapId);

            this.CharacterLeave(character);
            //NetMessage message = new NetMessage();
            //message.Response = new NetMessageResponse();
            //message.Response.gameLeave = new UserGameLeaveResponse();
            //message.Response.gameLeave.Result = Result.Success;
            //message.Response.gameLeave.Errormsg = "None";
            //byte[] data = PackageHandler.PackMessage(message);
            //sender.SendData(data, 0, data.Length);
            sender.Session.Response.gameLeave = new UserGameLeaveResponse();
            sender.Session.Response.gameLeave.Result = Result.Success;
            sender.Session.Response.gameLeave.Errormsg = "None";
            sender.SendResponse();

        }

        public void CharacterLeave(Character character)
        {
            Log.InfoFormat("CharacterLeave: characterID:{0}:{1}", character.Id, character.Info.Name);
            SessionManager.Instance.RemoveSession(character.Id);
            CharacterManager.Instance.RemoveCharacter(character.Id);
            character.Clear();
            MapManager.Instance[character.Info.mapId].CharacterLeave(character);
        }
    }
}
