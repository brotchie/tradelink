#ifndef BUSINESS_API_H
#define BUSINESS_API_H

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the BUSINESS_EXPORTS
// symbol defined on the command line. this symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// BUSINESS_API functions as being imported from a DLL, wheras this DLL sees symbols
// defined with this macro as being exported.
#ifdef BUSINESS_EXPORTS
#define BUSINESS_API __declspec(dllexport)
#else
#define BUSINESS_API __declspec(dllimport)
#endif

const char* const BusinessHeaderVersion = "2.6.8.11";

#include "ObserverApi.h"
#include "CommonIds.h"

//class MsgLogon;
//class MsgLogonNew;

typedef unsigned int time32;
/*
template <class T>
class BUSINESS_API MathValue
{
public:
    virtual bool operator==(const T& m) const = 0;
    virtual bool operator<(const T& m) const = 0;
    bool operator!=(const T& t) const{return !operator==(t);}
    bool operator<=(const T& t) const{return operator<(t) || operator==(t);}
    bool operator>(const T& t) const{return !operator<=(t);}
    bool operator>=(const T& t) const{return !operator<(t);}
};
class BUSINESS_API Money : public MathValue<Money>
*/

const unsigned int B_MULTIPLIER = 10;
const unsigned int B_DECMULTIPLIER = 1000;

const unsigned int B_BINMULTIPLIER = 1 << B_MULTIPLIER;
const int MONEY_FRACTION = (int)B_DECMULTIPLIER;
const unsigned int MONEY_DEC_DIGITS = 3;

class MoneyPrecise;

class BUSINESS_API Money
{
public:
    Money():
        m_whole(0),
        m_fraction(0)
    {
    }

    explicit Money(int serverPrice)
    {
        m_whole = ConvertServerPriceToDecimal(serverPrice, m_fraction);
    }

    Money(int whole, short thousandsFraction):
        m_whole(whole),
        m_fraction(thousandsFraction)
    {
        Normalize();
    }

    Money(int whole, short thousandsFraction, int size):
        m_whole(whole * size)
    {
        int fraction = thousandsFraction * size;
        NormalizeMoney(m_whole, fraction);
        m_fraction = fraction;
    }

    Money(const MoneyPrecise& other);

    virtual ~Money(){}

    bool operator!=(const Money& t) const{return !operator==(t);}
    bool operator<=(const Money& t) const{return !operator>(t);}
    bool operator>=(const Money& t) const{return !operator<(t);}

    Money& Money::operator++()
    {
        m_fraction++;
        Normalize();
        return *this;
    }

    Money Money::operator++(int)
    {
        Money temp(*this);
        ++*this;
        return temp;
    }

    Money& Money::operator--()
    {
        m_fraction--;
        Normalize();
        return *this;
    }

    Money Money::operator--(int)
    {
        Money temp(*this);
        --*this;
        return temp;
    }

    virtual void InitMoneyFromServerValue(int serverPrice)
    {
        m_whole = ConvertServerPriceToDecimal(serverPrice, m_fraction);
    }

    void InitMoney(int whole, short thousandsFraction)
    {
        m_whole = whole;
        m_fraction = thousandsFraction;
        Normalize();
    }

    Money GetAbsoluteValue() const{return isNegative() ? -(*this) : *this;}

    virtual unsigned int GetDecMultiplier() const{return B_DECMULTIPLIER;}
    int GetMoneyFraction() const{return GetDecMultiplier();}

    Money Round() const
    {
        Money m = GetAbsoluteValue();
        short cents10 = m.m_fraction / 10 * 10;
        if(m.m_fraction - cents10 > 4)
        {
            m.m_fraction = cents10 + 10;
            if(m.m_fraction >= GetMoneyFraction())
            {
                m.m_whole++;
                m.m_fraction -= GetMoneyFraction();
            }
        }
        else
        {
            m.m_fraction = cents10;
        }
        return isNegative() ? -m : m;
    }

    Money Round(bool down) const
    {
        Money m;
        if(isNegative())
        {
            m = -(*this);
            down = !down;
        }
        else
        {
            m = *this;
        }
        if(down)
        {
            m.m_fraction /= 10;
            m.m_fraction *= 10;
        }
        else
        {
            short cents10 = m.m_fraction / 10 * 10;
            if(m.m_fraction > cents10)
            {
                m.m_fraction = cents10 + 10;
                if(m.m_fraction >= GetMoneyFraction())
                {
                    m.m_whole++;
                    m.m_fraction -= GetMoneyFraction();
                }
            }
            else
            {
                m.m_fraction = cents10;
            }
        }
        return isNegative() ? -m : m;
    }

    Money Round(const Money& round, bool down) const
    {
        if(round.isZero())
        {
            return *this;
        }
        Money m;
        if(isNegative())
        {
            m = -(*this);
            down = !down;
        }
        else
        {
            m = *this;
        }
        unsigned int rounded;
        unsigned int tenthCents = m.m_whole * GetMoneyFraction() + m.m_fraction;
        unsigned int tenthCentsRound = round.m_whole * GetMoneyFraction() + round.m_fraction;
        rounded = tenthCents/tenthCentsRound;
        rounded *= tenthCentsRound;
        if(!down && rounded != tenthCents)
        {
            rounded += tenthCentsRound;
        }
        m.m_whole = rounded / GetDecMultiplier();
        m.m_fraction = rounded - GetDecMultiplier() * m.m_whole;
        return isNegative() ? -m : m;
    }

    static void NormalizeMoneyValues(int& whole, int& fraction, int multiplier);
    void NormalizeMoney(int& whole, int& fraction)
    {
        NormalizeMoneyValues(whole, fraction, GetDecMultiplier());
    }

    static int GetMoneyForServer(int wholePart, short thousandsFractionPart);
    
    static int ConvertServerPriceToDecimal(int serverPrice, short& thousandsFractionPart);

    virtual int GetMoneyValueForServer() const
    {
        return GetMoneyForServer(m_whole, m_fraction);
    }

    int GetWhole() const{return m_whole;}
    short GetThousandsFraction() const{return m_fraction;}
    bool isNegative() const{return m_whole < 0 || m_fraction < 0;}
    bool isZero() const{return m_whole == 0 && m_fraction == 0;}
    bool isPositive() const{return m_whole > 0 || m_fraction > 0;}

    void SetZero(){m_whole = 0; m_fraction = 0;}

    bool operator==(const Money& m) const
    {
        return m_whole == m.m_whole && m_fraction == m.m_fraction;
    }
    bool operator<(const Money& m) const
    {
        return m_whole != m.m_whole ? m_whole < m.m_whole : m_fraction < m.m_fraction;
    }
    bool operator>(const Money& m) const
    {
        return m_whole != m.m_whole ? m_whole > m.m_whole : m_fraction > m.m_fraction;
    }

    Money operator-() const
    {
        return Money(-m_whole, -m_fraction);
    }

    double toDouble() const{return (double)m_whole + (double)m_fraction / GetMoneyFraction();}
    int toInt() const{return m_whole * GetMoneyFraction() + m_fraction;}

    Money& operator+=(const Money& m)
    {
        m_whole += m.m_whole;
        m_fraction += m.m_fraction;
        Normalize();
        return *this;
    }

    Money& operator-=(const Money& m)
    {
        m_whole -= m.m_whole;
        m_fraction -= m.m_fraction;
        Normalize();
        return *this;
    }

    Money& operator*=(int size)
    {
        m_whole *= size;
        int fraction = m_fraction * size;
        NormalizeMoney(m_whole, fraction);
        m_fraction = fraction;
        return *this;
    }

    virtual Money& operator/=(int size);

    Money MulDiv(int numerator, int denominator) const;

    Money operator+(const Money& m) const
    {
        Money result(*this);
        result += m;
        return result;
    }

    Money operator-(const Money& m) const
    {
        Money result(*this);
        result -= m;
        return result;
    }

    Money operator*(int s) const
    {
        Money result(*this);
        result *= s;
        return result;
    }

    Money operator/(int s) const
    {
        Money result(*this);
        result /= s;
        return result;
    }
protected:
    void Normalize()
    {
        int fraction = m_fraction;
        NormalizeMoney(m_whole, fraction);
        m_fraction = fraction;
    }
    int m_whole;
    short m_fraction;
};


class BUSINESS_API MoneyPrecise : public Money
{
public:
    MoneyPrecise(){}

    MoneyPrecise(const Money& money):Money(money)
    {
        m_fraction *= 10;
    }

    MoneyPrecise(const MoneyPrecise& other):Money()
    {
        Money::operator=(other);
    }

    explicit MoneyPrecise(int serverPrice):
        Money(serverPrice)
    {
        m_fraction *= 10;
    }

    MoneyPrecise(int whole, short tenThousandsFraction):
        Money()
    {
        m_whole = whole;
        m_fraction = tenThousandsFraction;
        Normalize();
    }

    MoneyPrecise(int whole, short tenThousandsFraction, int size):
        Money()
    {
        m_whole = whole * size;
        int fraction = tenThousandsFraction * size;
        NormalizeMoney(m_whole, fraction);
        m_fraction = fraction;
    }

    const MoneyPrecise& operator=(const Money& other)
    {
        Money::operator=(other);
//        m_whole = other.m_whole;
//        m_fraction = other.m_fraction;
        return *this;
    }

    virtual void InitMoneyFromServerValue(int serverPrice)
    {
		Money::InitMoneyFromServerValue(serverPrice);
        m_fraction *= 10;
    }

	virtual unsigned int GetDecMultiplier() const{return 10000;}

	virtual int GetMoneyValueForServer() const
    {
        return GetMoneyForServer(m_whole, m_fraction / 10);
    }

    MoneyPrecise operator+(const MoneyPrecise& m) const
    {
        MoneyPrecise result(*this);
        result += m;
        return result;
    }

    MoneyPrecise operator-(const MoneyPrecise& m) const
    {
        MoneyPrecise result(*this);
        result -= m;
        return result;
    }

    MoneyPrecise operator*(int s) const
    {
        MoneyPrecise result(*this);
        result *= s;
        return result;
    }

    MoneyPrecise operator/(int s) const
    {
        MoneyPrecise result(*this);
        result /= s;
        return result;
    }
};


class BUSINESS_API MoneySize : public Money
{
public:
//    MoneySize(unsigned int shares, int price);
    MoneySize(unsigned int shares, const Money& money);
    MoneySize():m_shares(0){}
    unsigned int GetSize() const{return m_shares;}
//    virtual unsigned int GetSize() const{return m_shares;}
//    unsigned int GetShares() const{return m_shares;}
    bool operator==(const MoneySize& t) const
    {
        return Money::operator==(t) && m_shares == t.m_shares;
    }
    bool operator!=(const MoneySize& t) const
    {
        return !operator==(t);
    }
    bool SameMoney(const Money& money) const{return money == *this;}
    bool LessMoney(const Money& money) const{return money > *this;}
    bool GreaterMoney(const Money& money) const{return money < *this;}
    void SetMoney(const Money& money){m_whole = money.GetWhole(); m_fraction = money.GetThousandsFraction();}
    void SetShares(unsigned int shares){m_shares = shares;}
    void IncrementShares(unsigned int shares){m_shares += shares;}
    bool DecrementShares(unsigned int shares)
    {
        if(shares < m_shares)
        {
            m_shares -= shares;
            return false;
        }
        else
        {
            m_shares = 0;
            return true;
        }
    }
    Money GetTotalMoney() const{return *(Money*)this * m_shares;}
protected:
    unsigned int m_shares;
};

enum PrintSourse
{
    PS_NASDAQ,
    PS_NYSE,
    PS_AMEX,
    PS_REGIONAL,
    PS_NSD,

    PS_LAST
};

class BUSINESS_API Transaction : public MoneySize//PriceSize
{
public:
    virtual time32 GetTime() const{return 0;}
    virtual unsigned char GetSide() const{return ' ';}
    virtual unsigned int GetStatus() const{return 0xFFFFFFFF;}
	virtual char GetSaleCondition() const{return '\0';}
    virtual unsigned short GetPriority() const{return 0xFFFF;}
    virtual const char* GetMmid() const{return GetExecutionExchangeName();}//"";
	virtual unsigned int GetMmNumId() const{return *(unsigned int*)GetMmid();}
    virtual const char* GetCounterParty() const{return "";}
//    virtual unsigned int Get100Blocks() const{return m_shares / 100;}
    virtual bool lessChronologically(const Transaction& t) const{return false;}
    virtual char GetPrimaryExchange() const{return ANY;}
    virtual char GetExecutionExchange() const{return ExecExch_ANY;}
    virtual const char* GetExecutionExchangeName() const{return "";}
    virtual bool isExecutionExchangeRegional() const{return false;}
    virtual bool isExecutionExchangeRegionalOrNas() const{return false;}
    virtual bool isExecutionExchangeCaes() const{return false;}
//    virtual bool isExecutionExchangeCaesOrSize() const{return false;}
    virtual bool isHidden() const{return false;}
    bool operator==(const Transaction& t) const
    {
        return 
            Money::operator==(t) &&
            GetSize() == t.GetSize();
    }
protected:
//    Transaction(unsigned int shares, int price);
    Transaction(unsigned int shares, const Money& price);
};

class Order;

class BUSINESS_API BookEntry : public Transaction
{
public:
    virtual const Order* GetOrder() const{return NULL;}
    virtual const char* GetMmid() const = 0;
	virtual BookEntry* Clone() const = 0;
    virtual unsigned __int64 GetOrders() const{return 1;}
    virtual bool isDirect() const{return true;}
    virtual bool isPriceImproved() const{return false;}
    virtual bool isEcn() const;
//    virtual bool isSuperMontageEcn() const;
    
    virtual char GetAttributes() const;
    virtual char GetQuoteCondition() const{return '\0';}
	virtual void SetQuoteCondition(char quoteCondition){}

    virtual bool lessPrice(const BookEntry& be) const
    {
        return Money::operator<(be);
    }

    virtual void UpdateSortPriority(){}
    virtual bool lessIfPriceEqual(const BookEntry& be) const
    {
        return 
            GetSortPriority() != be.GetSortPriority() ? GetSortPriority() < be.GetSortPriority() :
            GetPriority() < be.GetPriority();
    }

//    virtual bool lessBeforePrice(const BookEntry& be) const{return false;}

    virtual unsigned short GetPriority() const = 0;
    virtual unsigned short GetSortPriority() const = 0;
    virtual bool operator==(const BookEntry& e) const{return Transaction::operator==(e) && isMmid(e.GetMmid());}
    unsigned __int64 GetOrdinal() const{return m_ordinal;}
	virtual void SetOrdinal(unsigned __int64 ordinal){}

    virtual bool isMmid(const char* mmid) const = 0;//{return isMmid(*(unsigned int*)mmid);}
protected:
//    BookEntry(unsigned int shares, int price/*, unsigned int priority*/, unsigned int ordinal);
    BookEntry(unsigned int shares, const Money& price/*, unsigned int priority*/, unsigned __int64 ordinal);
    unsigned __int64 m_ordinal;
};


class BUSINESS_API MarketIndex : public Observable  
{
public:
//    virtual ~MarketIndex(){}
    const char* GetSymbol() const{return m_symbol;}
    const char* GetDescription() const{return m_description;}
    const Money& GetValue() const{return m_value;}
    const Money& GetNetChange() const{return m_netChange;}
    const Money& GetHigh() const{return m_high;}
    const Money& GetLow() const{return m_low;}
    const Money& GetCloseValue() const{return m_closeValue;}
    const Money& GetOpenValue() const{return m_openValue;}
    const Money& GetMarketOpenValue() const{return m_marketOpenValue;}
    int	GetTickDirection() const{return m_tickDirection;}
    bool isInitialized() const{return m_initialized;}
    virtual Observable* GetChart() = 0;
    virtual void Reset();
    virtual unsigned int GetChartPointCount() const = 0;
    virtual const Money& GetHistoricalValue(unsigned int minute) const = 0;
    virtual const Money* GetValueFromBack(unsigned int minute) const = 0;
protected:
    MarketIndex(const char* symbol);
    char m_symbol[ LENGTH_SYMBOL ];
    char m_description[ LENGTH_SEQURITYNAME ];
    Money			m_value;
    Money			m_netChange;
    Money			m_high;
    Money			m_low;
    Money           m_closeValue;
    Money           m_openValue;
    int             m_tickDirection;
    Money           m_marketOpenValue;
    bool m_initialized;
};


enum SendOrderError
{
    SO_OK,
    SO_NO_ACCOUNT,
    SO_NO_SERVER_CONNECTION,
    SO_STOCK_NOT_INITIALIZED,
    SO_OPTION_TRADING_NOT_ALLOWED,
	SO_OPTION_CANNOT_SELL_UNCOVERED,
    SO_BUYING_POWER_EXCEEDED,
    SO_SIZE_ZERO,
    SO_SIZE_TOO_SMALL,
    SO_INCORRECT_PRICE,
    SO_INCORRECT_SIDE,
    SO_NO_BULLETS_FOR_CHEAP_STOCK,
    SO_NO_SHORTSELL_FOR_CHEAP_STOCK,
    SO_NO_ONOPENORDER_FOR_NASDAQ_STOCK,
    SO_NO_ONCLOSEORDER_FOR_NASDAQ_STOCK,
    SO_NO_ONCLOSEORDER_AGAINST_IMBALANCE_AFTER_1540,
    SO_NO_ONCLOSEORDER_AFTER_1600,
    SO_NO_SIZEORDER_FOR_NON_NASDAQ_STOCK,
    SO_NO_STOPORDER_FOR_NASDAQ_STOCK,
    SO_MAX_ORDER_SIZE_EXCEEDED,
    SO_MAX_POSITION_SIZE_EXCEEDED,
    SO_MAX_POSITION_VALUE_EXCEEDED,
    SO_TRADING_LOCKED,
    SO_TRADING_HISTORY_NOT_LOADED,
    SO_NO_SOES_ORDER_WHEN_MARKET_CLOSED,
    SO_NO_SDOT_ORDER_WHEN_MARKET_CLOSED,
    SO_MAX_OPEN_POSITIONS_EXCEEDED,
    SO_MAX_POSITION_PENDING_ORDERS_EXCEEDED,
    SO_MAX_TOTAL_SHARES_EXCEEDED,
    SO_MAX_TRADED_SHARES_EXCEEDED,
    SO_AMEX_ORDER_EXECUTION_BLOCKED,
    SO_NYSE_ORDER_EXECUTION_BLOCKED,
    SO_NASDAQ_ORDER_EXECUTION_BLOCKED,
	SO_ARCA_ORDER_EXECUTION_BLOCKED,
	SO_VENUE_BLOCKED,
	SO_ARCA_ODD_LOT_ORDER_BLOCKED,
    SO_MAX_LOSS_EXCEEDED,
    SO_MAX_LOSS_PER_STOCK_EXCEEDED,
    SO_MAX_OPEN_LOSS_PER_STOCK_EXCEEDED,
    SO_NYSE_ODD_LOT_VIOLATION,
	SO_INVALID_STAGING_CONTEXT,

    SO_SHORT_EXEMPT_NOT_INSTITUTIONAL,
    SO_SELL_SIZE_GREATER_THAN_POSITION,
    SO_NO_SHORT_BEFORE_SELL_COVER_POSITION,
    SO_SHORT_CAN_EXECUTE_BEFORE_SELL,
    SO_SAME_PRICE_VENUE_OVERSELL,
    SO_STAGING_TICKET_EXCEEDED,

	SO_DESTINATION_NOT_RECOGNIZED,

	SO_HIT_OWN_ORDERS,

//    SO_SHORT_SELL_VIOLATION,
	SO_POTENTIAL_OVERSELL,
};

class BUSINESS_API TradePack : public MoneySize
{
public:
    const char* GetSymbol() const{return m_symbol;}
    unsigned int GetId() const{return m_id;}
    char GetSide() const{return m_side;}
    time32 GetTimeEntered() const{return m_timeEntered;}
    virtual time32 GetTimeCreated() const{return m_timeEntered;}
	virtual const Money& GetPnl() const;
    unsigned int GetChronologyOrdinal() const{return m_chronologyOrdinal;}
    const char* GetDestination() const{return m_destination;}
    const char* GetReference() const{return m_reference;}
    bool isShort() const{return m_side != 'B' && m_side != 'S' && m_side != 'L';}
protected:
//    TradePack(const char* symbol, unsigned int id, char side, unsigned int shares, int price, const char* destination, time32 timeEntered = 0, const char* reference = "");
    TradePack(const char* symbol, unsigned int id, char side, unsigned int shares, const Money& price, const char* destination, time32 timeEntered = 0, const char* reference = "");
    char m_symbol[LENGTH_SYMBOL + 1];
    char m_destination[LENGTH_SYMBOL];
    unsigned int m_id;
    char m_side;
    time32 m_timeEntered;
    unsigned int m_chronologyOrdinal;
  	char m_reference[15];
};

enum ExecutionFlags
{
    EF_OVERNIGHT = 1,
    EF_BULLETS = 2,
    EF_LASTVALIDFLAG,
    EF_NOMATCHINGORDER = 4,
    EF_WRONGDIRECTION = 8,
    EF_WRONGSYMBOL = 16,
};

class BUSINESS_API OptionData
{
public:
	OptionData(const char* underlierSymbol,
		const Money& strikePrice,
		const char* strikePriceSymbol,
		unsigned short expirationYear,
		unsigned char expirationMonth,
		unsigned char expirationDay,
		bool call,
		unsigned int sharesPerContract = 100):

		m_strikePrice(strikePrice),
		m_sharesPerContract(sharesPerContract),
		m_expirationYear(expirationYear),
		m_expirationMonth(expirationMonth),
		m_expirationDay(expirationDay),
		m_call(call)
	{
		SetUnderlierSymbol(underlierSymbol);
		SetStrikePriceSymbol(strikePriceSymbol);
	}
	bool isCall() const{return m_call;}
	bool isPut() const{return !m_call;}
	const Money* GetStrikePrice() const{return m_strikePrice.isZero() ? NULL : &m_strikePrice;}
	const Money& GetStrikePriceRef() const{return m_strikePrice;}
	const char* GetStrikePriceSymbol() const{return m_strikePriceSymbol;}
	unsigned int GetSharesPerContract() const{return m_sharesPerContract;}
	unsigned int GetExpirationDate() const{return (m_expirationYear << 9) | (m_expirationMonth << 5) | m_expirationDay;}
	unsigned short GetExpirationYear() const{return m_expirationYear;}
	unsigned char GetExpirationMonth() const{return m_expirationMonth;}
	unsigned char GetExpirationDay() const{return m_expirationDay;}
	int GetDaysToExpiration() const{return m_daysToExpiration;}
	const char* GetUnderlierSymbol() const{return m_underlierSymbol;}

	void SetCall(bool call){m_call = call;}
	void SetStrikePrice(const Money& strikePrice){m_strikePrice = strikePrice;}
	void SetStrikePriceSymbol(const char* strikePriceSymbol)
	{
		memset(m_strikePriceSymbol, 0, sizeof(m_strikePriceSymbol));
		if(strikePriceSymbol)
		{
			strncpy_s(m_strikePriceSymbol, sizeof(m_strikePriceSymbol), strikePriceSymbol, sizeof(m_strikePriceSymbol) - 1);
		}
	}
	void SetSharesPerContract(unsigned int spc){m_sharesPerContract = spc;}

	void SetExpirationDate(unsigned short expirationYear, unsigned char expirationMonth, unsigned char expirationDay);

	void SetUnderlierSymbol(const char* underlierSymbol)
	{
		memset(m_underlierSymbol, 0, sizeof(m_underlierSymbol));
		if(underlierSymbol)
		{
			strncpy_s(m_underlierSymbol, sizeof(m_underlierSymbol), underlierSymbol, sizeof(m_underlierSymbol) - 1);
		}
	}
protected:
	char m_underlierSymbol[9];
	Money m_strikePrice;
	char m_strikePriceSymbol[9];
	unsigned int m_sharesPerContract;
	int m_daysToExpiration;
	unsigned short m_expirationYear;
	unsigned char m_expirationMonth;
	unsigned char m_expirationDay;
	bool m_call;
};

class BUSINESS_API Execution : public TradePack
{
public:
	virtual ~Execution();
//    Execution(const char* symbol, unsigned int id, char side, unsigned int shares, int price, const char* counterParty, time32 timeEntered, const char* reference);
    unsigned int GetFlags() const{return m_flags;}
    virtual const MoneySize& GetOrderPriceSize() const = 0;
    virtual bool isMarketOrder() const = 0;
    virtual const Order* GetOrder() const = 0;
    const char* GetUserOrderDescription() const;
    unsigned int GetUserOrderType() const;
	const Money& GetLevel1Price() const;
	const Money& GetLevel2Price() const;
    bool isProactive() const;
    bool isISO() const;
    char GetStockExchange() const;
	unsigned __int64 GetCmta() const;

	unsigned int GetSharesPerContract() const{return m_optionData ? m_optionData->GetSharesPerContract() : 0;}
	bool isCall() const{return m_optionData && m_optionData->isCall();}
	bool isPut() const{return m_optionData && m_optionData->isPut();}
	const Money* GetStrikePrice() const{return m_optionData ? m_optionData->GetStrikePrice() : NULL;}
	const char* GetStrikePriceSymbol() const{return m_optionData ? m_optionData->GetStrikePriceSymbol() : NULL;}
	unsigned int GetExpirationDate() const{return m_optionData ? m_optionData->GetExpirationDate() : 0;}
	unsigned short GetExpirationYear() const{return m_optionData ? m_optionData->GetExpirationYear() : 0;}
	unsigned char GetExpirationMonth() const{return m_optionData ? m_optionData->GetExpirationMonth() : 0;}
	unsigned char GetExpirationDay() const{return m_optionData ? m_optionData->GetExpirationDay() : 0;}
	int GetDaysToExpiration() const{return m_optionData ? m_optionData->GetDaysToExpiration() : 0;}
	const char* GetUnderlierSymbol() const{return m_optionData ? m_optionData->GetUnderlierSymbol() : NULL;}

    bool isOvernight() const{return (m_flags & EF_OVERNIGHT) != 0;}
    bool isBullets() const{return (m_flags & EF_BULLETS) != 0;}
    bool isInvalid() const{return m_flags > EF_LASTVALIDFLAG;}
    int GetPositionSize() const{return m_positionSize;}
    const Money& GetPositionClosedPnl() const{return m_positionClosedPnl;}
    const Money& GetPositionMoneyInvested() const{return m_positionMoneyInvested;}
    const Money& GetPositionMoneyExposed() const{return m_positionMoneyExposed;}
    const Money& GetTotalClosedPnl() const{return m_totalClosedPnl;}
    unsigned int GetPositionSharesTradedToday() const{return m_positionSharesTradedToday;}
    unsigned int GetTotalSharesTradedToday() const{return m_totalSharesTradedToday;}
	unsigned int GetUniqueExecutionId() const{return m_uniqueExecutionId;}
    bool isClosing() const{return m_closing;}
	virtual const Money& GetPnl() const{return m_pnl;}
	const OptionData* GetOptionData() const{return m_optionData;}
	void SetOptionData(const OptionData* optionData);
protected:
    Execution(const char* symbol,
		const OptionData* optionData,
		unsigned int executionId,
		unsigned int orderId,
		char side,
		unsigned int shares,
		const Money& price,
		const char* counterParty,
		time32 timeEntered,
		const char* reference);
	OptionData* m_optionData;
    unsigned int m_flags;
    int m_positionSize;
    Money m_pnl;
    Money m_positionClosedPnl;
    Money m_positionMoneyInvested;
    Money m_positionMoneyExposed;
    Money m_totalClosedPnl;
    unsigned int m_positionSharesTradedToday;
    unsigned int m_totalSharesTradedToday;
	unsigned int m_uniqueExecutionId;
    bool m_closing;
//    unsigned int m_orderId;
};

enum OrderVisibilityMode
{
    OVM_VISIBLE = 0,
    OVM_HIDDEN = 1,
    OVM_DISCRETION = 2,
//    OVM_HUNTER = 4,
};


enum StopTriggerType
{
    TT_PRICE,
    TT_SAMESIDEQUOTE,
    TT_OPPOSITESIDEQUOTE,
};

enum StopTriggerPrintType
{
	STPT_EXCHANGE = 1,
	STPT_MM = 2,
	STPT_ECN = 4,

	STPT_ALL = STPT_ECN * 2 - 1
};

enum OversellHandling
{
    OS_REJECT = 0,
    OS_RESIZE,
    OS_SPLIT,
};

class Position;
class UserOrderDescription;

enum StagingOrderLockStatus
{
	SOLS_AVAILABLE = 'A',
	SOLS_EXCLUDED = 'E',
	SOLS_INCLUDED = 'I',
};

enum SecurityType
{
	ST_STOCK,
	ST_OPTION,
	ST_FUTURE,

	ST_LAST
};

class BUSINESS_API StagingOrder : public Observable
{
public:
	virtual ~StagingOrder(){}
	Position* GetPosition(){return m_position;}
	const Position* GetPosition() const{return m_position;}
	unsigned int GetId() const{return m_id;}
	char GetSide() const{return m_side;}
	bool isValid() const{return m_position != NULL;}
	bool isOrderValidInStagingContext(const Money& orderPrice, unsigned int size, char side) const;
	virtual bool isOverfilled() const{return false;}
	virtual bool isFilled() const{return false;}
	virtual int GetSharesToFill() const{return (int)m_size;}
	virtual void SetSize(unsigned int size){m_size = size;}
	virtual unsigned int GetAllocatedSize() const{return 0;}
	virtual int IncreaseAllocatedSize(Order* order, int increase){return 0;}
    virtual bool isDead() const{return false;}
	virtual void Cancel() = 0;
	virtual bool isBeingCanceled() const{return false;}
	virtual bool isConfirmed() const{return true;}
    virtual bool isCancelConfirmed() const{return false;}
    virtual time32 GetTimeCanceled() const{return 0;}
    virtual time32 GetTimeCancelSent() const{return 0;}

	unsigned char* GetStagingOrderInfo(unsigned short& stagingOrderInfoSize) const;
	unsigned int GetPendingSize() const{return m_pendingSize;}
	virtual void IncrementPendingSize(unsigned int size){}
	virtual unsigned int DecrementPendingSize(unsigned int size){return 0;}
    virtual unsigned int GetCanceledSize() const
	{
		if(isDead())
		{
			unsigned int allocatedSize = GetAllocatedSize();
			return m_size > allocatedSize ? m_size - allocatedSize : 0;
		}
		else
		{
			return 0;
		}
	}
    virtual unsigned int GetRejectedSize() const{return 0;}
    virtual unsigned int GetKilledSize() const{return 0;}
    virtual unsigned int GetExecutedSize() const{return GetAllocatedSize();}
    virtual unsigned int GetRemainingSize() const
	{
		if(isDead())
		{
			return 0;
		}
		else
		{
			unsigned int allocatedSize = GetAllocatedSize();
			return m_size > allocatedSize ? m_size - allocatedSize : 0;
		}
	}
	virtual const char* GetSymbol() const;
    time32 GetTimeEntered() const{return m_timeEntered;}
    time32 GetTimeReceived() const{return m_timeReceived;}
	const Money& GetLimitPrice() const{return m_limitPrice;}
	const char* GetComment() const{return m_comment;}
    time32 GetLastExecutionTime() const{return m_lastExecutionTime;}
    unsigned int GetChronologyOrdinal() const{return m_chronologyOrdinal;}
    virtual const MoneyPrecise& GetExecutedPrice() const = 0;
	unsigned int GetSize() const{return m_size;}
    bool isUserCancelled() const{return m_userCancelled;}
    void SetUserCancelled(){m_userCancelled = true;}

	bool isCurrent() const{return m_current;}
	char GetLockStatus() const{return m_lockStatus;}

	bool SendLockRequest(bool unlock);
	bool SetCurrent(bool current);

	bool DoSetCurrent(bool current);//do not call
	bool SetLockStatus(StagingOrderLockStatus lockStatus);//do not call
protected:
	StagingOrder(unsigned int id, const char* accountName, const char* symbol, char side, const Money& limitPrice, unsigned int size, time32 timeEntered, const char* comment);
	StagingOrder(unsigned int id, Position* position, char side, const Money& limitPrice, unsigned int size, time32 timeEntered, const char* comment);
	Position* m_position;
	unsigned int m_id;
	Money m_limitPrice;
	unsigned int m_size;
	unsigned int m_pendingSize;
	char m_comment[32];
    unsigned int m_chronologyOrdinal;
    time32 m_timeEntered;
	time32 m_timeReceived;
	time32 m_lastExecutionTime;
	char m_side;
    bool m_userCancelled;
	bool m_current;
	char m_lockStatus;
	bool m_lockRequestSent;
};

class BUSINESS_API UserOrderData
{
public:
    virtual ~UserOrderData(){}
    UINT_PTR GetId() const{return m_id;}
protected:
    UserOrderData(UINT_PTR id):m_id(id){}
    UINT_PTR m_id;
};

enum OptionTraderStatus
{
	OTS_DEFAULT,
	OTS_FIRM,
	OTS_CUSTOMER
};

enum OptionOrderStatus
{
	OOS_DEFAULT,
	OOS_OPEN,
	OOS_CLOSE
};

class OrderTradeAllocation;

class BUSINESS_API Order : public TradePack
{
public:
    virtual ~Order();

	const OrderTradeAllocation* GetAllocation() const{return m_allocation;}
	OrderTradeAllocation* GetAllocation(){return m_allocation;}
	OrderTradeAllocation* CreateTradeAllocation(StagingOrder* order);

	const StagingOrder* GetStagingOrder() const;
	virtual unsigned int GetAllocatedSize() const{return 0;}
	virtual int IncreaseAllocatedSize(StagingOrder* order, int increase){return 0;}

	virtual void AddObserver(Observer*){}
	virtual void RemoveObserver(Observer*){}
	virtual void ClearObservers(){}
	virtual const Observable* GetOrderObservable() const{return NULL;}

	virtual unsigned short GetOverstayFeeTime() const{return 0;}
    virtual unsigned int GetOrdinalCreated() const{return GetChronologyOrdinal();}
    virtual time32 GetTimeCanceled() const = 0;
    virtual time32 GetTimeCancelSent() const = 0;
	virtual bool isCancelConfirmed() const{return false;}
    virtual time32 GetTimeStartOverstayFeeCount() const{return GetTimeEntered();}
    time32 GetTimeOfDeath() const{return m_timeOfDeath;}
    unsigned int GetTimeElapsed() const;
    virtual unsigned int GetNysFeeTimer() const{return 0;}
    virtual const char* GetCounterparty() const = 0;
    virtual void RequestStatus() const = 0;
    virtual void Cancel() = 0;
    virtual void CancelReplace(bool replace, bool marketOrder,
		bool institutionalDisclaimerVisible,
		OptionOrderStatus ooStatus,
		OptionTraderStatus otStatus,
		bool keepTimeCreated,
//		bool* delayShortTillUptick = NULL,
		const Money* price = NULL, const Money* stopPrice = NULL,
        unsigned int size = 0, int sizeDifference = 0,
        unsigned int* visibilityMode = NULL, const Money* discretionaryPrice = NULL,
        unsigned int* visibleSize = NULL, unsigned int* tif = NULL,
		const char* destination = NULL,
		const char* preferredMM = NULL,
        unsigned int replacementTifMillisec = 0, bool cumulativeTif = false) = 0;

    virtual bool IncrementPriceSize(bool institutionalDisclaimerVisible,
		OptionOrderStatus ooStatus,
		OptionTraderStatus otStatus,
		bool keepTimeCreated,
//		bool delayShortTillUptick,
		const Money& priceDifference, int sizeDifference = 0, unsigned int replacementTifMillisec = 0) = 0;

    virtual const char* GetFinalDestination() const{return GetDestination();}

	virtual bool isHistorical() const{return false;}
    virtual bool isForeign() const{return false;}

//    virtual time32 GetLastExecutionTime() const{return m_lastExecutionTime;}
    virtual time32 GetLastExecutionTime() const{return 0;}

    virtual bool isStockOrder() const{return false;}
    virtual bool isOrderSupervisor() const{return false;}
    virtual bool isSmartOrder() const{return false;}
    virtual bool isProbeOrder() const{return false;}
    virtual bool isStagingTicket() const{return false;}
    virtual unsigned int GetPendingChildOrderCount() const{return 0;}
	virtual bool isStrategy() const{return false;}
    bool isOrderChild() const{return m_child;}

    virtual const Order* GetOrderSupervisor() const{return m_orderSupervisor;}
    virtual void SetOrderSupervisor(Order* order);

	virtual void* CreateExecutionIterator() const{return NULL;}//returns NULL if there are no executions, otherwise - the iterator handle that can be used in B_GetNextExecution() and is supposed to be destroyed with B_DestroyIterator() when no longer needed.

    virtual bool isNative() const{return false;}
    virtual const Observable* GetAccount() const = 0;
    virtual const Position* FindPosition() const;
	virtual const Position* GetPosition() const{return FindPosition();}
	virtual bool isIdReassigned() const{return true;}
    virtual bool isConfirmed() const = 0;
    virtual bool isOnMarket() const = 0;
    virtual bool isMarketOrder() const = 0;
    virtual bool isAcknoledgedByMarket() const{return false;}
	virtual bool isCancelRejected() const{return false;}
    virtual bool isBulletOrder() const = 0;
    virtual bool isDead() const{return false;}
    virtual unsigned int GetExecutedSize() const = 0;
    virtual unsigned int GetCanceledSize() const = 0;
    virtual unsigned int GetKilledSize() const = 0;
    virtual unsigned int GetRejectedSize() const = 0;
    virtual unsigned int GetRemainingSize() const = 0;
    virtual unsigned int GetVisibleRemainingSize() const = 0;
	virtual unsigned int GetMarketSize() const{return GetVisibleRemainingSize();}
	virtual const Money& GetOrderPrice() const{return *this;}
    virtual const Money& GetExecutedPrice() const = 0;
    virtual const Money* GetStopPrice() const{return NULL;}

    virtual const Money& GetMoneyUsed() const = 0;
//	virtual const Money& GetLevel12Diff() const;
	virtual const Money& GetLevel1Price() const;
	virtual const Money& GetLevel2Price() const;

	virtual const Money* GetPriceOffset() const
    {
        const Order* supervisor = GetOrderSupervisor();
        return supervisor == NULL || supervisor == this ? NULL : supervisor->GetPriceOffset();
    }
    virtual const Money* GetAbsoluteLimitPrice() const
    {
        const Order* supervisor = GetOrderSupervisor();
        return supervisor == NULL || supervisor == this ? NULL : supervisor->GetAbsoluteLimitPrice();
    }
    virtual const Money& GetDiscretionaryPrice() const;
    const Money& GetVisiblePrice() const
    {
        const Money& discPrice = GetDiscretionaryPrice();
        return discPrice.isZero() ? GetOrderPrice() : discPrice;
    }
    virtual bool isToCancel() const= 0;
    virtual bool isBeingCanceled() const= 0;
    virtual unsigned char GetCancelCount() const = 0;
    virtual bool isPassive() const{return false;}
    virtual unsigned int GetPositionSharesTradedToday() const = 0;
    virtual unsigned int GetTotalSharesTradedToday() const = 0;
    virtual const char* GetStopOrderPostDestination() const{return "";}
//    virtual bool isDelayedTillUptick() const{return false;}

    virtual const Money* GetFrom() const{return NULL;}
    virtual StopTriggerType GetStopTriggerType() const{return TT_PRICE;}
    virtual unsigned int GetStopTriggerPrintType() const{return STPT_ALL;}
    virtual const Money* GetStopTriggerPrintOffset() const{return NULL;}
    virtual bool isTrailing() const{return false;}
    virtual bool isStopLossOnly() const{return false;}

    virtual void* CreateInclusionIterator() const{return 0;}

//These functions are only useful for Anvil executable, not for an Anvil extension DLL.
    virtual void SetActive(bool active){}
    virtual bool isActive() const{return true;}
	virtual void Modify(bool replace,
		bool marketOrder,
		bool institutionalDisclaimerVisible,
		OptionOrderStatus ooStatus,
		OptionTraderStatus otStatus,
		const Money& priceOffset,
		const Money* stopPriceOffset,
		const Money* discretionaryDelta,
		unsigned int size,
		unsigned int visibleSize,
		unsigned int visibilityMode,
		bool proactive,
		unsigned int tif,
		const char* destination = NULL,
		const char* preferredMM = NULL){}
	virtual bool isPriceRelative() const{return false;}
    virtual bool isStopPriceRelative() const{return false;}
    virtual bool isAltPriceRelative() const{return false;}
    virtual bool isAltPriceEdit() const{return false;}
	virtual const Money* GetAltPrice() const{return NULL;}
	virtual bool isTifMeaningful() const{return true;}
    virtual bool isBaseOppositeSide() const{return false;}

    virtual bool toString(char* buf, unsigned int bufsize) const;
    virtual char GetOrderClass() const{return 'O';}
    
///////////////////////////////////////////////////////////////////////////////////////////

    bool isUserCancelled() const{return m_userCancelled;}
    void SetUserCancelled(){m_userCancelled = true;}

    virtual BookEntry* GetBookEntry(){return NULL;}
//    virtual unsigned int GetQuoteId() const{return 0;}
//    virtual bool isQuoteUpdatedOnTheMarket() const{return false;}
//    virtual bool isCancelUpdatedOnTheMarket() const{return false;}
    virtual unsigned short GetQuoteToUpdateBookId() const{return 0xFFFF;}
    virtual bool isQuoteUpdated() const{return false;}
	bool isWaitingToBecomeVisibleOnTheMarket() const{return GetQuoteToUpdateBookId() != 0xFFFF && !isQuoteUpdated();}

    virtual unsigned int GetTimeInForce() const{return m_tif;}
    unsigned int GetSentTimeInForce() const{return m_tif;}
    unsigned int GetVisibleShares() const{return m_visibleShares;}
    unsigned int GetVisibility() const{return m_visibility;}
	virtual time32 GetEndTime() const;

    unsigned int GetUserType() const;
    const char* GetUserDescription() const;
    void SetUserDescription(unsigned int type, const char* description = NULL);
    void ResetUserDescription();

    enum OrderType
    {
        OT_REGULAR,
        OT_SIZE,
        OT_ONCLOSE,
        OT_ONOPEN,
        OT_SMART,
//        OT_LATENCY_TEST
    };
    unsigned int GetType() const{return m_type;}
    bool isProactive() const{return m_proactive;}
    bool isPrincipal() const{return m_principalOrAgency;}
    char GetSuperMontageAlgorithm() const{return m_superMontageAlgorithm;}
    unsigned int GetDestinationExchange() const{return m_destinationExchange;}

    void SetUserData(UserOrderData* data){m_userData = data;}
    UserOrderData* GetUserData(){return m_userData;}

	const char* GetCommandName() const{return m_commandName;}
	const char* GetCommandComment() const{return m_commandComment;}

    char GetStockExchange() const{return m_stockExchange;}
    const char* GetPreferredMM() const{return m_preferredMM;}
	bool isISO() const{return m_iso;}
	SecurityType GetSecurityType() const{return m_securityType;}

	const OptionData* GetOptionData() const{return m_optionData;}
	void SetOptionData(const OptionData* optionData);
	bool isCall() const{return m_optionData && m_optionData->isCall();}
	bool isPut() const{return m_optionData && m_optionData->isPut();}
	const Money* GetStrikePrice() const{return m_optionData ? m_optionData->GetStrikePrice() : NULL;}
	const char* GetStrikePriceSymbol() const{return m_optionData ? m_optionData->GetStrikePriceSymbol() : NULL;}
	unsigned int GetSharesPerContract() const{return m_optionData ? m_optionData->GetSharesPerContract() : 0;}
	unsigned int GetExpirationDate() const{return m_optionData ? m_optionData->GetExpirationDate() : 0;}
	unsigned short GetExpirationYear() const{return m_optionData ? m_optionData->GetExpirationYear() : 0;}
	unsigned char GetExpirationMonth() const{return m_optionData ? m_optionData->GetExpirationMonth() : 0;}
	unsigned char GetExpirationDay() const{return m_optionData ? m_optionData->GetExpirationDay() : 0;}
	int GetDaysToExpiration() const{return m_optionData ? m_optionData->GetDaysToExpiration() : 0;}
	const char* GetUnderlierSymbol() const{return m_optionData ? m_optionData->GetUnderlierSymbol() : NULL;}
	OptionOrderStatus GetOptionOrderStatus() const{return (OptionOrderStatus)(m_optionOrderTraderStatus & 15);}
	OptionTraderStatus GetOptionTraderStatus() const{return (OptionTraderStatus)((m_optionOrderTraderStatus >> 4) & 15);}
	void SetOptionOrderTraderStatus(OptionOrderStatus ooStatus, OptionTraderStatus otStatus){m_optionOrderTraderStatus = (unsigned char)((otStatus << 4) | ooStatus);}
	int GetPositionSize() const{return m_positionSize;}
	unsigned __int64 GetCmta() const{return m_cmta;}
protected:
    Order(const char* symbol, char stockExchange,
		const OptionData* optionData,
        unsigned int id,
        unsigned int shares,
        unsigned int visibility,
        unsigned int visibleShares,
        const Money& price,
        char side,
        unsigned int tif,
        const char* destination,
        unsigned int destinationExchange,
		SecurityType securityType,
        bool proactive,
        bool principalOrAgency,
        char superMontageAlgorithm,
        const char* preferredMM,
        unsigned int type,
        unsigned int userType,
        const char* userDescription,
		int positionSize,
		unsigned __int64 cmta,
		const char* commandName = NULL,
		const char* commandComment = NULL,
		Order* orderSupervisor = NULL,
		OptionOrderStatus ooStatus = OOS_DEFAULT,
		OptionTraderStatus otStatus = OTS_DEFAULT,
		bool iso = false);

	OptionData* m_optionData;
    unsigned __int64 m_cmta;

	char m_preferredMM[5];
    char m_superMontageAlgorithm;
    bool m_userCancelled;
	bool m_iso;

	SecurityType m_securityType;

	unsigned int m_visibleShares;
    unsigned int m_tif;

    unsigned int m_destinationExchange;

	unsigned int m_type;
    unsigned int m_visibility;
    UserOrderDescription* m_userDescription;
    UserOrderData* m_userData;
    Order* m_orderSupervisor;
	char* m_commandName;
	char* m_commandComment;
	OrderTradeAllocation* m_allocation;
//    time32 m_lastExecutionTime;
    time32 m_timeOfDeath;
	int m_positionSize;
	unsigned char m_optionOrderTraderStatus;
    char m_stockExchange;
    bool m_proactive;
    bool m_principalOrAgency;
private:
	bool m_child;
    virtual void OrderSubordinateDestroyed(Order* order){}
};

enum StockExchange
{
    EX_NASDAQ,
    EX_SMALLCAP,
    EX_NYSE,
    EX_AMEX,
    EX_ARCA,
    EX_CBOE,
    EX_OTHER,
    EX_OPTION,

    EX_LAST
};

class BUSINESS_API Exchange
{
public:
	Exchange(StockExchange stockExchangeId, RExchange primaryExchange, const char* name, unsigned int tierSize, const char* description = NULL):
		m_tierSize(tierSize),
		m_stockExchangeId(stockExchangeId),
		m_primaryExchange(primaryExchange)
	{
		*(unsigned __int64*)m_name = 0;
		*(unsigned __int64*)m_unknownName = 0;
		memcpy_s(m_name, sizeof(m_name) - 1, name, strlen(name));
		const char* srcCursor = m_name;
		char* dstCursor = m_unknownName;
		unsigned int i;
		char c;
		for(i = 0; i < sizeof(m_unknownName); ++i, ++srcCursor, ++dstCursor)
		{
			c = *srcCursor;
			if(c)
			{
				*dstCursor = c >= 'A' && c <= 'Z' ? c + 0x20 : c;
			}
			else
			{
				break;
			}
		}
		if(!description)
		{
			description = name;
		}
		*(unsigned __int64*)m_description = 0;
		memcpy_s(m_description, sizeof(m_description) - 1, description, strlen(description));

		srcCursor = m_description;
		dstCursor = m_unknownDescription;
		for(i = 0; i < sizeof(m_unknownDescription); ++i, ++srcCursor, ++dstCursor)
		{
			c = *srcCursor;
			if(c)
			{
				*dstCursor = c >= 'A' && c <= 'Z' ? c + 0x20 : c;
			}
			else
			{
				break;
			}
		}
	}

	StockExchange GetId() const{return m_stockExchangeId;}
	RExchange GetPrimaryExchange() const{return m_primaryExchange;}
	const char* GetName() const{return m_name;}
	const char* GetDescription() const{return m_description;}
	const char* GetUnknownName() const{return m_unknownName;}
	const char* GetUnknownDescription() const{return m_unknownDescription;}
	unsigned int GetTierSize() const{return m_tierSize;}
	void SetTierSize(unsigned int tierSize){m_tierSize = tierSize;}
protected:
	unsigned int m_tierSize;
private:
	const StockExchange m_stockExchangeId;
	const RExchange m_primaryExchange;
    char m_name[sizeof(unsigned __int64)];
    char m_unknownName[sizeof(unsigned __int64)];
	char m_description[sizeof(unsigned __int64)];
	char m_unknownDescription[sizeof(unsigned __int64)];
};

class BUSINESS_API StockBase;

class BUSINESS_API Underlier : public Observable
{
public:
	virtual ~Underlier(){}
	const char* GetSymbol() const{return m_symbol;}
	virtual unsigned int GetOptionCount() const{return 0;}
	virtual void* CreateOptionIterator() const{return NULL;}//Use B_GetNextOption to iterate; B_DestroyIterator to destroy
	virtual bool isValid() const{return false;}//The server responded favorably to subscription
	virtual bool isLoaded() const{return false;}//The underlier is completely loaded from the server
	virtual bool isTradable() const{return false;}//There is no restrictions in trading the options for this underlier
	virtual const StockBase* GetUnderlierSecurity() const{return NULL;}
protected:
	Underlier(const char* symbol);
	char m_symbol[LENGTH_SYMBOL];
};

class BUSINESS_API StockBase
{
public:
    virtual ~StockBase(){}
	virtual bool isStockMovement() const{return false;}
    const char* GetSymbol() const{return m_symbol;}
    const char* GetSecurityName() const{return m_securityName;}//This is OPRA for Options
    char GetPrimaryExchange() const{return m_primaryExchange;}
    char GetStockAttributes() const{return m_stockAttributes;}

	unsigned int GetRoundLot() const{return m_roundLot;}

	bool isPrimaryExchangeUnknown() const{return m_primaryExchangeUnknown;}
	virtual bool SetPrimaryExchange(char primaryExchange, bool smallCap)
	{
		if(m_primaryExchangeUnknown && (m_primaryExchange != primaryExchange || m_smallCap != smallCap))
		{
			m_primaryExchange = primaryExchange;
			m_smallCap = smallCap;
			return true;
		}
		return false;
	}
	virtual void Add(Observer*){}
	virtual void Remove(Observer*){}
//Option functions
	virtual const OptionData* GetOptionData() const{return NULL;}
	virtual bool isCall() const{return false;}
	virtual bool isPut() const{return false;}
	virtual const Money* GetStrikePrice() const{return NULL;}
	virtual const char* GetStrikePriceSymbol() const{return NULL;}
//	virtual const char* GetOpra() const{return NULL;}
	virtual unsigned int GetSharesPerContract() const{return 0;}
	virtual unsigned int GetExpirationDate() const{return 0;}
	virtual unsigned short GetExpirationYear() const{return 0;}
	virtual unsigned char GetExpirationMonth() const{return 0;}
	virtual unsigned char GetExpirationDay() const{return 0;}
	int GetDaysToExpiration() const{return 0;}
	virtual const char* GetUnderlierSymbol() const{return NULL;}
	virtual const Underlier* GetUnderlier() const{return NULL;}//Can return NULL if the Option does not belong to any Underlier yet.
	virtual SecurityType GetSecurityType() const{return ST_STOCK;}
//////////////////
	virtual void Resubscribe(){}

    virtual const Observable* GetLevel1Observable() const{return NULL;}
    virtual const Observable* GetLevel2Observable() const{return NULL;}
    virtual const Observable* GetLevel2FilteredObservable() const{return NULL;}
	virtual unsigned int GetLastPrintStatus() const{return TRADE_NONE;}

	bool isTestStock() const{return m_testStock;}

    bool isTradable() const{return m_tradable && !m_foreignNotTradable;}

	bool isForeignNotTradable() const{return m_foreignNotTradable;}

    bool isShortable() const{return (m_stockAttributes & STOCKATTR_SHORTABLE) != 0;}
//    bool isSupersoesable() const{return (m_stockAttributes & STOCKATTR_SUPERSOESABLE) != 0;}
    bool isIpo() const{return (m_stockAttributes & STOCKATTR_IPO) != 0;}
	unsigned char GetSplit() const{return m_split;}
	const Money& GetDividend() const{return m_dividend;}
    bool isUpc11830() const{return (m_stockAttributes & STOCKATTR_UPC11830) != 0;}
	bool isNasdaqStock() const{return NASDAQ == m_primaryExchange;}
	bool isNyseStock() const{return NYSE == m_primaryExchange;}
	bool isAmexStock() const{return AMEX == m_primaryExchange;}
	bool isArcaStock() const{return ARCA == m_primaryExchange;}
	bool isCboeStock() const{return CBOE == m_primaryExchange;}
//    bool isSmallCap() const{return *(unsigned int*)m_securityName == smallCapInt;}
    bool isSmallCap() const{return m_smallCap;}
    static bool isSmallCap(const char* securityName){return *(unsigned int*)securityName == smallCapInt;}
    bool isNNM() const{return *(unsigned int*)m_securityName == NNMInt;}

    char GetNyseImbalanceType() const{return m_nyseImbalanceType;}
	unsigned int GetNyseImbalanceTime() const{return m_nyseImbalanceTime;}
	unsigned int GetNysePreviousImbalanceTime() const{return m_nysePreviousImbalanceTime;}
    int GetNyseImbalance() const{return m_nyseImbalance;}
    int GetNysePreviousImbalance() const{return m_nysePreviousImbalance;}
    unsigned int GetNyseImbalanceMatchedShares() const{return m_nyseImbalanceMatchedShares;}

    char GetNasdaqImbalanceType() const{return m_nasdaqImbalanceType;}
	unsigned int GetNasdaqImbalanceTime() const{return m_nasdaqImbalanceTime;}
	unsigned int GetNasdaqPreviousImbalanceTime() const{return m_nasdaqPreviousImbalanceTime;}
    int GetNasdaqImbalance() const{return m_nasdaqImbalance;}
    int GetNasdaqPreviousImbalance() const{return m_nasdaqPreviousImbalance;}
    unsigned int GetNasdaqImbalanceMatchedShares() const{return m_nasdaqImbalanceMatchedShares;}
    const Money& GetNasdaqImbalanceCurrentReferencePrice() const{return m_nasdaqImbalanceCurrentReferencePrice;}
    const Money& GetNasdaqImbalanceNearIndicativeClearingPrice() const {return m_nasdaqImbalanceNearIndicativeClearingPrice;}
    const Money& GetNasdaqImbalanceFarIndicativeClearingPrice() const {return m_nasdaqImbalanceFarIndicativeClearingPrice;}

    unsigned __int64 GetTradeMoney() const{return m_tradeMoney;}//in thousandths of a dollar
    const Money& GetVwap() const{return m_vwap;}
    const Money& GetVwapNetChange() const{return m_vwapNetChange;}
    const Money& GetVwapPercentChange() const{return m_vwapPercentChange;}

	virtual void* GetLevel2AndBookIterator(bool side) const{return NULL;}

	virtual const Observable* GetMmBookObservable(){return NULL;}
	virtual void AddObserverToMmBook(Observer* observer, bool remove){}
    virtual bool isObserverAttachedToMmBook(Observer* observer) const{return false;}
	virtual void* CreateMmBookIterator() const{return NULL;}//Destroy with B_DestroyIterator; Call B_StartIteration before iterating (you can use it multiple times). Call B_GetNextMmBook to iterate.

	virtual char GetNysQuoteCondition(bool side) const = 0;
    virtual const Money& GetLRP(bool side) const = 0;
	virtual unsigned int GetMmBookCount() const{return 0;}
	virtual bool LoadHistoryChart(unsigned int from, unsigned int to, unsigned int unit = 0) const{return false;}
	virtual bool isValid() const{return false;}
    virtual bool isLoaded() const{return false;}
    virtual bool isChartLoaded() const{return false;}
    virtual bool isHalted() const{return false;}
	virtual const void* GetHistoryChart(unsigned int day) const{return NULL;}
    virtual char GetBidTick() const = 0;
	virtual const Money& GetFirstPrice() const = 0;
    virtual const MoneySize& GetLevel2BestBid() const;
    virtual const MoneySize& GetLevel2BestAsk() const;
    virtual const Money& GetBid() const = 0;
    virtual const Money& GetAsk() const = 0;
    virtual unsigned int GetBidSize() const = 0;
    virtual unsigned int GetAskSize() const = 0;
    virtual const Money& GetIntradayHigh() const = 0;
    virtual const Money& GetIntradayLow() const = 0;
    virtual const Money& GetDayHigh() const = 0;
    virtual const Money& GetDayLow() const = 0;
    virtual unsigned char GetLastTradeExecExchange() const = 0;
    virtual const Money& GetLastTradePrice() const = 0;
    virtual unsigned int GetLastTradeSize() const = 0;
    virtual unsigned int GetLastTradeTime() const = 0;
    virtual unsigned __int64 GetVolume() const = 0;
    virtual unsigned __int64 GetYesterdaysVolume() const = 0;
    virtual const Money& GetYesterdaysLastExchangePrice() const = 0;
    virtual const MoneySize& GetNyseBid() const = 0;
    virtual const MoneySize& GetNyseAsk() const = 0;
    virtual const MoneySize& GetNyseQuote(bool side) const = 0;
    virtual const Money& GetNyseDayHigh() const = 0;
    virtual const Money& GetNyseDayLow() const = 0;
    virtual unsigned __int64 GetNyseVolume() const = 0;
    virtual time32 GetNyseLastTradeTime() const = 0;
	virtual const MoneySize& GetNyseLastTrade() const = 0;

    virtual const Money& GetHistoricalPrice(unsigned int minute, bool nyseOnly) const = 0;
    virtual const Money* GetPriceFromBack(unsigned int minute, bool nyseOnly) const = 0;
    virtual __int64 GetVolumeFromBack(unsigned int minute) const = 0;
    virtual unsigned short GetPointCount() const = 0;
    virtual unsigned int GetAverageVolumeRate(unsigned int minute) const = 0;
    virtual const Money& GetMinuteMinPrice(bool nyseOnly) const = 0;
    virtual const Money& GetMinuteMaxPrice(bool nyseOnly) const = 0;

    virtual unsigned __int64 GetAmexVolume() const = 0;
    virtual time32 GetAmexLastTradeTime() const = 0;
	virtual const MoneySize& GetAmexLastTrade() const = 0;

    virtual unsigned __int64 GetNasdaqVolume() const = 0;
    virtual time32 GetNasdaqLastTradeTime() const = 0;
	virtual const MoneySize& GetNasdaqLastTrade() const = 0;

    unsigned __int64 GetExchangeVolume() const
	{
		switch(m_primaryExchange)
		{
			case NASDAQ:
			return GetNasdaqVolume();

			case NYSE:
			return GetNyseVolume();

			case AMEX:
			return GetAmexVolume();

			default:
			return 0;
		}
	}

	virtual unsigned __int64 GetMsVolume() const = 0;
	virtual void GetLevelPrice(bool side, unsigned int level, Money& price, bool price2DecPlaces = true, unsigned int block = 100) const{price.SetZero();}
    virtual const Money& GetOpenPrice() const = 0;
    virtual const Money& GetClosePrice() const = 0;

	const Money& GetCloseBid() const{return m_closeBid;}
    const Money& GetCloseAsk() const{return m_closeAsk;}
	const Money& GetCloseQuote(bool side) const{return side ? m_closeBid : m_closeAsk;}

	virtual unsigned int GetHistoryPrintCount() const{return 0;}
	virtual unsigned int GetPrintPointCount() const{return 0;}
	virtual unsigned int GetFirstPrintSecond() const{return 0;}
	virtual const MoneySize& GetLastHistoryPrint() const;
	virtual bool LoadHistoryPrints(){return false;}
	virtual bool ClearHistoryPrints(){return false;}
	virtual void* CreateHistoryPrintIterator() const{return NULL;}
	virtual void* CreatePrintChartPointIterator() const{return NULL;}
	virtual bool isHistoryPrintsLoaded() const{return false;}
	virtual bool isHistoryPrintsValid() const{return false;}

	virtual void LogState() const{}//bool price2DecPlaces = false, unsigned int block = 100) const{}
    virtual bool Subscribe(bool enforce = false) const{return false;}

    bool isQuoteFiltered(const BookEntry* be) const;
    static bool isQuoteFiltered(const char* mmid, char primaryExchange, StockExchange stockExchange, bool ecn);
    static bool isQuoteFiltered(unsigned int mmid, char primaryExchange, StockExchange stockExchange, bool ecn);
    
	static bool isQuoteConditionFiltered(char quoteCondition);
	static bool isQuoteConditionFiltered(const BookEntry* be);
//    bool isShortSaleViolation(const Money& price) const;
    static StockExchange GetStockExchange(char primaryExchange, bool smallCap)
    {
        switch(primaryExchange)
        {
            case NASDAQ:
            return smallCap ? EX_SMALLCAP : EX_NASDAQ;

            case NYSE:
            return EX_NYSE;

            case AMEX:
            return EX_AMEX;

            case ARCA:
            return EX_ARCA;

            case CBOE:
            return EX_CBOE;

            default:
            return EX_OTHER;
        }
    }
    StockExchange GetStockExchange() const
    {
        return GetStockExchange(m_primaryExchange, isSmallCap());
    }
    const Money& GetTodaysClosePrice() const{return m_todaysClosePrice;}
    virtual const Money& GetPreMarketIndicatorBid() const = 0;
    virtual const Money& GetPreMarketIndicatorAsk() const = 0;
    virtual unsigned int GetPreMarketIndicatorTime() const = 0;
    virtual unsigned int GetPreMarketIndicatorOrdinal() const = 0;
    const Money& GetExchangeOpenPrice() const{return m_exchangeOpenPrice;}
protected:
    StockBase(const char* symbol);
    void UpdateVwap();
    char m_symbol[LENGTH_SYMBOL + 1];
    char m_securityName[LENGTH_SEQURITYNAME];
	char m_primaryExchange;
	bool m_smallCap;
	bool m_primaryExchangeUnknown;
	char m_stockAttributes;

    char m_nyseImbalanceType;
    unsigned int m_nyseImbalanceTime;
    unsigned int m_nysePreviousImbalanceTime;
    int m_nyseImbalance;
    int m_nysePreviousImbalance;
    unsigned int m_nyseImbalanceMatchedShares;

    char m_nasdaqImbalanceType;
    unsigned int m_nasdaqImbalanceTime;
    unsigned int m_nasdaqPreviousImbalanceTime;
    int m_nasdaqImbalance;
    int m_nasdaqPreviousImbalance;
    unsigned int m_nasdaqImbalanceMatchedShares;

	Money m_nasdaqImbalanceCurrentReferencePrice;
    Money m_nasdaqImbalanceNearIndicativeClearingPrice;
    Money m_nasdaqImbalanceFarIndicativeClearingPrice;

    Money m_todaysClosePrice;
	Money m_exchangeOpenPrice;

    unsigned __int64 m_tradeMoney;
    Money m_vwap;
    Money m_vwapNetChange;
    Money m_vwapPercentChange;
	Money m_dividend;

	Money m_closeBid;
	Money m_closeAsk;

    mutable bool m_tradable;
    bool m_foreignNotTradable;
//    bool m_regSho;
//    bool m_hybrid;
    bool m_testStock;
	unsigned char m_split;

	unsigned int m_roundLot;

    static const unsigned int smallCapInt;
    static const unsigned int NNMInt;
};

class BUSINESS_API StockMovement : public StockBase, public Observable
{
public:
	virtual bool isStockMovement() const{return true;}
	virtual void Add(Observer* o){Observable::Add(o);}
	virtual void Remove(Observer* o){Observable::Remove(o);}
	virtual char GetNysQuoteCondition(bool side) const{return side ? m_nysBidCondition : m_nysAskCondition;}
	virtual const Money& GetLRP(bool side) const{return side ? m_bidLRP : m_askLRP;}
    virtual const Money& GetBid() const{return m_bid;}
    virtual const Money& GetAsk() const{return m_ask;}
    virtual unsigned int GetBidSize() const{return m_bidSize;}
    virtual unsigned int GetAskSize() const{return m_askSize;}
    virtual const Money& GetOpenPrice() const{return m_openPrice;}
    virtual const Money& GetClosePrice() const{return m_closePrice;}
    virtual const Money& GetIntradayHigh() const{return m_intradayHigh;}
    virtual const Money& GetIntradayLow() const{return m_intradayLow;}
    virtual const Money& GetDayHigh() const{return m_dayHigh;}
    virtual const Money& GetDayLow() const{return m_dayLow;}
    virtual unsigned char GetLastTradeExecExchange() const{return m_lastTradeExecExchange;}
    virtual const Money& GetLastTradePrice() const{return m_tradePrice;}
    virtual unsigned int GetLastTradeSize() const{return m_tradeSize;}
    virtual unsigned int GetLastTradeTime() const{return m_tradeTime;}
	char GetLastTradeSaleCondition() const{return m_lastTradeSaleCondition;}
	virtual const Money& GetFirstPrice() const{return m_firstPrice;}
    virtual char GetBidTick() const{return m_bidTick;}
	virtual unsigned __int64 GetMsVolume() const{return m_volume;}
    virtual unsigned __int64 GetVolume() const{return m_volume;}
    virtual unsigned __int64 GetYesterdaysVolume() const{return m_yesterdaysVolume;}
    virtual const Money& GetYesterdaysLastExchangePrice() const{return m_yesterdaysLastExchangePrice;}
    virtual bool isHalted() const{return m_halted;}
    virtual const Money& GetPreMarketIndicatorBid() const{return m_preMarketIndicatorBid;}
    virtual const Money& GetPreMarketIndicatorAsk() const{return m_preMarketIndicatorAsk;}
//    virtual const Money& GetPreMarketIndicatorSpread() const{return m_preMarketIndicatorSpread;}
    virtual unsigned int GetPreMarketIndicatorTime() const{return m_preMarketIndicatorTime;}
    virtual unsigned int GetPreMarketIndicatorOrdinal() const{return m_preMarketIndicatorOrdinal;}

    virtual const MoneySize& GetNyseBid() const{return m_nyseBid;}
    virtual const MoneySize& GetNyseAsk() const{return m_nyseAsk;}
	virtual const MoneySize& GetNyseQuote(bool side) const{return side ? m_nyseBid : m_nyseAsk;}
//    unsigned int GetNyseBidSize() const{return m_nyseBidSize;}
//    unsigned int GetNyseAskSize() const{return m_nyseAskSize;}
    virtual const Money& GetNyseDayHigh() const{return m_nyseDayHigh;}
    virtual const Money& GetNyseDayLow() const{return m_nyseDayLow;}

    virtual unsigned __int64 GetNyseVolume() const{return m_nyseVolume;}
    virtual time32 GetNyseLastTradeTime() const{return m_nyseTradeTime;}
	virtual const MoneySize& GetNyseLastTrade() const{return m_nyseLastTrade;}

    virtual unsigned __int64 GetAmexVolume() const{return m_amexVolume;}
    virtual time32 GetAmexLastTradeTime() const{return m_amexTradeTime;}
	virtual const MoneySize& GetAmexLastTrade() const{return m_amexLastTrade;}

    virtual unsigned __int64 GetNasdaqVolume() const{return m_nasdaqVolume;}
    virtual time32 GetNasdaqLastTradeTime() const{return m_nasdaqTradeTime;}
	virtual const MoneySize& GetNasdaqLastTrade() const{return m_nasdaqLastTrade;}
protected:
    StockMovement(const char* symbol):
        StockBase(symbol),
        m_bidSize(0),
        m_askSize(0),
        m_tradeSize(0),
        m_tradeTime(0),
        m_volume(0),
        m_nyseVolume(0),
        m_amexVolume(0),
        m_nasdaqVolume(0),
        m_yesterdaysVolume(0),
        m_preMarketIndicatorTime(0),
        m_preMarketIndicatorOrdinal(0),
        m_bidTick(' '),
        m_halted(false),
        m_lastTradeExecExchange(ExecExch_ANY),
		m_lastTradeSaleCondition('\0'),
//        m_nyseBidSize(0),
//        m_nyseAskSize(0),
        m_nyseTradeTime(0),
        m_amexTradeTime(0),
        m_nasdaqTradeTime(0),
		m_nysBidCondition('\0'),
		m_nysAskCondition('\0')
        {}
    Money m_bid;
    Money m_ask;
    unsigned int m_bidSize;
    unsigned int m_askSize;
//    Money m_minBid;
//    Money m_maxBid;
    Money m_openPrice;
    Money m_closePrice;
    Money m_tradePrice;
    unsigned int m_tradeSize;
    unsigned int m_tradeTime;
    Money m_intradayHigh;
    Money m_intradayLow;
    Money m_dayHigh;
    Money m_dayLow;
    Money m_firstPrice;
    Money m_preMarketIndicatorBid;
    Money m_preMarketIndicatorAsk;
    unsigned int m_preMarketIndicatorTime;
    unsigned int m_preMarketIndicatorOrdinal;
    unsigned __int64 m_volume;
    unsigned __int64 m_nyseVolume;
    unsigned __int64 m_amexVolume;
    unsigned __int64 m_nasdaqVolume;
    unsigned __int64 m_yesterdaysVolume;
    char m_bidTick;
    bool m_halted;
    unsigned char m_lastTradeExecExchange;
	char m_lastTradeSaleCondition;
    MoneySize m_nyseBid;
    MoneySize m_nyseAsk;
//    unsigned int m_nyseBidSize;
//    unsigned int m_nyseAskSize;
    unsigned int m_nyseTradeTime;
    unsigned int m_amexTradeTime;
    unsigned int m_nasdaqTradeTime;

    Money m_nyseDayHigh;
    Money m_nyseDayLow;
    MoneySize m_nyseLastTrade;
    MoneySize m_amexLastTrade;
    MoneySize m_nasdaqLastTrade;
	Money m_yesterdaysLastExchangePrice;
	Money m_bidLRP;
	Money m_askLRP;
	char m_nysBidCondition;
	char m_nysAskCondition;
};

enum ClosePositionMethod
{
    CPM_SOES_MARKET,
    CPM_SWIPE,
    CPM_SMART_SWIPE,
    CPM_ON_CLOSE,
};

class BUSINESS_API Position : public Observable
{
public:
//    virtual ~Position(){}
    const char* GetSymbol() const{return m_symbol;}
    unsigned int GetBullets() const{return m_bullets;}
    unsigned int GetSharesTraded() const{return m_sharesTradedLong + m_sharesTradedShort;}
    unsigned int GetSharesClosed() const{return GetSharesTraded() - (m_overnightSize < 0 ? -m_overnightSize : m_overnightSize);}
    Money GetMoneyClosed() const{return m_moneyTradedLong + m_moneyTradedShort - m_overnightMoney;}

	virtual SecurityType GetSecurityType() const{return ST_STOCK;}
	virtual bool isCall() const{return false;}
	virtual bool isPut() const{return false;}
	virtual const Money* GetStrikePrice() const{return NULL;}
	virtual const char* GetStrikePriceSymbol() const{return NULL;}
	virtual unsigned int GetSharesPerContract() const{return 0;}
	virtual unsigned int GetExpirationDate() const{return 0;}
	virtual unsigned short GetExpirationYear() const{return 0;}
	virtual unsigned char GetExpirationMonth() const{return 0;}
	virtual unsigned char GetExpirationDay() const{return 0;}
	virtual int GetDaysToExpiration() const{return 0;}
 	virtual const char* GetUnderlierSymbol() const{return NULL;}
	virtual const OptionData* GetOptionData() const{return NULL;}
	virtual int GetOptionSize(bool calls, bool side) const{return 0;}
	virtual int GetOptionPotentialSellSize(bool calls) const{return 0;}
	virtual int GetOptionSharesPending(bool calls, bool side) const{return 0;}
	virtual int GetSizeSupportingCoveredSell(bool calls) const{return (calls ? m_size - m_sharesPendingLong : -m_size - m_sharesPendingShort);}

	virtual const Position* FindOptionPosition(const char* symbol) const{return NULL;}
	virtual const Position* FindCall(const char* symbol) const{return NULL;}
	virtual const Position* FindPut(const char* symbol) const{return NULL;}
	virtual void* CreateCallIterator() const{return NULL;}
	virtual void* CreatePutIterator() const{return NULL;}
	virtual void* CreateOptionPositionIterator(bool side) const{return NULL;}

	unsigned int GetSharesClosedLong() const{return GetSharesTradedLong() - (m_overnightSize < 0 ? 0 : m_overnightSize);}
    unsigned int GetSharesClosedShort() const{return GetSharesTradedShort() - (m_overnightSize < 0 ? -m_overnightSize : 0);}
    unsigned int GetSharesClosedSellLong() const{return GetSharesTradedSellLong();}
    unsigned int GetSharesClosedSellShort() const{return GetSharesTradedSellShort() - (m_overnightSize < 0 ? -m_overnightSize : 0);}
    Money GetMoneyClosedLong() const{return m_overnightSize < 0 ? GetMoneyTradedLong() : GetMoneyTradedLong() - m_overnightMoney;}
    Money GetMoneyClosedShort() const{return m_overnightSize < 0 ? GetMoneyTradedShort() - m_overnightMoney : GetMoneyTradedShort();}
    Money GetMoneyClosedSellLong() const{return GetMoneyTradedSellLong();}
    Money GetMoneyClosedSellShort() const{return m_overnightSize < 0 ? GetMoneyTradedSellShort() - m_overnightMoney : GetMoneyTradedSellShort();}

	virtual bool hasAnyPendingOrders() const{return false;}

	virtual const StagingOrder* FindPendingStagingOrder(unsigned int id) const{return NULL;}
	virtual const StagingOrder* FindStagingOrder(unsigned int id) const{return NULL;}

	virtual const Money& GetAccountCommissionRate() const = 0;

	virtual const StockBase* GetStockBase() const{return NULL;}
	virtual const Observable* GetLevel1() const{return NULL;}
	virtual const Observable* GetLevel2() const{return NULL;}

	virtual bool isStockSubscribedTo() const{return false;}
    virtual unsigned int GetTotalSharesTradedToday() const = 0;
    virtual time32 GetLastExecutionTime() const = 0;
	virtual const Execution* GetLastExecution() const{return NULL;}
    unsigned int GetBidTick() const{return m_bidTick;}
    int GetSharesPendingLong() const{return m_sharesPendingLong;}
    int GetSharesPendingShort() const{return m_sharesPendingShort;}

	StagingOrder* GetCurrentStagingOrder(){return m_currentStagingOrder;}
	const StagingOrder* GetCurrentStagingOrder() const{return m_currentStagingOrder;}
	virtual bool SetCurrentStagingOrder(StagingOrder* stagingOrder) = 0;
	virtual void StagingOrderGotFilled(StagingOrder* stagingOrder) = 0;
	bool isOrderValidInStagingContext(const Money& orderPrice, unsigned int size, char side) const;

	virtual unsigned int GetTotalPendingSize(bool buy) const = 0;
    virtual unsigned int GetPendingOrderCount(bool buy) const = 0;
    virtual unsigned int GetTotalSellSize() const = 0;
    virtual unsigned int GetSellOrderCount() const = 0;

    int GetMaxProjectedSize(int additionalSize) const;
    const Money& GetOversoldMoney() const{return m_oversoldMoney;}
    unsigned int GetOversoldShares() const{return m_oversoldShares;}
    unsigned int GetIrrecoverableOversoldShares() const{return m_irrecoverableOversoldShares;}
    unsigned int GetOversellSecond() const{return m_oversellSecond;}
    unsigned int GetOversellSecondsToExpire() const;

	virtual bool hasOrder(bool side, const char* mmid, const Money& quote) const{return false;}
	virtual const Money* GetBestNonZeroOrderPrice(bool side) const{return NULL;}
	virtual const Money* GetWorstNonZeroOrderPrice(bool side) const{return NULL;}

	virtual void ClosePosition(bool institutionalDisclaimerVisible,
		OptionOrderStatus ooStatus,
		OptionTraderStatus otStatus,
		const char* commandName,
		char side, bool cancelAllOrders,
		unsigned int size,
		ClosePositionMethod method,
		bool close = true) = 0;

    virtual bool isMaxLossExceeded() const = 0;
    virtual bool isMaxOpenLossExceeded() const = 0;

    virtual unsigned int GetSizeShortableOnDownTick(unsigned int shortSellMultiplier = 0) const;
    virtual const StockBase* GetStockHandle() const = 0;
    virtual Observable* GetAccount() const = 0;
    virtual unsigned int GetCountStagingOrdersPendingBuy() const = 0;
    virtual unsigned int GetCountStagingOrdersPendingSell() const = 0;
    virtual unsigned int GetCountOrdersPendingBuy() const = 0;
    virtual unsigned int GetCountOrdersPendingSell() const = 0;
    virtual unsigned int GetCountOrdersPendingMarkedShort() const = 0;
    virtual unsigned int GetCountOrdersUnconfirmedBuy() const = 0;
    virtual unsigned int GetCountOrdersUnconfirmedSell() const = 0;
    virtual unsigned int GetCountOrdersUnconfirmedMarkedShort() const = 0;
    virtual unsigned int GetCountSmartOrdersBuy() const = 0;
    virtual unsigned int GetCountSmartOrdersSell() const = 0;
    virtual unsigned int GetCountSmartDeadOrdersBuy() const = 0;
    virtual unsigned int GetCountSmartDeadOrdersSell() const = 0;

    virtual unsigned int GetCountOrdersPendingAndUnconfirmedBuy() const = 0;
    virtual unsigned int GetCountOrdersPendingAndUnconfirmedSell() const = 0;
    virtual unsigned int GetCountOrdersPendingAndUnconfirmed() const = 0;

    virtual void CancelAllOrders(unsigned int flags, const char* destination, bool includeSmartOrders, bool except, bool includeChildOrders) = 0;
    virtual void CancelAllEcnOrders(unsigned int flags, bool includeSmartOrders, bool except, bool includeChildOrders) = 0;
    virtual Order* CancelLastOrder(unsigned int flags, const char* destination, bool includeSmartOrders, bool includeChildOrders) = 0;
    virtual Order* CancelOldestOrder(unsigned int flags, const char* destination, bool includeSmartOrders, bool includeChildOrders) = 0;
    virtual void CancelAllButLastOrder(unsigned int flags, const char* destination, bool includeSmartOrders, bool includeChildOrders) = 0;
    virtual void CancelAllButOldestOrder(unsigned int flags, const char* destination, bool includeSmartOrders, bool includeChildOrders) = 0;
    virtual void CancelIllegalSellOrders() = 0;
    virtual void CancelPotentialOversellOrders() = 0;
    virtual void CancelAllShortOrders() = 0;
    virtual const Money& GetTodaysClosePrice() const = 0;
	virtual const Money& GetTodaysCloseQuote(bool side) const = 0;

    virtual unsigned int CancelStockBestWorstOrder(unsigned int flags, const char* destination, bool worst, bool includeChildOrders) = 0;
    virtual unsigned int CancelAllButBestWorstOrder(unsigned int flags, const char* destination, bool worst, bool includeChildOrders) = 0;
/*NXDELAY
    virtual unsigned int GetSecondsToNxAvailability(bool side) const = 0;
*/
	virtual unsigned int GetRoundLot() const{return 100;}

	virtual const Order* GetStagingTicket(bool side) const = 0;
    virtual const Order* SetStagingTicket(bool side, unsigned int shares) = 0;

    const Money& GetBulletsMoney() const{return m_bulletsMoney;}

	virtual bool hasInventory() const{return false;}
	virtual const Money& GetMoneyExposed() const{return m_moneyExposed;}//for closed PNL calculations for options.
	virtual const Money& GetMoneyInvested() const{return m_moneyInvested;}
    virtual const Money& GetMaxMoneyInvestedLong() const{return m_maxMoneyInvestedLong;}
    virtual const Money& GetMaxMoneyInvestedShort() const{return m_maxMoneyInvestedShort;}
    virtual const MoneyPrecise& GetPreciseAveragePrice() const{return m_averagePricePrecise;}

    const Money& GetMoneyPendingLong() const{return m_moneyPendingLong;}
    const Money& GetMoneyPendingShort() const{return m_moneyPendingShort;}
/*
	virtual unsigned int GetSharesTradedLong() const{return m_sharesTradedLong;}
    virtual unsigned int GetSharesTradedShort() const{return m_sharesTradedShort;}
    virtual unsigned int GetSharesTradedSellLong() const{return m_sharesTradedSellLong;}
    virtual unsigned int GetSharesTradedSellShort() const{return m_sharesTradedSellShort;}

	virtual const Money& GetMoneyTradedLong() const{return m_moneyTradedLong;}
    virtual const Money& GetMoneyTradedShort() const{return m_moneyTradedShort;}
    virtual const Money& GetMoneyTradedSellLong() const{return m_moneyTradedSellLong;}
    virtual const Money& GetMoneyTradedSellShort() const{return m_moneyTradedSellShort;}
*/
	unsigned int GetSharesTradedLong() const{return m_sharesTradedLong;}
    unsigned int GetSharesTradedShort() const{return m_sharesTradedShort;}
    unsigned int GetSharesTradedSellLong() const{return m_sharesTradedSellLong;}
    unsigned int GetSharesTradedSellShort() const{return m_sharesTradedSellShort;}
    const Money& GetMoneyTradedLong() const{return m_moneyTradedLong;}
    const Money& GetMoneyTradedShort() const{return m_moneyTradedShort;}
    const Money& GetMoneyTradedSellLong() const{return m_moneyTradedSellLong;}
    const Money& GetMoneyTradedSellShort() const{return m_moneyTradedSellShort;}

    virtual const Money& GetAveragePrice() const{return m_averagePrice;}
    virtual const Money& GetClosedPnl() const{return m_closedPnl;}
    virtual const Money& GetOpenPnl() const{return m_openPnl;}
    virtual const Money& GetPriceOpenPnl() const{return m_priceOpenPnl;}
    virtual const Money& GetClosePriceOpenPnl() const{return m_closePriceOpenPnl;}
    virtual const Money& GetCloseQuoteOpenPnl() const{return m_closeQuoteOpenPnl;}
    virtual int GetSize() const{return m_size;}
    virtual unsigned int GetAbsSize() const{return m_size >= 0 ? m_size : -m_size;}
    virtual const MoneyPrecise& GetAverageBuyPrice() const{return m_averageBuyPrice;}
    virtual const MoneyPrecise& GetAverageSellPrice() const{return m_averageSellPrice;}
    virtual const MoneyPrecise& GetAverageSellLongPrice() const{return m_averageSellLongPrice;}
    virtual const MoneyPrecise& GetAverageSellShortPrice() const{return m_averageSellShortPrice;}
    virtual const MoneyPrecise& GetAverageExecPrice() const{return m_averageExecPrice;}

    const MoneySize& GetInsideBid() const{return m_insideBid;}
    const MoneySize& GetInsideAsk() const{return m_insideAsk;}
    const MoneySize& GetLastTrade() const{return m_lastTrade;}
    const Money& GetMyLastPrice() const{return m_myLastPrice;}
    int GetMyLastSize() const{return m_myLastSize;}//negative for sell
    
    Money GetMoneyInUse(const Money& moneyPendingLong, int sharesPendingLong, const Money& moneyPendingShort, int sharesPendingShort) const;
    const Money& GetMoneyInUse() const{return m_moneyInUse;}
/*
    {
        return GetMoneyInUse(m_moneyPendingLong, m_sharesPendingLong, m_moneyPendingShort, m_sharesPendingShort);
    }
*/
    Money GetProjectedMoneyInUseIncrease(bool side, const Money& price, int size) const;

    int GetPhantomSize() const{return m_phantomSize;}

    const Money& GetPhantomAveragePrice() const{return m_phantomAveragePrice;}
    const Money& GetPhantomLastPrice() const{return m_phantomLastPrice;}
    int GetPhantomLastSize() const{return m_phantomLastSize;}//negative for sell

    bool isOvernight() const{return m_overnightSize != 0;}
    int GetOvernightSize() const{return m_overnightSize;}
    const Money& GetOvernightMoney() const{return m_overnightMoney;}

    int GetInstitutionalSize() const{return m_institutionalSize;}
    const Money& GetInstitutionalMoney() const{return m_institutionalMoney;}
    const Money& GetInstitutionalAveragePrice() const{return m_institutionalAveragePrice;}

	virtual int GetInventory() const{return 0;}

	const Money& GetMarketStatusOpenPnlProper() const;
    const Money& GetAveragePriceProper() const{return m_averagePrice;}
    const MoneyPrecise& GetPreciseAveragePriceProper() const{return m_averagePricePrecise;}
    const Money& GetClosedPnlProper() const{return m_closedPnl;}
    const Money& GetOpenPnlProper() const{return m_openPnl;}
    const Money& GetPriceOpenPnlProper() const{return m_priceOpenPnl;}
    const Money& GetClosePriceOpenPnlProper() const{return m_closePriceOpenPnl;}
    const Money& GetCloseQuoteOpenPnlProper() const{return m_closeQuoteOpenPnl;}
    int GetSizeProper() const{return m_size;}
    unsigned int GetAbsSizeProper() const{return m_size >= 0 ? m_size : -m_size;}
    const MoneyPrecise& GetAverageBuyPriceProper() const{return m_averageBuyPrice;}
    const MoneyPrecise& GetAverageSellPriceProper() const{return m_averageSellPrice;}
    const MoneyPrecise& GetAverageSellLongPriceProper() const{return m_averageSellLongPrice;}
    const MoneyPrecise& GetAverageSellShortPriceProper() const{return m_averageSellShortPrice;}
    const MoneyPrecise& GetAverageExecPriceProper() const{return m_averageExecPrice;}

	const Money& GetMoneyExposedProper() const{return m_moneyExposed;}
	const Money& GetMoneyInvestedProper() const{return m_moneyInvested;}
    const Money& GetMaxMoneyInvestedLongProper() const{return m_maxMoneyInvestedLong;}
    const Money& GetMaxMoneyInvestedShortProper() const{return m_maxMoneyInvestedShort;}

	const Money& GetMarketStatusOpenPnlInventory(bool inventory) const;
	const Money& GetAveragePriceInventory(bool inventory) const{return inventory ? GetAveragePrice() : m_averagePrice;}
    const MoneyPrecise& GetPreciseAveragePriceInventory(bool inventory) const{return inventory ? GetPreciseAveragePrice() : m_averagePricePrecise;}
    const Money& GetClosedPnlInventory(bool inventory) const{return inventory ? GetClosedPnl() : m_closedPnl;}
    const Money& GetOpenPnlInventory(bool inventory) const{return inventory ? GetOpenPnl() : m_openPnl;}
    const Money& GetPriceOpenPnlInventory(bool inventory) const{return inventory ? GetPriceOpenPnl() : m_priceOpenPnl;}
    const Money& GetClosePriceOpenPnlInventory(bool inventory) const{return inventory ? GetClosePriceOpenPnl() : m_closePriceOpenPnl;}
    const Money& GetCloseQuoteOpenPnlInventory(bool inventory) const{return inventory ? GetCloseQuoteOpenPnl() : m_closeQuoteOpenPnl;}
    int GetSizeInventory(bool inventory) const{return inventory ? GetSize() : m_size;}
    unsigned int GetAbsSizeInventory(bool inventory) const{return inventory ? GetAbsSize() : m_size >= 0 ? m_size : -m_size;}
    const MoneyPrecise& GetAverageBuyPriceInventory(bool inventory) const{return inventory ? GetAverageBuyPrice() : m_averageBuyPrice;}
    const MoneyPrecise& GetAverageSellPriceInventory(bool inventory) const{return inventory ? GetAverageSellPrice() : m_averageSellPrice;}
    const MoneyPrecise& GetAverageSellLongPriceInventory(bool inventory) const{return inventory ? GetAverageSellLongPrice() : m_averageSellLongPrice;}
    const MoneyPrecise& GetAverageSellShortPriceInventory(bool inventory) const{return inventory ? GetAverageSellShortPrice() : m_averageSellShortPrice;}
    const MoneyPrecise& GetAverageExecPriceInventory(bool inventory) const{return inventory ? GetAverageExecPrice() : m_averageExecPrice;}

	const Money& GetMoneyExposedInventory(bool inventory) const{return inventory ? GetMoneyExposed() : m_moneyExposed;}
	const Money& GetMoneyInvestedInventory(bool inventory) const{return inventory ? GetMoneyInvested() : m_moneyInvested;}
    const Money& GetMaxMoneyInvestedLongInventory(bool inventory) const{return inventory ? GetMaxMoneyInvestedLong() : m_maxMoneyInvestedLong;}
    const Money& GetMaxMoneyInvestedShortInventory(bool inventory) const{return inventory ? GetMaxMoneyInvestedShort() : m_maxMoneyInvestedShort;}

/*
    const Money& GetMoneyPendingLongProper() const{return m_moneyPendingLong;}
    const Money& GetMoneyPendingShortProper() const{return m_moneyPendingShort;}
*/
/*
	unsigned int GetSharesTradedLongProper() const{return m_sharesTradedLong;}
    unsigned int GetSharesTradedShortProper() const{return m_sharesTradedShort;}
    unsigned int GetSharesTradedSellLongProper() const{return m_sharesTradedSellLong;}
    unsigned int GetSharesTradedSellShortProper() const{return m_sharesTradedSellShort;}
    const Money& GetMoneyTradedLongProper() const{return m_moneyTradedLong;}
    const Money& GetMoneyTradedShortProper() const{return m_moneyTradedShort;}
    const Money& GetMoneyTradedSellLongProper() const{return m_moneyTradedSellLong;}
    const Money& GetMoneyTradedSellShortProper() const{return m_moneyTradedSellShort;}
*/
protected:
    Position(const char* symbol);
    char m_symbol[LENGTH_SYMBOL + 1];
    unsigned int m_bullets;
    int m_size;

    int m_sharesPendingLong;
    int m_sharesPendingShort;

    unsigned int m_sharesTradedLong;
    unsigned int m_sharesTradedShort;
    unsigned int m_sharesTradedSellLong;
    unsigned int m_sharesTradedSellShort;
    unsigned int m_bidTick;

    Money m_moneyInUse;
    Money m_moneyExposed;//for closed PNL calculations for options.
    Money m_moneyInvested;
    Money m_maxMoneyInvestedLong;
    Money m_maxMoneyInvestedShort;
    Money m_moneyPendingLong;
    Money m_moneyPendingShort;
    MoneyPrecise m_averagePricePrecise;
    Money m_averagePrice;
    Money m_closedPnl;
    Money m_openPnl;
    Money m_priceOpenPnl;
    Money m_closePriceOpenPnl;
    Money m_closeQuoteOpenPnl;
    Money m_moneyTradedLong;
    Money m_moneyTradedShort;
    Money m_moneyTradedSellLong;
    Money m_moneyTradedSellShort;

    MoneySize m_insideBid;
    MoneySize m_insideAsk;
    MoneySize m_lastTrade;
    MoneyPrecise m_averageExecPrice;
    MoneyPrecise m_averageBuyPrice;
    MoneyPrecise m_averageSellPrice;
    MoneyPrecise m_averageSellLongPrice;
    MoneyPrecise m_averageSellShortPrice;

    int m_phantomSize;
    int m_overnightSize;
    Money m_overnightMoney;

    Money m_myLastPrice;
    int m_myLastSize;//negative for sell

    Money m_phantomMoneyInvested;
    Money m_phantomAveragePrice;
    Money m_phantomLastPrice;
    int m_phantomLastSize;//negative for sell

    Money m_bulletsMoney;

    Money m_oversoldMoney;
    unsigned int m_oversoldShares;
    unsigned int m_oversellSecond;
    unsigned int m_irrecoverableOversoldShares;

    int m_institutionalSize;
    Money m_institutionalMoney;
    Money m_institutionalAveragePrice;

	StagingOrder* m_currentStagingOrder;
};

class BUSINESS_API Quote
{
public:
    Quote(const char* mm, const Money& money) : m_money(money)
    {
        strncpy_s(m_mm, sizeof(m_mm), mm, sizeof(m_mm) - 1);
        m_mm[sizeof(m_mm) - 1] = '\0';
    }
    Quote()
    {
        *m_mm = '\0';
    }
    virtual ~Quote(){}
    void Init(const char* mm, const Money& money)
    {
        strncpy_s(m_mm, sizeof(m_mm), mm, sizeof(m_mm) - 1);
        m_mm[sizeof(m_mm) - 1] = '\0';
        m_money = money;
    }
    void Init(const Quote& q)
    {
        strncpy_s(m_mm, sizeof(m_mm), q.m_mm, sizeof(m_mm) - 1);
        m_mm[sizeof(m_mm) - 1] = '\0';
        m_money = q.m_money;
    }
    void Reset(){*m_mm = '\0'; m_money.SetZero();}
    const char* GetMM() const{return m_mm;}
    const Money& GetMoney() const{return m_money;}
    bool operator<(const Quote& q) const
    {
        int res = strcmp(m_mm, q.m_mm);
        return res == 0 ? m_money < q.m_money : res < 0;
    }
    bool operator==(const Quote& q) const
    {
        return strcmp(m_mm, q.m_mm) == 0 && m_money == q.m_money;
    }
    bool operator!=(const Quote& q) const
    {
        return !operator==(q);
    }

protected:
    char m_mm[LENGTH_SYMBOL];
    Money m_money;
};

class BUSINESS_API QuoteSize : public Quote
{
public:
    QuoteSize(const char* mm, const Money& money, unsigned int size) : Quote(mm, money), m_size(size){}
    QuoteSize(const char* mm, const MoneySize& moneySize) : Quote(mm, moneySize), m_size(moneySize.GetSize()){}
    QuoteSize():m_size(0){}
    void Init(const char* mm, const Money& money, unsigned int size){Quote::Init(mm, money); m_size = size;}
    void Init(const Quote& q, unsigned int size){Quote::Init(q); m_size = size;}
    void Reset(){Quote::Reset(); m_size = 0;}
    unsigned int GetSize() const{return m_size;}
    void SetSize(unsigned int size){m_size = size;}
protected:
    unsigned int m_size;
};


class BUSINESS_API StockCalc
{
public:
    virtual ~StockCalc(){}
    bool isBid() const{return m_side;}
    virtual const StockBase* GetStockHandle() const = 0;
    virtual const char* GetStockSymbol() const = 0;
    virtual unsigned int GetLevelSize(unsigned int levelFrom, unsigned int levelTo, unsigned int& participantCount) const = 0;
    virtual unsigned int GetLevelEcnSize(unsigned int levelFrom, unsigned int levelTo, unsigned int& participantCount) const = 0;
    virtual const Money* GetLevelPrice(unsigned int level) const = 0;
    virtual unsigned int GetSizeBetweenPrices(const Money& throughFrom, const Money& throughTo, unsigned int& participantCount, unsigned int startFromLevel = 0) const = 0;
    virtual unsigned int GetEcnSizeBetweenPrices(const Money& throughFrom, const Money& throughTo, unsigned int& participantCount, unsigned int startFromLevel = 0) const = 0;
	virtual void GetSizeBetweenMultiplePrices(unsigned int priceCount, const Money* moneyArray, unsigned int* sizeArray, unsigned int* participantCountArray = NULL, unsigned int startFromLevel = 0) const = 0;
	virtual void GetEcnSizeBetweenMultiplePrices(unsigned int priceCount, const Money* moneyArray, unsigned int* sizeArray, unsigned int* participantCountArray = NULL, unsigned int startFromLevel = 0) const = 0;

    virtual bool GetMmidFirstQuote(const char* mmid, unsigned int& level, MoneySize& quote) const = 0;
    virtual unsigned int GetMmidLevelSize(const char* mmid, unsigned int levelFrom, unsigned int levelTo, unsigned int& participantCount) const = 0;
    virtual unsigned int GetMmidSizeBetweenPrices(const char* mmid, const Money& throughFrom, const Money& throughTo, unsigned int& participantCount, unsigned int startFromLevel = 0) const = 0;

    virtual void* CreateLevelIterator() const = 0;

    virtual bool AddMMP(const char* mmid){return false;}
    virtual bool RemoveMMP(const char* mmid){return false;}
	virtual bool SetStockCalcParticipants(const char* mmids){return false;}//"XX\0YY\0ZZ\0\0"
//	virtual bool isMmidParticipating(const char* mmid){return true;}
	virtual bool isMmidParticipating(unsigned int mmid) const{return true;}

    virtual bool AddTickMonitor(unsigned int seconds){return false;}
    virtual bool RemoveTickMonitor(unsigned int seconds){return false;}
    virtual bool ReplaceTickMonitor(unsigned int oldSeconds, unsigned int newSeconds){return false;}
    virtual unsigned int ClearTickMonitors(){return 0;}

	virtual void GetTicks(unsigned int seconds, unsigned int& upTicks, unsigned int& downTicks, Money* netChange = NULL, Money* minQuote = NULL, Money* maxQuote = NULL) const
    {
        upTicks = downTicks = 0;
        if(netChange)netChange->SetZero();
        if(minQuote)minQuote->SetZero();
        if(maxQuote)maxQuote->SetZero();
    }
//Returns the size of a PriceLevel participant that is the highest among all participants of the PriceLevels from levelFrom to levelTo
    virtual unsigned int GetLevelParticipantMaxSize(unsigned int levelFrom, unsigned int levelTo, unsigned int& participantCount) const = 0;
//Returns the size of a PriceLevel participant that is the highest among all participants of the PriceLevels between prices throughFrom and throughTo
    virtual unsigned int GetParticipantMaxSizeBetweenPrices(const Money& throughFrom, const Money& throughTo, unsigned int& participantCount, unsigned int startFromLevel = 0) const = 0;
protected:
    StockCalc(bool side, unsigned int minSize = 100):m_side(side), m_minSize(minSize > 100 ? 100 : minSize){}
    bool m_side;
    unsigned int m_minSize;
};


enum AdditionalInfoIds
{
    M_AI_LEVEL2_QUOTE =             40001,
    M_AI_BOOK_QUOTE,
    M_AI_EXECUTION,
    M_AI_ORDER,
    M_AI_ORDER_USER_DESCRIPTION,
    M_AI_POSITION,
    M_AI_PRINT_STATUS,
    M_AI_INDEX_DIRECTION,
    M_AI_CHART_POINT,
    M_AI_OBSERVABLE_PTR,
    M_AI_STOCK_MOVEMENT,
	M_AI_STOCK_BASE,
	M_AI_STOCK_EXCHANGE_CHANGED,
//    M_AI_BOOK_EXEC_PRICE_SIZE,
    M_AI_BOOK_EXECUTION,
    M_AI_PRINT,
    M_AI_ACCOUNT_CHANGE,
	M_AI_NEW_MM_BOOK,
	M_AI_MM_BOOK_FLUSH,
	M_AI_NYSE_BOOK,

    M_POSITION_INVESTMENTCHANGE =   41001,
    M_POSITION_OVERNIGHTCHANGE,
    M_POSITION_PENDINGMONEYCHANGE,
    M_POSITION_INSTITUTIONAL_ADDED,
    M_POSITION_PENDINGORDERSCOUNTCHANGE,
    M_POSITION_BULLETSCHANGE,
    M_POSITION_PHANTOMCHANGE,
    M_POSITION_NEW,
    M_POSITION_DELETED,
	M_POSITION_INVENTORY_CHANGE,
	M_POSITION_INVENTORY_POP,

	M_POSITION_STOCK_SUBSCRIPTION_CHANGE,
/*NXDELAY
    M_POSITION_NXSTATUSCHANGE,
*/
    M_SMART_ORDER_ACTIVATED,

    M_CORRECTION_VOLUME,
    M_CORRECTION_CLOSE_PRICE,

	M_MARKET_CLOSE_QUOTE,

	M_MS_ECN_TRADE,
	M_MS_NON_ECN_TRADE,

    M_MS_L2_LARGE_QUOTE,

    M_TEXT,

    M_NEXT_MINUTE,
    M_NEXT_DAY,

    M_OVERSELL_EXPIRATION,
    M_OVERSELL_COVERED_BY_MKT_ORDER,

    M_SMARTORDER_ADD,
    M_SMARTORDER_MODIFY,
    M_SMARTORDER_REMOVE,
    M_SMARTORDER_EXECUTION,
    M_SMARTORDER_CANCEL,

    M_ORDER_REROUTE_ADD,
    M_ORDER_REROUTE_REMOVE,

    M_ORDER_SUPERVISOR_SET,

    M_ORDER_NYS_FEE_WARNING,
    M_ORDER_NYS_FEE_IMPOSITION,

    M_INDEX_NEW,
    M_CURRENT_ACCOUNT_CHANGE,
    M_MS_NASDAQ_VOLUME,
    M_MS_NYSE_VOLUME,
    M_MS_AMEX_VOLUME,
    M_MS_ARCA_VOLUME,
    M_MS_CBOE_VOLUME,

    M_ORDER_DELETED,
    M_ACCOUNT_CONSTRAINTS_CHANGE,
    M_ACCOUNT_DESTROYED,
    M_ACCOUNT_DISCONNECTED,
    M_ACCOUNT_EXECUTION_LOADING_DONE,

    M_STOCKS_RESORTED,
    M_QUOTE_FILTERING_CHANGED,

    M_MS_LATENCY,

    M_STOCK_UNSUBSCRIBED,
    M_STOCK_DELETED,
    M_OPTION_UNSUBSCRIBED,
    M_OPTION_DELETED,
	M_UNDERLIER_DELETED,

//    M_MS_FYI,

    M_USER_MARKET_STATUS_CAHNGE,

    M_CUSTOM_RECORD_UPDATE,
    M_CUSTOM_RECORD_DELETE,
    M_CUSTOM_RECORD_ADD,
    M_CUSTOM_RECORD_CLEAR,
    M_CUSTOM_RECORD_NEW_ACCOUNT_OBSERVABLE,
    M_CUSTOM_RECORD_REPAINT,
    M_CUSTOM_RECORD_UPDATE_COLUMN_TITLES,

    M_CLOSE_PNL_OFFSET_CHANGE,
	M_COMMISSION_RATE_CHANGE,
	M_WARNING_MAX_OPEN_LOSS_EXCEEDED,
	M_WARNING_MAX_MARKED_NET_LOSS_EXCEEDED,

//    M_ACCOUNT_MAX_LOSS_EXCEEDED,
//    M_POSITION_MAX_LOSS_EXCEEDED,
    M_POSITION_OPEN_MAX_LOSS_EXCEEDED,
	M_POSITION_STAGING_ORDER,
	M_AI_STAGING_ORDER,
	M_STAGING_ORDER_CURRENT,
	M_STAGING_ORDER_LOCK_SENT,
    M_STAGING_ORDER_DELETED,
	M_STAGING_ORDER_PENDING_SIZE_INCREMENT,
	M_STAGING_ORDER_ALLOCATED,

	M_OPEN_PRICE_CHANGE,
    M_TODAYS_CLOSE_PRICE_CHANGE,
    M_PRE_MARKET_INDICATOR,

    M_BOOL,

//    M_TOTAL_VIEW,

    M_MMACTIVE_STOCK_CHANGED,

    M_NYS_FEE_SHORTEN_ORDER_ADDREMOVE,
    M_NYS_FEE_WARNING_ORDER_ADDREMOVE,
    M_NYS_FEE_RENEW_ORDER_ADDREMOVE,

	M_ORDER_SET_MARKET_SIZE,
	M_ORDER_UPDATE_MARKET_SIZE,
	M_ORDER_REMOVE_MARKET_SIZE,

	M_ACQUIRED_INVENTORY,

	M_BOOK_INTEGRATION_CHANGED,
	M_DISPLAY_LEVEL2_ECN_CHANGED,

	M_HISTORY_PRINTS_CLEARED,

	M_MARKET_DATA_PRINT,//Used for debugging only

	M_SYSTEM_IDLE,
	M_IDLE_INTERRUPTED,
	M_IDLE_RESUMED,

	M_MARKET_CLOSE_SECOND_CHANGED,
};

enum IdleInterruptionCode
{
	II_SIZEMOVE,
	II_MENULOOP,
	II_SCROLL,
	II_DIALOG,
};

class BUSINESS_API MsgMarketDataPrint : public Message//Used for debugging only
{
public:
	MsgMarketDataPrint(unsigned int printSize, const Money& price, unsigned int status, char executionExchange, char saleCondition):
		Message(M_MARKET_DATA_PRINT, sizeof(MsgMarketDataPrint)),
		m_printSize(printSize),
		m_status(status),
		m_price(price),
		m_executionExchange(executionExchange),
		m_saleCondition(saleCondition)
	{}
	unsigned int m_printSize;
	unsigned int m_status;
	Money m_price;
	char m_executionExchange;
	char m_saleCondition;
};

class BUSINESS_API MsgSystemIdle : public Message
{
public:
	MsgSystemIdle(LONG count):Message(M_SYSTEM_IDLE, sizeof(MsgSystemIdle)), m_count(count){}
	LONG m_count;
};
/*
class BUSINESS_API MsgExtensionIdle : public Message
{
public:
	MsgExtensionIdle(LONG count):Message(M_EXTENSION_IDLE, sizeof(MsgExtensionIdle)), m_count(count){}
	LONG m_count;
};
*/
class BUSINESS_API MsgIdleInterrupted : public Message
{
public:
	MsgIdleInterrupted(unsigned int code):Message(M_IDLE_INTERRUPTED, sizeof(MsgIdleInterrupted)), m_code(code){}
	unsigned int m_code;
};

class BUSINESS_API MsgIdleResumed : public Message
{
public:
	MsgIdleResumed(unsigned int code):Message(M_IDLE_RESUMED, sizeof(MsgIdleResumed)), m_code(code){}
	unsigned int m_code;
};

class BUSINESS_API MsgWarningMaxOpenLossExceeded : public Message
{
public:
	MsgWarningMaxOpenLossExceeded():Message(M_WARNING_MAX_OPEN_LOSS_EXCEEDED, sizeof(MsgWarningMaxOpenLossExceeded)){}
};

class BUSINESS_API MsgWarningMaxMarkedNetLossExceeded : public Message
{
public:
	MsgWarningMaxMarkedNetLossExceeded():Message(M_WARNING_MAX_MARKED_NET_LOSS_EXCEEDED, sizeof(MsgWarningMaxMarkedNetLossExceeded)){}
};

class BUSINESS_API MsgMmActiveStockChanged : public Message
{
public:
    MsgMmActiveStockChanged(const StockBase* newStockHandle, const StockBase* oldStockHandle):
        Message(M_MMACTIVE_STOCK_CHANGED, sizeof(MsgMmActiveStockChanged)),
        m_newStockHandle(newStockHandle),
        m_oldStockHandle(oldStockHandle)
    {
    }
    const StockBase* m_newStockHandle;
    const StockBase* m_oldStockHandle;
};
/*
class BUSINESS_API MsgTotalView : public Message
{
public:
    MsgTotalView(unsigned short bookId, const char* mmid):
        Message(M_TOTAL_VIEW, sizeof(MsgTotalView)),
        m_bookId(bookId)
    {
        strncpy_s(m_mmid, sizeof(m_mmid), mmid, sizeof(m_mmid) - 1);
        m_mmid[sizeof(m_mmid) - 1] = '\0';
    }
    unsigned short m_bookId;
    char m_mmid[LENGTH_SYMBOL];
};
*/
class BUSINESS_API MsgBool : public Message
{
public:
    MsgBool(bool value):
        Message(M_BOOL, sizeof(MsgBool)),
        m_value(value){}
    bool m_value;
};

class BUSINESS_API MsgMmBookFlush : public Message
{
public:
	MsgMmBookFlush(const void* bookHolder, unsigned int mmid, unsigned short bookId):
        Message(M_AI_MM_BOOK_FLUSH, sizeof(MsgMmBookFlush)),
		m_bookHolder(bookHolder),
		m_mmid(mmid),
		m_bookId(bookId)
	{
	}
	const void* m_bookHolder;
	unsigned int m_mmid;//0 for ALL BOOKS
	unsigned short m_bookId;//0xFFFF if not an ECN book
};

class BUSINESS_API MsgNewMmBook : public Message
{
public:
	MsgNewMmBook(const void* bookHolder, unsigned int mmid):
        Message(M_AI_NEW_MM_BOOK, sizeof(MsgNewMmBook)),
		m_bookHolder(bookHolder),
		m_mmid(mmid)
	{
	}
	const void* m_bookHolder;
	unsigned int m_mmid;
};

//class BUSINESS_API MsgBookExecPriceSize : public Message
class BUSINESS_API MsgTransaction : public Message
{
public:
//    Money m_price;
//    unsigned int m_size;
//    char m_mmid[LENGTH_SYMBOL];
//	unsigned short m_bookId;
	const Transaction* m_transaction;
//	const void* m_mmBookHolder;//NULL fof ECN executions
protected:
//    MsgBookExecPriceSize(const Money& price, unsigned int size, const char* mmid, unsigned short bookId, const Transaction* transaction, const void* mmBookHolder = NULL):
    MsgTransaction(unsigned short type, unsigned short size, const Transaction* transaction):
//        Message(M_AI_BOOK_EXEC_PRICE_SIZE, sizeof(MsgBookExecPriceSize)),
        Message(type, size),
//        m_price(price),
//        m_size(size),
//		m_bookId(bookId),
		m_transaction(transaction)
//		m_mmBookHolder(mmBookHolder)
	{
//        strncpy_s(m_mmid, sizeof(m_mmid), mmid, sizeof(m_mmid) - 1);
//        m_mmid[sizeof(m_mmid) - 1] = '\0';
	}
};

class BUSINESS_API MsgTransactionInside : public MsgTransaction
{
public:
    MoneySize m_bestBid;
    MoneySize m_bestAsk;
protected:
    MsgTransactionInside(unsigned short type, unsigned short size,
		const Transaction* transaction,
		const MoneySize& bestBid,
		const MoneySize& bestAsk):
        MsgTransaction(type, size, transaction),
		m_bestBid(bestBid),
		m_bestAsk(bestAsk)
		{}
};

class BUSINESS_API MsgPrint : public MsgTransaction
{
public:
    MsgPrint(const Transaction* transaction, unsigned int printStatus):
        MsgTransaction(M_AI_PRINT, sizeof(MsgPrint), transaction),
		m_printStatus(printStatus)
	{
	}
	unsigned int m_printStatus;
};

class BUSINESS_API MsgBookExecution : public MsgTransactionInside
{
public:
    MsgBookExecution(const Transaction* transaction,
		unsigned short bookId,
		const void* mmBookHolder,
		const MoneySize& bid,
		const MoneySize& ask):
        MsgTransactionInside(M_AI_BOOK_EXECUTION, sizeof(MsgBookExecution), transaction, bid, ask),
		m_bookId(bookId),
		m_mmBookHolder(mmBookHolder)
	{
	}
	unsigned short m_bookId;
	const void* m_mmBookHolder;//NULL fof ECN executions
};

class BUSINESS_API MsgTodaysClosePriceChange : public Message
{
public:
    MsgTodaysClosePriceChange(const Money& price):
        Message(M_TODAYS_CLOSE_PRICE_CHANGE, sizeof(MsgTodaysClosePriceChange)),
        m_todaysClosePrice(price){}
    Money m_todaysClosePrice;
};

class BUSINESS_API MsgOpenPriceChange : public Message
{
public:
    MsgOpenPriceChange(const Money& price):
        Message(M_OPEN_PRICE_CHANGE, sizeof(MsgOpenPriceChange)),
        m_openPrice(price){}
    Money m_openPrice;
};

class BUSINESS_API MsgPreMarketIndicator : public Message
{
public:
    MsgPreMarketIndicator(const Money& bid, const Money& ask):
        Message(M_PRE_MARKET_INDICATOR, sizeof(MsgPreMarketIndicator)),
        m_preMarketIndicatorBid(bid),
        m_preMarketIndicatorAsk(ask){}
    Money m_preMarketIndicatorBid;
    Money m_preMarketIndicatorAsk;
};

class BUSINESS_API MsgCommissionRateChange : public Message
{
public:
    MsgCommissionRateChange():Message(M_COMMISSION_RATE_CHANGE, sizeof(MsgCommissionRateChange)){}
};

class BUSINESS_API MsgClosePnlOffsetChange : public Message
{
public:
    MsgClosePnlOffsetChange():Message(M_CLOSE_PNL_OFFSET_CHANGE, sizeof(MsgClosePnlOffsetChange)){}
};

class BUSINESS_API MsgCustomRecordUpdate : public Message
{
public:
    MsgCustomRecordUpdate():Message(M_CUSTOM_RECORD_UPDATE, sizeof(MsgCustomRecordUpdate)){}
};

class BUSINESS_API MsgCustomRecordDelete : public Message
{
public:
    MsgCustomRecordDelete():Message(M_CUSTOM_RECORD_DELETE, sizeof(MsgCustomRecordDelete)){}
};

class BUSINESS_API MsgCustomRecordClear : public Message
{
public:
    MsgCustomRecordClear():Message(M_CUSTOM_RECORD_CLEAR, sizeof(MsgCustomRecordClear)){}
};

class BUSINESS_API MsgCustomRecordRepaint : public Message
{
public:
    MsgCustomRecordRepaint():Message(M_CUSTOM_RECORD_REPAINT, sizeof(MsgCustomRecordRepaint)){}
};

class BUSINESS_API MsgCustomRecordUpdateColumnTitles : public Message
{
public:
    MsgCustomRecordUpdateColumnTitles():Message(M_CUSTOM_RECORD_UPDATE_COLUMN_TITLES, sizeof(MsgCustomRecordUpdateColumnTitles)){}
};

class BUSINESS_API MsgCustomRecordAdd : public Message
{
public:
    MsgCustomRecordAdd(Observable* object):Message(M_CUSTOM_RECORD_ADD, sizeof(MsgCustomRecordAdd)), m_object(object){}
    Observable* m_object;
};

class BUSINESS_API MsgCustomRecordNewAccountObservable : public Message
{
public:
    MsgCustomRecordNewAccountObservable(Observable* account, Observable* object, HINSTANCE dllInstance):
        Message(M_CUSTOM_RECORD_NEW_ACCOUNT_OBSERVABLE, sizeof(MsgCustomRecordNewAccountObservable)),
        m_account(account),
        m_object(object),
        m_dllInstance(dllInstance){}
    Observable* m_account;
    Observable* m_object;
    HINSTANCE m_dllInstance;
};

class BUSINESS_API MsgUserMarketStatusChange : public Message
{
public:
    MsgUserMarketStatusChange(unsigned int marketStatus):Message(M_USER_MARKET_STATUS_CAHNGE, sizeof(MsgUserMarketStatusChange)), m_marketStatus(marketStatus){}
	unsigned int m_marketStatus;
};

class BUSINESS_API MsgLatency : public Message
{
public:
    MsgLatency(void* id, unsigned short delay, unsigned char latencyType):
        Message(M_MS_LATENCY, sizeof(MsgLatency)),
        m_id(id),
        m_delay(delay),
        m_latencyType(latencyType)
        {}
    enum LatencyType
    {
        LT_REMOTE,
        LT_MASTER,
        LT_DESTINATION,
        LT_CANCEL,
        LT_QUOTE_ADD,
        LT_QUOTE_REMOVE,

        LT_DONE = 255
    };
    void* m_id;
    unsigned short m_delay;
    unsigned char m_latencyType;
};


class BUSINESS_API MsgExecutionLoadingDone : public Message
{
public:
    MsgExecutionLoadingDone():Message(M_ACCOUNT_EXECUTION_LOADING_DONE, sizeof(MsgExecutionLoadingDone)){}
};

class BUSINESS_API MsgStocksResorted : public Message
{
public:
    MsgStocksResorted():Message(M_STOCKS_RESORTED, sizeof(MsgStocksResorted)){}
};

class BUSINESS_API MsgQuoteFilteringChanged : public Message
{
public:
    MsgQuoteFilteringChanged():Message(M_QUOTE_FILTERING_CHANGED, sizeof(MsgQuoteFilteringChanged)){}
};

class BUSINESS_API MsgService : public Message
{
public:
	MsgService(bool startes):Message(MSGID_SERVICE, sizeof(MsgService)),m_startes(startes){}
    bool m_startes;
};

class BUSINESS_API MsgAccount : public Message
{
public:
    Observable* m_account;
protected:
    MsgAccount(Observable* account, unsigned int type, unsigned int size):
        Message(type, size),
        m_account(account){}
};

class BUSINESS_API MsgAccountChange : public MsgAccount
{
public:
    MsgAccountChange(Observable* account) : MsgAccount(account, M_AI_ACCOUNT_CHANGE, sizeof(MsgAccountChange)){}
};

class BUSINESS_API MsgAccountDestroyed : public MsgAccount
{
public:
    MsgAccountDestroyed(Observable* account):MsgAccount(account, M_ACCOUNT_DESTROYED, sizeof(MsgAccountDestroyed)){}
};

class BUSINESS_API MsgAccountDisconnected : public MsgAccount
{
public:
    MsgAccountDisconnected(Observable* account):MsgAccount(account, M_ACCOUNT_DISCONNECTED, sizeof(MsgAccountDestroyed)){}
};

class BUSINESS_API MsgAccountConstraintsChange : public Message
{
public:
    MsgAccountConstraintsChange(unsigned int changeFlags):
        Message(M_ACCOUNT_CONSTRAINTS_CHANGE, sizeof(MsgAccountConstraintsChange)),
        m_changeFlags(changeFlags){}
    unsigned int m_changeFlags;
};

class BUSINESS_API MsgCurrentAccountChange : public Message
{
public:
    MsgCurrentAccountChange(Observable* accountOld, Observable* accountNew):
        Message(M_CURRENT_ACCOUNT_CHANGE, sizeof(MsgCurrentAccountChange)),
        m_accountOld(accountOld),
        m_accountNew(accountNew){}
    Observable* m_accountOld;
    Observable* m_accountNew;
};

class BUSINESS_API MsgNewIndex : public Message
{
public:
    MsgNewIndex(MarketIndex* index) : Message(M_INDEX_NEW, sizeof(MsgNewIndex)), m_index(index){}
    MarketIndex* m_index;
};

class BUSINESS_API MsgOrderChange : public Message
{
public:
    Order* m_order;
protected:
    MsgOrderChange(Order* order, unsigned int type, unsigned int size):
        Message(type, size),
        m_order(order){}
};

class BUSINESS_API MsgOrderAddRemove : public MsgOrderChange
{
public:
    bool m_add;
protected:
    MsgOrderAddRemove(Order* order, bool add, unsigned int type, unsigned int size):
		MsgOrderChange(order, type, size),
		m_add(add){}
};

class BUSINESS_API MsgNysFeeShortenOrderAddRemove : public MsgOrderAddRemove
{
public:
    MsgNysFeeShortenOrderAddRemove(Order* order, bool add) : MsgOrderAddRemove(order, add, M_NYS_FEE_SHORTEN_ORDER_ADDREMOVE, sizeof(MsgNysFeeShortenOrderAddRemove)){}
};

class BUSINESS_API MsgNysFeeWarningOrderAddRemove : public MsgOrderAddRemove
{
public:
    MsgNysFeeWarningOrderAddRemove(Order* order, bool add) : MsgOrderAddRemove(order, add, M_NYS_FEE_WARNING_ORDER_ADDREMOVE, sizeof(MsgNysFeeWarningOrderAddRemove)){}
};

class BUSINESS_API MsgNysFeeRenewOrderAddRemove : public MsgOrderAddRemove
{
public:
    MsgNysFeeRenewOrderAddRemove(Order* order, bool add) : MsgOrderAddRemove(order, add, M_NYS_FEE_RENEW_ORDER_ADDREMOVE, sizeof(MsgNysFeeRenewOrderAddRemove)){}
};

class BUSINESS_API MsgOrderSetMarketSize : public MsgOrderChange
{
public:
    MsgOrderSetMarketSize(Order* order) : MsgOrderChange(order, M_ORDER_SET_MARKET_SIZE, sizeof(MsgOrderSetMarketSize)){}
};

class BUSINESS_API MsgOrderUpdateMarketSize : public MsgOrderChange
{
public:
    MsgOrderUpdateMarketSize(Order* order) : MsgOrderChange(order, M_ORDER_UPDATE_MARKET_SIZE, sizeof(MsgOrderUpdateMarketSize)){}
};

class BUSINESS_API MsgOrderRemoveMarketSize : public MsgOrderChange
{
public:
    MsgOrderRemoveMarketSize(Order* order) : MsgOrderChange(order, M_ORDER_REMOVE_MARKET_SIZE, sizeof(MsgOrderRemoveMarketSize)){}
};

class BUSINESS_API MsgNysFeeWarning : public MsgOrderChange
{
public:
    MsgNysFeeWarning(Order* order) : MsgOrderChange(order, M_ORDER_NYS_FEE_WARNING, sizeof(MsgNysFeeWarning)){}
};

class BUSINESS_API MsgNysFeeImposition : public MsgOrderChange
{
public:
    MsgNysFeeImposition(Order* order) : MsgOrderChange(order, M_ORDER_NYS_FEE_IMPOSITION, sizeof(MsgNysFeeImposition)){}
};

class BUSINESS_API MsgOrderSupervisorSet : public MsgOrderChange
{
public:
    MsgOrderSupervisorSet(Order* order) : MsgOrderChange(order, M_ORDER_SUPERVISOR_SET, sizeof(MsgOrderSupervisorSet)){}
};

class BUSINESS_API MsgOrderReRouteAdd : public MsgOrderChange
{
public:
    MsgOrderReRouteAdd(Order* order, Observable* orderReRoute) : MsgOrderChange(order, M_ORDER_REROUTE_ADD, sizeof(MsgOrderReRouteAdd)), m_orderReRoute(orderReRoute){}
    Observable* m_orderReRoute;
};

class BUSINESS_API MsgOrderReRouteRemove : public MsgOrderChange
{
public:
    MsgOrderReRouteRemove(Order* order, Observable* orderReRoute) : MsgOrderChange(order, M_ORDER_REROUTE_REMOVE, sizeof(MsgOrderReRouteRemove)), m_orderReRoute(orderReRoute){}
    Observable* m_orderReRoute;
};

class BUSINESS_API MsgSmartOrderAdd : public MsgOrderChange
{
public:
    MsgSmartOrderAdd(Order* order) : MsgOrderChange(order, M_SMARTORDER_ADD, sizeof(MsgSmartOrderAdd)){}
};

class BUSINESS_API MsgSmartOrderModify : public MsgOrderChange
{
public:
    MsgSmartOrderModify(Order* order) : MsgOrderChange(order, M_SMARTORDER_MODIFY, sizeof(MsgSmartOrderModify)){}
};

class BUSINESS_API MsgSmartOrderRemove : public MsgOrderChange
{
public:
    MsgSmartOrderRemove(Order* order) : MsgOrderChange(order, M_SMARTORDER_REMOVE, sizeof(MsgSmartOrderRemove)){}
};

class BUSINESS_API MsgSmartOrderCancel : public MsgOrderChange
{
public:
    MsgSmartOrderCancel(Order* order) : MsgOrderChange(order, M_SMARTORDER_CANCEL, sizeof(MsgSmartOrderCancel)){}
};

class BUSINESS_API MsgSmartOrderExecution : public MsgOrderChange
{
public:
    MsgSmartOrderExecution(Order* order) : MsgOrderChange(order, M_SMARTORDER_EXECUTION, sizeof(MsgSmartOrderExecution)){}
};

class BUSINESS_API MsgOrderDeleted : public MsgOrderChange
{
public:
    MsgOrderDeleted(Order* order) : MsgOrderChange(order, M_ORDER_DELETED, sizeof(MsgOrderDeleted)){}
};

class BUSINESS_API MsgObservable : public Message
{
public:
    MsgObservable(Observable* observable, unsigned int type, unsigned int size) : Message(type, size), m_observable(observable){}
    Observable* m_observable;
};

class BUSINESS_API MsgObservablePtr : public MsgObservable
{
public:
    MsgObservablePtr(Observable* observable):MsgObservable(observable, M_AI_OBSERVABLE_PTR, sizeof(MsgObservablePtr)){}
};

class BUSINESS_API MsgStockMovement : public Message
{
public:
    MsgStockMovement(StockMovement* stock):
        Message(M_AI_STOCK_MOVEMENT, sizeof(MsgStockMovement)),
        m_stock(stock){}
    StockMovement* m_stock;
};

class BUSINESS_API MsgStockBase : public Message
{
public:
    MsgStockBase(StockBase* stock):
        Message(M_AI_STOCK_BASE, sizeof(MsgStockBase)),
        m_stock(stock){}
    StockBase* m_stock;
};

class BUSINESS_API MsgStockExchangeChanged : public Message
{
public:
    MsgStockExchangeChanged(StockBase* stock, char oldExchange, bool oldSmallCap):
        Message(M_AI_STOCK_EXCHANGE_CHANGED, sizeof(MsgStockBase)),
        m_stock(stock),
		m_oldExchange(oldExchange),
		m_oldSmallCap(oldSmallCap){}
    StockBase* m_stock;
	char m_oldExchange;
	char m_oldSmallCap;
};

class BUSINESS_API ChartPoint
{
public:
    ChartPoint(const Money& priceStart,
		const Money& priceMin,
		const Money& priceMax,
		const Money& priceEnd,
		unsigned __int64 moneyTraded,
		unsigned int volume):

		m_priceStart(priceStart),
		m_priceMin(priceMin),
		m_priceMax(priceMax),
		m_priceEnd(priceEnd),
		m_moneyTraded(moneyTraded),
		m_volume(volume)
	{
		UpdateVWAP();
	}

	ChartPoint(const Money& price, unsigned int volume){SetPoint(price, volume);}
	ChartPoint(const MoneySize& trade){SetPoint(trade, trade.GetSize());}
	ChartPoint():m_volume(0){}

	bool isInitialized() const{return m_volume != 0;}

	const Money& GetVWAP() const{return m_vwap;}
	unsigned __int64 GetMoneyTraded() const{return m_moneyTraded;}
    const Money& GetPriceEnd() const{return m_priceEnd;}
    const Money& GetPriceStart() const{return m_priceStart;}
    const Money& GetPriceMin() const{return m_priceMin;}
    const Money& GetPriceMax() const{return m_priceMax;}
	unsigned int GetVolume() const{return m_volume;}

	void AddPrint(const MoneySize& trade)
	{
		m_priceEnd = trade;
//		unsigned int shares = trade.GetShares();
		unsigned int shares = trade.GetSize();
		if(m_volume == 0)//not initialized
		{
			m_volume = shares;
			m_priceStart = m_priceMin = m_priceMax = m_priceEnd;
			m_moneyTraded = (unsigned __int64)shares * (m_priceEnd.GetWhole() * 1000 + m_priceEnd.GetThousandsFraction());
		}
		else
		{
			m_volume += shares;
			m_moneyTraded += (unsigned __int64)shares * (m_priceEnd.GetWhole() * 1000 + m_priceEnd.GetThousandsFraction());
			if(m_priceEnd < m_priceMin)
			{
				m_priceMin = m_priceEnd;
			}
			if(m_priceMax < m_priceEnd)
			{
				m_priceMax = m_priceEnd;
			}
		}
		UpdateVWAP();
	}

	void Add(int vwap, unsigned int vol, int priceStart, int priceMin, int priceMax, int priceEnd);

	void SetPoint(const Money& price, unsigned int volume)
	{
		m_priceEnd = price;
		m_moneyTraded = (__int64)volume * (m_priceEnd.GetWhole() * 1000 + m_priceEnd.GetThousandsFraction());
		m_volume = volume;
		UpdateVWAP();
	}

	void Reset()
	{
		m_priceStart.SetZero();
		m_priceMin.SetZero();
		m_priceMax.SetZero();
		m_priceEnd.SetZero();
		m_moneyTraded = 0;
		m_vwap.SetZero();
		m_volume = 0;
	}
protected:
	void UpdateVWAP()
	{
		if(m_volume == 0)
		{
			m_vwap = m_priceEnd;
		}
		else
		{
			__int64 value = m_moneyTraded / (__int64)m_volume;
			__int64 dollars = value / 1000;
			m_vwap.InitMoney((int)dollars, (short)(value - 1000 * dollars));
		}
	}

    Money m_vwap;
    __int64 m_moneyTraded;
    Money m_priceStart;
    Money m_priceMin;
    Money m_priceMax;
    Money m_priceEnd;
	unsigned int m_volume;
};

class BUSINESS_API MsgChartPoint : public Message
{
public:
    MsgChartPoint(unsigned int minute, const ChartPoint& point, const MoneySize& lastTrade):
        Message(M_AI_CHART_POINT, sizeof(MsgChartPoint)),
        m_minute(minute),
        m_chartPoint(point),
		m_lastTrade(lastTrade){}
    unsigned int m_minute;
	const ChartPoint& m_chartPoint;
	const MoneySize& m_lastTrade;
};

class BUSINESS_API MsgIndexDirection : public Message
{
public:
    MsgIndexDirection(int indexDirection):
        Message(M_AI_INDEX_DIRECTION, sizeof(MsgIndexDirection)),
        m_indexDirection(indexDirection){}
    int m_indexDirection;
};

class BUSINESS_API MsgPositionChange : public Message
{
public:
    const Position* m_position;
protected:
    MsgPositionChange(const Position* position, unsigned int type, unsigned int size):
        Message(type, size),
        m_position(position){}
};
/*
class BUSINESS_API MsgAccountMaxLoss : public Message
{
public:
    MsgAccountMaxLoss(bool exceeded):
        Message(M_ACCOUNT_MAX_LOSS_EXCEEDED, sizeof(MsgAccountMaxLoss)),
        m_exceeded(exceeded)
        {}
    bool m_exceeded;
};

class BUSINESS_API MsgPositionMaxLoss : public MsgPositionChange
{
public:
    MsgPositionMaxLoss(const Position* position, bool exceeded):
        MsgPositionChange(position, M_POSITION_MAX_LOSS_EXCEEDED, sizeof(MsgPositionMaxLoss)),
        m_exceeded(exceeded)
        {}
    bool m_exceeded;
};
*/

class BUSINESS_API MsgStagingOrderChange : public Message
{
public:
    const StagingOrder* m_stagingOrder;
protected:
    MsgStagingOrderChange(const StagingOrder* stagingOrder, unsigned int type, unsigned int size):
        Message(type, size),
        m_stagingOrder(stagingOrder){}
};

class BUSINESS_API MsgStagingOrderSizeIncrement : public MsgStagingOrderChange
{
public:
	unsigned int m_incrementSize;
	bool m_increment;
protected:
    MsgStagingOrderSizeIncrement(const StagingOrder* stagingOrder, unsigned int incrementSize, bool increment, unsigned int type, unsigned int size):
		MsgStagingOrderChange(stagingOrder, type, size),
		m_incrementSize(incrementSize),
		m_increment(increment)
	{}
};

class BUSINESS_API MsgStagingOrderPendingSizeIncrement : public MsgStagingOrderSizeIncrement
{
public:
    MsgStagingOrderPendingSizeIncrement(const StagingOrder* stagingOrder, unsigned int incrementSize, bool increment):
		MsgStagingOrderSizeIncrement(stagingOrder, incrementSize, increment, M_STAGING_ORDER_PENDING_SIZE_INCREMENT, sizeof(MsgStagingOrderPendingSizeIncrement))
	{}
};

class BUSINESS_API MsgStagingOrderAllocatedSizeIncrement : public MsgStagingOrderSizeIncrement
{
public:
    MsgStagingOrderAllocatedSizeIncrement(const StagingOrder* stagingOrder, unsigned int incrementSize, bool increment):
		MsgStagingOrderSizeIncrement(stagingOrder, incrementSize, increment, M_STAGING_ORDER_ALLOCATED, sizeof(MsgStagingOrderAllocatedSizeIncrement))
	{}
};

class BUSINESS_API MsgAiStagingOrder : public MsgStagingOrderChange
{
public:
    MsgAiStagingOrder(const StagingOrder* stagingOrder) : MsgStagingOrderChange(stagingOrder, M_AI_STAGING_ORDER, sizeof(MsgAiStagingOrder)){}
};

class BUSINESS_API MsgStagingOrderDeleted : public MsgStagingOrderChange
{
public:
    MsgStagingOrderDeleted(const StagingOrder* stagingOrder) : MsgStagingOrderChange(stagingOrder, M_STAGING_ORDER_DELETED, sizeof(MsgStagingOrderDeleted)){}
};

class BUSINESS_API MsgStagingOrderCurrent : public MsgStagingOrderChange
{
public:
    MsgStagingOrderCurrent(const StagingOrder* stagingOrder, bool current):
        MsgStagingOrderChange(stagingOrder, M_STAGING_ORDER_CURRENT, sizeof(MsgStagingOrderCurrent)),
		m_current(current)
        {}
	bool m_current;
};

class BUSINESS_API MsgStagingOrderLockSent : public MsgStagingOrderChange
{
public:
    MsgStagingOrderLockSent(const StagingOrder* stagingOrder, bool unlock):
        MsgStagingOrderChange(stagingOrder, M_STAGING_ORDER_LOCK_SENT, sizeof(MsgStagingOrderLockSent)),
		m_unlock(unlock)
        {}
	bool m_unlock;
};

class BUSINESS_API MsgPositionStagingOrder : public MsgPositionChange
{
public:
    MsgPositionStagingOrder(const Position* position, StagingOrder* currentStagingOrder, StagingOrder* previousStagingOrder):
        MsgPositionChange(position, M_POSITION_STAGING_ORDER, sizeof(MsgPositionStagingOrder)),
        m_currentStagingOrder(currentStagingOrder),
		m_previousStagingOrder(previousStagingOrder)
        {}
	StagingOrder* m_currentStagingOrder;
	StagingOrder* m_previousStagingOrder;
};


class BUSINESS_API MsgPositionOpenMaxLoss : public MsgPositionChange
{
public:
    MsgPositionOpenMaxLoss(const Position* position, bool exceeded):
        MsgPositionChange(position, M_POSITION_OPEN_MAX_LOSS_EXCEEDED, sizeof(MsgPositionOpenMaxLoss)),
        m_exceeded(exceeded)
        {}
    bool m_exceeded;
};

class BUSINESS_API MsgOversellExpiraion : public MsgPositionChange
{
public:
    MsgOversellExpiraion(const Position* position):
        MsgPositionChange(position, M_OVERSELL_EXPIRATION, sizeof(MsgOversellExpiraion))
        {}
};

class BUSINESS_API MsgOversellCoveredByMktOrder : public MsgPositionChange
{
public:
    MsgOversellCoveredByMktOrder(const Position* position, unsigned int coveredSharesCovered):
        MsgPositionChange(position, M_OVERSELL_COVERED_BY_MKT_ORDER, sizeof(MsgOversellCoveredByMktOrder)),
        m_coveredSharesCovered(coveredSharesCovered)
        {}
    unsigned int m_coveredSharesCovered;
};
/*NXDELAY
class BUSINESS_API MsgPositionNxStatusChange : public MsgPositionChange
{
public:
    MsgPositionNxStatusChange(const Position* position, bool nxAvailable, bool side):
        MsgPositionChange(position, M_POSITION_NXSTATUSCHANGE, sizeof(MsgPositionNxStatusChange)),
        m_nxAvailable(nxAvailable),
        m_side(side)
    {
    }
    bool m_nxAvailable;
    bool m_side;
};
*/
class BUSINESS_API MsgPositionPhantomChange : public MsgPositionChange
{
public:
    MsgPositionPhantomChange(const Position* position, int shareIncrement):
        MsgPositionChange(position, M_POSITION_PHANTOMCHANGE, sizeof(MsgPositionPhantomChange)),
        m_shareIncrement(shareIncrement)
    {
    }
    int m_shareIncrement;
};

class BUSINESS_API MsgPositionStockSubscriptionChange : public MsgPositionChange
{
public:
    MsgPositionStockSubscriptionChange(const Position* position, bool subscribed):
        MsgPositionChange(position, M_POSITION_STOCK_SUBSCRIPTION_CHANGE, sizeof(MsgPositionStockSubscriptionChange)),
		m_subscribed(subscribed)
    {
    }
	bool m_subscribed;
};

class BUSINESS_API MsgPositionBulletsChange : public MsgPositionChange
{
public:
    MsgPositionBulletsChange(const Position* position):
        MsgPositionChange(position, M_POSITION_BULLETSCHANGE, sizeof(MsgPositionBulletsChange))
    {
    }
};

class BUSINESS_API MsgPositionInventoryChange : public MsgPositionChange
{
public:
    MsgPositionInventoryChange(const Position* position):
        MsgPositionChange(position, M_POSITION_INVENTORY_CHANGE, sizeof(MsgPositionInventoryChange))
    {
    }
};

class BUSINESS_API MsgHistoryPrintsCleared : public Message
{
public:
	MsgHistoryPrintsCleared(const StockMovement* stock):
		Message(M_HISTORY_PRINTS_CLEARED, sizeof(MsgHistoryPrintsCleared)),
		m_stock(stock)
	{
	}
	const StockMovement* m_stock;
};

class BUSINESS_API MsgBookIntegrationChanged : public Message
{
public:
	MsgBookIntegrationChanged(bool bookLines, bool mmLines):
		Message(M_BOOK_INTEGRATION_CHANGED, sizeof(MsgBookIntegrationChanged)),
		m_bookLines(bookLines),
		m_mmLines(mmLines)
	{
	}
	bool m_bookLines;
	bool m_mmLines;
};

class BUSINESS_API MsgDisplayLevel2EcnChanged : public Message
{
public:
	MsgDisplayLevel2EcnChanged(unsigned short bookId, bool display):
		Message(M_DISPLAY_LEVEL2_ECN_CHANGED, sizeof(MsgDisplayLevel2EcnChanged)),
		m_bookId(bookId),
		m_display(display)
	{
	}
	unsigned short m_bookId;
	bool m_display;
};

class BUSINESS_API MsgAcquiredInventory : public Message
{
public:
	MsgAcquiredInventory(Observable* account, bool acquired):Message(M_ACQUIRED_INVENTORY, sizeof(MsgAcquiredInventory)),m_account(account), m_acquired(acquired){}
    Observable* m_account;
	bool m_acquired;
};

class BUSINESS_API MsgPositionInventoryPop : public MsgPositionChange
{
public:
    MsgPositionInventoryPop(const Position* position, bool inventory):
        MsgPositionChange(position, M_POSITION_INVENTORY_POP, sizeof(MsgPositionInventoryPop)),
		m_inventory(inventory)
    {
    }
	bool m_inventory;
};

class BUSINESS_API MsgPositionInvestmentChange : public MsgPositionChange
{
public:
    MsgPositionInvestmentChange(const Position* position):
        MsgPositionChange(position, M_POSITION_INVESTMENTCHANGE, sizeof(MsgPositionInvestmentChange))
    {
    }
};

class BUSINESS_API MsgPositionOvernightChange : public MsgPositionChange
{
public:
    MsgPositionOvernightChange(const Position* position):
        MsgPositionChange(position, M_POSITION_OVERNIGHTCHANGE, sizeof(MsgPositionOvernightChange))
    {
    }
};

class BUSINESS_API MsgPositionPendingMoneyChange : public MsgPositionChange
{
public:
    MsgPositionPendingMoneyChange(const Position* position, int size, const Money& price):
        MsgPositionChange(position, M_POSITION_PENDINGMONEYCHANGE, sizeof(MsgPositionPendingMoneyChange)),
        m_size(size),
        m_price(price)
    {
    }
    const int m_size;
    const Money m_price;
};

class BUSINESS_API MsgPositionInstitutionalSharesAdded : public MsgPositionChange
{
public:
    MsgPositionInstitutionalSharesAdded(const Position* position, int size, const Money& price):
        MsgPositionChange(position, M_POSITION_INSTITUTIONAL_ADDED, sizeof(MsgPositionInstitutionalSharesAdded)),
        m_size(size),
        m_price(price)
    {
    }
    const int m_size;
    const Money m_price;
};

class BUSINESS_API MsgPositionPendingOrdersCountChange : public MsgPositionChange
{
public:
    MsgPositionPendingOrdersCountChange(const Position* position):
        MsgPositionChange(position, M_POSITION_PENDINGORDERSCOUNTCHANGE, sizeof(MsgPositionPendingOrdersCountChange))
    {
    }
};

class BUSINESS_API MsgPositionNew : public MsgPositionChange
{
public:
    MsgPositionNew(const Position* position):
        MsgPositionChange(position, M_POSITION_NEW, sizeof(MsgPositionNew))
    {
    }
};

class BUSINESS_API MsgPositionDeleted : public MsgPositionChange
{
public:
    MsgPositionDeleted(const Position* position):
        MsgPositionChange(position, M_POSITION_DELETED, sizeof(MsgPositionDeleted))
    {
    }
};

class BUSINESS_API MsgStock : public Message
{
public:
    const StockBase* m_stockHandle;
    char m_symbol[LENGTH_SYMBOL + 1];
protected:
    MsgStock(const StockBase* stockHandle, const char* symbol, unsigned int type, unsigned int size);
};

class BUSINESS_API MsgStockDeleted : public MsgStock
{
public:
    MsgStockDeleted(const StockBase* stockHandle, const char* symbol):
        MsgStock(stockHandle, symbol, M_STOCK_DELETED, sizeof(MsgStockDeleted))
    {
    }
};

class BUSINESS_API MsgUnderlierDeleted : public Message
{
public:
	MsgUnderlierDeleted(const Underlier* underlier)://, const char* symbol):
        Message(M_UNDERLIER_DELETED, sizeof(MsgUnderlierDeleted)),
		m_underlier(underlier)
    {
    }
	const Underlier* m_underlier;
};

class BUSINESS_API MsgOptionDeleted : public MsgStock
{
public:
    MsgOptionDeleted(const StockBase* stockHandle, const char* symbol):
        MsgStock(stockHandle, symbol, M_OPTION_DELETED, sizeof(MsgOptionDeleted))
    {
    }
};

class BUSINESS_API MsgStockUnsubscribed : public Message
{
public:
    MsgStockUnsubscribed(const char* symbol);
	char m_symbol[LENGTH_SYMBOL + 1];
};
/*
class BUSINESS_API MsgOptionUnsubscribed : public Message
{
public:
    MsgOptionUnsubscribed(const char* symbol);
	char m_symbol[LENGTH_SYMBOL+1];
};
*/
/*
class BUSINESS_API MsgFyi : public Message
{
public:
    MsgFyi():
        Message(M_MS_FYI, sizeof(MsgFyi))
    {
        *m_symbol = '\0';
        *m_contra = '\0';
    }
    char m_symbol[LENGTH_SYMBOL];
    char m_contra[LENGTH_SYMBOL];
    char m_side;
    unsigned int m_bookId;
    Money m_price;
    unsigned int m_tradeSize;
    Money m_bid;
    Money m_ask;
    char m_bidTick;
};
*/
class BUSINESS_API MsgMsTrade : public Message
{
public:
    const StockMovement* m_stockMovement;
	const Transaction* m_transaction;
    Money m_price;
    unsigned short m_bookId;
    unsigned int m_mmid;
    unsigned int m_tradeSize;
    unsigned int m_status;
    char m_side;
	bool m_hidden;
    char m_saleCondition;
//    Money m_bid;
//    Money m_ask;
//    char m_bidTick;
protected:
    MsgMsTrade(const StockMovement* stockMovement, unsigned short type, unsigned short size):
        Message(type, size),
        m_stockMovement(stockMovement),
		m_transaction(NULL),
        m_bookId(0xFFFF),
		m_mmid(0),
        m_tradeSize(0),
        m_status(TRADE_BETWEENBIDASK),
        m_side('\0'),//('B'),
		m_hidden(false),
		m_saleCondition('\0')
//        m_bidTick(' ')
    {
    }
};

class BUSINESS_API MsgEcnTrade : public MsgMsTrade
{
public:
    MsgEcnTrade(const StockMovement* stockMovement):MsgMsTrade(stockMovement, M_MS_ECN_TRADE, sizeof(MsgEcnTrade)){}
};

class BUSINESS_API MsgNonEcnTrade : public MsgMsTrade
{
public:
    MsgNonEcnTrade(const StockMovement* stockMovement):MsgMsTrade(stockMovement, M_MS_NON_ECN_TRADE, sizeof(MsgNonEcnTrade)){}
};

class BUSINESS_API MsgL2LargeQuote : public Message
{
public:
    MsgL2LargeQuote():
        Message(M_MS_L2_LARGE_QUOTE, sizeof(MsgL2LargeQuote)),
        m_side('B'),
        m_quoteSize(0)
    {
        *m_symbol = '\0';
        *m_mmid = '\0';
    }
    char m_symbol[LENGTH_SYMBOL + 1];
    char m_mmid[LENGTH_SYMBOL];
    char m_side;
    Money m_price;
    unsigned int m_quoteSize;
};

class BUSINESS_API MsgMarketCloseSecondChanged : public Message
{
public:
    MsgMarketCloseSecondChanged(unsigned int closeSecond):Message(M_MARKET_CLOSE_SECOND_CHANGED, sizeof(MsgMarketCloseSecondChanged)),m_closeSecond(closeSecond){}
    unsigned int m_closeSecond;
};

class BUSINESS_API MsgMarketVolume : public Message
{
public:
    MsgMarketVolume(unsigned __int64 volume, unsigned int type, unsigned int size):Message(type, size),m_volume(volume){}
    unsigned __int64 m_volume;
};

class BUSINESS_API MsgNasdaqVolume : public MsgMarketVolume
{
public:
    MsgNasdaqVolume(unsigned __int64 volume):MsgMarketVolume(volume, M_MS_NASDAQ_VOLUME, sizeof(MsgNasdaqVolume)){}
};

class BUSINESS_API MsgNyseVolume : public MsgMarketVolume
{
public:
    MsgNyseVolume(unsigned __int64 volume):MsgMarketVolume(volume, M_MS_NYSE_VOLUME, sizeof(MsgNyseVolume)){}
};

class BUSINESS_API MsgAmexVolume : public MsgMarketVolume
{
public:
    MsgAmexVolume(unsigned __int64 volume):MsgMarketVolume(volume, M_MS_AMEX_VOLUME, sizeof(MsgAmexVolume)){}
};

class BUSINESS_API MsgArcaVolume : public MsgMarketVolume
{
public:
    MsgArcaVolume(unsigned __int64 volume):MsgMarketVolume(volume, M_MS_ARCA_VOLUME, sizeof(MsgArcaVolume)){}
};

class BUSINESS_API MsgCboeVolume : public MsgMarketVolume
{
public:
    MsgCboeVolume(unsigned __int64 volume):MsgMarketVolume(volume, M_MS_CBOE_VOLUME, sizeof(MsgCboeVolume)){}
};

enum StockMove
{
    SM_NONE,
    SM_PREPEND,
    SM_JOIN,
    SM_LEAVE,
    SM_DROP,
};

class BUSINESS_API AIMsgBookQuote : public MsgTransactionInside
{
public:
/*
    AIMsgBookQuote(const MoneySize& bid, const MoneySize& ask, const MoneySize& transaction, const char* mmid, StockMove m = SM_NONE) : 
        Message(M_AI_BOOK_QUOTE, sizeof(AIMsgBookQuote)),
        m_bestBid(bid),
        m_bestAsk(ask),
        m_transaction(transaction),
        m_move(m)
    {
        strncpy(m_mmid, mmid, sizeof(m_mmid) - 1);
        m_mmid[sizeof(m_mmid) - 1] = '\0';
    }
*/
    AIMsgBookQuote(bool side,
		unsigned short bookId,
		const MoneySize& bid,
		const MoneySize& ask,
		const MoneySize& lastTrade,
//		const MoneySize* execution,
		const char* mmid,
		StockMove m = SM_NONE,
		const void* mmBookHolder = NULL,
		const Transaction* transaction = NULL):
        MsgTransactionInside(M_AI_BOOK_QUOTE, sizeof(AIMsgBookQuote), transaction, bid, ask),
//        m_bestBid(bid),
//        m_bestAsk(ask),
        m_lastTrade(lastTrade),
//		m_execution(execution ? *execution : MoneySize()),
        m_move(m),
        m_bookId(bookId),
        m_side(side),
		m_mmBookHolder(mmBookHolder)
//        m_transaction(transaction)
    {
        if(*mmid)
        {
            strncpy_s(m_mmid, sizeof(m_mmid), mmid, sizeof(m_mmid) - 1);
            m_mmid[sizeof(m_mmid) - 1] = '\0';
        }
        else
        {
            memcpy(m_mmid, 0, sizeof(m_mmid));
        }
    }

    void Reset(){m_move = SM_NONE;}
    StockMove m_move;
//    MoneySize m_bestBid;
//    MoneySize m_bestAsk;
    MoneySize m_was;
    MoneySize m_became;
    MoneySize m_lastTrade;
//    MoneySize m_execution;
//	const Transaction* m_transaction;
    char m_mmid[LENGTH_SYMBOL];
    unsigned short m_bookId;
    bool m_side;
	const void* m_mmBookHolder;//NULL for ECN books
//	const Transaction* m_transaction;
};

class BUSINESS_API AIMsgNyseBook : public Message
{
public:
	AIMsgNyseBook() : Message(M_AI_NYSE_BOOK, sizeof(AIMsgNyseBook)),
		m_bidChanged(false),
		m_askChanged(false)
	{}
	bool m_bidChanged;
	bool m_askChanged;
};

class BUSINESS_API AIMsgMMLevel2Quote : public Message
{
public:
	AIMsgMMLevel2Quote() : Message(M_AI_LEVEL2_QUOTE, sizeof(AIMsgMMLevel2Quote))
    {
        Reset();
    }

    void Reset()
    {
        bidChanged = false;
        askChanged = false;
//        bookId = 0xFFFFFFFF;//0xFFFFFFFF for non-ECNs
        bookId = 0xFFFF;//0xFFFF for non-ECNs
        isDirectEcn = false;
		isDirectMm = false;
		m_mmBook = NULL;
        bidAction = SM_NONE;
        askAction = SM_NONE;

        m_wasBid.SetZero();
        m_becameBid.SetZero();
        m_wasAsk.SetZero();
        m_becameAsk.SetZero();
        m_lastTrade.SetZero();
		m_wasLevel2BestBid.SetZero();
		m_wasLevel2BestAsk.SetZero();
		m_becameLevel2BestBid.SetZero();
		m_becameLevel2BestAsk.SetZero();
		m_wasBestBid.SetZero();
		m_wasBestAsk.SetZero();
		m_becameBestBid.SetZero();
		m_becameBestAsk.SetZero();

		m_wasAsk.SetShares(0);
        m_becameAsk.SetShares(0);
        m_wasBid.SetShares(0);
        m_becameBid.SetShares(0);
        m_lastTrade.SetShares(0);
		m_wasLevel2BestBid.SetShares(0);
		m_wasLevel2BestAsk.SetShares(0);
		m_becameLevel2BestBid.SetShares(0);
		m_becameLevel2BestAsk.SetShares(0);
		m_wasBestBid.SetShares(0);
		m_wasBestAsk.SetShares(0);
		m_becameBestBid.SetShares(0);
		m_becameBestAsk.SetShares(0);


//        memset(m_mmid, 0, sizeof(m_mmid));
    }
    bool isBookIntegrated() const;
    bool bidChanged;
    bool askChanged;
    unsigned short bookId; //0xFFFF for non-ECNs
    bool isDirectEcn;
    bool isDirectMm;
    StockMove bidAction;
    StockMove askAction;
    char m_mmid[LENGTH_SYMBOL + 1];
    unsigned int m_ecnType;
    MoneySize m_wasLevel2BestBid;
    MoneySize m_wasLevel2BestAsk;
    MoneySize m_becameLevel2BestBid;
    MoneySize m_becameLevel2BestAsk;
    MoneySize m_wasBestBid;
    MoneySize m_wasBestAsk;
    MoneySize m_becameBestBid;
    MoneySize m_becameBestAsk;
    MoneySize m_wasBid;
    MoneySize m_becameBid;
    MoneySize m_wasAsk;
    MoneySize m_becameAsk;
    MoneySize m_lastTrade;
	Observable* m_mmBook;
};

/*
class BUSINESS_API AIMsgOrderChange : public Message
{
public:
    Order* m_order;
protected:
    AIMsgOrderChange(Order* order, unsigned short type, unsigned short size):
        Message(type, size),
        m_order(order){}
};
*/
class BUSINESS_API AIMsgOrderUserDescription : public MsgOrderChange
{
public:
    AIMsgOrderUserDescription(Order* order):MsgOrderChange(order, M_AI_ORDER_USER_DESCRIPTION, sizeof(AIMsgOrderUserDescription)){}
};

class BUSINESS_API AIMsgOrder : public MsgOrderChange
{
public:
    AIMsgOrder(Order* order, const Position* pos):MsgOrderChange(order, M_AI_ORDER, sizeof(AIMsgOrder)), m_position(pos){}
    const Position* m_position;
};

class BUSINESS_API MsgSmartOrderActivated : public MsgOrderChange
{
public:
    MsgSmartOrderActivated(Order* order):MsgOrderChange(order, M_SMART_ORDER_ACTIVATED, sizeof(MsgSmartOrderActivated)){}
};

class BUSINESS_API AIMsgPosition : public MsgPositionChange
{
public:
    AIMsgPosition(const Position* position):MsgPositionChange(position, M_AI_POSITION, sizeof(AIMsgPosition)){}
};

class BUSINESS_API AIMsgExecution : public MsgPositionChange
{
public:
    AIMsgExecution(const Position* position, const Execution* execution, bool historical = false, int oversoldShares = 0):
        MsgPositionChange(position, M_AI_EXECUTION, sizeof(AIMsgExecution)),
        m_execution(execution),
        m_order(NULL),
        m_historical(historical),
        m_oversoldShares(oversoldShares){}
    const Execution* m_execution;
    Order* m_order;//NULL for overnite or historical executions
    bool m_historical;
    int m_oversoldShares;
};

class BUSINESS_API AIMsgPrintStatus : public Message
{
public:
	AIMsgPrintStatus(unsigned int printStatus):
        Message(M_AI_PRINT_STATUS, sizeof(AIMsgPrintStatus)),
        m_printStatus(printStatus)
        {}
    unsigned int m_printStatus;
};

class BUSINESS_API MsgCorrectionVolume : public Message
{
public:
	MsgCorrectionVolume(unsigned __int64 volumeCorrection):
        Message(M_CORRECTION_VOLUME, sizeof(MsgCorrectionVolume)),
        m_volumeCorrection(volumeCorrection)
        {}
	unsigned __int64 m_volumeCorrection;
};

class BUSINESS_API MsgCorrectionClosePrice : public Message
{
public:
	MsgCorrectionClosePrice(const Money& closePriceCorrection):
        Message(M_CORRECTION_CLOSE_PRICE, sizeof(MsgCorrectionClosePrice)),
        m_closePriceCorrection(closePriceCorrection)
        {}
	Money m_closePriceCorrection;
};

class BUSINESS_API MsgMarketCloseQuote : public Message
{
public:
	MsgMarketCloseQuote(const Money& closeBid, const Money& closeAsk):
        Message(M_MARKET_CLOSE_QUOTE, sizeof(MsgCorrectionClosePrice)),
        m_closeBid(closeBid),
        m_closeAsk(closeAsk)
        {}
	Money m_closeBid;
	Money m_closeAsk;
};

enum MessageType
{
    MTYPE_UNIMPORTANT,
    MTYPE_ORDER_NOT_SENT,
    MTYPE_ORDER_NOT_CANCELLED,
    MTYPE_POSITION_NOT_CLOSED,
    MTYPE_CLOSING_LOSING_POSITION,
};

class BUSINESS_API MsgText : public Message
{
public:
    MsgText(const char* symbol, const char* message, MessageType messageType = MTYPE_UNIMPORTANT):
        Message(M_TEXT, sizeof(MsgText)),
		m_symbol(symbol),
        m_message(message),
        m_messageType(messageType)
        {}
//    std::string m_message;
	const char* m_symbol;
    const char* m_message;
    MessageType m_messageType;
};

class BUSINESS_API MsgNextMinute : public Message
{
public:
    MsgNextMinute(unsigned int minute):
        Message(M_NEXT_MINUTE, sizeof(MsgNextMinute)),
        m_minute(minute)
        {}
    unsigned int m_minute;
};

class BUSINESS_API MsgNextDay : public Message
{
public:
    MsgNextDay():
        Message(M_NEXT_DAY, sizeof(MsgNextDay))
        {}
};


enum PositionFlags
{
    POSITION_FLAT = 1,
    POSITION_LONG = 2,
    POSITION_SHORT = 4,
    POSITION_BULLETS = 8,
    POSITION_LONGPHANTOM = 16,
    POSITION_SHORTPHANTOM = 32,
    POSITION_INVENTORY = 64,
};


enum CancelOrderFlags
{
    CO_BUY = 1,
    CO_SELL = 2,
    CO_NONDAY = 4,
    CO_DAY = 8,
    CO_GTC = 16,

    CO_LAST
};

enum OrderStatus
{
    OS_CANCELLED = 1,
    OS_FILLED = 2,
    OS_PENDINGLONG = 4,
    OS_PENDINGSHORT = 8,

    OS_LAST
};
/*
class BUSINESS_API BasketItem
{
friend class Basket;
public:
    BasketItem(const char* name = ""):m_name(name){}
    virtual ~BasketItem(){}
    const std::string& GetName() const{return m_name;}
	virtual bool operator==(const BasketItem& b) const
	{
		return m_name == b.m_name;
	}
	bool operator<(const BasketItem& bs) const
	{
		return m_name < bs.m_name;
	}

    virtual bool isBasket() const{return false;}
    virtual bool isTemporary() const{return false;}
	virtual const BasketItem* Uses(const BasketItem* b) const{return NULL;}
	virtual bool UsesRecursevely(const BasketItem* b) const{return false;}
	virtual bool isUsed() const{return false;}

    virtual unsigned int GetUsedByPathIterator(const BasketItem* item) const{return 0;}
	unsigned int GetLoopPathIterator() const
	{
		return GetUsedByPathIterator(this);
	}

    virtual void AddItem(BasketItem* i, unsigned int count, bool increment = false){}
    virtual void RemoveItem(BasketItem* i){}
    virtual void ClearItems(){}
protected:
	virtual void AddMeToExpandedBasket(BasketItem* b, unsigned int count) const;
    virtual void AddUsedBy(Basket* b){}
    virtual void RemoveUsedBy(Basket* b){}
//    virtual void RestoreLinks(BasketMap& basketMap){}
    virtual void AddUsesExpanded(const BasketItem* b, unsigned int count, bool increment){}
	std::string m_name;
};
*/
#ifdef __cplusplus
extern "C" {
#endif

void WINAPI B_Initialize();
void WINAPI B_Terminate();

void WINAPI B_SetMarketReceiver(Observable* receiver);
Observable* WINAPI B_GetMarketReceiver();

Observable* WINAPI B_GetAdminObservable();
Observable* WINAPI B_GetNewPositionObservable();
Observable* WINAPI B_GetPositionSizeObservable(Observable* account);
Observable* WINAPI B_GetIdleObservable();
Observable* WINAPI B_GetServiceObservable();
//void WINAPI B_SetApplicationVersion(const char* version, const char* clientId, unsigned int numericVersion);

Observable* WINAPI B_GetNysQuoteConditionObservable();
Observable* WINAPI B_GetMarketImbalanceObservable();
Observable* WINAPI B_GetPreMarketIndicationObservable();
Observable* WINAPI B_GetMMBoxActiveStockObservable();
const StockBase* WINAPI B_GetMMActiveStock();

Observable* WINAPI B_CreateAccount(const char* name, const char* password, const Message* logonMessage, bool simulation, bool compression, unsigned int orderLoadMode, bool cancelAllOnDisconnect, const char* ip, unsigned short port, const char* localIp, Observable* receiver = NULL);
Observable* WINAPI B_UpdateAccountConnection(const char* name, const char* password, bool simulation, bool compression, unsigned int orderLoadMode, bool cancelAllOnDisconnect, const char* ip, unsigned short port, const char* localIp);
Observable* WINAPI B_GetCurrentAccount();
Observable* WINAPI B_SetCurrentAccountByName(const char* name);
Observable* WINAPI B_SetCurrentAccount(Observable* account);
Observable* WINAPI B_GetAccount(const char* name);
Observable* WINAPI B_GetPrimaryAccount();
//Observable* WINAPI B_GetAccountByReceiver(Observable* receiver);//Use B_CreateAccountIterator instead, because multiple accounts can be associated with one receiver
const char* WINAPI B_GetAccountName(const Observable* account = NULL);
const char* WINAPI B_GetAccountTraderName(const Observable* account = NULL);
bool WINAPI B_ExtractLogonTokenValue(const Message* message, const char* key, char* buffer, unsigned int bufLen);
const char* WINAPI B_GetAccountPassword(const Observable* account = NULL);
const char* WINAPI B_GetAccountIp(const Observable* account = NULL);
unsigned short WINAPI B_GetAccountPort(const Observable* account = NULL);
Observable* WINAPI B_GetAccountReceiver(const Observable* account = NULL);
bool WINAPI B_IsAccountLogged(const Observable* account = NULL);
bool WINAPI B_IsAccountLoggedToMarketData(const Observable* account = NULL);
bool WINAPI B_IsAccountLoggedToExecutor(const Observable* account = NULL);
bool WINAPI B_IsAccountPrimary(const Observable* account = NULL);
const char* WINAPI B_GetAccountFirm(const Observable* account = NULL);
bool WINAPI B_IsAccountSimulation(const Observable* account = NULL);
bool WINAPI B_IsAccountToUseCompression(const Observable* account = NULL);
unsigned int WINAPI B_GetAccountOrderLoadMode(const Observable* account = NULL);
bool WINAPI B_IsAccountCancelAllOrdersOnDisconnect(const Observable* account = NULL);
//const Position* WINAPI B_GetAccountPosition(const char* symbol, const Observable* account = NULL);
const Money& WINAPI B_GetAccountMaxOpenPositionValue(const Observable* account = NULL);
unsigned int WINAPI B_GetShortSellMultiplier(const Observable* account = NULL);
const Money& WINAPI B_GetShortSellPriceLimit(const Observable* account = NULL);
unsigned int WINAPI B_GetMaxOpenPositionSize(const Observable* account = NULL);
unsigned int WINAPI B_GetMaxOrderSize(const Observable* account = NULL);
bool WINAPI B_IsPrpTrader(const Observable* account = NULL);
bool WINAPI B_IsInstitutionalTrader(const Observable* account = NULL);
bool WINAPI B_IsAccountInstitutional(const Observable* account = NULL);
bool WINAPI B_SetAccountInstitutional(bool institutional, const Observable* account = NULL);
bool WINAPI B_IsAccountDemo(const Observable* account = NULL);

const Money& WINAPI B_GetPriceOpenPnlInventoryOffset(const Observable* account = NULL);
const Money& WINAPI B_GetClosePriceOpenPnlInventoryOffset(const Observable* account = NULL);
const Money& WINAPI B_GetCloseQuoteOpenPnlInventoryOffset(const Observable* account = NULL);
const Money& WINAPI B_GetOpenPnlInventoryOffset(const Observable* account = NULL);
const Money& WINAPI B_GetOpenPnlLongInventoryOffset(const Observable* account = NULL);
const Money& WINAPI B_GetOpenPnlShortInventoryOffset(const Observable* account = NULL);
const Money& WINAPI B_GetClosedPnlInventoryOffset(const Observable* account = NULL);
const Money& WINAPI B_GetMoneyInvestedInventoryOffset(const Observable* account = NULL);
const Money& WINAPI B_GetMoneyInvestedLongInventoryOffset(const Observable* account = NULL);
const Money& WINAPI B_GetMoneyInvestedShortInventoryOffset(const Observable* account = NULL);
const Money& WINAPI B_GetAccountCapNetInventoryOffset(const Observable* account = NULL);

const Money& WINAPI B_GetAccountBuyingPower(const Observable* account = NULL);

const Money& WINAPI B_GetAccountBuyingPowerInUse(const Observable* account = NULL);
const Money& WINAPI B_GetAccountMaxBuyingPowerInUse(const Observable* account = NULL);
const Money& WINAPI B_GetAccountOpenPnl(const Observable* account = NULL);
const Money& WINAPI B_GetAccountOpenPnlLong(const Observable* account = NULL);
const Money& WINAPI B_GetAccountOpenPnlShort(const Observable* account = NULL);
const Money& WINAPI B_GetAccountPriceOpenPnl(const Observable* account = NULL);
const Money& WINAPI B_GetAccountClosePriceOpenPnl(const Observable* account = NULL);
const Money& WINAPI B_GetAccountCloseQuoteOpenPnl(const Observable* account = NULL);
const Money& WINAPI B_GetAccountClosedPnl(const Observable* account = NULL);

const Money& WINAPI B_GetAccountMoneyInvested(const Observable* account = NULL);
const Money& WINAPI B_GetAccountMoneyInvestedLong(const Observable* account = NULL);
const Money& WINAPI B_GetAccountMoneyInvestedShort(const Observable* account = NULL);
const Money& WINAPI B_GetAccountCapNet(const Observable* account = NULL);
const Money& WINAPI B_GetAccountMarkedNetPnl(const Observable* account = NULL);
const Money& WINAPI B_GetAccountNetPnl(const Observable* account = NULL);

const Money& WINAPI B_GetAccountMaxMoneyInvested(const Observable* account = NULL);
const Money& WINAPI B_GetAccountMaxMoneyInvestedLong(const Observable* account = NULL);
const Money& WINAPI B_GetAccountMaxMoneyInvestedShort(const Observable* account = NULL);

unsigned int WINAPI B_GetAccountSharesTraded(const Observable* account = NULL);
unsigned int WINAPI B_GetAccountSharesTradedLong(const Observable* account = NULL);
unsigned int WINAPI B_GetAccountSharesTradedShort(const Observable* account = NULL);
unsigned int WINAPI B_GetAccountSharesTradedSellLong(const Observable* account = NULL);
unsigned int WINAPI B_GetAccountSharesTradedSellShort(const Observable* account = NULL);

const Money& WINAPI B_GetAccountMoneyTradedLong(const Observable* account = NULL);
const Money& WINAPI B_GetAccountMoneyTradedShort(const Observable* account = NULL);
const Money& WINAPI B_GetAccountMoneyTradedSellLong(const Observable* account = NULL);
const Money& WINAPI B_GetAccountMoneyTradedSellShort(const Observable* account = NULL);
const Money& WINAPI B_GetAccountMoneyTraded(const Observable* account = NULL);

unsigned int WINAPI B_GetAccountTotalShares(const Observable* account = NULL);
unsigned int WINAPI B_GetAccountTotalSharesLong(const Observable* account = NULL);
unsigned int WINAPI B_GetAccountTotalSharesShort(const Observable* account = NULL);

unsigned int WINAPI B_GetAccountTotalSharesProper(const Observable* account = NULL);
unsigned int WINAPI B_GetAccountTotalSharesLongProper(const Observable* account = NULL);
unsigned int WINAPI B_GetAccountTotalSharesShortProper(const Observable* account = NULL);

int WINAPI B_GetAccountTotalSharesLongInventoryOffset(const Observable* account = NULL);
int WINAPI B_GetAccountTotalSharesShortInventoryOffset(const Observable* account = NULL);

unsigned int WINAPI B_GetAccountTotalInventoryShares(const Observable* account = NULL);
unsigned int WINAPI B_GetAccountTotalInventorySharesLong(const Observable* account = NULL);
unsigned int WINAPI B_GetAccountTotalInventorySharesShort(const Observable* account = NULL);

unsigned int WINAPI B_GetAccountSharesTradedOvernight(const Observable* account = NULL);
const Money& WINAPI B_GetAccountMoneyTradedLongOvernight(const Observable* account = NULL);
const Money& WINAPI B_GetAccountMoneyTradedShortOvernight(const Observable* account = NULL);
const Money& WINAPI B_GetAccountMoneyTradedOvernight(const Observable* account = NULL);

void WINAPI B_DestroyAccount(const Observable* account = NULL);
void WINAPI B_DestroyAllAccounts();
unsigned int WINAPI B_GetAccountCount();
void WINAPI B_ReconnectAllAccounts(const char* ip, unsigned short port, const char* localIp, const char* executorIp, unsigned short executorPort, const char* executorLocalIp, bool simulation, bool multicast, bool compression, unsigned int orderLoadMode, bool cancelAllOnDisconnect);

const StockBase* WINAPI B_FindStockHandle(const char* symbol);
const StockBase* WINAPI B_GetStockHandle(const char* symbol);
void WINAPI B_DeleteStock(StockBase* stockHandle);
bool WINAPI B_IsStockValid(const StockBase* stockHandle);
bool WINAPI B_IsStockLoaded(const StockBase* stockHandle);

const StockBase* WINAPI B_GetSecurityHandle(const char* symbol);
const StockBase* WINAPI B_FindSecurityHandle(const char* symbol);

MarketIndex* WINAPI B_GetIndex(const char* symbol);
MarketIndex* WINAPI B_FindIndex(const char* symbol, bool exact = true);

void WINAPI B_GetStockLevelPrice(const StockBase* stockHandle, bool side, unsigned int level, Money& price, bool price2DecPlaces = true, unsigned int block = 100);
Observable* WINAPI B_GetLevel1(const StockBase* stockHandle);
Observable* WINAPI B_GetAggregatedBook(const StockBase* stockHandle, unsigned short bookId);
Observable* WINAPI B_GetExpandedBook(const StockBase* stockHandle, unsigned short bookId);
Observable* WINAPI B_GetAggregatedBookByMmid(const StockBase* stockHandle, const char* mmid);
Observable* WINAPI B_GetExpandedBookByMmid(const StockBase* stockHandle, const char* mmid);
unsigned short WINAPI B_GetBookId(const Observable* book);
Observable* WINAPI B_GetLevel2(const StockBase* stockHandle);
Observable* WINAPI B_GetChart(const StockBase* stockHandle);
const char* WINAPI B_GetStockSymbol(const StockBase* stockHandle);
const char* WINAPI B_GetStockDescription(const StockBase* stockHandle);
char WINAPI B_GetStockPrimaryExchange(const StockBase* stockHandle);
char WINAPI B_GetStockAttributes(const StockBase* stockHandle);
//bool WINAPI B_IsStockRegSho(const StockBase* stockHandle);
bool WINAPI B_IsStockTest(const StockBase* stockHandle);
bool WINAPI B_IsStockSmallCap(const StockBase* stockHandle);
bool WINAPI B_IsStockHalted(const StockBase* stockHandle);
const Money& WINAPI B_GetStockTodaysClosePrice(const StockBase* stockHandle);

const Money& WINAPI B_GetStockNasdaqCurrentReferencePrice(const StockBase* stockHandle);
const Money& WINAPI B_GetStockNasdaqNearIndicativeClearingPrice(const StockBase* stockHandle);
const Money& WINAPI B_GetStockNasdaqFarIndicativeClearingPrice(const StockBase* stockHandle);

unsigned int WINAPI B_GetStockNasdaqImbalanceMatchedShares(const StockBase* stockHandle);
unsigned int WINAPI B_GetStockNyseImbalanceMatchedShares(const StockBase* stockHandle);

bool WINAPI B_IsNysQuoteOpen(const StockBase* stockHandle);
bool WINAPI B_IsAseQuoteOpen(const StockBase* stockHandle);
bool WINAPI B_IsStockExchangeOpen(const StockBase* stockHandle);

unsigned int WINAPI B_GetBookQuoteCount(const Observable* book, bool side);
unsigned int WINAPI B_GetBookQuoteCountById(const StockBase* stockHandle, unsigned short bookId, bool aggregated, bool side);
bool WINAPI B_IsBookEmpty(const Observable* book, bool side);
bool WINAPI B_IsBookBothSidesEmpty(const Observable* book);
bool WINAPI B_IsBookIdEmpty(const StockBase* stockHandle, unsigned short bookId, bool aggregated, bool side);
bool WINAPI B_IsBookIdBothSidesEmpty(const StockBase* stockHandle, unsigned short bookId, bool aggregated);

unsigned int WINAPI B_FindBookQuoteSize(const Observable* book, const Money& price, bool side);
unsigned int WINAPI B_FindBookQuoteSizeById(const StockBase* stockHandle, unsigned short bookId, const Money& price, bool side);

void* WINAPI B_CreateBookBlockIterator(const StockBase* stockHandle, unsigned short bookId, bool side, bool price2DecPlaces, unsigned int linesIntegrated = 0xFFFFFFFF, unsigned int block = 100);
void* WINAPI B_CreateMmBookBlockIterator(const StockBase* stockHandle, unsigned int mmid, bool side, bool price2DecPlaces, unsigned int linesIntegrated = 0xFFFFFFFF, unsigned int block = 100);

void* WINAPI B_CreateBookIterator(Observable* bookHandle, bool side);
void* WINAPI B_CreateLevel2Iterator(Observable* bookHandle, bool side, unsigned int ecnFilter = 0xFFFFFFFF, bool ecnsOnly = false);

void WINAPI B_Level2IteratorSetEcnsOnly(void* iteratorHandle, bool ecnsOnly, Observer* observerToAddToBooks);
void WINAPI B_Level2IteratorSetPrice2DecPlaces(void* iteratorHandle, bool price2DecPlaces);

void* WINAPI B_CreateMultiBookIterator(const StockBase* stockHandle, bool side, bool ecnsOnly, unsigned int ecnFilter, unsigned int mmLines, bool aggregated, Observer* observerToAddToBooks);
void WINAPI B_MultiBookIteratorSetMmLines(void* iteratorHandle, unsigned int mmLines, Observer* observerToAddToBooks, bool force = false);
void WINAPI B_MultiBookIteratorSetEcnFilter(void* iteratorHandle, unsigned int ecnFilter, unsigned int mmLines, Observer* observerToAddToBooks);
void WINAPI B_MultiBookIteratorSetEcnVisible(void* iteratorHandle, unsigned short bookId, bool visible, Observer* observerToAddToBooks);
void WINAPI B_MultiBookIteratorSetAggregated(void* iteratorHandle, bool aggregated, Observer* observerToAddToBooks);

void* WINAPI B_CreateMultiBookBlockIterator(const StockBase* stockHandle, bool side, bool ecnsOnly, bool price2DecPlaces, unsigned int ecnFilter, unsigned int mmLines, Observer* observerToAddToBooks, unsigned int block = 100);

void* WINAPI B_CreateLevel2AndBookIterator(const StockBase* stockHandle, bool side, bool ecnsOnly, bool price2DecPlaces, const unsigned int* linesIntegrated, unsigned int allMmLinesIntegrated, Observer* observerToAddToBooks, unsigned int block = 100);
void WINAPI B_Level2AndBookIteratorSetLinesIntegrated(void* iteratorHandle, const unsigned int* linesIntegrated, unsigned int allMmLinesIntegrated, Observer* observerToAddToBooks);
void WINAPI B_Level2AndBookIteratorSetDisplayLevel2Ecn(void* iteratorHandle, unsigned short bookId, bool display);
void WINAPI B_Level2AndBookIteratorSetDisplayLevel2AllEcns(void* iteratorHandle, bool display);
bool WINAPI B_Level2AndBookIteratorIsDisplayLevel2Ecn(void* iteratorHandle, unsigned short bookId);
bool WINAPI B_IsShowNysWithNyb();
void WINAPI B_SetShowNysWithNyb(bool nysWithNyb);

bool WINAPI B_IsEcnBook(const StockBase* stockHandle, const Observable* observable, bool aggregated);

void WINAPI B_StartIteration(void* iteratorHandle, unsigned int startIndex = 0);
const BookEntry* WINAPI B_GetNextBookEntry(void* iteratorHandle);

unsigned int WINAPI B_GetIterationSize(void* iteratorHandle);
void WINAPI B_DestroyIterator(void* iteratorHandle);
void WINAPI B_IncrementIterator(void* iteratorHandle, unsigned int size);

const BookEntry* WINAPI B_FindLevel2QuoteByMmid(Observable* level2Handle, bool side, const char* mmid);

Observable* WINAPI B_GetPrints(const StockBase* stockHandle);
Observable* WINAPI B_GetBookExecutions(const StockBase* stockHandle, unsigned short bookId);
Observable* WINAPI B_GetBookCancels(const StockBase* stockHandle, unsigned short bookId);
//Observable* WINAPI B_GetBookFyis(const StockBase* stockHandle, unsigned short bookId);
const Transaction* WINAPI B_GetPrintAt(const StockBase* stockHandle, PrintSourse printSource, unsigned int index, bool reverseChronologicalOrder = true);
const Transaction* WINAPI B_GetBookExecutionAt(const StockBase* stockHandle, unsigned short bookId, unsigned int index, bool reverseChronologicalOrder = true);
const Transaction* WINAPI B_GetBookCancelAt(const StockBase* stockHandle, unsigned int bookId, unsigned int index, bool reverseChronologicalOrder = true);
//const Transaction* WINAPI B_GetBookFyiAt(const StockBase* stockHandle, unsigned int bookId, unsigned int index, bool reverseChronologicalOrder = true);

void* WINAPI B_CreatePrintsIterator(const StockBase* stock, bool reverse = true);

void* WINAPI B_CreatePrintsAndBookExecutionsIterator(const StockBase* stockHandle, unsigned int sourceFilter, unsigned int ecnFilter, bool mms, Observer* o = NULL, bool reverse = true);
void* WINAPI B_CreateExecutionsAndCancelsIterator(const StockBase* stockHandle, unsigned int ecnFilter, bool mms, bool cancels, Observer* o = NULL, bool reverse = true);

//void WINAPI B_TransactionIteratorSetFilter(void* iterator, unsigned int filter);
//void WINAPI B_AddPrintsToTransactionIterator(void* iterator, const StockBase* stockHandle, Observer* o = NULL, bool reverseChronologicalOrder = true);
//void WINAPI B_AddCancelsToTransactionIterator(void* iterator, const StockBase* stockHandle, Observer* o = NULL, bool reverseChronologicalOrder = true);
//void WINAPI B_AddBookExecutionsToTransactionIterator(void* iterator, const StockBase* stockHandle, unsigned int ecnFilter = 0xFFFFFFFF, Observer* o = NULL, bool reverseChronologicalOrder = true);
//void WINAPI B_RemovePrintsFromTransactionIterator(void* iterator, const StockBase* stockHandle, Observer* o = NULL);
//void WINAPI B_SetBookExecutionsToTransactionIterator(void* iterator, const StockBase* stockHandle, unsigned int ecnFilter = 0xFFFFFFFF, Observer* o = NULL, bool reverse = true);
//void WINAPI B_RemoveCancelsFromTransactionIterator(void* iterator, const StockBase* stockHandle, Observer* o = NULL);
//void WINAPI B_RemoveBookExecutionsFromTransactionIterator(void* iterator, const StockBase* stockHandle, unsigned int ecnFilter = 0xFFFFFFFF, Observer* o = NULL);
void WINAPI B_ClearTransactionIterator(void* iterator, Observer* o = NULL);


const Transaction* WINAPI B_GetNextPrintsEntry(void* iteratorHandle);

const char* WINAPI B_GetHostName();
const char* WINAPI B_GetIpAddresses();
const unsigned int* WINAPI B_GetNumericIpAddresses();
bool WINAPI B_GetModulePath(HINSTANCE hInstance, char* pathbuf, unsigned int bufsize);
int WINAPI B_CompareDates(const SYSTEMTIME& t1, const SYSTEMTIME& t2);
bool WINAPI B_LogMessage(const char* prefix, const char* text, Observable* account = NULL, bool controlNumber = false, bool important = false);
//void WINAPI B_SetEraseMMsAfterClose(bool erase);
int WINAPI B_ConvertPriceToDecimal(int price, short& fraction);

unsigned int WINAPI B_GetBidTickStatus(const Observable* level1);
const MoneySize& WINAPI B_GetBid(const Observable* level1);
const MoneySize& WINAPI B_GetAsk(const Observable* level1);

const Money& WINAPI B_GetPreMarketIndicationBid(const Observable* level1);
const Money& WINAPI B_GetPreMarketIndicationAsk(const Observable* level1);

const MoneySize& WINAPI B_GetLastTrade(const Observable* level1);
time32 WINAPI B_GetLastTradeTime(const Observable* level1);
__int64 WINAPI B_GetVolume(const Observable* level1);
const Money& WINAPI B_GetOpenPrice(const Observable* level1);
const Money& WINAPI B_GetClosePrice(const Observable* level1);
const Money& WINAPI B_GetIntraDayHigh(const Observable* level1);
const Money& WINAPI B_GetIntraDayLow(const Observable* level1);
const Money& WINAPI B_GetNetChange(const Observable* level1);
const Money& WINAPI B_GetOpenNetChange(const Observable* level1);
int WINAPI B_GetNyseImbalance(const StockBase* stockHandle);
char WINAPI B_GetNyseImbalanceType(const StockBase* stockHandle);
int WINAPI B_GetNasdaqImbalance(const StockBase* stockHandle);
char WINAPI B_GetNasdaqImbalanceType(const StockBase* stockHandle);

LONGLONG WINAPI B_GetServerTimeDifference();
unsigned int WINAPI B_GetMarketStatus();
const Transaction* WINAPI B_GetLastPrint(const StockBase* stockHandle);
unsigned int WINAPI B_GetLastPrintStatus(const StockBase* stockHandle);
const Transaction* WINAPI B_GetLastBookExecution(const StockBase* stockHandle, unsigned short bookId);
unsigned int WINAPI B_GetLastBookExecutionStatus(const StockBase* stockHandle, unsigned short bookId);
const Transaction* WINAPI B_GetAllBooksAndNasdaqLastPrint(const StockBase* stockHandle);

unsigned short WINAPI B_GetEcnIdByName(const char* mmName);
unsigned short WINAPI B_GetEcnIdByNameId(unsigned int mmid);

void* WINAPI B_CreateAllPositionIterator(const Observable* account = NULL);
void* WINAPI B_CreatePositionIterator(unsigned int flags, unsigned int securityFilter, const Observable* account = NULL);
const Position* WINAPI B_GetNextPosition(void* iteratorHandle);
Position* WINAPI B_FindPosition(const char* symbol, const Observable* account = NULL);

void* WINAPI B_CreateStagingOrderIterator(unsigned int orderStatusFlags, const Observable* account = NULL);
void* WINAPI B_CreatePositionStagingOrderIterator(unsigned int orderStatusFlags, const Position* position);
StagingOrder* WINAPI B_GetNextStagingOrder(void* iteratorHandle);
StagingOrder* WINAPI B_FindStagingOrder(unsigned int id, const Observable* account = NULL);

void* WINAPI B_CreateOrderIterator(unsigned int orderStatusFlags, unsigned int securutyType, const Observable* account = NULL);
void* WINAPI B_CreatePositionOrderIterator(unsigned int orderStatusFlags, const Position* position);
Order* WINAPI B_GetNextOrder(void* iteratorHandle);
Order* WINAPI B_FindOrder(unsigned int id, const Observable* account = NULL);

Order* WINAPI B_CancelLastOrder(unsigned int flags, unsigned int securityFilter, const char* destination = NULL, bool includeSmartOrders = false, bool includeChildOrders = true, const Observable* account = NULL);
Order* WINAPI B_CancelOldestOrder(unsigned int flags, unsigned int securityFilter, const char* destination = NULL, bool includeSmartOrders = false, bool includeChildOrders = true, const Observable* account = NULL);
void WINAPI B_CancelAllOrdersInAllAccounts(unsigned int flags, unsigned int securityFilter, const char* destination = NULL, bool includeSmartOrders = false, bool includeChildOrders = true);
void WINAPI B_CancelAllOrders(unsigned int flags, unsigned int securityFilter, const char* destination = NULL, bool includeSmartOrders = false, bool includeChildOrders = true, const Observable* account = NULL);
void WINAPI B_CancelAllStockOrders(const char* symbol, unsigned int flags, const char* destination = NULL, bool includeSmartOrders = false, bool includeChildOrders = true, const Observable* account = NULL);
void WINAPI B_CancelAllStockEcnOrders(const char* symbol, unsigned int flags, bool includeSmartOrders = false, bool includeChildOrders = true, const Observable* account = NULL);

void* WINAPI B_CreateExecutionIterator(const Observable* account = NULL);
void* WINAPI B_CreatePositionExecutionIterator(const Position* position);
void* WINAPI B_CreateOrderExecutionIterator(const Order* order);//returns 0 if there are no executions, otherwise - the iterator handle that can be used in B_GetNextExecution() and is supposed to be destroyed with B_DestroyIterator() when no longer needed.
void* WINAPI B_CreateErrorExecutionIterator(const Observable* account = NULL);
const Execution* WINAPI B_GetNextExecution(void* iteratorHandle);

void* WINAPI B_CreateAccountIterator();
void* WINAPI B_CreateReceiverAccountIterator(const Observable* receiver);
Observable* WINAPI B_GetNextAccount(void* iteratorHandle);

/*
enum TierSizeCategories
{
	TS_NASDAQ,
    TS_SMALLCAP,
    TS_NYSE,
	TS_AMEX,
	TS_ARCA,
    TS_OTHER
};

unsigned int WINAPI B_GetStockDefaultTierSize(const StockBase* stockHandle, const Observable* account = NULL);
unsigned int WINAPI B_GetDefaultTierSize(unsigned int exchangeId, const Observable* account = NULL);
void WINAPI B_SetDefaultTierSize(unsigned int exchangeId, unsigned int size, Observable* account = NULL);

unsigned int WINAPI B_GetDefaultOrderSizeForStock(const char* symbol, const Observable* account = NULL);
void WINAPI B_SetDefaultOrderSizeForStock(const char* symbol, unsigned int s, Observable* account = NULL);
void WINAPI B_RemoveDefaultOrderSizeForStock(const char* symbol, Observable* account = NULL);
void WINAPI B_ClearDefaultOrderSizeForAllStocks(Observable* account = NULL);
*/
void WINAPI B_SetDefaultTierSizeAllAccounts(unsigned int exchangeId, unsigned int size);
void WINAPI B_SetDefaultOrderSizeForStockAllAccounts(const char* symbol, unsigned int s);
void WINAPI B_RemoveDefaultOrderSizeForStockAllAccounts(const char* symbol);
void WINAPI B_ClearDefaultOrderSizeForAllStocksAllAccounts();
unsigned int WINAPI B_GetExchangeTierSize(unsigned int exchangeId);
unsigned int WINAPI B_GetDefaultOrderSizeForStock(const char* symbol);

//bool WINAPI B_GetInsideLevel2AndBookPrice(const StockBase* stockHandle, bool side, bool ecnsOnly, bool price2DecPlaces, Money& price);
bool WINAPI B_GetInsideLevel2AndBookPrice(const StockBase* stockHandle, bool side, Money& price);
//bool WINAPI B_GetSafeInsidePrice(const StockBase* stockHandle, bool buy, bool side, bool ecnsOnly, bool price2DecPlaces, Money& price);
bool WINAPI B_GetSafeInsidePrice(const StockBase* stockHandle, bool buy, bool side, bool ecnsOnly, Money& price);
bool WINAPI B_GetInsideLevel2Price(const StockBase* stockHandle, bool side, bool mmsOnly, Money& price);
bool WINAPI B_IsOrderLoadingDone(const Observable* account = NULL);
bool WINAPI B_IsExecutionLoadingDone(const Observable* account = NULL);
time_t WINAPI B_GetCurrentServerTime();
time_t WINAPI B_GetApplicationStartServerTime();
time_t WINAPI B_GetCurrentServerTimeAndMilliseconds(unsigned int& milliseconds);
time_t WINAPI B_GetCurrentServerNYTimeAndMilliseconds(unsigned int& milliseconds);
time_t WINAPI B_GetCurrentServerNYTime();
void WINAPI B_GetCurrentServerNYTimeTokens(unsigned int& hour, unsigned int& minute, unsigned int& second);
void WINAPI B_GetNYTimeTokens(time_t time, unsigned int& hour, unsigned int& minute, unsigned int& second);
void WINAPI B_GetCurrentNYTime(SYSTEMTIME& t);

unsigned int WINAPI B_SendOrder(const StockBase* stockHandle,
    char side,
    const char* destination,
    unsigned int size,
    unsigned int visibilityMode,
    unsigned int visibleSize,
    const Money& price,//0 for Market
    const Money* stopPrice,
    const Money* discrtetionaryPrice,
    unsigned int timeInForce,
    bool proactive,
    bool principalOrAgency, //principal - true, agency - false
    char superMontageAlgorithm,
    char oversellHandling,
//    bool delayShortTillUptick,
    unsigned int destinationExchange,// = DE_DEFAULT,
//    const Order** orderSent = NULL,
    Order** orderSent = NULL,
    Observable* account = NULL,
    unsigned int orderIdToReplace = 0,
    bool noShortSellMultiplier = false,
    unsigned int userType = 0,
    const char* userDescription = NULL,
//    Order** orderSentNxSplitSpecialist = NULL,
    Order** orderSentOversellSplitShort = NULL,
//    Order** orderSentSplitShortOnUptick = NULL,
    bool probe = false,
	unsigned __int64 cmta = 0,
	const char* commandName = NULL,
	const char* commandComment = NULL,
	bool iso = false,
	bool institutionalDisclaimerVisible = false,
	OptionOrderStatus ooStatus = OOS_DEFAULT,
	OptionTraderStatus otStatus = OTS_DEFAULT,
	const StagingOrder* stagingOrder = NULL);

Observable* WINAPI B_SendSmartStopOrder(const StockBase* stockHandle,
    char side,
    unsigned int size,
    const Money* priceOffset,//NULL for Stop Market
    const Money& stopPriceOffset,
    bool price2DecPlaces,
    bool ecnsOnlyBeforeAfterMarket,
    bool mmsBasedForNyse,
    unsigned int stopTimeInForce,
    unsigned int timeInForceAfterStopReached,
    const char* postQuoteDestination,
    const char* redirection,
    bool proactive,
    bool principalOrAgency, //principal - true, agency - false
    char superMontageAlgorithm,
    char oversellHandling,
//    bool delayShortTillUptick,
    unsigned int destinationExchange,
    StopTriggerType triggerType,
    bool trailing,
    unsigned int userType,
    const char* userDescription,
	const char* regionalProactiveDestination,
	unsigned int triggerPrintType,
	const Money& triggerPrintOffset,
	bool stopLossOnly,
	Order** orderSent = NULL,
    Observable* account = NULL,
    unsigned int block = 100,
    const Money* from = NULL,
	unsigned __int64 cmta = 0,
	const char* commandName = NULL,
	const char* commandComment = NULL,
	bool institutionalDisclaimerVisible = false,
	OptionOrderStatus ooStatus = OOS_DEFAULT,
	OptionTraderStatus otStatus = OTS_DEFAULT,
	const StagingOrder* stagingOrder = NULL);

void WINAPI B_AddEcn(const char* mm, unsigned int type = 1);


unsigned int WINAPI B_SendSwipeOrder(const StockBase* stockHandle,
    char side,
    unsigned int size,
//    bool hidden,
    bool swipeAllEcns,
    bool swipeAllMarketMakers,
    bool ecnsOnly,//for after market. Ignores all market makers, even in inclusionMMECNList. Price is based on ECN prices only.
    bool mmsBasedForNyse,
    bool price2DecPlaces,
    const Money& priceOffset,
    unsigned int maxOrderCount,
    unsigned int timeInForce,
    const char* redirection,//NULL - no redirection
//    bool sendSoesForMMs,
    bool proactive,
	bool proactiveIfNotIso,
    bool principalOrAgency, //principal - true, agency - false
    char superMontageAlgorithm,
    char oversellHandling,
//    bool delayShortTillUptick,
    unsigned int visibilityMode,
    const Money* discretionaryDelta,
    const Money* soesLimitThrough,
    unsigned int destinationExchange,
    const char* inclusionMMECNList = NULL,
    const char* exclusionMMECNList = NULL,
    const Money* sendTheRestToSoesPriceOffset = NULL, //NULL if not to send the rest to SOES
    bool sendTheRestToSoesMarketOrder = false,
    bool basePriceOnOppositeSide = true,
    bool sendAvailableSizeOnly = true,
    bool sendQuotedPrice = true,
    Order** orderSentBuf = NULL,
    unsigned int orderSentBufLen = 0,
    Observable* account = NULL,
    unsigned int block = 100,
	unsigned __int64 cmta = 0,
	const char* commandName = NULL,
	const char* commandComment = NULL,
	bool institutionalDisclaimerVisible = false,
	OptionOrderStatus ooStatus = OOS_DEFAULT,
	OptionTraderStatus otStatus = OTS_DEFAULT,
	const StagingOrder* stagingOrder = NULL);

Observable* WINAPI B_SendSmartOrder(const StockBase* stockHandle,
    char side,
    unsigned int size,
//    bool hidden,
    bool swipeAllEcns,
    bool swipeAllMarketMakers,
    bool ecnsOnly,//for after market. Ignores all market makers, even in inclusionMMECNList. Price is based on ECN prices only.
    bool mmsBasedForNyse,
    bool price2DecPlaces,
    const Money& priceOffset,
    const char* redirection,//NULL - no redirection
//    bool sendSoesForMMs,
    unsigned int timeInForce,
    unsigned int destinationExchange,
    bool proactive,
	bool proactiveIfNotIso,
	bool swipePriceLevel,
    bool principalOrAgency, //principal - true, agency - false
    char superMontageAlgorithm,
    char oversellHandling,
//    bool delayShortTillUptick,
    unsigned int visibilityMode,
    const Money* discretionaryDelta,
    const Money* soesLimitThrough,
    const char* inclusionMMECNList,// = NULL,
    const char* exclusionMMECNList,// = NULL,
    const Money* sendTheRestToSoesPriceOffset, //NULL if not to send the rest to SOES
    bool sendTheRestToSoesMarketOrder,
    unsigned int userType,
    const char* userDescription,
	const char* regionalProactiveDestination,
	const char* finalProactiveDestination,
    bool basePriceOnOppositeSide = true,
	const char* postDestination = NULL,
	unsigned int postVisibleSize = 0,
	unsigned int postTif = TIF_DAY,
    Order** orderSent = NULL,
    Observable* account = NULL,
    unsigned int block = 100,
	unsigned __int64 cmta = 0,
	const char* commandName = NULL,
	const char* commandComment = NULL,
	bool institutionalDisclaimerVisible = false,
	OptionOrderStatus ooStatus = OOS_DEFAULT,
	OptionTraderStatus otStatus = OTS_DEFAULT,
	const StagingOrder* stagingOrder = NULL);

Observable* WINAPI B_SendProbeOrder(const StockBase* stockHandle,
    char side,
    unsigned int size,
    bool sizeMinTrueOrMaxFalse,
    bool sizeIgnoreIfZero,
    unsigned int visibleSize,
    bool baseOppositeSide,
    bool ecnsOnly,//for after market. Ignores all market makers, even in inclusionMMECNList. Price is based on ECN prices only.
    bool price2DecDigits,
    const Money& priceOffset,
    const Money* limitPriceOffset,//not to exceed a specific price when sending pegged orders
    unsigned int timeInForce,
//    unsigned int destinationExchange,
//    unsigned int levelToPeg,
    bool proactive,
    bool principalOrAgency, //principal - true, agency - false
    char superMontageAlgorithm,
    char oversellHandling,
    unsigned int visibilityMode,
    const Money* discretionaryDelta,
	bool altPriceEdit,

    unsigned int repeatTime,
    unsigned int probeTif,

    const char* destinationList,
    unsigned int userType,
    const char* userDescription,
	const char* regionalProactiveDestination,
    Order** orderSent = NULL,
    Observable* account = NULL,
    unsigned int block = 100,
	unsigned __int64 cmta = 0,
	const char* commandName = NULL,
	const char* commandComment = NULL,
	bool institutionalDisclaimerVisible = false,
	OptionOrderStatus ooStatus = OOS_DEFAULT,
	OptionTraderStatus otStatus = OTS_DEFAULT,
	const StagingOrder* stagingOrder = NULL);

Observable* WINAPI B_SendPeggedOrder(const StockBase* stockHandle,
    char side,
    unsigned int size,
    bool sizeMinTrueOrMaxFalse,
    bool sizeIgnoreIfZero,
    unsigned int visibleSize,
    bool baseOppositeSide,
    bool ecnsOnly,//for after market. Ignores all market makers, even in inclusionMMECNList. Price is based on ECN prices only.
    bool price2DecPlaces,
    const Money& priceOffset,
    const Money* limitPriceOffset,//not to exceed a specific price when sending pegged orders
    unsigned int timeInForce,
    unsigned int levelToPeg,
    unsigned int destinationExchange,
    bool proactive,
    bool principalOrAgency, //principal - true, agency - false
    char superMontageAlgorithm,
    char oversellHandling,
    unsigned int visibilityMode,
    const Money* discretionaryDelta,
	bool altPriceEdit,
    const char* inclusionMMECNList,
    const char* toFollowList,
    const char* probeList,
    unsigned int probeSize,
    unsigned int noProbeSizeLeft,
    bool noOverfill,
    unsigned int probeRepeat,
    bool probePriceBaseSame,
    int probePriceOffset,
    const char* oddLotMarket,
    unsigned int userType,
    const char* userDescription,
	const char* regionalProactiveDestination,
//    bool basePriceOnOppositeSide,
    Order** orderSent = NULL,
    Observable* account = NULL,
    unsigned int block = 100,
	unsigned __int64 cmta = 0,
	const char* commandName = NULL,
	const char* commandComment = NULL,
	bool institutionalDisclaimerVisible = false,
	OptionOrderStatus ooStatus = OOS_DEFAULT,
	OptionTraderStatus otStatus = OTS_DEFAULT,
	const StagingOrder* stagingOrder = NULL);

enum TvolCurve
{
	CURVE_TVOL,
	CURVE_TWAP,
	CURVE_VWAP,
	CURVE_CWAP,

	CURVE_COUNT
};

Observable* B_SendTvolOrder(const StockBase* stockHandle,
    char side,
    unsigned int size,
    bool sizeMinTrueOrMaxFalse,
    bool sizeIgnoreIfZero,
    unsigned int visibleSize,
    bool baseOppositeSide,
    bool ecnsOnly,//for after market. Ignores all market makers, even in inclusionMMECNList. Price is based on ECN prices only.
    bool price2DecPlaces,
    const Money& priceOffset,
    const Money* limitPriceOffset,//not to exceed a specific price when sending orders
    bool throughMarket,
    const Money& swipeThrough,
    unsigned int timeInForceFrom,
    unsigned int timeInForceTo,
    bool proactive,
    bool principalOrAgency, //principal - true, agency - false
    char superMontageAlgorithm,
    char oversellHandling,
    unsigned int visibilityMode,
    const Money* discretionaryDelta,
	bool altPriceEdit,
    const char* inclusionMMECNList,
    unsigned int volumeFrom,
    unsigned int volumeTo,
    unsigned int minPercent,
    unsigned int maxPercent,
    const Money& minPriceOffset,
    const Money& maxPriceOffset,

    unsigned int userType,
    const char* userDescription,
	const char* regionalProactiveDestination,
	const char* finalProactiveDestination,
	unsigned int initialMarketOrderSize,
	unsigned int maxPrintSize,
	unsigned int start,
	unsigned int end,
	unsigned int duration,
	TvolCurve tvolCurve,
	unsigned int curveStart,
	unsigned int curveGranularity,
	unsigned int curvePointCount,
	const unsigned int* curve,
	Order** orderSent = NULL,
    Observable* account = NULL,
    unsigned int block = 100,
	unsigned __int64 cmta = 0,
	const char* commandName = NULL,
	const char* commandComment = NULL,
	bool institutionalDisclaimerVisible = false,
	OptionOrderStatus ooStatus = OOS_DEFAULT,
	OptionTraderStatus otStatus = OTS_DEFAULT,
	const StagingOrder* stagingOrder = NULL);

Observable* WINAPI B_SendSpiderOrder(const StockBase* stockHandle,
    char side,
    unsigned int size,
    bool sizeMinTrueOrMaxFalse,
    bool sizeIgnoreIfZero,
    bool baseOppositeSide,
//    bool ecnsOnly,//for after market. Ignores all market makers, even in inclusionMMECNList. Price is based on ECN prices only.
    bool price2DecDigits,
    const Money& priceOffset,
    const Money* limitPriceOffset,//not to exceed a specific price when sending pegged orders
    unsigned int timeInForce,
    bool proactive,
    char superMontageAlgorithm,
    char oversellHandling,
//    unsigned int visibilityMode,
//    const Money* discretionaryDelta,
//	bool altPriceEdit,

    unsigned int m_tifEcn,
    unsigned int m_tifSoes,
    unsigned int m_tifNys,
    unsigned int m_tifAse,
    unsigned int m_tifRegional,

    unsigned int userType,
    const char* userDescription,
	const char* regionalProactiveDestination,
    Order** orderSent = NULL,
    Observable* account = NULL,
    unsigned int block = 100,
	unsigned __int64 cmta = 0,
	const char* commandName = NULL,
	const char* commandComment = NULL,
	bool institutionalDisclaimerVisible = false,
	OptionOrderStatus ooStatus = OOS_DEFAULT,
	OptionTraderStatus otStatus = OTS_DEFAULT,
	const StagingOrder* stagingOrder = NULL);

Observable* WINAPI B_SendPassiveOrder(const StockBase* stockHandle,
    char side,
    unsigned int size,
    bool swipeAllEcns,
    bool swipeAllMarketMakers,
    bool ecnsOnly,//for after market. Ignores all market makers, even in inclusionMMECNList. Price is based on ECN prices only.
    bool mmsBasedForNyse,
    bool price2DecPlaces,
    const Money& priceOffset,
    const char* redirection,//NULL - no redirection
//    bool sendSoesForMMs,
    unsigned int timeInForce,
    unsigned int destinationExchange,
    bool proactive,
    bool principalOrAgency, //principal - true, agency - false
    char superMontageAlgorithm,
    char oversellHandling,
//    bool delayShortTillUptick,
    unsigned int userType,
    const char* userDescription,
	const char* regionalProactiveDestination,
    Order** orderSent = NULL,
    const char* inclusionMMECNList = NULL,
    const char* exclusionMMECNList = NULL,
    Observable* account = NULL,
    unsigned int block = 100,
	unsigned __int64 cmta = 0,
	const char* commandName = NULL,
	const char* commandComment = NULL,
	bool institutionalDisclaimerVisible = false,
	OptionOrderStatus ooStatus = OOS_DEFAULT,
	OptionTraderStatus otStatus = OTS_DEFAULT,
	const StagingOrder* stagingOrder = NULL);

const char* WINAPI B_GetEcnNameById(unsigned short id);
const MoneySize& WINAPI B_GetLevel2BestBid(const StockBase* stockHandle);
const MoneySize& WINAPI B_GetLevel2BestAsk(const StockBase* stockHandle);
const StockBase* WINAPI B_GetPositionStock(const Position* pos);

void WINAPI B_GetStockBookBestQuote(bool side, MoneySize& bestQuote, const StockBase* stockHandle, unsigned short bookId, bool roundedTo2Digits, unsigned int minSize = 100, bool zeroIfInsufficientSize = false);
void WINAPI B_GetBookBestQuote(bool side, MoneySize& bestQuote, const Observable* book, bool roundedTo2Digits, unsigned int minSize = 100, bool zeroIfInsufficientSize = false);

void* WINAPI B_CreateIndexIterator();
const MarketIndex* WINAPI B_GetNextIndex(void* iteratorHandle);

Position* WINAPI B_SetPhantomShares(const char* symbol, int size, const Observable* account = NULL);
Position* WINAPI B_IncrementPhantomShares(const char* symbol, int size, const Observable* account = NULL);

unsigned int WINAPI B_GetCountOptionOrdersPendingLong(const Observable* account = NULL);
unsigned int WINAPI B_GetCountOptionOrdersPendingShort(const Observable* account = NULL);
unsigned int WINAPI B_GetCountOptionOrdersPending(const Observable* account = NULL);

unsigned int WINAPI B_GetCountOrdersPendingLong(const Observable* account = NULL);
unsigned int WINAPI B_GetCountOrdersPendingShort(const Observable* account = NULL);
unsigned int WINAPI B_GetCountOrdersPending(const Observable* account = NULL);
unsigned int WINAPI B_GetCountOrdersSmart(const Observable* account = NULL);
unsigned int WINAPI B_GetCountOrdersPendingAndSmart(const Observable* account = NULL);
bool WINAPI B_IsServerTimeSynchronized();
void WINAPI B_CopySymbol(char dest[LENGTH_SYMBOL], const char* src);

const ChartPoint* WINAPI B_GetChartPoint(const Observable* chart, unsigned int minute);
unsigned int WINAPI B_GetChartStartMinute(const Observable* chart);
unsigned int WINAPI B_GetChartEndMinute(const Observable* chart);
unsigned int WINAPI B_GetChartSize(const Observable* chart);
void* WINAPI B_CreateChartIterator(const Observable* chart);
const ChartPoint* WINAPI B_GetNextChartPoint(void* iteratorHandle);
const ChartPoint* WINAPI B_GetLastChartPoint(const Observable* chart);

const Money& WINAPI B_GetChartMinValue(const Observable* chart);
const Money& WINAPI B_GetChartMaxValue(const Observable* chart);
unsigned int WINAPI B_GetChartMinVolume(const Observable* chart);
unsigned int WINAPI B_GetChartMaxVolume(const Observable* chart);

unsigned short WINAPI B_GetChartDayStartMinute();
unsigned short WINAPI B_GetChartDayEndMinute();
unsigned short WINAPI B_GetChartMarketOpenMinute();
unsigned short WINAPI B_GetChartMarketClosedMinute();

unsigned short WINAPI B_GetMaxDayCharts();
void WINAPI B_GetHistoryChartStartEndDates(const void* chart, unsigned int& startDate, unsigned int& endDate);
const ChartPoint* WINAPI B_GetHistoryChartPoint(const void* chart, unsigned int minute);
unsigned int WINAPI B_GetHistoryChartUnit(const void* chart);
unsigned int WINAPI B_GetHistoryChartStart(const void* chart);
unsigned int WINAPI B_GetHistoryChartEnd(const void* chart);
unsigned int WINAPI B_GetHistoryChartSize(const void* chart);
void* WINAPI B_CreateHistoryChartIterator(const void* chart);
const Money& WINAPI B_GetHistoryChartMinValue(const void* chart);
const Money& WINAPI B_GetHistoryChartMaxValue(const void* chart);
unsigned int WINAPI B_GetHistoryChartMinVolume(const void* chart);
unsigned int WINAPI B_GetHistoryChartMaxVolume(const void* chart);
//const ChartPoint* WINAPI B_GetNextHistoryChartPoint(void* iteratorHandle);

void WINAPI B_LogTraderPnl(const Observable* account = NULL);

bool WINAPI B_UnloadExtension(HINSTANCE dllHandle);
bool WINAPI B_UnloadAllExtensions();
HINSTANCE WINAPI B_LoadExtension(const char* dllPath, bool& newExtensionLoaded, char* errorBuffer = NULL, unsigned int bufSize = 0);
unsigned int WINAPI B_LoadAllExtensions(const char* path);
unsigned int WINAPI B_GetLoadedExtensionCount();

void* WINAPI B_CreateExtensionIterator();
const char* WINAPI B_GetNextExtension(void* iteratorHandle, HINSTANCE* hinstance = NULL);
unsigned int WINAPI B_GetExtensionCount();
/*
BasketItem* WINAPI B_FindBasket(const char* name);
const BasketItem* WINAPI B_GetNextUsedByBasket(unsigned int usedByPathIterator);

struct BasketItemStruct
{
    char* m_name;
    unsigned int m_count;
};

struct BasketStruct
{
    char* m_name;
    unsigned int m_itemCount;
    BasketItemStruct* m_items;
};

BasketItem* WINAPI B_SetBasket(const struct BasketStruct& basketContent);
BasketItem* WINAPI B_RemoveBasket(const char* name);
unsigned int WINAPI B_CreateBasketIterator();
const BasketItem* WINAPI B_GetNextBasket(void* iteratorHandle);
unsigned int WINAPI B_CreateBasketItemIterator(BasketItem* basket);
BasketItem* WINAPI B_GetNextBasketItem(void* iteratorHandle, unsigned int* count);
*/
unsigned int WINAPI B_IsMarketableDestination(const char* destination);
unsigned int WINAPI B_IsEcn(const char* marketMaker);
bool WINAPI B_IsDirectEcn(const char* marketMaker);

bool WINAPI B_IsEcnsOnly();
bool WINAPI B_IsEcnsOnlyBeforeAfterMarket();
bool WINAPI B_SetEcnsOnlyBeforeAfterMarket(bool b);
bool WINAPI B_IsEcnsOnlyAlways();
bool WINAPI B_SetEcnsOnlyAlways(bool b);
bool WINAPI B_SetLevel2BestQuoteRoundedTo2Digits(bool b);//returns true if changed
void WINAPI B_SetUserMarketOpenTime(unsigned int secondFromMidnight);//0 if you want to rely on the MARKET_OPEN message from the server
unsigned int WINAPI B_GetUserMarketOpenTime();//0 if you want to rely on the MARKET_OPEN message from the server

Observable* WINAPI B_GetMarketReceiverMulticast();
void WINAPI B_SetMarketReceiverMulticast(Observable* receiver);
Observable* WINAPI B_GetMarketSummaryReceiver();
void WINAPI B_SetMarketSummaryReceiver(Observable* receiver);

unsigned int WINAPI B_GetLoadedStockCount();
void WINAPI B_EndDayCleanup();
/*
void WINAPI B_SetNxOn(bool onoff, Observable* account = NULL);
bool WINAPI B_IsNxOn(Observable* account = NULL);
void WINAPI B_SetNxOnForAllAccounts(bool onoff);
bool WINAPI B_IsNxOnForAllAccounts();
void WINAPI B_SetSplitNxQuantity(unsigned int nxQuantity, Observable* account = NULL);
unsigned int WINAPI B_GetSplitNxQuantity(Observable* account = NULL);
void WINAPI B_SetSplitNxQuantityForAllAccounts(unsigned int nxQuantity);
unsigned int WINAPI B_GetSplitNxQuantityForAllAccounts();

*/
enum HitOwnOrdersPolicy
{
    HOOP_DISALLOW,
    HOOP_CANCEL_AND_SEND,
    HOOP_ALLOW,
};

HitOwnOrdersPolicy WINAPI B_GetHitOwnOrdersPolicyConstraint(const Observable* account = NULL);
void WINAPI B_SetHitOwnOrdersPolicy(HitOwnOrdersPolicy hoop, Observable* account = NULL);
HitOwnOrdersPolicy WINAPI B_GetHitOwnOrdersPolicy(const Observable* account = NULL);
void WINAPI B_SetDefaultHitOwnOrdersPolicy(HitOwnOrdersPolicy hoop);
HitOwnOrdersPolicy WINAPI B_GetDefaultHitOwnOrdersPolicy();

void WINAPI B_EnableLog(bool enable, bool timeWithMilliseconds, bool granularityShare);
bool WINAPI B_IsLogEnabled();
bool WINAPI B_IsLogMilliseconds();
bool WINAPI B_IsLogGranularityShare();
const char* WINAPI B_GetLogFilePath();
void WINAPI B_SetLog(const char* logFileName = "");//NULL - no log, "" - log file name is <traderId>.log, otherwise log file name is <logFileName>.log

void* WINAPI B_CreateStockIterator();
const StockBase* WINAPI B_GetNextStock(void* iteratorHandle);

void* WINAPI B_CreateStockMovementIterator();
const StockMovement* WINAPI B_GetNextStockMovement(void* iteratorHandle);
const StockMovement* WINAPI B_FindStockMovement(const char* symbol);

bool WINAPI B_IsDaylightSavingsTimeOn();

//void WINAPI B_SetLevel2SortOptions(const char* ecnsOnTop, const char* ecnsAboveMMs);//ecnsOnTop in order of priority, ecnsAboveMMs - if "" then none; if NULL then all; Size is used to prioritize ecnsAboveMMs among themselves

void* WINAPI B_CreateEcnIdIterator();
unsigned int WINAPI B_GetNextEcnId(void* iteratorHandle, char* ecnBuffer, unsigned int bufferLength);

//Observable* WINAPI B_GetFyiObservable();
Observable* WINAPI B_GetAllEcnPrintsObservable();
Observable* WINAPI B_GetLargeQuotesObservable();
Observable* WINAPI B_GetNewIndexObservable();

void* WINAPI B_CreateSmartOrderIterator(const Observable* account);
void* WINAPI B_CreateSmartDeadOrderIterator(const Observable* account);
void* WINAPI B_CreatePositionSmartOrderIterator(const Position* position, bool buy);
void* WINAPI B_CreatePositionSmartDeadOrderIterator(const Position* position, bool buy);
const Order* WINAPI B_GetNextSmartOrder(void* iteratorHandle);

const char* WINAPI B_GetPrimaryExchangeName(unsigned int index);
const char* WINAPI B_GetExecutionExchangeName(unsigned int index);
bool WINAPI B_IsExecutionExchangeRegional(unsigned int index);
bool WINAPI B_IsExecutionExchangeRegionalOrNas(unsigned int index);
bool WINAPI B_IsMmidRegional(const char* mmid);
bool WINAPI B_IsMmidRegionalOrNas(const char* mmid);
bool WINAPI B_IsMmidCaes(const char* mmid);
//bool WINAPI B_IsMmidCaesOrSize(const char* mmid);

void WINAPI B_SendInitialRequests();
void WINAPI B_CancelSmartOrders(unsigned int flags, const char* destination, const char* symbol, Observable* account = NULL);//destination = NULL - all destinations, symbol = NULL - all symbols
unsigned int WINAPI B_CancelStockBestWorstOrder(Position* pos, unsigned int flags, const char* destination, bool worst, bool includeChildOrders);
unsigned int WINAPI B_CancelStockAllButBestWorstOrder(Position* pos, unsigned int flags, const char* destination, bool worst, bool includeChildOrders);

unsigned int WINAPI B_GetCurrentDaySecond();

void WINAPI B_CloseAccountPositions(const char* commandName,
	unsigned int securityFilter,
	ClosePositionMethod method,
	bool maxOpenLossOnly,
//	bool principalOrAgency,
	bool longPositions,
	bool shortPositions,
	bool cancelAllOrders,
	Observable* account = NULL);

void WINAPI B_CloseAllPositions(const char* commandName,
	unsigned int securityFilter,
	ClosePositionMethod method,
	bool maxOpenLossOnly,
//	bool principalOrAgency,
	bool longPositions,
	bool shortPositions,
	bool cancelAllOrders);

void* WINAPI B_CreateMarketWithDefaultTifIterator();
const char* WINAPI B_GetNextMarketWithDefaultTif(void* iteratorHandle);

unsigned int WINAPI B_GetDefaultTif(unsigned int stockExchange, const char* market, bool crossingMarket, bool marketOpen, const Observable* account);
bool WINAPI B_SetDefaultTif(unsigned int stockExchange, const char* market, bool crossingMarket, bool marketOpen, unsigned int tif, const Observable* account);

Observable* WINAPI B_GetNasdaqVolumeObservable();
Observable* WINAPI B_GetNyseVolumeObservable();
Observable* WINAPI B_GetAmexVolumeObservable();
Observable* WINAPI B_GetArcaVolumeObservable();
Observable* WINAPI B_GetCboeVolumeObservable();
unsigned __int64 WINAPI B_GetNasdaqVolume();
unsigned __int64 WINAPI B_GetNyseVolume();
unsigned __int64 WINAPI B_GetAmexVolume();
unsigned __int64 WINAPI B_GetArcaVolume();
unsigned __int64 WINAPI B_GetCboeVolume();

bool WINAPI B_IsMarketSummaryPopulationDone();

enum MarketSummarySubscription
{
    MSS_ALLECNPRINTS,
    MSS_LARGEQUOTES,
    MSS_FYI,//can be reused
    MSS_IMBALANCES,
	MSS_INDEXES,
	MSS_MARKETSTATUS,

    MSS_LAST
};

bool WINAPI B_MarketSummarySubscribe(MarketSummarySubscription marketSummarySubscription);
bool WINAPI B_MarketSummaryUnsubscribe(MarketSummarySubscription marketSummarySubscription);

void WINAPI B_AddObserverToBooks(const StockBase* stockHandle, const unsigned int* linesIntegrated, bool aggregated, Observer* o);
void WINAPI B_AddObserverToBooksByFilter(const StockBase* stockHandle, unsigned int filter, bool aggregated, Observer* o);
void WINAPI B_RemoveObserverFromBooks(const StockBase* stockHandle, bool aggregated, Observer* o);

unsigned int WINAPI B_GetMarketSummaryCompatibilityNumber();
void WINAPI B_LoadChart(Observable* chart);

const ChartPoint* WINAPI B_GetStockChartPoint(StockBase* stockHandle, unsigned int minutesAgo);

//returns size up to the specified price of Level2 without direct books
unsigned int WINAPI B_GetLevel2SizeByPrice(const StockBase* stockHandle, bool side, const Money& price, bool roundedTo2Digits, bool ecnsOnly, unsigned int ecnFilter = 0xFFFFFFFF, unsigned int* participants = NULL);
//returns size up to the specified price of a particular ECN Book
unsigned int WINAPI B_GetBookSizeByPrice(const StockBase* stockHandle, unsigned short bookId, bool side, const Money& price, bool roundedTo2Digits);
//returns size up to the specified price of Level2 including direct books
unsigned int WINAPI B_GetStockSizeByPrice(const StockBase* stockHandle, unsigned short bookId, bool side, const Money& price, bool roundedTo2Digits, bool ecnsOnly, unsigned int* participants = NULL);

bool WINAPI B_IsMarketReceiverConnected();
bool WINAPI B_IsMarketSummaryReceiverConnected();
bool WINAPI B_IsMarketSummaryReceiverConnecting();

StockCalc* WINAPI B_CreateStockCalcPriceThrough(StockBase* stockHandle, bool side, bool ecnsOnlyBeforeAfterMarket, bool ecnsOnlyDuringMarket, bool price2DecPlaces, const unsigned int* bookLinesIntegrated, unsigned int mmLines, const Money& throughTo, Observer* o = NULL, unsigned int minSize = 100, unsigned short baseLevel = 0, bool maintainSizeMmidMap = false);
StockCalc* WINAPI B_CreateStockCalcPriceLevel(StockBase* stockHandle, bool side, bool ecnsOnlyBeforeAfterMarket, bool ecnsOnlyDuringMarket, bool price2DecPlaces, const unsigned int* bookLinesIntegrated, unsigned int mmLines, unsigned short levelTo, Observer* o = NULL, unsigned int minSize = 100, bool maintainSizeMmidMap = false);
//StockCalc* WINAPI B_CreateStockCalcTicks(const StockBase* stockHandle, bool side, bool ecnsOnlyBeforeAfterMarket, bool ecnsOnlyDuringMarket, bool price2DecPlaces, unsigned int maxSeconds, Observer* o = NULL, unsigned int minSize = 100, bool maintainSizeMmidMap = false);
StockCalc* WINAPI B_CreateStockCalcBestQuote(StockBase* stockHandle, bool side, bool price2DecPlaces, bool useMmBooks, unsigned int minSize = 100, bool maintainSizeMmidMap = false);
StockCalc* WINAPI B_CreateStockCalcPriceThroughExclusions(StockBase* stockHandle, bool side, bool ecnsOnlyBeforeAfterMarket, bool ecnsOnlyDuringMarket, bool price2DecPlaces, const unsigned int* bookLinesIntegrated, unsigned int mmLines, const Money& throughTo, Observer* o = NULL, unsigned int minSize = 100, unsigned short baseLevel = 0, bool maintainSizeMmidMap = false);
StockCalc* WINAPI B_CreateStockCalcPriceLevelExclusions(StockBase* stockHandle, bool side, bool ecnsOnlyBeforeAfterMarket, bool ecnsOnlyDuringMarket, bool price2DecPlaces, const unsigned int* bookLinesIntegrated, unsigned int mmLines, unsigned short levelTo, Observer* o = NULL, unsigned int minSize = 100, bool maintainSizeMmidMap = false);

void* WINAPI B_CreateStockCalcLevelIterator(StockCalc* stockCalc);
void* WINAPI B_GetStockCalcNextLevelInfo(void* iterator);
const MoneySize& WINAPI B_GetLevelPriceSize(void* levelHandle);
unsigned int WINAPI B_GetLevelParticipantCount(void* levelHandle);
void WINAPI B_GetMmidLevelPriceSize(void* levelHandle, const char* mmid, MoneySize& moneySize);
unsigned int WINAPI B_GetLevelEcnCount(void* levelHandle);
unsigned int WINAPI B_GetLevelEcnSize(void* levelHandle);

void* WINAPI B_CreateLevelSizeIterator(void* levelHandle);
unsigned int WINAPI B_GetLevelNextSize(void* levelSizeIterator, unsigned int& mmidSetCount, void*& mmidSetHandle);
void* WINAPI B_CreateLevelSizeMmidIterator(void* mmidSetHandle);
unsigned int WINAPI B_GetLevelSizeNextMmid(void* levelSizeMmidIterator);

void* WINAPI B_CreateLevelParticipantIterator(void* levelHandle);
unsigned int WINAPI B_GetLevelNextParticipant(void* iterator, unsigned int& size);

void WINAPI B_DestroyStockCalc(StockCalc* stockCalc);
bool WINAPI B_IsMarketStatusInitialized();

const char* WINAPI B_GetDllVersion(bool externally);
const char* WINAPI B_GetDllPath();
const char* WINAPI B_GetDllBuild();

void WINAPI B_SendStockToMainWindow(const char* symbol);

bool WINAPI B_SetStockCalcThroughLimit(StockCalc* stockCalc, const Money& through);
bool WINAPI B_SetStockCalcBaseLevel(StockCalc* stockCalc, unsigned short baseLevel);
bool WINAPI B_SetStockCalcLevelLimit(StockCalc* stockCalc, unsigned int levelTo);
bool WINAPI B_SetStockCalcEcnsOnlyBeforeAfterMarket(StockCalc* stockCalc, bool ecnsOnly);
bool WINAPI B_SetStockCalcEcnsOnlyDuringMarket(StockCalc* stockCalc, bool ecnsOnly);
bool WINAPI B_SetStockCalcMmBookLines(StockCalc* stockCalc, unsigned int mmLines);

bool WINAPI B_AddStockCalcParticipant(StockCalc* stockCalc, const char* mmid);
bool WINAPI B_RemoveStockCalcParticipant(StockCalc* stockCalc, const char* mmid);
bool WINAPI B_SetStockCalcParticipants(StockCalc* stockCalc, const char* mmids);//"XX\0YY\0ZZ\0\0"

bool WINAPI B_AddStockCalcTickMonitor(StockCalc* stockCalc, unsigned int seconds);
bool WINAPI B_RemoveStockCalcTickMonitor(StockCalc* stockCalc, unsigned int seconds);
bool WINAPI B_ReplaceStockCalcTickMonitor(StockCalc* stockCalc, unsigned int oldSeconds, unsigned int newSeconds);
unsigned int WINAPI B_ClearStockCalcTickMonitors(StockCalc* stockCalc);

bool WINAPI B_IsAccountTradingLocked(Observable* account);
void WINAPI B_SetAccountTradingLocked(Observable* account, bool lock);
void WINAPI B_SetAllAccountsTradingLocked(bool lock);

void WINAPI B_SetCancelIllegalSell(Observable* account, bool cancelOversell);
void WINAPI B_SetCancelIllegalSellForAllAccounts(bool cancelOversell);
void WINAPI B_CancelIllegalSell(Observable* account);
void WINAPI B_CancelIllegalSellForAllAccounts();
void WINAPI B_SetCancelOverfill(Observable* account, bool cancelOverfill);
void WINAPI B_SetCancelOverfillForAllAccounts(bool cancelOverfill);

unsigned int WINAPI B_GetCountOrdersUnconfirmed(Observable* account);
unsigned int WINAPI B_GetCountOrdersUnconfirmedBuy(Observable* account);
unsigned int WINAPI B_GetCountOrdersUnconfirmedSell(Observable* account);

unsigned int WINAPI B_GetCountOptionOrdersUnconfirmed(Observable* account);
unsigned int WINAPI B_GetCountOptionOrdersUnconfirmedBuy(Observable* account);
unsigned int WINAPI B_GetCountOptionOrdersUnconfirmedSell(Observable* account);

//bool WINAPI B_IsMarketDirect(const char* mmid);

const Money& WINAPI B_GetAccountMoneyPendingLong(const Observable* account);
const Money& WINAPI B_GetAccountMoneyPendingShort(const Observable* account);
unsigned int WINAPI B_GetAccountSharesPendingLong(const Observable* account);
unsigned int WINAPI B_GetAccountSharesPendingShort(const Observable* account);
unsigned int WINAPI B_GetAccountOversoldShares(const Observable* account);

void WINAPI B_KeepCancelledOrders(bool keep);
bool WINAPI B_IsToKeepCancelledOrders();

void WINAPI B_KeepSmartDeadOrders(bool keep);
bool WINAPI B_IsToKeepSmartDeadOrders();

unsigned int WINAPI B_GetAccountMarketAccessFlags(const Observable* account);
unsigned int WINAPI B_GetMarketAccessFlags();

enum
{ 
	RESP_RESTRICT_ARCA_MD_OTC		= 0x00008000,
	RESP_RESTRICT_ARCA_OE_OTC		= 0x00010000,
	RESP_RESTRICT_ARCA_MD_LISTED	= 0x00020000,
	RESP_RESTRICT_ARCA_OE_LISTED	= 0x00040000,
};

bool WINAPI B_IsPrimaryAccountNasdaqOrderExecutionAvailable();
bool WINAPI B_IsPrimaryAccountNyseOrderExecutionAvailable();
bool WINAPI B_IsPrimaryAccountAmexOrderExecutionAvailable();
bool WINAPI B_IsPrimaryAccountArcaOrderExecutionOtcAvailable();
bool WINAPI B_IsPrimaryAccountArcaOrderExecutionListedAvailable();

bool WINAPI B_IsNasdaqMarketDataAvailable();
bool WINAPI B_IsNasdaqOrderExecutionAvailable(const Observable* account = NULL);
bool WINAPI B_IsNyseMarketDataAvailable();
bool WINAPI B_IsNyseOrderExecutionAvailable(const Observable* account = NULL);
bool WINAPI B_IsAmexMarketDataAvailable();
bool WINAPI B_IsAmexOrderExecutionAvailable(const Observable* account = NULL);
bool WINAPI B_IsNyseBookAvailable();
bool WINAPI B_IsArcaBookOtcAvailable();
bool WINAPI B_IsArcaBookListedAvailable();
//bool WINAPI B_IsArcaOrderExecutionAvailable(const Observable* account = NULL);

bool WINAPI B_IsAccountNasdaqMarketDataAvailable(const Observable* account = NULL);
bool WINAPI B_IsAccountNasdaqOrderExecutionAvailable(const Observable* account = NULL);
bool WINAPI B_IsAccountNyseMarketDataAvailable(const Observable* account = NULL);
bool WINAPI B_IsAccountNyseOrderExecutionAvailable(const Observable* account = NULL);
bool WINAPI B_IsAccountAmexMarketDataAvailable(const Observable* account = NULL);
bool WINAPI B_IsAccountAmexOrderExecutionAvailable(const Observable* account = NULL);
bool WINAPI B_IsAccountNyseBookAvailable(const Observable* account = NULL);
bool WINAPI B_IsAccountArcaBookOtcAvailable(const Observable* account = NULL);
bool WINAPI B_IsAccountArcaOrderExecutionOtcAvailable(const Observable* account = NULL);
bool WINAPI B_IsAccountArcaBookListedAvailable(const Observable* account = NULL);
bool WINAPI B_IsAccountArcaOrderExecutionListedAvailable(const Observable* account = NULL);

void* WINAPI B_CreateOrderMonitor(const Position* position);
bool WINAPI B_OrderMonitorAddOrder(void* orderMonitorHandle, const Order* order);
unsigned int WINAPI B_GetOrderMonitorOrderCount(const void* orderMonitorHandle);
unsigned int WINAPI B_GetOrderMonitorRemainingSize(const void* orderMonitorHandle, bool visible, unsigned int& orderCount);
unsigned int WINAPI B_GetOrderMonitorRemainingPriceSize(const void* orderMonitorHandle, const Money& price, bool visible, unsigned int& orderCount);
unsigned int WINAPI B_GetOrderMonitorRemainingSizeBelowPrice(const void* orderMonitorHandle, const Money& price, bool inclusive, bool visible, unsigned int& orderCount);
unsigned int WINAPI B_GetOrderMonitorRemainingSizeAbovePrice(const void* orderMonitorHandle, const Money& price, bool inclusive, bool visible, unsigned int& orderCount);
unsigned int WINAPI B_GetOrderMonitorMarketSizeBelowPrice(const void* orderMonitorHandle, const Money& price, bool inclusive);
unsigned int WINAPI B_GetOrderMonitorMarketSizeAbovePrice(const void* orderMonitorHandle, const Money& price, bool inclusive);
unsigned int WINAPI B_GetOrderMonitorSizeBeingCancelled(const void* orderMonitorHandle, unsigned int& orderCount);
bool WINAPI B_OrderMonitorHasOrder(const void* orderMonitorHandle, const Order* order);
const Order* WINAPI B_OrderMonitorGetOrderJustRemoved(const void* orderMonitorHandle);
bool WINAPI B_OrderMonitorHasOrJustRemovedOrder(const void* orderMonitorHandle, const Order* order);
void WINAPI B_DestroyOrderMonitor(void* orderMonitorHandle);
unsigned int WINAPI B_OrderMonitorCancelAllOrders(void* orderMonitorHandle);
unsigned int WINAPI B_OrderMonitorCancelPriceOrders(void* orderMonitorHandle, const Money& price);
unsigned int WINAPI B_OrderMonitorCancelNotPriceOrders(void* orderMonitorHandle, const Money& price);
unsigned int WINAPI B_OrderMonitorCancelOrdersSize(void* orderMonitorHandle, unsigned int size);
unsigned int WINAPI B_OrderMonitorCancelOrdersBelowPrice(void* orderMonitorHandle, const Money& price, bool inclusive, unsigned int& orderCount);
unsigned int WINAPI B_OrderMonitorCancelOrdersAbovePrice(void* orderMonitorHandle, const Money& price, bool inclusive, unsigned int& orderCount);
const Money& WINAPI B_GetOrderMonitorMinPrice(const void* orderMonitorHandle);
const Money& WINAPI B_GetOrderMonitorMaxPrice(const void* orderMonitorHandle);
void WINAPI B_OrderMonitorResetUserInfoForAllOrders(const void* orderMonitorHandle);
//unsigned int WINAPI B_GetDestinationSizeAtPrice(const void* orderMonitorHandle, const char* destination, const Money& price, bool visible, unsigned int& orderCount);
//unsigned int WINAPI B_GetDestinationSizeAboveBelowPrice(const void* orderMonitorHandle, const char* destination, bool above, const Money& price, bool inclusive, bool visible, unsigned int& orderCount);
bool WINAPI B_HasDestinationOrdersAboveBelowPrice(const void* orderMonitorHandle, const char* destination, bool above, const Money& price, bool inclusive);
bool WINAPI B_HasDestinationOrdersAtPrice(const void* orderMonitorHandle, const char* destination, const Money& price);
bool WINAPI B_CancelDestinationOrdersAboveBelowPrice(void* orderMonitorHandle, const char* destination, bool above, const Money& price, bool inclusive);
bool WINAPI B_CancelDestinationOrdersAtPrice(void* orderMonitorHandle, const char* destination, const Money& price);
unsigned int WINAPI B_GetDestinationSizeAboveBelowPrice(const void* orderMonitor, const char* destination, bool above, const Money& price, bool inclusive, bool visible);
unsigned int WINAPI B_GetDestinationMarketSizeAboveBelowPrice(const void* orderMonitor, const char* destination, bool above, const Money& price, bool inclusive);

void* WINAPI B_CreateOrderMonitorPriceIterator(void* orderMonitorHandle, bool reverse);
const Money* WINAPI B_GetNextOrderMonitorPrice(void* iteratorHandle, void*& orderMonitorPriceHandle);

unsigned int WINAPI B_GetOrderSizesFromOrderMonitorPriceHandle(const void* orderMonitorPriceHandle, struct OrderSizes& orderSizes);
unsigned int WINAPI B_GetRemainingSizeFromOrderMonitorPriceHandle(const void* orderMonitorPriceHandle, bool visible);
unsigned int WINAPI B_GetMarketSizeFromOrderMonitorPriceHandle(const void* orderMonitorPriceHandle);
unsigned int WINAPI B_GetOrderCountFromOrderMonitorPriceHandle(const void* orderMonitorPriceHandle);
unsigned int WINAPI B_CancelAllOrdersInMonitorPriceHandle(void* orderMonitorPriceHandle);

void* WINAPI B_CreateOrderMonitorPriceOrderIterator(void* orderMonitorPriceHandle);
void* WINAPI B_CreateOrderMonitorOrderIterator(void* orderMonitorHandle);
struct OrderSizes
{
	OrderSizes(unsigned int remainingSize = 0,
		unsigned int visibleRemainingSize = 0,
		unsigned int marketSize = 0):
		m_remainingSize(remainingSize),
		m_visibleRemainingSize(visibleRemainingSize),
		m_marketSize(marketSize){}
    unsigned int m_remainingSize;
    unsigned int m_visibleRemainingSize;
	unsigned int m_marketSize;
};
Order* WINAPI B_GetNextOrderMonitorPriceOrder(void* iteratorHandle, const struct OrderSizes*& orderSizes);


const Money& WINAPI B_GetAccountBuyingPowerCap(const Observable* account = NULL);
const Money& WINAPI B_GetAccountBuyingPowerUserCap(const Observable* account = NULL);
bool WINAPI B_IsAccountBuyingPowerUserCapSet(const Observable* account = NULL);
const Money& WINAPI B_SetAccountBuyingPowerUserCap(const Money& money, const Observable* account = NULL);
const Money& WINAPI B_RemoveAccountBuyingPowerUserCap(const Observable* account = NULL);

const Money& WINAPI B_GetAccountMaxLossCap(const Observable* account);
const Money& WINAPI B_GetAccountMaxLossUserCap(const Observable* account);
bool WINAPI B_IsAccountMaxLossUserCapSet(const Observable* account);
const Money& WINAPI B_SetAccountMaxLossUserCap(const Money& money, const Observable* account);
const Money& WINAPI B_RemoveAccountMaxLossUserCap(const Observable* account);

const Money& WINAPI B_GetAccountWarnMaxMarkedNetLoss(const Observable* account);
const Money& WINAPI B_GetAccountWarnMaxMarkedNetLossCap(const Observable* account);
const Money& WINAPI B_GetAccountWarnMaxMarkedNetLossUserCap(const Observable* account);
bool WINAPI B_IsAccountWarnMaxMarkedNetLossUserCapSet(const Observable* account);
const Money& WINAPI B_SetAccountWarnMaxMarkedNetLossUserCap(const Money& money, const Observable* account);
const Money& WINAPI B_RemoveAccountWarnMaxMarkedNetLossUserCap(const Observable* account);

const Money& WINAPI B_GetAccountWarnMaxOpenLoss(const Observable* account);
const Money& WINAPI B_GetAccountWarnMaxOpenLossCap(const Observable* account);
const Money& WINAPI B_GetAccountWarnMaxOpenLossUserCap(const Observable* account);
bool WINAPI B_IsAccountWarnMaxOpenLossUserCapSet(const Observable* account);
const Money& WINAPI B_SetAccountWarnMaxOpenLossUserCap(const Money& money, const Observable* account);
const Money& WINAPI B_RemoveAccountWarnMaxOpenLossUserCap(const Observable* account);

const Money& WINAPI B_GetAccountMaxLossPerStockCap(const Observable* account);
const Money& WINAPI B_GetAccountMaxLossPerStockUserCap(const Observable* account);
bool WINAPI B_IsAccountMaxLossPerStockUserCapSet(const Observable* account);
const Money& WINAPI B_SetAccountMaxLossPerStockUserCap(const Money& money, const Observable* account);
const Money& WINAPI B_RemoveAccountMaxLossPerStockUserCap(const Observable* account);

const Money& WINAPI B_GetAccountMaxOpenLossPerStockCap(const Observable* account);
const Money& WINAPI B_GetAccountMaxOpenLossPerStockUserCap(const Observable* account);
bool WINAPI B_IsAccountMaxOpenLossPerStockUserCapSet(const Observable* account);
const Money& WINAPI B_SetAccountMaxOpenLossPerStockUserCap(const Money& money, const Observable* account);
const Money& WINAPI B_RemoveAccountMaxOpenLossPerStockUserCap(const Observable* account);

const Money& WINAPI B_GetAccountMaxOpenPositionValueCap(const Observable* account = NULL);
const Money& WINAPI B_GetAccountMaxOpenPositionValueUserCap(const Observable* account = NULL);
bool WINAPI B_IsAccountMaxOpenPositionValueUserCapSet(const Observable* account = NULL);
const Money& WINAPI B_SetAccountMaxOpenPositionValueUserCap(const Money& money, const Observable* account = NULL);
const Money& WINAPI B_RemoveAccountMaxOpenPositionValueUserCap(const Observable* account = NULL);

unsigned int WINAPI B_GetShortSellMultiplierCap(const Observable* account = NULL);
unsigned int WINAPI B_GetShortSellMultiplierUserCap(const Observable* account = NULL);
bool WINAPI B_IsShortSellMultiplierUserCapSet(const Observable* account = NULL);
unsigned int WINAPI B_SetShortSellMultiplierUserCap(unsigned int value, const Observable* account = NULL);
unsigned int WINAPI B_RemoveShortSellMultiplierUserCap(const Observable* account = NULL);

const Money& WINAPI B_GetShortSellPriceLimitFloor(const Observable* account = NULL);
const Money& WINAPI B_GetShortSellPriceLimitUserFloor(const Observable* account = NULL);
bool WINAPI B_IsShortSellPriceLimitUserFloorSet(const Observable* account = NULL);
const Money& WINAPI B_SetShortSellPriceLimitUserFloor(const Money& money, const Observable* account = NULL);
const Money& WINAPI B_RemoveShortSellPriceLimitUserFloor(const Observable* account = NULL);

unsigned int WINAPI B_GetMaxOpenPositionSizeCap(const Observable* account = NULL);
unsigned int WINAPI B_GetMaxOpenPositionSizeUserCap(const Observable* account = NULL);
bool WINAPI B_IsMaxOpenPositionSizeUserCapSet(const Observable* account = NULL);
unsigned int WINAPI B_SetMaxOpenPositionSizeUserCap(unsigned int value, const Observable* account = NULL);
unsigned int WINAPI B_RemoveMaxOpenPositionSizeUserCap(const Observable* account = NULL);

unsigned int WINAPI B_GetMaxOrderSizeCap(const Observable* account = NULL);
unsigned int WINAPI B_GetMaxOrderSizeUserCap(const Observable* account = NULL);
bool WINAPI B_IsMaxOrderSizeUserCapSet(const Observable* account = NULL);
unsigned int WINAPI B_SetMaxOrderSizeUserCap(unsigned int value, const Observable* account = NULL);
unsigned int WINAPI B_RemoveMaxOrderSizeUserCap(const Observable* account = NULL);

unsigned int WINAPI B_GetMaxOpenPositions(const Observable* account = NULL);
unsigned int WINAPI B_GetMaxOpenPositionsUserCap(const Observable* account = NULL);
bool WINAPI B_IsMaxOpenPositionsUserCapSet(const Observable* account = NULL);
unsigned int WINAPI B_SetMaxOpenPositionsUserCap(unsigned int value, const Observable* account = NULL);
unsigned int WINAPI B_RemoveMaxOpenPositionsUserCap(const Observable* account = NULL);

unsigned int WINAPI B_GetMaxPositionPendingOrders(const Observable* account = NULL);
unsigned int WINAPI B_GetMaxPositionPendingOrdersUserCap(const Observable* account = NULL);
bool WINAPI B_IsMaxPositionPendingOrdersUserCapSet(const Observable* account = NULL);
unsigned int WINAPI B_SetMaxPositionPendingOrdersUserCap(unsigned int value, const Observable* account = NULL);
unsigned int WINAPI B_RemoveMaxPositionPendingOrdersUserCap(const Observable* account = NULL);

enum AccountConstraints
{
    AC_BUYING_POWER = 0,
    AC_MAX_OPEN_POSITION_VALUE,
    AC_SHORT_SELL_MULTIPLIER,
    AC_SHORT_SELL_PRICE_LIMIT,
    AC_MAX_OPEN_POSITION_SIZE,
    AC_MAX_ORDER_SIZE,
    AC_MAX_OPEN_POSITIONS,
    AC_MAX_POSITION_PENDING_ORDERS,
    AC_MAX_CANCEL_COUNT,
    AC_MAX_TOTAL_SHARES,
    AC_MAX_TRADED_SHARES,
    AC_MAX_LOSS,
    AC_MAX_LOSS_PER_STOCK,
    AC_MAX_OPEN_LOSS_PER_STOCK,
    AC_INSTITUTIONAL,
	AC_WARN_MAX_MARKED_NET_LOSS,
	AC_WARN_MAX_OPEN_LOSS
};

unsigned int WINAPI B_SetAccountConstraints(const Money* buyingPowerUserCap,
    const Money* maxOpenPositionValueUserCap,
    unsigned int* shortSellMultiplierUserCap,
    const Money* shortSellPriceLimitUserFloor,
    unsigned int* maxOpenPositionSizeUserCap,
    unsigned int* maxOrderSizeUserCap,
    unsigned int* maxOpenPositionsUserCap,
    unsigned int* maxPositionPendingOrdersUserCap,
    unsigned int* maxCancelCounUserCap,
    unsigned int* maxTotalSharesUserCap,
    unsigned int* maxTradedSharesUserCap,
    const Money* maxLossUserCap,
    const Money* maxLossPerStockUserCap,
    const Money* maxOpenLossPerStockUserCap,
    const Money* warnMaxMarkedNetLossUserCap,
    const Money* warnMaxOpenLossUserCap,
    bool institutional,
    Observable* account = NULL);


bool WINAPI B_IsAccountMaxLossExceeded(const Observable* account = NULL);
bool WINAPI B_IsAccountMaxOpenPnlWarningExceeded(const Observable* account = NULL);
bool WINAPI B_IsAccountMaxMarkedNetPnlWarningExceeded(const Observable* account = NULL);

void WINAPI B_LogStockState(const StockBase* stockHandle);//, bool price2DecPlaces, unsigned int block = 100);

unsigned int WINAPI B_GetOrderCount(const Observable* account = NULL);
unsigned int WINAPI B_GetExecutedOrderCount(const Observable* account = NULL);
unsigned int WINAPI B_GetDestinationOrderCount(const char* destination, const Observable* account = NULL);
unsigned int WINAPI B_GetDestinationExecutedOrderCount(const char* destination, const Observable* account = NULL);
unsigned int WINAPI B_GetDestinationExecutedSharesCount(const char* destination, const Observable* account = NULL);
unsigned int WINAPI B_GetNyseOrderCount(const Observable* account = NULL);
unsigned int WINAPI B_GetNyseExecutedOrderCount(const Observable* account = NULL);
unsigned int WINAPI B_GetNyseExecutedSharesCount(const Observable* account = NULL);
unsigned int WINAPI B_GetAmexOrderCount(const Observable* account = NULL);
unsigned int WINAPI B_GetAmexExecutedOrderCount(const Observable* account = NULL);
unsigned int WINAPI B_GetAmexExecutedSharesCount(const Observable* account = NULL);

unsigned int WINAPI B_GetMaxCancelCount(const Observable* account = NULL);
unsigned int WINAPI B_GetMaxCancelCountCap(const Observable* account = NULL);
unsigned int WINAPI B_GetMaxCancelCountUserCap(const Observable* account = NULL);
bool WINAPI B_IsMaxCancelCountUserCapSet(const Observable* account = NULL);
unsigned int WINAPI B_SetMaxCancelCountUserCap(unsigned int value, const Observable* account = NULL);
unsigned int WINAPI B_RemoveMaxCancelCountUserCap(const Observable* account = NULL);

unsigned int WINAPI B_GetMaxTotalShares(const Observable* account = NULL);
unsigned int WINAPI B_GetMaxTotalSharesCap(const Observable* account = NULL);
unsigned int WINAPI B_GetMaxTotalSharesUserCap(const Observable* account = NULL);
bool WINAPI B_IsMaxTotalSharesUserCapSet(const Observable* account = NULL);
unsigned int WINAPI B_SetMaxTotalSharesUserCap(unsigned int value, const Observable* account = NULL);
unsigned int WINAPI B_RemoveMaxTotalSharesUserCap(const Observable* account = NULL);

unsigned int WINAPI B_GetMaxTradedShares(const Observable* account = NULL);
unsigned int WINAPI B_GetMaxTradedSharesCap(const Observable* account = NULL);
unsigned int WINAPI B_GetMaxTradedSharesUserCap(const Observable* account = NULL);
bool WINAPI B_IsMaxTradedSharesUserCapSet(const Observable* account = NULL);
unsigned int WINAPI B_SetMaxTradedSharesUserCap(unsigned int value, const Observable* account = NULL);
unsigned int WINAPI B_RemoveMaxTradedSharesUserCap(const Observable* account = NULL);
/*
bool WINAPI B_IsSoesableEcn(const char* mm);
bool WINAPI B_SetSoesableEcn(const char* mm, bool direct);
bool WINAPI B_IsPotentiallySoesableEcn(const char* mm);
bool WINAPI B_IsTradeVenueSoesable(void* tradeVenueHandle);
bool WINAPI B_IsTradeVenuePotentiallySoesable(void* tradeVenueHandle);
*/
bool WINAPI B_IsEcnSupportProactivity(const char* mm);

bool WINAPI B_IsDirectTradeEcn(const char* mm);
bool WINAPI B_SetDirectTradeEcn(const char* mm, bool direct);
bool WINAPI B_IsPotentiallyDirectTradeEcn(const char* mm);
bool WINAPI B_IsTradeEcnCurrentlyDirect(const char* mm);
bool WINAPI B_IsTradeEcnLineDown(const char* mm);

bool WINAPI B_IsTradeVenue(const char* mm);
void* WINAPI B_CreateTradeVenueIterator();
void* WINAPI B_GetNextTradeVenue(void* iteratorHandle);
const char* WINAPI B_GetTradeVenueName(void* tradeVenueHandle);
bool WINAPI B_IsTradeVenueDirect(void* tradeVenueHandle);
bool WINAPI B_IsTradeVenuePotentiallyDirect(void* tradeVenueHandle);
bool WINAPI B_IsTradeVenueCurrentlyDirect(void* tradeVenueHandle);
bool WINAPI B_IsTradeVenueLineDown(void* tradeVenueHandle);

unsigned int WINAPI B_GetEcnOrderVisibilitySupport(const char* mm);

unsigned int WINAPI B_GetCancelCount(const Observable* account = NULL);
unsigned int WINAPI B_GetNyseCancelCount(const Observable* account = NULL);
unsigned int WINAPI B_GetAmexCancelCount(const Observable* account = NULL);
unsigned int WINAPI B_GetDestinationCancelCount(const char* destination, const Observable* account = NULL);

void WINAPI B_SetSortMap(const char** mmArray, const unsigned short* priorityArray, bool ecnsAboveMMs, bool regionalsAtBottom);

void WINAPI B_ResubscribeToStock(StockBase* stockHandle);
void WINAPI B_ResubscribeToAllStocks();

void WINAPI B_SetSmartOrdersActiveInMarketHoursOnly(bool marketHours);
bool WINAPI B_IsSmartOrdersActiveInMarketHoursOnly();
bool WINAPI B_IsSmartOrdersActivated(Observable* account = NULL);
void WINAPI B_ActivateSmartOrders(Observable* account = NULL);

unsigned int WINAPI B_GetArcaSmartIOC();
void WINAPI B_SetArcaSmartIOC(unsigned int smartIOC);
//unsigned int WINAPI B_GetBrutSmartIOC();
//void WINAPI B_SetBrutSmartIOC(unsigned int smartIOC);
unsigned int WINAPI B_GetStockDirectEcns(const StockBase* stockHandle);

void WINAPI B_FilterQuotes(bool enableRegional, bool enableCaes, unsigned int quoteConditionFilter, const char* marketOpenQuoteFilter[EX_LAST], const char* marketClosedQuoteFilter[EX_LAST]);

void WINAPI B_SetQuoteConditionFilter(unsigned int quoteConditionFilter);
unsigned int WINAPI B_GetQuoteConditionFilter();

bool WINAPI B_AddPositionFile(const char* path, unsigned int filter);
bool WINAPI B_RemovePositionFile(const char* path);
void WINAPI B_ClearPositionFiles();

const char* WINAPI B_GetOrderErrorMessage(unsigned int error);

bool WINAPI B_AddUserOrderDescription(unsigned int userType, const char* userOrderDescription, Observable* account);

bool WINAPI B_IsBlockForeignOrdersInSimulation();
void WINAPI B_SetBlockForeignOrdersInSimulation(bool block);

void* WINAPI B_CreateUserOrderTypeIterator(Observable* account);
const char* WINAPI B_GetNextUserOrderType(void* iteratorHandle, unsigned int& orderType);

bool WINAPI B_TestLatency(const char* destination, void* id);

bool WINAPI B_GetModuleVersion(bool externally, HINSTANCE moduleInstanceHandle, unsigned short& v1, unsigned short& v2, unsigned short& v3, unsigned short& v4);

bool WINAPI B_HasOversoldPositions(const Observable* account = NULL);
void* WINAPI B_CreateOversoldPositionIterator(const Observable* account = NULL);
const Position* WINAPI B_GetNextOversoldPosition(void* iteratorHandle);

unsigned int WINAPI B_GetPlatform();
bool WINAPI B_IsDebug();
const char* WINAPI B_GetHeaderVersion();

const Money& WINAPI B_GetInstitutionalMoney(const Observable* account = NULL);
unsigned int WINAPI B_GetInstitutionalShares(const Observable* account = NULL);
//const Position* WINAPI B_AddInstitutionalPosition(const char* symbol, bool side, int size, const Money& price, bool useClosingPrice, const Observable* account = NULL);
const Position* WINAPI B_AddInstitutionalPosition(const char* symbol, int size, const Money& price, bool useClosingPrice, char side, const Observable* account = NULL);
const Position* WINAPI B_ClearInstitutionalPosition(const char* symbol, const Observable* account = NULL);
void WINAPI B_ClearAllInstitutionalPositions(const Observable* account = NULL);


enum ImportMode
{
    IM_ADD = 1,
    IM_REPLACE = 2,
    IM_CLEAR_AND_REPLACE = 3,
};

bool WINAPI B_ImportInstitutionalPositions(const char* filePath, bool useClosingPrice, ImportMode importMode, const Observable* account = NULL);

enum ExportInstitutionalPositionsFlags
{
    EP_INSTITUTIONAL = 1,
    EP_TRADED = 2,
    EP_NOTTRADED = 4,
};

enum ExportInstitutionalSizeFlags
{
    ES_INSTITUTIONAL = 1,
    ES_POSITION = 2,
};

bool WINAPI B_ExportInstitutionalPositions(const char* filePath,
    unsigned int positionsFlags,
    unsigned int sizeFlags,
    const Observable* account = NULL);

const StockBase* WINAPI B_UnsubscribeStock(const char* symbol);
bool WINAPI B_DestroyStock(const char* symbol);
unsigned int WINAPI B_UnsubscribeUnusedStocks();
unsigned int WINAPI B_DestroyUnusedStocks();

void WINAPI B_NotifyAdminObserver(const Message* message, Observable* from = NULL, const Message* additionalInfo = NULL);
//void WINAPI B_SynchronizeWithServer(unsigned int delayMilliseconds = 0xFFFFFFFF);
//void WINAPI B_SynchronizeWithSystemTime();
//bool WINAPI B_IsSynchronizedWithSystemTime();

const Money& WINAPI B_GetClosedPnlOffset(const Observable* account = NULL);
bool WINAPI B_SetClosedPnlOffset(const Money& pnlOffset, Observable* account = NULL);

const Money& WINAPI B_GetDefaultCommissionRate();
void WINAPI B_SetDefaultCommissionRate(const Money& rate);
const Money& WINAPI B_GetAccountCommissionRate(const Observable* account = NULL);
bool WINAPI B_SetAccountCommissionRate(const Money& rate, Observable* account = NULL);
bool WINAPI B_SetAccountDefaultCommissionRate(Observable* account = NULL);

void WINAPI B_SetDefaultExecutorIpPort(const char* ip, unsigned short port, const char* localIp);
const char* WINAPI B_GetDefaultExecutorIp();
unsigned short WINAPI B_GetDefaultExecutorPort();
const char* WINAPI B_GetDefaultExecutorLocalIp();

const Money& WINAPI B_GetMoneyZero();
const Money& WINAPI B_GetMoneyPenny();
const MoneyPrecise& WINAPI B_GetPreciseMoneyZero();
const MoneySize& WINAPI B_GetMoneySizeZero();

void WINAPI B_SetPrimaryUserIdAndPassword(const char* userId, const char* password);

void WINAPI B_LogBookState(const StockBase* stockHandle, unsigned int bookId, unsigned int lines);

bool WINAPI B_IsMarketDataMulticastConnected();
bool WINAPI B_IsMarketDataConnected();
bool WINAPI B_IsMarketSummaryConnected();
bool WINAPI B_IsMarketSummaryConnecting();
bool WINAPI B_IsAccountConnected(const Observable* account = NULL);

bool WINAPI B_IsToAutoCloseMaxOpenLosers(const Observable* account = NULL);
void WINAPI B_SetToAutoCloseMaxOpenLosers(bool autoClose, bool autoCloseOnlyWhenMarketOpen, bool autoCloseByPricePnl, const Money& autoClosePrintOffset, ClosePositionMethod method, Observable* account = NULL);
ClosePositionMethod WINAPI B_GetAutoCloseMaxOpenLosersMethod(const Observable* account = NULL);
bool WINAPI B_IsAutoCloseOnlyWhenMarketOpen(const Observable* account = NULL);
bool WINAPI B_IsAutoCloseByPricePnl(const Observable* account = NULL);
const Money& WINAPI B_GetAutoClosePrintOffset(const Observable* account = NULL);

bool WINAPI B_IsAccountPrincipal(const Observable* account = NULL);

enum PropertyType
{
    PROPERTY_BOOL,
    PROPERTY_UINT,
    PROPERTY_INT,
    PROPERTY_USHORT,
    PROPERTY_SHORT,
    PROPERTY_STRING,
    PROPERTY_MONEY,

    PROPERTY_LAST
};

void WINAPI B_InvokeMappedCommand(unsigned int ascii, bool control, bool alt, bool shift, bool capslock);
unsigned int WINAPI B_GetMarketSummaryVersionNumber();
unsigned int WINAPI B_GetMsParentVersion();
unsigned int WINAPI B_GetMsParentIp();
unsigned short WINAPI B_GetMsParentPort();
unsigned short WINAPI B_GetMsParentApp();

bool WINAPI B_IsStockTradable(const StockBase* stockHandle);
bool WINAPI B_IsStockForeignNotTradable(const StockBase* stockHandle);

//void WINAPI B_SetHiddenVisibility(const unsigned short* visibility);
void WINAPI B_SetEcnHiddenVisibility(const char* mmid, unsigned short visibility);
//unsigned short B_GetEcnHiddenVisibility(unsigned short ecnId);
unsigned short B_GetEcnHiddenVisibility(const char* mmid);

unsigned int WINAPI B_GetSlowOrderSwipeTif();
void WINAPI B_SetSlowOrderSwipeTif(unsigned int tif);

const BookEntry* WINAPI B_GetBookFirstEntry(bool side, const Observable* book);

void WINAPI B_ReadTradableStockList();
void* WINAPI B_CreateTradableStockIterator();
const char* WINAPI B_GetNextTradableStock(void* tradableStockIterator);
unsigned int WINAPI B_GetTradableStockCount();

void WINAPI B_ReadNotTradableStockList();
void* WINAPI B_CreateNotTradableStockIterator();
unsigned int WINAPI B_GetNotTradableStockCount();

unsigned int WINAPI B_GetFailedStockCount();
void* WINAPI B_CreateFailedStockIterator();
unsigned int WINAPI B_GetFailedStockReloadAttempts();

void* WINAPI B_GetPreProcessPtr(unsigned int value);
void WINAPI B_AttachPreProcessObserver(Observer* observer, bool detach = false);

void WINAPI B_SetLogStock(const char* symbol, Observable* account = NULL);
const char* WINAPI B_GetLogStock(const Observable* account = NULL);
void WINAPI B_SetLogStockForAllAccounts(const char* symbol);
const char* WINAPI B_GetLogStockForAllAccounts();

//bool WINAPI B_IsServerSupportsBrutHunter();
//void WINAPI B_SetServerSupportsBrutHunter(bool supports);

bool WINAPI B_Logout(unsigned int waitMilliseconds);
bool WINAPI B_IsExiting();

enum PriceOffsetType
{
    OP_ABSOLUTE_VALUE,
    OP_PREV_ORDER_PRICE_OFFSET,
    OP_MM_SAME_SIDE_OFFSET,
    OP_MM_OPPOSITE_SIDE_OFFSET
};

Observable* WINAPI B_CreateOrderReRoute(Order* order,
    const char* destination,
    unsigned int* destinationExchange,
    PriceOffsetType priceOffsetType,
    const Money* priceOffset,
    const char* mmBase,
    const Money* discretionaryDelta,
    unsigned int* visibleShares,
    unsigned int* timeInForce,
    char oversellHandling);
//    bool delayShortTillUptick);

bool WINAPI B_Encrypt(const char* str, const char* password, char* buf, unsigned int bufsize);
bool WINAPI B_Decrypt(const char* encryptedString, const char* password, char* buf, unsigned int bufsize);

const char* WINAPI B_GetNextString(void* stringIterator);
bool WINAPI B_GetModuleFolder(HINSTANCE hInstance, char* pathbuf, unsigned int bufsize, bool lastBackslash = true);

bool WINAPI B_IsSubscribedToMsImbalances();
bool WINAPI B_IsGetImbalancesFromMs();
bool WINAPI B_SetNysOrderChannel(const char* nysOrderChannel);
bool WINAPI B_SetNysOrderChannelById(unsigned int id);
const char* WINAPI B_GetNysOrderChannel();
const char* WINAPI B_GetNysOrderChannelById(unsigned int id);
unsigned int WINAPI B_GetNysOrderChannelIdByName(const char* channel);
unsigned int WINAPI B_GetNysOrderChannelCount();

//void WINAPI B_SetChannelNysOrdersThrouhBrut(bool channel);
//bool WINAPI B_IsChannelNysOrdersThrouhBrut();

bool WINAPI B_IsMarketClosedForToday();

unsigned int WINAPI B_GetPositionsCount(const Observable* account);
unsigned int WINAPI B_GetLongPositionsCount(const Observable* account = NULL);
unsigned int WINAPI B_GetShortPositionsCount(const Observable* account = NULL);
unsigned int WINAPI B_GetFlatPositionProperCount(const Observable* account = NULL);
unsigned int WINAPI B_GetLongPositionProperCount(const Observable* account = NULL);
unsigned int WINAPI B_GetShortPositionProperCount(const Observable* account = NULL);

//void WINAPI B_SetCaesOrdersSentToSize(bool sendToSize);
//bool WINAPI B_IsCaesOrdersSentToSize();

void WINAPI B_SetRenewNysFeeOrderTime(bool cancelOrRenew, unsigned int tif);//cancelOrRenew: true - renew, false - cancel; tif = 0 - no action
bool WINAPI B_IsNysFeeOrderRenewOrCancel();//true - renew, false - cancel
unsigned int WINAPI B_GetNysFeeOrderTif();//tif = 0 - no action

void WINAPI B_SetNysFeeWarningTime(unsigned int warningTime);//0 - no warning; otherwise message MsgNysFeeWarning(M_ORDER_NYS_FEE_WARNING) is sent when time elapsed reaches "warningTime".
unsigned int WINAPI B_GetNysFeeWarningTime();

//void WINAPI B_SetNysFeeApplicableToMarketOrders(bool applicable);
//bool WINAPI B_IsNysFeeApplicableToMarketOrders();

unsigned int WINAPI B_GetNysFeeTime();

void WINAPI B_SystemTimeChanged(unsigned int time);

time_t WINAPI B_GetMarketDataConnectionTime();
unsigned int WINAPI B_GetSecondsSinceMarketDataConnection();
unsigned int WINAPI B_GetSecondsSinceMarketSummaryConnection();
time_t WINAPI B_GetMarketSummaryConnectionTime();

const Order* WINAPI B_GetStagingTicket(const StockBase* stockHandle, bool side, const Observable* account = NULL);
const Order* WINAPI B_SetStagingTicket(const StockBase* stockHandle, bool side, unsigned int shares, const Observable* account = NULL);

bool WINAPI B_IsAccountBuyingPowerConstrained(const Observable* account = NULL);
bool WINAPI B_IsAccountMaxOpenPositionValueConstrained(const Observable* account = NULL);
bool WINAPI B_IsShortSellMultiplierConstrained(const Observable* account = NULL);
bool WINAPI B_IsShortSellPriceLimitConstrained(const Observable* account = NULL);
bool WINAPI B_IsMaxOpenPositionSizeConstrained(const Observable* account = NULL);
bool WINAPI B_IsMaxOrderSizeConstrained(const Observable* account = NULL);
bool WINAPI B_IsMaxOpenPositionsConstrained(const Observable* account = NULL);
bool WINAPI B_IsMaxPositionPendingOrdersConstrained(const Observable* account = NULL);
bool WINAPI B_IsMaxCancelCountConstrained(const Observable* account = NULL);
bool WINAPI B_IsMaxTotalSharesConstrained(const Observable* account = NULL);
bool WINAPI B_IsMaxTradedSharesConstrained(const Observable* account = NULL);
bool WINAPI B_IsAccountMaxLossConstrained(const Observable* account = NULL);
bool WINAPI B_IsAccountMaxLossPerStockConstrained(const Observable* account = NULL);
bool WINAPI B_IsAccountMaxOpenLossPerStockConstrained(const Observable* account = NULL);
bool WINAPI B_IsAccountWarnMaxMarkedNetLossConstrained(const Observable* account = NULL);
bool WINAPI B_IsAccountWarnMaxOpenLossConstrained(const Observable* account = NULL);

unsigned int WINAPI B_GetOrdersToRenewCount(const Observable* account = NULL);
unsigned int WINAPI B_GetNysFeeWarningOrdersCount(const Observable* account = NULL);
unsigned int WINAPI B_GetNysFeeShortenOrdersCount(const Observable* account = NULL);
void WINAPI B_CancelNysFeeOrders(Observable* account = NULL);
void WINAPI B_CancelNysFeeOrdersForAllAccounts();
bool WINAPI B_IsDoingOrderDeleteJob();
bool WINAPI B_IsDoingSmartOrderDeleteJob();

bool WINAPI B_SetOrderToRenew(Order* order, Observable* account = NULL);
void WINAPI B_SetAllOrdersToRenew(Observable* account = NULL);

void WINAPI B_AllowTradingInPrimaryAccountOnly(bool primaryOnly);

unsigned int WINAPI B_GetUpgradeVersion();

int WINAPI B_GetPositionInventory(const char* symbol, Observable* account = NULL);
void WINAPI B_SetPositionInventory(const char* symbol, int inventory, Observable* account = NULL);
void WINAPI B_AddPositionInventory(const char* symbol, int inventory, Observable* account = NULL);
void WINAPI B_ClearPositionInventory(const char* symbol, Observable* account = NULL);
void WINAPI B_ClearAllInventories(Observable* account = NULL);
unsigned int B_GetInventoryPositionCount(Observable* account = NULL);

unsigned int WINAPI B_GetDayStartSecond();
unsigned int WINAPI B_GetCurrentNYDate();
unsigned short WINAPI B_GetCurrentNYDay();
unsigned short WINAPI B_GetCurrentNYMonth();
unsigned short WINAPI B_GetCurrentNYYear();
unsigned short WINAPI B_GetCurrentNYDayOfWeek();
unsigned short WINAPI B_GetDayOfWeek(unsigned int date);
unsigned short WINAPI B_GetMarketSummaryCurrentMinute();

bool WINAPI B_IsDestinationRecognized(const char* destination);
void WINAPI B_DestroyDestinationRecognizer();
void WINAPI B_ClearDestinationRecognizer();
unsigned int WINAPI B_GetRecognizedDestinationCount();
bool WINAPI B_AddRecognizedDestination(const char* destination);
bool WINAPI B_RemoveRecognizedDestination(const char* destination);

void WINAPI B_SetUnsubscribeWhenFlat(bool unsubscribe);
bool WINAPI B_IsUnsubscribeWhenFlat();
unsigned int WINAPI B_GetMarketDataVersion();
unsigned int WINAPI B_GetAccountTransEngineVersion(const Observable* account);
unsigned int WINAPI B_GetMmBookCount(const StockBase* stock);

const void* WINAPI B_FindMmBothBooks(const StockBase* stock, const char* mmid);
Observable* WINAPI B_FindMmAggregatedBook(const StockBase* stockHandle, const char* mmid);
void* WINAPI B_CreateMmBooksIterator(const StockBase* stock);
const void* WINAPI B_GetNextMmBookHolder(void* iterator);
const Observable* WINAPI B_GetMmAggregatedBook(const void* bookHolder);
const Observable* WINAPI B_GetMmExpandedBook(const void* bookHolder);
const Observable* WINAPI B_GetMmExecutions(const void* bookHolder);
const Observable* WINAPI B_GetMmCancels(const void* bookHolder);
//const Observable* WINAPI B_GetMmFyis(const void* bookHolder);
/*
void WINAPI B_MultiBookIteratorMmBookAdded(void* iteratorHandle, unsigned int mmid);
void WINAPI B_MultiBookIteratorMmBookRemoved(void* iteratorHandle, unsigned int mmid);
*/
void WINAPI B_TransactionIteratorMmBookAdded(void* iteratorHandle, unsigned int mmid);
void WINAPI B_TransactionIteratorMmBookRemoved(void* iteratorHandle, unsigned int mmid);
void WINAPI B_TransactionIteratorSetCancels(void* iteratorHandle, bool cancels, Observer* observerToAdd);
void WINAPI B_TransactionIteratorSetMMs(void* iteratorHandle, bool mms, Observer* observerToAdd);
void WINAPI B_TransactionIteratorSetSourceFilter(void* iteratorHandle, unsigned int sourceFilter, Observer* observerToAdd);
void WINAPI B_TransactionIteratorSetEcnFilter(void* iteratorHandle, unsigned int ecnFilter, Observer* observerToAdd);
void WINAPI B_TransactionIteratorSetStock(void* iteratorHandle, const StockBase* stockHandle, Observer* observerToAdd);

const StockBase* WINAPI B_TransactionIteratorGetStock(const void* iteratorHandle);
bool WINAPI B_TransactionIteratorIsMMs(const void* iteratorHandle);
bool WINAPI B_TransactionIteratorIsCancels(const void* iteratorHandle);
unsigned int WINAPI B_TransactionIteratorGetEcnFilter(const void* iteratorHandle);
unsigned int WINAPI B_TransactionIteratorGetSourceFilter(const void* iteratorHandle);

const Exchange* WINAPI B_GetExchange(unsigned int id);
unsigned int WINAPI B_GetExchangeCount();
void* WINAPI B_CreateExchangeIterator();
const Exchange* WINAPI B_GetNextExchange(void* iterator);

const Transaction* WINAPI B_GetLastMmBookExecution(const StockBase* stockHandle, unsigned int mmid);
unsigned int WINAPI B_GetLastMmBookExecutionStatus(const StockBase* stockHandle, unsigned int mmid);
const Transaction* WINAPI B_GetLastMmExecution(const void* bookHolder);

unsigned int WINAPI B_GetDelayedCancelCount();

void WINAPI B_SetAllEcnLinesIntegrated(unsigned int ecnLines[MAX_BOOKS], unsigned int mmLines);
//void WINAPI B_SetEcnLinesIntegrated(unsigned short id, unsigned int ecnLines);
unsigned int WINAPI B_GetEcnLinesIntegrated(unsigned short id);
const unsigned int* WINAPI B_GetAllEcnLinesIntegrated();

unsigned int WINAPI B_GetMmLinesIntegration();

unsigned int WINAPI B_MakePhoneCall();

bool WINAPI B_IsAccountIsoEligible(const Observable* account);

void* WINAPI B_CreateBatsDirectedIterator();
void* WINAPI B_CreateBrassDirectedIterator();
const char* const* WINAPI B_GetNextDirectedVenue(void* iteratorHandle);
const char* const* WINAPI B_GetNextDirectedVenueAndKey(void* iteratorHandle, unsigned int* key = NULL);

char WINAPI B_GetRegionalDirectMarket(const char* mmid);
void* WINAPI B_CreateRegionalDirectIterator();
unsigned int WINAPI B_GetNextRegionalDirectMarket(void* iteratorHandle, bool* direct, char* market);
bool WINAPI B_IsRegionalDirect(const char* mmid);
void WINAPI B_SetRegionalDirect(const char* mmid, bool direct);

const char* WINAPI B_GetIsoRedirectTo(const char* from);
void* WINAPI B_CreateIsoIncompatibleVenueIterator();
unsigned int WINAPI B_GetNextIsoIncompatibleVenue(void* iteratorHandle, bool* compatible, const char** redirectTo);
bool WINAPI B_IsVenueIsoCompatible(const char* mmid);
void WINAPI B_SetVenueIsoCompatible(const char* mmid, bool compatible);
const char* WINAPI B_GetRashDirectedTo(const char* from);
void* WINAPI B_CreateRashDirectedIterator();
unsigned int WINAPI B_GetNextRashDirectedVenue(void* iteratorHandle, const char** redirectTo);
/*
bool WINAPI B_IsUseRashDirected(const char* mmid);
void WINAPI B_SetUseRashDirected(const char* mmid, bool use);

void* WINAPI B_CreateRashDirectedMnemonicsIterator();
unsigned int WINAPI B_GetNextRashDirectedMnemonics(void* iteratorHandle, const char** mm, const char** redirectTo);
const char* WINAPI B_GetRashDirectedMnemonics(const char* mnemonics);
*/
unsigned int WINAPI B_GetMarketDataLogonFlags();
unsigned int WINAPI B_GetMarketDataLoadOrdersFlags();
bool WINAPI B_IsMarketDataCancelAllOnDisconnectMode();
bool WINAPI B_IsMarketDataReceivingMarketData();
bool WINAPI B_IsMarketDataReceivingTransactions();
bool WINAPI B_IsMarketDataUsingCompression();
bool WINAPI B_IsMarketDataUsingMulticast();
bool WINAPI B_IsMarketDataNoDelay();

unsigned int WINAPI B_GetAccountLogonFlags(const Observable* account);
unsigned int WINAPI B_GetAccountLoadOrdersFlags(const Observable* account);
bool WINAPI B_IsAccountCancelAllOnDisconnectMode(const Observable* account);
bool WINAPI B_IsAccountReceivingMarketData(const Observable* account);
bool WINAPI B_IsAccountReceivingTransactions(const Observable* account);
bool WINAPI B_IsAccountUsingCompression(const Observable* account);
bool WINAPI B_IsAccountUsingMulticast(const Observable* account);
bool WINAPI B_IsAccountNoDelay(const Observable* account);

bool WINAPI B_ConnectMarketSummary(bool retry);
void WINAPI B_DisconnectMarketSummary();

bool WINAPI B_IsExtensionEligible();
bool WINAPI B_IsStageOnly();
bool WINAPI B_IsOptionTrading();
bool WINAPI B_IsAccountOptionTrading(const Observable* account);
bool WINAPI B_IsLogTurnOffEligible();
bool WINAPI B_IsBlockDotd();
bool WINAPI B_IsAccountBlockDotd(const Observable* account);

bool WINAPI B_IsPreprocessorLibraryLoaded();

void* WINAPI B_CreateHistoryPrintIterator(const StockBase* stockHandle);
const Transaction* WINAPI B_GetNexHistoryPrint(void* iterator);
unsigned int WINAPI B_GetStocksWithLoadedPrintsCount();
void WINAPI B_ClearAllHistoryPrints();
void* WINAPI B_CreateStockWithHistoryPrintsIterator();
const StockBase* WINAPI B_GetNexStockWithHistoryPrints(void* iterator);

void* WINAPI B_CreatePrintChartPointIterator(const StockBase* stockHandle);
const void* WINAPI B_GetNexPrintChartPoint(void* iterator, unsigned int* nextOffset);
bool WINAPI B_IsChartPointEmpty(const void* chartPoint);
void* WINAPI B_CreateChartPrintPriceIterator(const void* chartPoint);
int WINAPI B_GetNexChartPrintPrice(void* iterator, unsigned int* totalSize, unsigned int* maxPrintSize);

bool WINAPI B_IsUseMarketSummaryAsPrintSource();
void WINAPI B_SetUseMarketSummaryAsPrintSource(bool useMS);

void WINAPI B_NotifyIdleInterrupted(unsigned int code);
void WINAPI B_NotifyIdleResumed(unsigned int code);
bool WINAPI B_NotifyExtensionIdle(LONG lCount);

unsigned int WINAPI B_GetMarketOpenSecond();
unsigned int WINAPI B_GetMarketCloseSecond();
void WINAPI B_SetMarketCloseSecond(unsigned int marketCloseSecond);
bool WINAPI B_IsSecondsBeforeOpening(unsigned int seconds);
bool WINAPI B_IsSecondsBeforeClosing(unsigned int seconds);

bool WINAPI B_IsMsMarketDataDisconnected();

Observer* WINAPI B_GetBusinessObserver();

const StockBase* WINAPI B_CreateEmptyStock(const char* symbol,
	const char* description,
	char exchange,
	char attributes,
    const Money& closePrice,
    bool tradable,
    bool foreignNotTradable,
    bool testStock,
	unsigned char split);

void WINAPI B_Level2AndBookIteratorSetEcnBookLinesIntegrated(void* iteratorHandle, unsigned short bookId, unsigned int lines, Observer* observerToAddToBook);

enum HOLI_DAY
{
	HD_WORK,
	HD_WEEKEND,
	HD_NEW_YEAR,
	HD_MARTIN_LUTHER_KING,
	HD_PRESIDENTS,
	HD_GOOD_FRIDAY,
	HD_MEMORIAL,
	HD_INDEPENDENCE,
	HD_LABOR,
	HD_THANKSGIVING,
	HD_CHRISTMAS
};

enum SHORT_DAY
{
	SD_FULL,
	SD_BEFORE_INDEPENDENCE,
	SD_AFTER_THANKSGIVING,
	SD_BEFORE_CHRISTMAS
};

HOLI_DAY WINAPI B_IsTodayHoliday();
SHORT_DAY WINAPI B_IsTodayShortDay();
HOLI_DAY WINAPI B_IsNextDayHoliday();
SHORT_DAY WINAPI B_IsNextDayShortDay();
unsigned int WINAPI B_GetHolidayDate(HOLI_DAY holiday, unsigned int year);
unsigned int WINAPI B_GetShortdayDate(SHORT_DAY shortday, unsigned int year);

void WINAPI B_ClearFailedOptions();
unsigned int WINAPI B_GetFailedOptionCount();
void* WINAPI B_CreateFailedOptionIterator();
unsigned int WINAPI B_GetFailedOptionReloadAttempts();
void WINAPI B_ResubscribeToOption(StockBase* stockHandle);
void WINAPI B_ResubscribeToAllOptions();
const StockBase* WINAPI B_UnsubscribeOption(const char* symbol);
bool WINAPI B_DestroyOption(const char* symbol);
unsigned int WINAPI B_UnsubscribeUnusedOptions();
unsigned int WINAPI B_DestroyUnusedOptions();
void* WINAPI B_CreateOptionIterator();
const StockBase* WINAPI B_GetNextOption(void* iteratorHandle);
const StockBase* WINAPI B_GetOptionHandle(const char* symbol);
const StockBase* WINAPI B_FindOption(const char* symbol);
//const StockBase* WINAPI B_GetOptionHandleByNumId(unsigned __int64 symbol);
const StockBase* WINAPI B_FindOptionByNumId(unsigned __int64 symbol);
const StockBase* WINAPI B_FindStockByNumId(unsigned __int64 symbol);
//void WINAPI B_DeleteOption(StockBase* stockHandle);//Use B_DeleteStock to delete Option
unsigned int WINAPI B_GetLoadedOptionCount();

void* WINAPI B_CreateUnderlierIterator();
const Underlier* WINAPI B_GetNextUnderlier(void* iteratorHandle);
const Underlier* WINAPI B_GetUnderlierHandle(const char* symbol);
const Underlier* WINAPI B_FindUnderlier(const char* symbol);
unsigned int WINAPI B_GetUnderlierCount();
void WINAPI B_ClearFailedUnderliers();
unsigned int WINAPI B_GetFailedUnderlierCount();
void* WINAPI B_CreateFailedUnderlierIterator();
unsigned int WINAPI B_GetFailedUnderlierReloadAttempts();

unsigned int WINAPI B_GetOptionLongPositionsCount(const Observable* account);
unsigned int WINAPI B_GetOptionShortPositionsCount(const Observable* account);
unsigned int WINAPI B_GetOptionPositionsCount(const Observable* account);

unsigned int WINAPI B_GetOptionFlatPositionProperCount(const Observable* account = NULL);
unsigned int WINAPI B_GetOptionLongPositionProperCount(const Observable* account = NULL);
unsigned int WINAPI B_GetOptionShortPositionProperCount(const Observable* account = NULL);

void* WINAPI B_CreateAllOptionPositionIterator(const Observable* account);

Order* WINAPI B_FindStockOrder(unsigned int id, const Observable* account);
Order* WINAPI B_FindOptionOrder(unsigned int id, const Observable* account);

bool WINAPI B_IsAllowArcaOddLotOrders();
void WINAPI B_SetAllowArcaOddLotOrders(bool allow);

bool WINAPI B_IsSuperUser();

bool WINAPI B_SetLogPath(const char* path);//Should be called before trader logon, if ever.
const char* WINAPI B_GetLogPath();

unsigned int WINAPI B_GetAccountCmtaCount(const Observable* account);
const unsigned int* WINAPI B_FindAccountCmta(const char* cmta, const Observable* account);
void* WINAPI B_CreateAccountCmtaIterator(const Observable* account);
unsigned __int64 WINAPI B_GetAccountNextCmta(void* iteratorHandle, unsigned int* value = NULL);
unsigned __int64 WINAPI B_GetAccountCmta(const Observable* account);
bool WINAPI B_SetAccountCmta(unsigned __int64 cmta, Observable* account);

#ifdef __cplusplus
} //extern "C"
#endif


#endif

