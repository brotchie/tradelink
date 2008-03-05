using System;


namespace TradeLib
{
    public class Account
    {
        public Account() { }
        public Account(string AccountID) { _id = AccountID; }
        public Account(string AccountID, string Description) { _id = AccountID; _desc = Description; }
        public bool isValid { get { return (ID!=null);} }
        string _id;
        public string ID { get { return _id; } }
        string _desc;
        public string Desc { get { return _desc; } set { _desc = value; } }
        public override string ToString()
        {
            return ID;
        }
    }
}
