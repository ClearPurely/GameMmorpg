using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Entities;
using SkillBridge.Message;

namespace Managers
{
    interface IEntityNotify
    {
        void OnEntityChanged(Entity entity);
        void OnEntityEvent(EntityEvent entityEvent,int param);
        void OnEntityRemove();
    }

    class EntityManager : Singleton<EntityManager>
    {
        Dictionary<int, Entity> entities = new Dictionary<int, Entity>();

        //发送通知给entityController
        Dictionary<int, IEntityNotify> notifiers = new Dictionary<int, IEntityNotify>();

        public void RegisterEntityChangeNotify(int entityId, IEntityNotify notify)
        {
            this.notifiers[entityId] = notify;
        }

        public void AddEntity(Entity entity)
        {
            entities[entity.entityId] = entity;
        }

        public void RemoveEntity(NEntity entity)
        {
            this.entities.Remove(entity.Id);
            if(notifiers.ContainsKey(entity.Id))
            {
                notifiers[entity.Id].OnEntityRemove();
                notifiers.Remove(entity.Id);
            }
        }

        //同步服务器传来的entity和客户端中的entity
        internal void OnEntitySync(NEntitySync data)
        {
            Entity entity = null;
            entities.TryGetValue(data.Id, out entity);
            if(entity != null)
            {
                if (data.Entity != null)
                    entity.EntityData = data.Entity;
                if(notifiers.ContainsKey(data.Id))
                {
                    notifiers[entity.entityId].OnEntityChanged(entity);
                    notifiers[entity.entityId].OnEntityEvent(data.Event, data.Param);
                }
            }
        }
    }
}
