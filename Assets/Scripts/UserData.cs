using System;

namespace Zeptolab
{
    [Serializable]
    public class UserData : IUserData
    {
        public string UserName;
        public int Coins;

        private string _userName;
        private int _coins;

        public UserData(string username, int coins)
        {
            UserName = username;
            Coins = coins;
        }
    }
}       