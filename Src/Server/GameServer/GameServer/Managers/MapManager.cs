using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Models;

namespace GameServer.Managers
{
    class MapManager : Singleton<MapManager>
    {
        Dictionary<int, Map> Maps = new Dictionary<int, Map>();

        
        public void Init()
        {
            foreach (var mapdefine in DataManager.Instance.Maps.Values)
            {
                Map map = new Map(mapdefine);   //根据配置创建地图
                Log.InfoFormat("MapManager.Init > Map:{0}:{1}", map.Define.ID, map.Define.Name);
                this.Maps[mapdefine.ID] = map;
            }
        }



        public Map this[int key]
        {
            get
            {
                return this.Maps[key];
            }
        }


        public void Update()            //当游戏运行时，地图还会一直在后台运行
        {
            foreach(var map in this.Maps.Values)
            {
                map.Update();
            }
        }
        
    }
}
