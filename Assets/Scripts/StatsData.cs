using System;
using System.Collections.Generic;

namespace Zeptolab
{
    [Serializable]
    public class StatsData : IStatsData
    {
        public List<IUserData> UsersStats { get { return usersData; }}
        private List<IUserData> usersData;

        public StatsData()
        {
            usersData = new List<IUserData>();
        }

        public void AddUserStat(IUserData userData)
        {
            // bool addData = CheckNewData(userData);
            //if (!addData) return;

            usersData.Add(userData);
            SortUserStats();
        }

        private bool CheckNewData(UserData newUserData)
        {
            return true;
        }

        public void SortUserStats()
        {
            usersData.Sort((pair1, pair2) => pair2.Coins.CompareTo(pair1.Coins));
        }
    }
}