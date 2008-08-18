
#include "ObserverApi.h"
#include "BusinessApi.h"

LPARAM ServiceMsg(const int gotType,CString gotMsg);
void TLUnload();
void AllClients(std::vector <CString> &subscriberids);
void Subscribers(CString stock,std::vector <CString> &subscriberids);
void TLKillDead(int deathInseconds);
void gsplit(CString msg, CString del, std::vector<CString>& rec);


enum TL2 {
	OK = 0,
	SENDORDER = 1,
	AVGPRICE,
	POSOPENPL,
	POSCLOSEDPL,
	POSLONGPENDSHARES,
	POSSHORTPENDSHARES,
	LRPBID,
	LRPASK,
	POSTOTSHARES,
	LASTTRADE,
	LASTSIZE,
	NDAYHIGH,
	NDAYLOW,
	INTRADAYHIGH,
	INTRADAYLOW,
	OPENPRICE,
	CLOSEPRICE,
	NLASTTRADE = 20,
	NBIDSIZE,
	NASKSIZE,
	NBID,	
	NASK,	
	ISSIMULATION,
	GETSIZE,
	YESTCLOSE,
	BROKERNAME,
    ACCOUNTOPENPL,
    ACCOUNTCLOSEDPL,
	TICKNOTIFY = 100,
	EXECUTENOTIFY,
	REGISTERCLIENT,
	REGISTERSTOCK,
	CLEARSTOCKS,
	CLEARCLIENT,
	HEARTBEAT,
	ORDERNOTIFY,
	INFO,
	QUOTENOTIFY,
	TRADENOTIFY,
	REGISTERINDEX,
	DAYRANGE,
	ACCOUNTRESPONSE = 500,
	ACCOUNTREQUEST,
	ORDERCANCELREQUEST,
	ORDERCANCELRESPONSE,
    FEATUREREQUEST,
    FEATURERESPONSE,
	FEATURE_NOT_IMPLEMENTED = 994,
	CLIENTNOTREGISTERED = 995,
	GOTNULLORDER = 996,
	UNKNOWNMSG,
	UNKNOWNSYM,
	TL_CONNECTOR_MISSING,
};

enum Brokers
{
    TradeLinkSimulation = 0,
    Assent,
    InteractiveBrokers,
    Genesis,
    Bright,
    Echo,
};

enum OrderField
{
	SYM = 0,
	SIDE,
	OSIZE,
	PRCE,
	STOP,
	USER,
	EXCH,
	ACCT,
	SECT,
	CURR,
	LSYM,
};



class TLIdx : public Observer
{
// Construction
public:
	TLIdx(CString symbol);   // standard constructor
	CString m_symbol;
	CString m_StaticSymbol;
	protected:

// Implementation
protected:

	afx_msg void OnChangeIndexSymbol();
	afx_msg void OnDynamicUpdate();
	void Load(CString symbol);
    virtual void Process(const Message* message, Observable* from, const Message* additionalInfo);
    void FillInfo();

    MarketIndex* m_index;
};



