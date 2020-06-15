using System;

namespace Zeptolab
{
    [Serializable]
    public class UserData
    {
        public string UserName;
        public int Coins;

        public UserData(string username, int coins)
        {
            UserName = username;
            Coins = coins;
        }
    }
}