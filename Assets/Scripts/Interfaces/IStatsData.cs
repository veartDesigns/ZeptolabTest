using System.Collections.Generic;

namespace Zeptolab
{
    public interface IStatsData
    {
        void AddUserStat( IUserData data);
        List<IUserData> UsersStats { get; }
    }
}