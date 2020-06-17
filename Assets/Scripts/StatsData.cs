using System;
using System.Collections.Generic;

namespace Zeptolab
{
    [Serializable]
    public class StatsData : IStatsData
    {
        public List<UserData> UsersStats;

        public StatsData()
        {
            UsersStats = new List<UserData>();
        }

        public void AddUserStat(UserData userData)
        {
            //bool addData = CheckNewData(userData);
            //if (!addData) return;

            UsersStats.Add(userData);
            SortUserStats();
        }

        //TODO Check if final stats are better than the old ones;
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