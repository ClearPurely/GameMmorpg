using System;
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
    class BagService : Singleton<BagService>
    {

        public BagService()
        {
            //消息分发器（MessageDistributer）进行订阅
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<BagSaveRequest>(this.OnBagSave);
        }


        public void Init()
        {

        }

        void OnBagSave(NetConnection<NetSession> sender, BagSaveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("BagSaveRequest:character:{0}:Unlocked{1} ", character.Id, request.BagInfo.Unlocked);

            if(request.BagInfo != null)
            {
                character.Data.Bag.Items = request.BagInfo.Items;
                DBService.Instance.Save();
            }

        }

    }
}
