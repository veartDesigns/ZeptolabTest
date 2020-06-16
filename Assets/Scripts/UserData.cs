using System;

namespace Zeptolab
{
    [Serializable]
    public class UserData : IUserData
    {
        public string UserName { get { return _userName; }}
        public int Coins { get { return _coins; }}

        private string _userName;
        private int _coins;

        public UserData(string username, int coins)
        {
            _userName = username;
            _coins = coins;
        }
    }
}       