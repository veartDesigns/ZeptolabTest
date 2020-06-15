using System;
using System.Collections.Generic;

namespace Zeptolab
{
    [Serializable]
    public class StatsData
    {
       public StatsData()
        {
            UsersStats = new List<UserData>();
        }

        public List<UserData> UsersStats;

        public void AddUserStat(UserData userData)
        {
           // bool addData = CheckNewData(userData);
            //if (!addData) return;

            UsersStats.Add(userData);
            SortUserStats();
        }

        private bool CheckNewData(UserData newUserData)
        {
            return true;
        }

        public void SortUserStats()
        {
            UsersStats.Sort((pair1, pair2) => pair2.Coins.CompareTo(pair1.Coins));
        }
    }
}