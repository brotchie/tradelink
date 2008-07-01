using System;


namespace TradeLib
{
    /// <summary>
    /// A token used to identify one brokerage account from another.
    /// </summary>
    public class Account
    {
        public Account() { }
        public Account(string AccountID) { _id = AccountID; }
        public Account(string AccountID, string Description) { _id = AccountID; _desc = Description; }
        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        public bool isValid { get { return (ID!=null) && (ID!="");} }
        bool _execute = true;
        bool _notify = true;
        public bool Notify { get { return _notify; } set { _notify = value; } }
        public bool Execute { get { return _execute; } set { _execute = value; } }
        string _id = "";
        /// <summary>
        /// Gets the ID of this account.
        /// </summary>
        /// <value>The ID.</value>
        public string ID { get { return _id; } }
        string _desc;
        /// <summary>
        /// Gets or sets the description for this account.
        /// </summary>
        /// <value>The desc.</value>
        public string Desc { get { return _desc; } set { _desc = value; } }
        public override string ToString()
        {
            return ID;
        }
        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Account o = (Account)obj;
            return Equals(o);
        }
        public bool Equals(Account a)
        {
            return this._id.Equals(a.ID);
        }
    }
}
