﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;
using SkillBridge.Message;

namespace Managers
{
    public class TeamManager : Singleton<TeamManager>
    {

        public void Init()
        {
        }

        public void UpdateTeamInfo(NTeamInfo team)
        {
            User.Instance.TeamInfo = team;
            ShowTeamUI(team != null);
        }

        public void ShowTeamUI(bool show)
        {
            if (UIMain.Instance != null)
            {
                UIMain.Instance.ShowTeamUI(show);
            }
        }
    }
}
