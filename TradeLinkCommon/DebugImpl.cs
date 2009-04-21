using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;

namespace TradeLink.Common
{
    /// <summary>
    /// Create and send debug messages
    /// </summary>
    [Serializable]
    public class DebugImpl : Debug
    {
        string _msg = "";
        DebugLevel _level = DebugLevel.Status;
        public static Debug Create(string message) { Debug d = new DebugImpl(message); return d; }
        public static Debug Create(string message, DebugLevel level) { Debug d = new DebugImpl(message, level); return d; }
        public static Debug Create(string message, int messagelevel) { Debug d = new DebugImpl(message, messagelevel); return d; }
        public DebugImpl(string msg) : this(msg, DebugLevel.Status) { }
        public DebugImpl(string msg, DebugLevel level) { _msg = msg; _level = level; }
        public DebugImpl(string msg, int level) { _msg = msg; _level = (DebugLevel)level; }
        public DebugImpl(Debug copy) { _msg = copy.Msg; _level = copy.Level; }
        public string Msg { get { return _msg; } } 
        public string Type { get { return Enum.GetName(typeof(DebugLevel), _level); } }
        public DebugLevel Level { get { return _level; } }
        public bool Relevant(int currentlevel) { return (int)_level <= currentlevel; }
        public bool Relevant(DebugLevel currentlevel) { return _level <= currentlevel; }

    }
}
