using System;
using System.Collections.Generic;
using System.Text;

namespace TradeLib
{
    // methods receiving news must have this signature
    /// <summary>
    /// Safely pass around Debug instances between TradeLink members.
    /// </summary>
    public delegate void DebugFullDelegate(Debug debug);

    public enum DebugLevel
    {
        None,
        Status,
        Debug = 5
    }

    // heres our debug object
    /// <summary>
    /// Create debug messages
    /// </summary>
    [Serializable]
    public class Debug
    {
        string _msg = "";
        DebugLevel _level = DebugLevel.Status;
        public static Debug Create(string message) { return new Debug(message); }
        public static Debug Create(string message, DebugLevel level) { return new Debug(message, level); }
        public static Debug Create(string message, int messagelevel) { return new Debug(message, messagelevel); }
        public Debug(string msg) : this(msg, DebugLevel.Status) { }
        public Debug(string msg, DebugLevel level) { _msg = msg; _level = level; }
        public Debug(string msg, int level) { _msg = msg; _level = (DebugLevel)level; }
        public Debug(Debug copy) { _msg = copy.Msg; _level = copy.Level; }
        public string Msg { get { return _msg; } } 
        public string Type { get { return Enum.GetName(typeof(DebugLevel), _level); } }
        public DebugLevel Level { get { return _level; } }
        public bool Relevant(int currentlevel) { return (int)_level <= currentlevel; }
        public bool Relevant(DebugLevel currentlevel) { return _level <= currentlevel; }

    }
}
