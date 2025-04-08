using Common;
using Common.Data;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class MapService : Singleton<MapService>
    {
        public MapService()
        {

            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapEntitySyncRequest>(this.OnMapEntitySync);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapTeleportRequest>(this.OnMapTeleport) ;

        }

        public void Init()
        {
            MapManager.Instance.Init();
        }


        private void OnMapEntitySync(NetConnection<NetSession> sender, MapEntitySyncRequest request)
        {
            Character character = sender.Session.Character;
            //解决选择角色时的错误
            if (character != null)
            {
                Log.InfoFormat("OnMapEntitySync:characterID:{0}:{1} Entity.Id:{2} Evt:{3} Entity:{4}", character.Id, character.Info.Name, request.entitySync.Id, request.entitySync.Event, request.entitySync.Entity.String());
                MapManager.Instance[character.Info.mapId].UpdateEntity(request.entitySync);

            }
        }


        internal void SendEntityUpdata(NetConnection<NetSession> connection, NEntitySync entity)
        {
            //NetMessage message = new NetMessage();
            //message.Response = new NetMessageResponse();
            //message.Response.mapEntitySync = new MapEntitySyncResponse();
            //message.Response.mapEntitySync.entitySyncs.Add(entity);
            //byte[] data = PackageHandler.PackMessage(message);
            //connection.SendData(data, 0, data.Length);
            connection.Session.Response.mapEntitySync = new MapEntitySyncResponse();
            connection.Session.Response.mapEntitySync.entitySyncs.Add(entity);
            connection.SendResponse();
        }

        private void OnMapTeleport(NetConnection<NetSession> sender, MapTeleportRequest request)
        {
            //获取当前请求传送的的角色
            Character character = sender.Session.Character;
            Log.InfoFormat("OnMapTeleport: characterID:{0}:{1} Teleporterld: {2}", character.Id, character.Data, request.teleporterId);
            
            //校验
            if (!DataManager.Instance.Teleporters.ContainsKey(request.teleporterId))
            {
                Log.WarningFormat("Source TeleporterID [{0}] not existed", request.teleporterId);
                return;
            }
            TeleporterDefine source = DataManager.Instance.Teleporters[request.teleporterId]; 
            if (source.LinkTo == 0 || !DataManager.Instance.Teleporters.ContainsKey(source.LinkTo))
            {
                Log.WarningFormat("Source TeleporterID[{0}] LinkTo ID[(1}] not existed", request.teleporterId, source.LinkTo);
            }

            //获取这个传送点的目的地
            TeleporterDefine target = DataManager.Instance.Teleporters[source.LinkTo];

            //设置角色在新地图的位置和朝向
            MapManager.Instance[source.MapID].CharacterLeave(character);
            character.Position = target.Position;
            character.Direction = target.Direction;
            MapManager.Instance[target.MapID].CharacterEnter(sender, character);

        }
    }
}
