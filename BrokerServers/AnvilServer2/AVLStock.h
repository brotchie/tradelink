#pragma once

#include "ObserverApi.h"
#include "BusinessApi.h"
#include "TradeLink_WM.h"


class ListBoxStock;

class AVLStock : public Observer//we derive this object from Obsererver to be able to get info about dynamic changes of the stock
                            //every Observer must have function virtual void Process(const Message* message, Observable* from, const Message* additionalInfo); which will be called when a stock update is received from the server.
{
public:
	AVLStock(const char* symbol, TradeLinkServer::TradeLink_WM* tl,bool load = false);

    virtual ~AVLStock();

//    static const unsigned int m_maxLevels;
//    static const Money m_maxThrough;
	TradeLinkServer::TradeLink_WM* tl;

    void Load();
//    void SetLoaded(bool loaded){m_loaded = loaded;}
    const StockBase* GetStockHandle() const{return m_stockHandle;}
    bool isLoaded() const{return m_stockHandle != NULL && B_IsStockLoaded(m_stockHandle);}

    void RemoveFromListBox();

    void SetIndex(unsigned int index){m_index = index;}
    unsigned int GetIndex() const{return m_index;}

    const std::string& GetSymbol() const{return m_symbol;}

    const MoneySize& GetLastTrade() const
    {
        return B_GetLastTrade((Observable*)m_level1);
    }
    const Money& GetClosePrice() const
    {
        return B_GetClosePrice((Observable*)m_level1);
    }
    char GetBidTick() const
    {
        unsigned int tick = B_GetBidTickStatus((Observable*)m_level1);
        return tick == UPTICK ? 'U' : tick == DOWNTICK ? 'D' : ' ';
    }

//    void CalculateQuotes(bool side);

    Observable* GetLevel1(){return m_level1;}
    Observable* GetLevel2(){return m_level2;}
    Observable* GetPrints(){return m_prints;}
//    Observable* GetAggregatedBook(){return m_aggregatedBook;}
//    Observable* GetExpandedBook(){return m_expandedBook;}
    void* GetBidIterator() const{return m_bidIterator;}
    void* GetAskIterator() const{return m_askIterator;}
    void SetOwner(CWnd* lb){m_owningListBox = lb;}

    const StockCalc* GetLevelBid() const {return m_stockCalcsLevelsBid;}
    const StockCalc* GetLevelAsk() const {return m_stockCalcsLevelsAsk;}
    const StockCalc* GetPriceBid() const {return m_stockCalcsPricesBid;}
    const StockCalc* GetPriceAsk() const {return m_stockCalcsPricesAsk;}

    void* GetLevelBidIterator() const{return m_levelBidIterator;}
    void* GetLevelAskIterator() const{return m_levelAskIterator;}
    void* GetPriceBidIterator() const{return m_priceBidIterator;}
    void* GetPriceAskIterator() const{return m_priceAskIterator;}

    void SetPosition(Position* pos);
    void SetMonitorThroughLimit(const Money& throughLimit); 
    void SetMonitorLevelLimit(unsigned int levelLimit); 
	void SetUseMmBooks(bool use);
    void SetEcnsOnlyBeforeAfterMarket(bool ecnsOnly);
    void SetEcnsOnlyDuringMarket(bool ecnsOnly);

    const Position* GetPosition() const{return m_position;}

    const MoneySize& GetNysBid() const{return m_nysBid;}
    const MoneySize& GetNysAsk() const{return m_nysAsk;}
    const MoneySize& GetNysPrint() const{return m_nysPrint;}
	void Clear();
protected:
    virtual void Process(const Message* message, Observable* from, const Message* additionalInfo);
    bool UpdateBestQuote(bool side);
//NYSINFO
    void UpdateNysQuote(bool side, const MoneySize& q);
    void InitNysQuotes();
    void InitNysQuote(bool side);



    std::string m_symbol;
    const StockBase* m_stockHandle;
    Observable* m_level1;
    Observable* m_level2;
    Observable* m_prints;
	Observable* m_account;

    void* m_bidIterator;
    void* m_askIterator;

    CWnd* m_owningListBox;//we will be updating the list box items, so we need a pointer to the list box owning the stock

//    bool m_loaded;
    unsigned int m_index;

    StockCalc* m_stockCalcsLevelsBid;
    StockCalc* m_stockCalcsLevelsAsk;
    StockCalc* m_stockCalcsPricesBid;
    StockCalc* m_stockCalcsPricesAsk;

    void* m_levelBidIterator;
    void* m_levelAskIterator;
    void* m_priceBidIterator;
    void* m_priceAskIterator;


    MoneySize m_isldBidQuote;
    MoneySize m_isldAskQuote;

    MoneySize m_bestBid;
    MoneySize m_bestAsk;

    Position* m_position;

    MoneySize m_nysBid;
    MoneySize m_nysAsk;
    MoneySize m_nysPrint;
};


