
namespace TradeLink.API
{
    public interface Debug
    {
        string Msg { get;  }
        DebugLevel Level { get; }
        bool Relevant(DebugLevel currentlevel);
        bool Relevant(int currentlevel);
    }


    public enum DebugLevel
    {
        None,
        Status,
        Debug = 5
    }
}
