using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillBridge.Message;

using Common;
using Common.Data;

using Network;
using GameServer.Managers;
using GameServer.Entities;
using GameServer.Services;

namespace GameServer.Models
{
    class Map
    {
        internal class MapCharacter
        {
            public NetConnection<NetSession> connection;
            public Character character;

            public MapCharacter(NetConnection<NetSession> conn, Character cha)
            {
                this.connection = conn;
                this.character = cha;
            }
        }

        public int ID
        {
            get { return this.Define.ID; }
        }
        internal MapDefine Define;
        /// <summary>
        /// 地图中的角色，以CharacterID为Key
        /// </summary>
        Dictionary<int, MapCharacter> MapCharacters = new Dictionary<int, MapCharacter>();


        /// <summary>
        /// 刷怪管理器
        /// </summary>
        SpawnManager SpawnManager = new SpawnManager();

        public MonsterManager MonsterManager = new MonsterManager();

        internal Map(MapDefine define)
        {
            this.Define = define;
            this.SpawnManager.Init(this);
            this.MonsterManager.Init(this);
        }

        internal void Update()
        {
            SpawnManager.Update();
        }

        /// <summary>
        /// 角色进入地图
        /// </summary>
        /// <param name="character"></param>
        internal void CharacterEnter(NetConnection<NetSession> conn, Character character)
        {
            Log.InfoFormat("CharacterEnter: Map:{0} characterId:{1}", this.Define.ID, character.Id);

            character.Info.mapId = this.ID;     //标记进入哪张地图
            this.MapCharacters[character.Id] = new MapCharacter(conn, character);

            //NetMessage message = new NetMessage();
            //message.Response = new NetMessageResponse();
            //message.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            //message.Response.mapCharacterEnter.mapId = this.Define.ID;
            //message.Response.mapCharacterEnter.Characters.Add(character.Info);
            conn.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            conn.Session.Response.mapCharacterEnter.mapId = this.Define.ID;
            foreach (var kv in this.MapCharacters)  //广播，把进入玩家的信息传给其他地图中的玩家
            {
                //message.Response.mapCharacterEnter.Characters.Add(kv.Value.character.Info);
                //this.SendCharacterEnterMap(kv.Value.connection, character.Info);
                conn.Session.Response.mapCharacterEnter.Characters.Add(kv.Value.character.Info);
                if (kv.Value.character != character)
                    this.AddCharacterEnterMap(kv.Value.connection, character.Info);
            }
            foreach(var kv in this.MonsterManager.Monsters)
            {
                conn.Session.Response.mapCharacterEnter.Characters.Add(kv.Value.Info);
            }
            //this.MapCharacters[character.Id] = new MapCharacter(conn, character);
            //byte[] data = PackageHandler.PackMessage(message);
            //conn.SendData(data, 0, data.Length);
            conn.SendResponse();
        }


        //角色离开地图
        internal void CharacterLeave(Character cha)
        {
            Log.InfoFormat("CharacterLeave: Map:{0} characterId:{1}", this.Define.ID, cha.Id);
            foreach(var kv in this.MapCharacters)
            {
                this.SendCharacterLeaveMap(kv.Value.connection, cha);
            }
            this.MapCharacters.Remove(cha.Id);
        }

        //角色进入游戏的消息的发送
        //void SendCharacterEnterMap(NetConnection<NetSession> conn, NCharacterInfo character)
        //{
        //    NetMessage message = new NetMessage();
        //    message.Response = new NetMessageResponse();
        //    message.Response.mapCharacterEnter = new MapCharacterEnterResponse();
        //    message.Response.mapCharacterEnter.mapId = this.Define.ID;
        //    message.Response.mapCharacterEnter.Characters.Add(character);
        //    byte[] data = PackageHandler.PackMessage(message);
        //    conn.SendData(data, 0, data.Length);
        //}
        void AddCharacterEnterMap(NetConnection<NetSession> conn, NCharacterInfo character)
        {
            if(conn.Session.Response.mapCharacterEnter == null)
            {
                conn.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse();
                conn.Session.Response.mapCharacterEnter.mapId = this.Define.ID;
            }
            conn.Session.Response.mapCharacterEnter.Characters.Add(character);
            conn.SendResponse();
        }


        private void SendCharacterLeaveMap(NetConnection<NetSession> conn, Character character)
        {
            //NetMessage message = new NetMessage();
            //message.Response = new NetMessageResponse();
            //message.Response.mapCharacterLeave = new MapCharacterLeaveResponse();
            //message.Response.mapCharacterLeave.characterId = character.Id;
            //byte[] data = PackageHandler.PackMessage(message);
            //conn.SendData(data, 0, data.Length);
            Log.InfoFormat("SendCharacterLeaveNap To{0};{1}:Map;{2}Character:{3}:{4}", conn.Session.Character.Id, conn.Session.Character.Info.Name, this.Define.ID, character.Id, character.Info.Name);
            conn.Session.Response.mapCharacterLeave = new MapCharacterLeaveResponse();
            conn.Session.Response.mapCharacterLeave.entityId = character.entityId;
            conn.SendResponse();
        }


        internal void UpdateEntity(NEntitySync entitySync)
        {
            foreach(var kv in this.MapCharacters)
            {
                if(kv.Value.character.entityId == entitySync.Id)
                {
                    kv.Value.character.Position = entitySync.Entity.Position;
                    kv.Value.character.Direction = entitySync.Entity.Direction;
                    kv.Value.character.Speed = entitySync.Entity.Speed;
                    if(entitySync.Event == EntityEvent.Ride)
                    {
                        kv.Value.character.Ride = entitySync.Param;     //把坐骑的id传入
                    }
                }
                else
                {
                    MapService.Instance.SendEntityUpdata(kv.Value.connection, entitySync);
                }
            }
        }


        /// <summary>
        /// 怪物进入地图
        /// </summary>
        internal void MonsterEnter(Monster monster)
        {
            Log.InfoFormat("MonsterEnter: Map:{0} monsterId:{1}", this.Define.ID, monster.Id);
            foreach (var kv in this.MapCharacters)
            {
                this.AddCharacterEnterMap(kv.Value.connection, monster.Info);
            }
        }
    }
}
