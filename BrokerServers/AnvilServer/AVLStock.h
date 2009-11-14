#pragma once
#include "ObserverApi.h"
#include "BusinessApi.h"
#include "TradeLibFast.h"
#include "AVL_TLWM.h"

unsigned int TIFId(CString name);

class AVL_TLWM;

class AVLStock : public Observer
	//we derive this object from Obsererver to be able to get info about dynamic changes of the stock
    //every Observer must have function virtual void Process(const Message* message, Observable* from, const Message* additionalInfo); which will be called when a stock update is received from the server.
{
public:
	AVLStock(const char* symbol, int id, AVL_TLWM* tl, bool load = true, int depth = 0);

    virtual ~AVLStock();


    void Load();
    const StockBase* GetStockHandle() const{return m_stockHandle;}
	bool isLoaded() const;
    const std::string& GetSymbol() const{return m_symbol;}
	AVL_TLWM * tl;


	void Clear();
protected:
	void TradeNotify();
	void QuoteNotify();
    virtual void Process(const Message* message, Observable* from, const Message* additionalInfo);
	
    Observable* GetLevel1(){return m_level1;}
    Observable* GetLevel2(){return m_level2;}
    Observable* GetPrints(){return m_prints;}

    std::string m_symbol;
    const StockBase* m_stockHandle;
    Observable* m_level1;
    Observable* m_level2;
    Observable* m_prints;
	Observable* m_account;
	TradeLibFast::TLTick kquote;
	TradeLibFast::TLTick ktrade;

	void* bidi;
	void* aski;
	void* pnti;
	uint booki[MAX_BOOKS];

	int depth;
	CString _sym;
	int _symid;
	int _date;


};

