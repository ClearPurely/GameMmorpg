﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Data;
using UnityEngine;

namespace Managers
{
    class NPCManager : Singleton<NPCManager>
    {

        public delegate bool NpcActionHandler(NpcDefine npc);

        Dictionary<NpcFunction, NpcActionHandler> eventMap = new Dictionary<NpcFunction, NpcActionHandler>();
        Dictionary<int, Vector3> npcPositions = new Dictionary<int, Vector3>();

        public void RegisterNpcEvent(NpcFunction function, NpcActionHandler action)
        {
            if (!eventMap.ContainsKey(function))
            {
                eventMap[function] = action;
            }
            else
                eventMap[function] += action;

        }


        public NpcDefine GetNpcDefine(int npcId)
        {
            NpcDefine npc = null;
            DataManager.Instance.Npcs.TryGetValue(npcId, out npc);
            return npc;
        }

        public bool Interactive(int npcId)
        {
            if(DataManager.Instance.Npcs.ContainsKey(npcId))
            {
                var npc = DataManager.Instance.Npcs[npcId];
                return Interactive(npc);
            }
            return false;
        }

        public bool Interactive(NpcDefine npc)
        {
            if(DoTaskInteractive(npc))
            {
                return true;
            }
            else if(npc.Type == NpcType.Functional)
            {

                return DoFunctionInteractive(npc);
            }
            return false;
        }


        private bool DoTaskInteractive(NpcDefine npc)
        {
            //MessageBox.Show("点击了NPC：" + npc.Name, "NPC对话");
            var status = QuestManager.Instance.GetQuestStatusByNpc(npc.ID);
            if (status == NpcQuestStatus.None)
                return false;
            return QuestManager.Instance.OpenNpcQuest(npc.ID);
        }

        private bool DoFunctionInteractive(NpcDefine npc)
        {
            if (npc.Type != NpcType.Functional)
                return false;
            if (!eventMap.ContainsKey(npc.Function))
                return false;

            return eventMap[npc.Function](npc);
        }

        internal void UpdateNpcPosition(int npc,Vector3 pos)
        {
            this.npcPositions[npc] = pos;
        }
        internal Vector3 GetNpcPosition(int npc)
        {
            return this.npcPositions[npc];
        }

    }
}
