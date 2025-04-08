using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;
using SkillBridge.Message;
using Services;
using System;

namespace Managers
{
    public class FriendManager : Singleton<FriendManager>
    {
       public List<NFriendInfo> allFriends;

        public void Init(List<NFriendInfo> friends)
        {
            this.allFriends = friends;
        }

    }
}
