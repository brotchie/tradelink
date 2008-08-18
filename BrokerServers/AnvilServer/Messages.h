#ifndef MESSAGESH
#define MESSAGESH

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "MessageIds.h"
#include "ObserverApi.h"

const char* const ReceiverHeaderVersion = "2.6.7.75";

const unsigned int totallyVisibleSizeHundreds = 999;

inline void FillStrBuffer(char* dest, size_t destSize, const char* src)
{
	for(; destSize > 1 && *src; --destSize, ++src, ++dest)
	{
		*dest = *src;
	}
	*dest = '\0';
/*
	const size_t len = strlen(src);
	if(len < destSize)
	{
		memcpy(dest, src, len + 1);
	}
	else
	{
		strncpy_s(dest, destSize, src, destSize - 1);
		dest[destSize - 1] = '\0';
	}
*/
}

inline void FillStrBufferAndPad(char* dest, size_t destSize, const char* src)
{
	for(; destSize > 1 && *src; --destSize, ++src, ++dest)
	{
		*dest = *src;
	}
	memset(dest, 0, destSize);
/*
	const size_t len = strlen(src);
	if(len < destSize)
	{
		memcpy(dest, src, len);
		memset(dest + len, 0, destSize - len);
	}
	else
	{
		strncpy_s(dest, destSize, src, destSize - 1);
		dest[destSize - 1] = '\0';
	}
*/
}

class MsgSubscription : public Message
{
public:
    unsigned int m_subscriptionId;
protected:
    MsgSubscription(unsigned int subscriptionId, const char* subscriptionName, unsigned short type):
        Message(type, sizeof(MsgSubscription) + (subscriptionName ? (unsigned short)strlen(subscriptionName) : 0)),
        m_subscriptionId(subscriptionId)
        {}
};

class MsgSubscribe : public MsgSubscription
{
public:
    MsgSubscribe(unsigned int subscriptionId, const char* subscriptionName):MsgSubscription(subscriptionId, subscriptionName, MSGID_SUBSCRIBE){}
};

class MsgUnsubscribe : public MsgSubscription
{
public:
    MsgUnsubscribe(unsigned int subscriptionId, const char* subscriptionName):MsgSubscription(subscriptionId, subscriptionName, MSGID_UNSUBSCRIBE){}
};

class MsgConnectionMade : public Message
{
public:
    MsgConnectionMade():Message(MSGID_CONNECTION_MADE, sizeof(MsgConnectionMade)){}
};

class MsgConnectionLost : public Message 
{
public:
    MsgConnectionLost():Message(MSGID_CONNECTION_LOST, sizeof(MsgConnectionLost)){}
};

class MsgClientAccepted : public Message 
{
public:
    MsgClientAccepted():Message(MSGID_CLIENT_ACCEPTED, sizeof(MsgConnectionLost)){}
};

class MsgBroadcastReceived : public Message 
{
public:
    MsgBroadcastReceived():Message(MSGID_BROADCAST_RECEIVED, sizeof(MsgBroadcastReceived)){}
};

class ReqResendPacket : public Message
{
public:
    ReqResendPacket(unsigned int sequenceNumber):
        Message(M_REQ_RESEND_PACKET, sizeof(ReqResendPacket)),
		m_sequenceNumber(sequenceNumber){}

    unsigned m_sequenceNumber;
};

class MsgMulticastPacket : public Message 
{
public:
    MsgMulticastPacket():Message(M_RESP_RESEND_PACKET, sizeof(MsgMulticastPacket)){}
	unsigned int m_sequenceNumber;
};


class MsgPoolInitialize:public Message
{
public:
    MsgPoolInitialize() : Message(M_POOL_INITIALIZE, sizeof(MsgPoolInitialize)){}

    unsigned int    x_OrderIdStart;
//    int             m_reservedField;
	char            x_UserId[ LENGTH_SYMBOL ];
};


class UserIdMessage : public Message
{
public:
    int m_reservedField;
	char m_userId[ LENGTH_SYMBOL ];
protected:
    UserIdMessage(unsigned int type, unsigned int size, const char* userId = NULL) : Message(type, size), m_reservedField(0)
	{
		if(userId)
		{
			FillStrBufferAndPad(m_userId, sizeof(m_userId), userId);
		}
		else
		{
			memset(m_userId, 0, sizeof(m_userId));
		}
	}
};

class ReqLoadOrderPoolFirst : public UserIdMessage
{
public:
    ReqLoadOrderPoolFirst(const char* userId) : UserIdMessage(M_REQ_LOAD_POOL_FIRST, sizeof(ReqLoadOrderPoolFirst), userId){}
};

class ReqLoadOrderPoolNext : public UserIdMessage
{
public:
    ReqLoadOrderPoolNext(const char* userId) : UserIdMessage(M_REQ_LOAD_POOL_NEXT, sizeof(ReqLoadOrderPoolNext), userId){}
};


class ReqLoadOrderPoolFirstEx : public UserIdMessage
{
public:
    ReqLoadOrderPoolFirstEx(const char* userId) : UserIdMessage(M_REQ_LOAD_POOL_FIRST_EX, sizeof(ReqLoadOrderPoolFirstEx), userId){}
};


class ReqLoadOrderPoolNextEx : public UserIdMessage
{
public:
    ReqLoadOrderPoolNextEx(const char* userId) : UserIdMessage(M_REQ_LOAD_POOL_NEXT_EX, sizeof(ReqLoadOrderPoolNextEx), userId){}
};

class MsgPoolExecution : public Message
{
public:
	MsgPoolExecution():
        Message(M_POOL_EXECUTION, sizeof(MsgPoolExecution)),
        x_ExecutionPrice(0),
        x_NumberOfShares(0),
        x_NumberOfSharesLeft(0),
        x_bMoreShares(false),
        x_Time(0),
        x_OrderId(0),
        x_executionId(0),
        m_bDecimal(1)
    {
        memset(x_UserId, 0, sizeof(x_UserId));
        memset(x_Symbol, 0, sizeof(x_Symbol));
        memset(x_CounterParty, 0, sizeof(x_CounterParty));
        memset(x_ExecutionType, 0, sizeof(x_ExecutionType));
        memset(m_achReference, 0, sizeof(m_achReference));
    }

	int 		    x_ExecutionPrice;
	long		    x_NumberOfShares;
	long		    x_NumberOfSharesLeft;
	bool		    x_bMoreShares;
	unsigned int    x_Time;
    char		    x_UserId[LENGTH_SYMBOL];
	char		    x_Symbol[LENGTH_SYMBOL];
	unsigned int	x_OrderId;
	char		    x_CounterParty[LENGTH_SYMBOL];
	char		    x_ExecutionType[4]; // SOEE for SOES, EEXO for Selectnet.
	unsigned long   x_executionId;
	char		    m_achReference[15];
	char		    m_bDecimal;

};


class MsgLoadOrderPoolCompleted : public UserIdMessage
{
public:
    MsgLoadOrderPoolCompleted() : UserIdMessage(M_RESP_LOAD_POOL_COMPLETED, sizeof(MsgLoadOrderPoolCompleted)){}
};

class R_Order
{
public:
    R_Order():
        x_OrderId(0),
        x_PendingRequest(0),
        x_Side(0),
        x_Market(0),
        x_Preferred(0),
        x_Tracking(0),
        x_CancelCounter(0),
        m_Flags(0),
        m_bDecimal(1),
        m_DestExchange(0),
        m_bBulletsUsed(0),
        m_bPossibleOverFill(0),
        m_bDowntick(0),
        m_bMarketLimit(0),
		m_bInstitutional(0),
		m_bAgency(0),
		m_bRetailAccount(0),

        x_Price(0),
        x_SizeRequested(0),
        x_SizeRemaining(0),

        x_Timeout(0),			// SNET:minutes, ISLD:seconds
        x_AcceptanceTime(0),	// To implement SNet 10 sec minimum order visibility
        m_OrderTime(0),		// When order has been placed
        m_CanceledTime(0),		// When order has been canceled if any
        m_StopPrice(0)		// Stop price
    {
        memset(m_achReference, 0, sizeof(m_achReference));
        memset(x_UserId, 0, sizeof(x_UserId));
        memset(x_Symbol, 0, sizeof(x_Symbol));
        memset(x_PreferredMM, 0, sizeof(x_PreferredMM));
        memset(m_FirmID, 0, sizeof(m_FirmID));
        *(unsigned int*)(m_FirmID + 4) |= (1 << 9);
    }
    char				x_UserId[LENGTH_SYMBOL];
	char				x_Symbol[LENGTH_SYMBOL];
	char				x_PreferredMM[LENGTH_SYMBOL];
	char				m_FirmID[LENGTH_SYMBOL];
	unsigned int		x_OrderId;

	//bit fields
	unsigned int		x_PendingRequest:2;
	unsigned int		x_Side:2;
	unsigned int		x_Market:4;
	unsigned int		x_Preferred:1;
	unsigned int		x_Tracking:4;
	unsigned int		x_CancelCounter:4;
	unsigned int		m_Flags:3;
	unsigned int		m_bDecimal:1;
	unsigned int		m_DestExchange:2;
	unsigned int		m_bBulletsUsed:1;
	unsigned int		m_bPossibleOverFill:1;
	unsigned int		m_bDowntick:1;
	unsigned int		m_bMarketLimit:1;
	unsigned int		m_bInstitutional:1;
	unsigned int		m_bAgency:1;
    unsigned int        m_bRetailAccount:1;

	int 				x_Price;
	unsigned int		x_SizeRequested;
	unsigned int		x_SizeRemaining;

	unsigned int		x_Timeout;			// SNET:minutes, ISLD:seconds
	unsigned int		x_AcceptanceTime;	// To implement SNet 10 sec minimum order visibility
	unsigned int		m_OrderTime;		// When order has been placed
	unsigned int		m_CanceledTime;		// When order has been canceled if any
	int 				m_StopPrice;		// Stop price

	char				m_achReference[15];
};


class MsgUpdateOrder : public Message
{
public:
    MsgUpdateOrder(const R_Order& order) : Message(M_POOL_UPDATE_ORDER, sizeof(MsgUpdateOrder)), m_order(order){}
    int m_reservedField;
    R_Order m_order;
};


class ReqNewOrder : public Message
{
public:
	ReqNewOrder(const R_Order& order) : Message(M_REQ_NEW_ORDER, sizeof(ReqNewOrder)), m_reservedField(0), m_order(order){}
	void SetIsoInfoLength(unsigned short len){m_size = sizeof(ReqNewOrder) + len;}
    int m_reservedField;
    R_Order m_order;
};


class SymbolMessage : public Message
{
public:
	char m_symbol[ LENGTH_SYMBOL ];//BSymbol
protected:
    SymbolMessage(unsigned short type, unsigned short size) : Message(type, size){}
	SymbolMessage(unsigned short type, unsigned short size, const char* symbol) : Message(type, size)
	{
		FillStrBufferAndPad(m_symbol, sizeof(m_symbol), symbol);
	}
};

///////////////////////////
//Options
class OptionId : public Message
{
public:
	unsigned __int64 m_symbol;
	unsigned __int64 m_strikePrice;
protected:
	OptionId(unsigned short type,
		unsigned short size,
		unsigned __int64 symbol,
		unsigned __int64 strikePrice):
		Message(type, size),
		m_symbol(symbol),
		m_strikePrice(strikePrice)
	{}
};

class OptionOrder : public OptionId
{
public:
    char m_userId[LENGTH_SYMBOL];
	unsigned int m_orderId;
	unsigned int m_parentId;
	int m_price;
	unsigned int m_orderSize;
	unsigned int m_sizeExecuted;
	unsigned int m_timeEntered;
	unsigned int m_tif;
	unsigned int m_flags;
	unsigned int m_side:2;
	unsigned int m_pendingRequest:2;
	unsigned int m_tracking:4;
	char m_marketId;
protected:
	OptionOrder(unsigned short type,
		unsigned short size,
		unsigned __int64 symbol,
		unsigned __int64 strikePrice,
		const char* userId,
		unsigned int orderId,
		unsigned int parentId,
		int price,
		unsigned int orderSize,
		unsigned int sizeExecuted,
		unsigned int timeEntered,
		unsigned int tif,
		unsigned int flags,
		unsigned int side,
		unsigned int pendingRequest,
		unsigned int tracking,
		char marketId):
		OptionId(type, size, symbol, strikePrice),
		m_orderId(orderId),
		m_parentId(parentId),
		m_price(price),
		m_orderSize(orderSize),
		m_sizeExecuted(sizeExecuted),
		m_timeEntered(timeEntered),
		m_tif(tif),
		m_flags(flags),
		m_side(side),
		m_pendingRequest(pendingRequest),
		m_tracking(tracking),
		m_marketId(marketId)
	{
		FillStrBufferAndPad(m_userId, sizeof(m_userId), userId);
	}
};

class ReqOptionOrderNew : public OptionOrder
{
public:
	ReqOptionOrderNew(unsigned __int64 symbol,
		unsigned __int64 strikePrice,
		const char* userId,
		unsigned int orderId,
		unsigned int parentId,
		int price,
		unsigned int orderSize,
		unsigned int sizeExecuted,
		unsigned int timeEntered,
		unsigned int tif,
		unsigned int flags,
		unsigned int side,
		unsigned int pendingRequest,
		unsigned int tracking,
		char marketId):
		OptionOrder(M_REQ_NEW_OPTION_ORDER, sizeof(ReqOptionOrderNew),
			symbol,
			strikePrice,
			userId,
			orderId,
			parentId,
			price,
			orderSize,
			sizeExecuted,
			timeEntered,
			tif,
			flags,
			side,
			pendingRequest,
			tracking,
			marketId)
	{}
	void SetIsoInfoLength(unsigned short len){m_size = sizeof(ReqNewOrder) + len;}
};

class MsgOptionOrderUpdate : public OptionOrder
{
public:
	MsgOptionOrderUpdate(unsigned __int64 symbol,
		unsigned __int64 strikePrice,
		const char* userId,
		unsigned int orderId,
		unsigned int parentId,
		int price,
		unsigned int orderSize,
		unsigned int sizeExecuted,
		unsigned int timeEntered,
		unsigned int tif,
		unsigned int flags,
		unsigned int side,
		unsigned int pendingRequest,
		unsigned int tracking,
		char marketId):
		OptionOrder(M_UPDATE_OPTION_ORDER, sizeof(MsgOptionOrderUpdate),
			symbol,
			strikePrice,
			userId,
			orderId,
			parentId,
			price,
			orderSize,
			sizeExecuted,
			timeEntered,
			tif,
			flags,
			side,
			pendingRequest,
			tracking,
			marketId)
	{}
};
///////////////////////////

///////////////////////////
//Staging
class ReqNewStagingOrder : public SymbolMessage
{
public:
	ReqNewStagingOrder(const char* symbol,
		const char* userId,
		const char* comment,
		unsigned int orderId,
		unsigned int parentId,
		unsigned int limitPrice,
		unsigned int orderSize,
		unsigned int sizeExecuted,
		unsigned int timeEntered,
		unsigned int side,
		unsigned int pendingRequest,
		unsigned int tracking):
		SymbolMessage(M_REQ_NEW_STAGING_ORDER, sizeof(ReqNewStagingOrder), symbol),
		m_orderId(orderId),
		m_parentId(parentId),
		m_limitPrice(limitPrice),
		m_orderSize(orderSize),
		m_sizeExecuted(sizeExecuted),
		m_timeEntered(timeEntered),
		m_side(side),
		m_pendingRequest(pendingRequest),
		m_tracking(tracking)
	{
		FillStrBufferAndPad(m_userId, sizeof(m_userId), userId);
		FillStrBufferAndPad(m_comment, sizeof(m_comment), comment);
	}
    char m_userId[LENGTH_SYMBOL];
	char m_comment[32];
	unsigned int m_orderId;
	unsigned int m_parentId;
	unsigned int m_limitPrice;
	unsigned int m_orderSize;
	unsigned int m_sizeExecuted;
	unsigned int m_timeEntered;
//	char m_side;
	unsigned int m_side:2;
	unsigned int m_pendingRequest:2;
	unsigned int m_tracking:4;
};

class MsgAssignStagingOrderId : public Message
{
public:
	MsgAssignStagingOrderId(unsigned uClientOrderId, unsigned uServerOrderID, const char* userId = NULL):
		Message(M_ASSIGN_STAGING_ORDER_ID, sizeof(MsgAssignStagingOrderId)),
		m_uClientOrderId(uClientOrderId),
		m_uServerOrderId(uServerOrderID)
	{
		if(userId)
		{
			FillStrBufferAndPad(m_userId, sizeof(m_userId), userId);
		}
		else
		{
			memset(m_userId, 0, sizeof(m_userId));
		}
	}
	unsigned int m_uClientOrderId;
    char  m_userId[LENGTH_SYMBOL];
	unsigned int m_uServerOrderId;
};

class MsgStagingOrderId : public Message
{
public:
	unsigned int m_orderId;
protected:
	MsgStagingOrderId(unsigned int orderId, unsigned short type, unsigned short size):
		Message(type, size),
		m_orderId(orderId)
	{
	}
};

class MsgUserStagingOrder : public Message
{
public:
    char m_userId[LENGTH_SYMBOL];
	unsigned int m_orderId;
protected:
	MsgUserStagingOrder(const char* userId, unsigned int orderId, unsigned short type, unsigned short size):
		Message(type, size),
		m_orderId(orderId)
	{
		FillStrBufferAndPad(m_userId, sizeof(m_userId), userId);
	}
};

class ReqCancelStagingOrder : public MsgStagingOrderId//MsgUserStagingOrder
{
public:
//	ReqCancelStagingOrder(const char* userId, unsigned int orderId):MsgUserStagingOrder(userId, orderId, M_REQ_CANCEL_STAGING_ORDER, sizeof(ReqCancelStagingOrder)){}
	ReqCancelStagingOrder(unsigned int orderId):MsgStagingOrderId(orderId, M_REQ_CANCEL_STAGING_ORDER, sizeof(ReqCancelStagingOrder)){}
};

class MsgStagingOrderCanceled : public MsgUserStagingOrder
{
public:
	MsgStagingOrderCanceled(const char* userId, unsigned int orderId):MsgUserStagingOrder(userId, orderId, M_STAGING_ORDER_CANCELED, sizeof(MsgStagingOrderCanceled)){}
};

class MsgStagingOrderCancelRejected : public MsgUserStagingOrder
{
public:
	MsgStagingOrderCancelRejected(const char* userId, unsigned int orderId):MsgUserStagingOrder(userId, orderId, M_STAGING_ORDER_CANCEL_REJECTED, sizeof(MsgStagingOrderCancelRejected)){}
};

class ReqLockStagingOrder : public MsgStagingOrderId//MsgUserStagingOrder
{
public:
//	ReqLockStagingOrder(const char* userId, unsigned int orderId):MsgUserStagingOrder(userId, orderId, M_REQ_LOCK_STAGING_ORDER, sizeof(ReqLockStagingOrder)){}
	ReqLockStagingOrder(unsigned int orderId):MsgStagingOrderId(orderId, M_REQ_LOCK_STAGING_ORDER, sizeof(ReqLockStagingOrder)){}
};

class MsgStagingOrderLocked : public MsgUserStagingOrder
{
public:
	MsgStagingOrderLocked(const char* userId, unsigned int orderId, unsigned int flags):
		MsgUserStagingOrder(userId, orderId, M_STAGING_ORDER_LOCKED, sizeof(MsgStagingOrderLocked)),
		m_flags(flags)
		{}
	unsigned int m_flags;
};

class MsgStagingOrderLockedByAnother : public MsgUserStagingOrder
{
public:
	MsgStagingOrderLockedByAnother(const char* userId, unsigned int orderId):MsgUserStagingOrder(userId, orderId, M_STAGING_ORDER_LOCKED_BY_ANOTHER, sizeof(MsgStagingOrderLockedByAnother)){}
};

class MsgStagingOrderLockRejected : public MsgUserStagingOrder
{
public:
	MsgStagingOrderLockRejected(const char* userId, unsigned int orderId):MsgUserStagingOrder(userId, orderId, M_STAGING_ORDER_LOCK_REJECTED, sizeof(MsgStagingOrderLockRejected)){}
};

class ReqUnlockStagingOrder : public MsgUserStagingOrder
{
public:
	ReqUnlockStagingOrder(const char* userId, unsigned int orderId):MsgUserStagingOrder(userId, orderId, M_REQ_UNLOCK_STAGING_ORDER, sizeof(ReqUnlockStagingOrder)){}
};

class MsgStagingOrderUnlocked : public MsgUserStagingOrder
{
public:
	MsgStagingOrderUnlocked(const char* userId, unsigned int orderId, unsigned int sizeUnlocked):
		MsgUserStagingOrder(userId, orderId, M_STAGING_ORDER_UNLOCKED, sizeof(MsgStagingOrderUnlocked)),
		m_sizeUnlocked(sizeUnlocked){}
	unsigned int m_sizeUnlocked;
};

class MsgStagingOrderUnlockRejected : public MsgUserStagingOrder
{
public:
	MsgStagingOrderUnlockRejected(const char* userId, unsigned int orderId):MsgUserStagingOrder(userId, orderId, M_STAGING_ORDER_UNLOCK_REJECTED, sizeof(MsgStagingOrderUnlockRejected)){}
};
////////////////

class ReqCancelOrder : public Message
{
public:
    ReqCancelOrder(unsigned int orderId) : Message(M_REQ_CANCEL_ORDER, sizeof(ReqCancelOrder)), x_OrderId(orderId){}
	unsigned int x_OrderId;
};

class ReqCancelWithReplacementOrder : public Message
{
public:
	ReqCancelWithReplacementOrder(unsigned int IDOfOrderToBeCancelled, const R_Order& newOrder):
        Message(M_REQ_CANCEL_WITH_REPLACEMENT_ORDER, sizeof(ReqCancelWithReplacementOrder)),
        m_IDOfOrderToBeCancelled(IDOfOrderToBeCancelled),
        m_newOrder(newOrder)
	{
    }
	unsigned int m_IDOfOrderToBeCancelled;
    ReqNewOrder m_newOrder;
};

class ReqOrderStatusReport : public Message
{
public:
    ReqOrderStatusReport(unsigned orderID):
		Message(M_REQ_ORDER_STATUS_REPORT, sizeof(ReqOrderStatusReport)), m_orderID(orderID){}
	unsigned int m_orderID;
};


class MsgMTOrderStatusReport : public Message
{
public:
	MsgMTOrderStatusReport(const char* message):
		Message(M_MT_ORDER_STATUS_REPORT, sizeof(MsgMTOrderStatusReport))
	{
		FillStrBufferAndPad(m_message, sizeof(m_message), message);
	}
    unsigned int reservedField;
	char         m_userId[ LENGTH_SYMBOL ];
	unsigned int m_orderId;
	unsigned int m_time;
    char         m_message[ 100 ];
};

class MsgAssignOptionOrderId : public Message
{
public:
	MsgAssignOptionOrderId(unsigned uClientOrderId, unsigned uServerOrderID, const char* userId = NULL):
		Message(M_POOL_ASSIGN_OPTION_ORDER_ID, sizeof(MsgAssignOptionOrderId)),
		m_uClientOrderId(uClientOrderId),
		m_uServerOrderId(uServerOrderID)
	{
		if(userId)
		{
			FillStrBufferAndPad(m_userId, sizeof(m_userId), userId);
		}
		else
		{
			memset(m_userId, 0, sizeof(m_userId));
		}
	}
	unsigned int m_uClientOrderId;
    char  m_userId[LENGTH_SYMBOL];
	unsigned int m_uServerOrderId;
};

class MsgAssignOrderId : public Message
{
public:
	MsgAssignOrderId(unsigned uClientOrderId, unsigned uServerOrderID, const char* userId = NULL):
		Message(M_POOL_ASSIGN_ORDER_ID, sizeof(MsgAssignOrderId)),
		m_uClientOrderId(uClientOrderId),
		m_uServerOrderId(uServerOrderID)
	{
		if(userId)
		{
			FillStrBufferAndPad(m_userId, sizeof(m_userId), userId);
		}
		else
		{
			memset(m_userId, 0, sizeof(m_userId));
		}
	}
	unsigned int m_uClientOrderId;
    char  m_userId[LENGTH_SYMBOL];
	unsigned int m_uServerOrderId;
};

class MsgCancelReject : public Message//see also MsgCancelRejected
{
public:
	MsgCancelReject(const char* userId = "", unsigned orderId = 0):
		Message(M_CANCEL_REJECTED, sizeof(MsgCancelReject)),
		m_orderId(orderId)
	{
		if(userId)
		{
			FillStrBufferAndPad(m_userId, sizeof(m_userId), userId);
		}
		else
		{
			memset(m_userId, 0, sizeof(m_userId));
		}
	}
	unsigned int m_orderId;
    char  m_userId[LENGTH_SYMBOL];
};

class MsgPoolCancel:public Message
{
public:
	MsgPoolCancel() : Message(M_POOL_CANCEL, sizeof(MsgPoolCancel)){}

	unsigned int	x_NumberOfShares;
	char		    x_UserId[LENGTH_SYMBOL];
	char    		x_Symbol[LENGTH_SYMBOL];
	unsigned int	x_OrderId;
};

class BulletsMessage : public Message
{
public:
	int		m_nSize;
	char    m_userId[LENGTH_SYMBOL];
	char    m_symbol[LENGTH_SYMBOL];
	char	m_bEnable;
protected:
	BulletsMessage(unsigned int type, unsigned int size, const char* userId, const char* symbol, int nSize, char bEnable):
		Message(type, size),
		m_nSize(nSize),
		m_bEnable(bEnable)
	{
		FillStrBufferAndPad(m_userId, sizeof(m_userId), userId);
		FillStrBufferAndPad(m_symbol, sizeof(m_symbol), symbol);
	}
};


class MsgPoolBulletsAdd : public BulletsMessage
{
public:
    MsgPoolBulletsAdd(const char* userId, const char* symbol, int nSize, char bEnable):
        BulletsMessage(M_POOL_BULLETS_ADD, sizeof(MsgPoolBulletsAdd), userId, symbol, nSize, bEnable){}
};


class ReqBulletsUpdate : public BulletsMessage
{
public:
    ReqBulletsUpdate(const char* userId, const char* symbol, int nSize, char bEnable):
        BulletsMessage(M_REQ_BULLETS_UPDATE, sizeof(ReqBulletsUpdate), userId, symbol, nSize, bEnable){}
};


class MsgBulletsUpdate : public BulletsMessage
{
public:
    MsgBulletsUpdate(const char* userId, const char* symbol, int nSize, char bEnable):
        BulletsMessage(M_RESP_BULLETS_UPDATE, sizeof(MsgBulletsUpdate), userId, symbol, nSize, bEnable){}
};


class MsgLoadOrderPoolNextEx : public Message
{
public:
	MsgLoadOrderPoolNextEx() : Message(M_RESP_LOAD_POOL_NEXT_EX, sizeof(MsgLoadOrderPoolNextEx)){}

	unsigned m_uMessageType;

	union RespMsgBuffer
	{
		char m_Execution[sizeof(MsgPoolExecution)];
		char m_Order[sizeof(MsgUpdateOrder)];
		char m_Cancel[sizeof(MsgPoolCancel)];
		char m_Bullet[sizeof(MsgBulletsUpdate)];
	};

	RespMsgBuffer m_RespMsg;
};


class OutsideTradeChangeMessage : public Message
{
public:
	unsigned int    x_ExecutionTime;
	char            x_TraderId[LENGTH_SYMBOL];
	char            x_Symbol[LENGTH_SYMBOL];
	char	        x_Side; // 'B' - Buy, 'S' - Sel, 'h' - Short Sell.
	int             x_Price;
	int		        x_Quantity;
	char	        x_Contra[LENGTH_SYMBOL];
	char	        x_ExecutionType[4]; // SOEE for SOES, EEXO for Selectnet.
	int		        x_Leaves; // Number of shares remaining.
	int		        x_BranchSeqId;
	unsigned int    m_OrderID;
	char	        m_achReference[15];
protected:
	OutsideTradeChangeMessage(unsigned int type, unsigned int size) : Message(type, size){}
};


class MsgTradeInsert: public OutsideTradeChangeMessage
{
public:
	MsgTradeInsert() : OutsideTradeChangeMessage(M_TRADE_INSERT, sizeof(MsgTradeInsert)), m_bDecimal(1){}
	char m_bDecimal;
};

class MsgTradeDelete: public OutsideTradeChangeMessage
{
public:
	MsgTradeDelete() : OutsideTradeChangeMessage(M_TRADE_DELETE, sizeof(MsgTradeDelete)){}
};


class OrderNotificationMessage : public Message
{
public:
    int m_reservedField;
	char x_UserId[LENGTH_SYMBOL];
	unsigned int x_OrderId;
protected:
	OrderNotificationMessage(unsigned int type, unsigned int size):
        Message(type, size),
        m_reservedField(0),
        x_OrderId(0)
    {
        memset(x_UserId, 0, sizeof(x_UserId));
    }
};


class MsgExecution : public OrderNotificationMessage
{
public:
	MsgExecution() : OrderNotificationMessage(M_MT_EXECUTION, sizeof(MsgExecution)){}
	unsigned int	x_Time;
	char			x_Contra[LENGTH_SYMBOL];
};


class MsgOrderFailure : public OrderNotificationMessage
{
public:
	int			 x_Reason;
	unsigned int x_Time;
protected:
	MsgOrderFailure(unsigned int type, unsigned int size):
        OrderNotificationMessage(type, size),
        x_Reason(0),
        x_Time(0)
    {}
};


class MsgUrOut:public MsgOrderFailure
{
public:
	MsgUrOut() : MsgOrderFailure(M_MT_UR_OUT, sizeof(MsgUrOut)){}
};

class MsgOrderRejected : public MsgOrderFailure
{
public:
	MsgOrderRejected() : MsgOrderFailure(M_MT_ORDER_REJECTED, sizeof(MsgOrderRejected)){}
};

class MsgCancelRejected : public MsgOrderFailure
{
public:
	MsgCancelRejected() : MsgOrderFailure(M_MT_CANCEL_REJECTED, sizeof(MsgCancelRejected)){}
};

class MsgOrderText:public Message
{
public:
	MsgOrderText() : Message(M_MT_TEXT, sizeof(MsgOrderText)){}

	enum {MAX_SIZE=240};

	enum{
		TYPE_OPENED,
		TYPE_CLOSED,
		TYPE_UR_OUT,
		TYPE_CANCEL_REJECTED,
		TYPE_ORDER_REJECTED,
		TYPE_FYI,
		TYPE_STATUS
	};

	int			    x_Type;
	char		    x_Symbol[LENGTH_SYMBOL];
	int 		    x_Price;
	unsigned int    x_SizeRemaining;
	char		    x_Text[MAX_SIZE];
};


//Orders

class MsgMarketClose : public Message
{
public:
	MsgMarketClose() : Message(M_MARKET_CLOSE , sizeof(MsgMarketClose)){}
};

class MsgMarketOpen : public Message
{
public:
	MsgMarketOpen() : Message(M_MARKET_OPEN, sizeof(MsgMarketOpen)){}
} ;

class MsgSoesSnetExecution : public Message
{
public:
	MsgSoesSnetExecution() : Message(M_SOES_SNET_EXECUTION, sizeof(MsgSoesSnetExecution)), m_bDecimal(1){}

	char x_BranchId[4];
	unsigned short x_SequenceNumber;
	char x_Side; // 'B' = Bot, 'S' = Sld, 'T' = Sld Shrt
    char m_bDecimal;
	unsigned int x_ExecutedShares;
	char x_Symbol[ LENGTH_SYMBOL ];
	int x_Price;
	unsigned int x_RemainingQuantity;
//	char x_OrderReference[5];
	char x_OrderReference[8];//to prpperly align
	char x_Contra[ LENGTH_SYMBOL ];
};



class MsgInvalidRequest : public Message  
{
public:
	MsgInvalidRequest() : Message(M_INVALID_REQUEST, sizeof(MsgInvalidRequest)){}
};

class MsgInvalidSymbol : public Message  
{
public:
	MsgInvalidSymbol() : Message(M_ERR_INVALID_SYMBOL, sizeof(MsgInvalidSymbol)){}
};

class MsgItch100VisibleExecution : public SymbolMessage
{
public:
	MsgItch100VisibleExecution(const char* symbol = ""):SymbolMessage(M_ITCH_100_VISIBLE_EXECUTION, sizeof(MsgItch100VisibleExecution), symbol){}
	unsigned int m_mmid;
	unsigned int m_Shares;
	int m_Price;
	char m_BuySellIndicator;
};

class MsgItch100ExecutionContraWithPrice : public Message
{
public:
	MsgItch100ExecutionContraWithPrice():Message(M_ITCH_100_EXECUTION_CONTRA_WITH_PRICE, sizeof(MsgItch100ExecutionContraWithPrice)){}
	unsigned int m_OrderReferenceNumber;
	char m_symbol[ LENGTH_SYMBOL ];//BSymbol
    char m_brokerCode[ LENGTH_SYMBOL ];
	unsigned int m_Shares;
	char m_BuySellIndicator;
    char m_reservedField[3];
	int m_Price;
};

class MsgAggregatedBook : public Message
{
public:
    MsgAggregatedBook() : Message(M_AGGREGATED_BOOK, sizeof(MsgAggregatedBook)){}
};


class MsgFlushAll : public SymbolMessage
{
public:
	MsgFlushAll() : SymbolMessage(M_FLUSH_ALL, sizeof(MsgFlushAll)){}
};


class MsgFlushCrossedLocked : public Message
{
public:
	MsgFlushCrossedLocked() : Message(M_FLUSH_CROSSED_LOCKED, sizeof(MsgFlushCrossedLocked)){}
};


class MsgFlushAllOpenBooks : public SymbolMessage
{
public:
	MsgFlushAllOpenBooks() : SymbolMessage(M_FLUSH_ALL_OPEN_BOOKS, sizeof(MsgFlushAllOpenBooks)){}
};

class MsgFlushBookForStock : public Message
{
public:
	MsgFlushBookForStock() : Message(M_FLUSH_BOOK_FOR_STOCK, sizeof(MsgFlushBookForStock)){}
	char m_chBookID;
    char m_reservedField[3];
	char m_symbol[ LENGTH_SYMBOL ];
};

class MsgFlushAttributedBook : public Message
{
public:
	MsgFlushAttributedBook(unsigned int mmid) : Message(M_FLUSH_ATTRIBUTED_BOOK, sizeof(MsgFlushAttributedBook)), m_mmid(mmid){}
	unsigned int m_mmid;
};

class MsgFlushAttributedBookForStock : public SymbolMessage
{
public:
	MsgFlushAttributedBookForStock(const char* symbol, unsigned int mmid) : SymbolMessage(M_FLUSH_ATTRIBUTED_BOOK_FOR_STOCK, sizeof(MsgFlushAttributedBookForStock), symbol), m_mmid(mmid){}
	unsigned int m_mmid;
};


/*
class MsgFYI : public Message
{
public:
	MsgFYI() : Message(M_FYI, sizeof(BMsgFYI)){}

	const BNasdaqStock* m_pNasdaqStock;
};
*/

class MsgTimeStamp : public Message
{
public:
	unsigned int x_ServerTime;
protected:
	MsgTimeStamp(unsigned short type, unsigned short size) : Message(type, size), x_ServerTime(0){}
};

class MsgServerHeartbeat : public MsgTimeStamp
{
public:
	MsgServerHeartbeat() : MsgTimeStamp(M_HEARTBEAT , sizeof(MsgServerHeartbeat)){}
};

class MsgServerKeepalive : public MsgTimeStamp
{
public:
	MsgServerKeepalive() : MsgTimeStamp(M_KEEPALIVE , sizeof(MsgServerKeepalive)){}
};

class MsgTransactionsKeepalive : public MsgTimeStamp
{
public:
	MsgTransactionsKeepalive() : MsgTimeStamp(M_TRANSACTIONS_KEEPALIVE , sizeof(MsgTransactionsKeepalive)){}
};

class MsgRemoteServer : public Message
{
public:
    MsgRemoteServer(unsigned char aProductNumber, unsigned long aIp):
		Message( M_REMOTE_SERVER , sizeof(MsgRemoteServer)),
		x_ProductNumber(aProductNumber),
		x_IpAddress(aIp){}
	
	unsigned char x_ProductNumber;
	unsigned long x_IpAddress;
};


class MsgSyncTime : public Message
{
public :
	MsgSyncTime() : Message(M_SYNC_TIME, sizeof(MsgSyncTime)){}
	
	SYSTEMTIME x_SystemTime;
};


class MsgNewSymbol : public SymbolMessage
{
public:
	MsgNewSymbol(const char* aSymbol) : SymbolMessage(M_NEW_SYMBOL, sizeof(MsgNewSymbol), aSymbol){}
};


class MsgResetSequenceNumbers : public Message
{
public:
	MsgResetSequenceNumbers() : Message(M_RESET_SEQUENCE_NUMBERS, sizeof(MsgResetSequenceNumbers)){}
};


class MsgFlushAllIsland : public Message
{
public:
	MsgFlushAllIsland()	: Message(M_FLUSH_ALL_ISLAND, sizeof(MsgFlushAllIsland)){}
};


class MsgUpdateIslandParty : public Message
{
public:
	MsgUpdateIslandParty() : Message(M_UPDATE_ISLAND_PARTY, sizeof(MsgUpdateIslandParty)){}
	
	unsigned int x_OrderReferenceNumber;
	char x_Symbol[LENGTH_SYMBOL];
	char x_Party[LENGTH_SYMBOL];
};


class MsgFlushVolume : public SymbolMessage
{
public:
	MsgFlushVolume() : SymbolMessage(M_FLUSH_VOLUME, sizeof(MsgFlushVolume)){}
};

class AttributedOrderMessage : public Message
{
public:
	unsigned __int64 m_OrderReferenceNumber;
	char m_symbol[LENGTH_SYMBOL];
	unsigned int m_mmid;
	unsigned int m_shares;
protected:
    AttributedOrderMessage(unsigned short type, unsigned short size) : Message(type, size){}
};

class MsgItch100AddAttributedOrder : public AttributedOrderMessage
{
public:
	MsgItch100AddAttributedOrder():AttributedOrderMessage(M_ITCH_1_00_NewVisibleAttributedOrder, sizeof(MsgItch100AddAttributedOrder)){}
	int m_price;
	unsigned char m_display;
	char m_BuySellIndicator;
};
 
class MsgItch100VisibleAttributedOrderExecution : public AttributedOrderMessage
{
public:
	MsgItch100VisibleAttributedOrderExecution():AttributedOrderMessage(M_ITCH_1_00_VisibleAttributedOrderExecution, sizeof(MsgItch100VisibleAttributedOrderExecution)){}
	unsigned __int64 m_matchNumber;
	char m_BuySellIndicator;
};

class MsgItch100AttributedCancelOrder : public AttributedOrderMessage
{
public:
	MsgItch100AttributedCancelOrder():AttributedOrderMessage(M_ITCH_1_00_ATTRIBUTED_CanceledOrder, sizeof(MsgItch100AttributedCancelOrder)){}
	char m_BuySellIndicator;
};

class MsgItch100HiddenAttributedOrderExecution : public Message
{
public:
	MsgItch100HiddenAttributedOrderExecution() : Message(M_ITCH_1_00_HiddenAttributedOrderExecution, sizeof(MsgItch100HiddenAttributedOrderExecution)){}

	unsigned __int64	m_OrderReferenceNumber;
	char				m_Symbol[LENGTH_SYMBOL];
	unsigned __int64	m_matchNumber;
	unsigned int		m_mmid;
	int 				m_Price;
	unsigned int		m_Shares;
	char				m_BuySellIndicator;
};

class MsgItch100AddOrder : public Message
{
public:
	MsgItch100AddOrder() : Message(M_ITCH_1_00_NewVisibleOrder, sizeof(MsgItch100AddOrder)){}

	unsigned int	m_OrderReferenceNumber;
	unsigned int	m_Shares;
	char			m_Symbol[LENGTH_SYMBOL];
	int 			m_Price;
	char			m_Party[LENGTH_SYMBOL];
    unsigned char   m_display;
	char			m_BuySellIndicator;
};

class MsgItch100VisibleOrderExecution : public Message
{
public:
	MsgItch100VisibleOrderExecution() : Message(M_ITCH_1_00_VisibleOrderExecution, sizeof(MsgItch100VisibleOrderExecution)){}
	unsigned int	m_OrderReferenceNumber;
	unsigned int	m_Shares;
	unsigned int	m_matchNumber;
	char			m_Symbol[LENGTH_SYMBOL];
    char			m_brokerCode[LENGTH_SYMBOL];
	char			m_BuySellIndicator;
};

class MsgItch100CancelOrder : public Message
{
public:
	MsgItch100CancelOrder() : Message(M_ITCH_1_00_CanceledOrder, sizeof(MsgItch100CancelOrder)){}

	unsigned int	m_OrderReferenceNumber;
	unsigned int	m_Shares;
	char			m_Symbol[LENGTH_SYMBOL];
	char			m_BuySellIndicator;
};

class MsgItch100HiddenOrderExecution : public Message
{
public:
	MsgItch100HiddenOrderExecution() : Message(M_ITCH_1_00_HiddenOrderExecution, sizeof(MsgItch100HiddenOrderExecution)){}

	unsigned int	m_OrderReferenceNumber;
	unsigned int	m_Shares;
	unsigned int	m_matchNumber;
	char			m_Symbol[LENGTH_SYMBOL];
	char			m_brokerCode[LENGTH_SYMBOL];
	int 			m_Price;
	char			m_BuySellIndicator;
};

class MsgItch100BrokenTrade : public Message
{
public:
	MsgItch100BrokenTrade() : Message(M_ITCH_1_00_BrokenTrade, sizeof(MsgItch100BrokenTrade)){}

	unsigned int m_matchNumber;
};



class MsgBook : public Message
{
public:
	unsigned int	m_OrderID;
	char			m_Symbol[ LENGTH_SYMBOL ];//BSymbol
	int 			m_Price;//BPrice
	unsigned int	m_Size;
	//bit fields
	unsigned		m_BookID	: 4;
	unsigned		m_Side		: 2;
	unsigned		m_MinSize	: 4;
	unsigned		m_Flags		: 22;
protected:
	MsgBook(unsigned short type, unsigned short size) : Message(type, size){}
};

class MsgBookNewOrder : public MsgBook
{
public:
	MsgBookNewOrder() : MsgBook(M_BOOK_NEW_ORDER, sizeof(MsgBookNewOrder)){}
};


class MsgBookModifyOrder : public MsgBook
{
public:
	MsgBookModifyOrder() : MsgBook(M_BOOK_MODIFY_ORDER, sizeof(MsgBookModifyOrder)){}
};

class MsgBookDeleteOrder : public Message
{
public:
	MsgBookDeleteOrder() : Message(M_BOOK_DELETE_ORDER, sizeof(MsgBookDeleteOrder)){}
	unsigned int	m_OrderID;
	char			m_Symbol[ LENGTH_SYMBOL ];//BSymbol
	unsigned int	m_Size;
	//bit fields
	unsigned		m_BookID	: 4;
	unsigned		m_Side		: 2;
	unsigned		m_MinSize	: 4;
	unsigned		m_Flags		: 22;
};

class MsgNyseOpenBook : public Message
{
public:
    MsgNyseOpenBook(unsigned short usNumberOfQuotes):
        Message(M_BOOK_NYSE_OPEN_BOOK, (sizeof(MsgNyseOpenBook) + usNumberOfQuotes * sizeof(struct NYSE_OPENBOOK_QUOTE)))
	{}

	struct NYSE_OPENBOOK_QUOTE
	{
		unsigned int    m_uQuoteID;
		int             m_Price;
		unsigned int    m_uQuantity;
	};

public:
	unsigned short	m_uNumberOfBuys;
	unsigned short	m_uNumberOfSells;
	char			m_Symbol[ LENGTH_SYMBOL ];
	// Contiguous list of Quotes.
	// !!! Should be always at the end of this class !!!
	// struct NYSE_OPENBOOK_QUOTE	m_NyseQuotes[ 1 ];
};

class MsgPopulationDone : public Message
{
public:
	MsgPopulationDone() : Message(M_RESP_POPULATION_DONE, sizeof(MsgPopulationDone) )
	{}
};

class ReqCacheSize : public Message
{
public:
	ReqCacheSize() : Message(M_REQ_CACHE_SIZE, sizeof(ReqCacheSize)){}
};

class MsgCacheSize : public Message
{
public:
	MsgCacheSize() : Message(M_RESP_CACHE_SIZE, sizeof(MsgCacheSize)){}
	unsigned x_Size;
};

class ReqStockCacheStatus : public Message
{
public:
	ReqStockCacheStatus() : Message(M_REQ_STOCK_CACHE_STATUS, sizeof(ReqStockCacheStatus)){}
};

//if this request issued server will filter messages only for symbols requested
class ReqFilterSymbols : public Message
{
public:
	ReqFilterSymbols() : Message(M_REQ_FILTER_SYMBOLS, sizeof(ReqFilterSymbols)){}
};

class MsgStockCacheStatus : public Message
{
public:
	MsgStockCacheStatus() : Message(M_RESP_STOCK_CACHE_STATUS, sizeof(MsgStockCacheStatus)){}

	RMarketStatus	m_MarketStatus;
	unsigned int	m_MaxGap[6];
};



class ReqRefreshSymbol : public SymbolMessage
{
public:
	ReqRefreshSymbol(const char* symbol = ""):SymbolMessage(M_REQ_REFRESH_SYMBOL, sizeof(ReqRefreshSymbol), symbol){}
};

class ReqUnsubscribeSymbol : public SymbolMessage
{
public:
    ReqUnsubscribeSymbol(const char* symbol =  ""):SymbolMessage(M_REQ_FROM_CLIENT_UNSUBSCRIBE_SYMBOL, sizeof(ReqUnsubscribeSymbol), symbol){}
};

class MsgUnsubscribeSymbolFailed : public SymbolMessage
{
public:
	MsgUnsubscribeSymbolFailed(const char* symbol = ""):SymbolMessage(M_RESP_TO_CLIENT_UNSUBSCRIBE_SYMBOL_FAILED, sizeof(MsgUnsubscribeSymbolFailed), symbol){}
};

class MsgSymbolUnsubscribed : public SymbolMessage
{
public:
	MsgSymbolUnsubscribed(const char* symbol = ""):SymbolMessage(M_RESP_TO_CLIENT_SYMBOL_UNSUBSCRIBED, sizeof(MsgSymbolUnsubscribed), symbol){}
};

class RStockTrade
{
public:
    int m_price;
    unsigned int m_shares;
//	time_t  m_time;
	unsigned int m_time;
	unsigned int m_status;
};


class RBookQuote
{
public:
	int             x_Price;
	unsigned int	x_Size;
	unsigned int	x_OrderReferenceNumber;
	unsigned int	m_BookID	: 4;//see enum BookIds in CommonIds.h
	unsigned int	m_MinSize	: 4;
	unsigned int	m_Unused	: 2;
	unsigned int	m_Flags		: 22;
	unsigned int	m_Reserved;

};

class RBookAttributedQuote
{
public:
	unsigned __int64	x_OrderReferenceNumber;
	int					x_Price;
	unsigned int		x_Size;
};

class RBookExecution
{
public:
    int	            x_Price;
	unsigned int	x_Size;
	unsigned int   	x_Time;			// Gives the time of Execution.
    unsigned int	x_ExecutionStatus;
	unsigned char	x_Side;			// Buy or Sell Side.
};

class RBookCancel
{
public:
	int 			x_Price;
	unsigned int	x_Size;
	unsigned int	x_Time;			// Gives the time of cancelation.
	unsigned char	x_Side;			// Buy or Sell Side.
};

class RBookFyi
{
public:
	char			m_MMID[ LENGTH_SYMBOL ];
	int 			m_Price;
	unsigned int	m_ExecutionTime;
	unsigned int	m_uSize;
	unsigned char	m_Side;
};

class MMQuote
{
public:
	unsigned	x_SeqNumber;
	char		x_MMID[ LENGTH_SYMBOL ];
	int		    x_Price;
	unsigned	x_Size;
	unsigned	x_Status:16;		// combination of status flags
	unsigned	m_QuoteCondition:8;		// Quote Condition (replaced m_Priority)
	int		    x_PrevPrice;		// previous price
	unsigned int x_Time;				// time of last change in status
		
	MMQuote():
		x_Status(0),
		x_Size(0),
		m_QuoteCondition('\0')
	{
		memset(x_MMID, 0, sizeof(x_MMID));
	}
		
	MMQuote(const char* MMID, int Price, unsigned int Size, int Status, int PrevPrice):
		x_Price(Price),
		x_Size(Size),
		x_Status(Status),
		x_PrevPrice(PrevPrice),
		m_QuoteCondition('\0')
	{
		FillStrBufferAndPad(x_MMID, sizeof(x_MMID), MMID);
	}

};


class MsgRefreshSymbol : public SymbolMessage
{
public:
    MsgRefreshSymbol(unsigned short aSize) : SymbolMessage(M_RESP_REFRESH_SYMBOL, aSize){}
	unsigned int m_refreshFlags;		// A bit field representing
										// all the steps that have
										// contributed for this message.
    enum
    {
	    REFRESHFLAG_LEVEL1 =    0x00000001,
    	REFRESHFLAG_LEVEL2 =    0x00000002,
    	REFRESHFLAG_BOOK =      0x00000004,
    	REFRESHFLAG_MMINFO =    0x00000008,
    	REFRESHFLAG_SOES =      0x00000010,
    	REFRESHFLAG_ATTRIB =    0x00000020,
    	REFRESHFLAG_END =		0x00000040,
		REFRESHFLAG_LRP =		0x00000080,
    };

};
//Option messages
class SymbolAsUIntMessage : public Message
{
public:
	unsigned int m_flags;
	unsigned __int64 m_symbol;
protected:
    SymbolAsUIntMessage(unsigned short type, unsigned short size, unsigned __int64 symbol, unsigned int flags) : Message(type, size),m_symbol(symbol),m_flags(flags){}
};

class MsgRefreshOption : public SymbolAsUIntMessage
{
public:
    MsgRefreshOption(unsigned short aSize, unsigned __int64 symbol = 0, unsigned int flags = 0) : SymbolAsUIntMessage(M_RESP_REFRESH_OPTION, aSize, symbol, flags){}
    enum
    {
	    REFRESHFLAG_LEVEL1 =    0x00000001,
    	REFRESHFLAG_LEVEL2 =    0x00000002,
    	REFRESHFLAG_BOOK =      0x00000004,
    	REFRESHFLAG_MMINFO =    0x00000008,
    	REFRESHFLAG_SOES =      0x00000010,
    	REFRESHFLAG_ATTRIB =    0x00000020,
    	REFRESHFLAG_END =		0x00000040,
		REFRESHFLAG_LRP =		0x00000080,
    };

};
class MsgRefreshOptionFailed : public SymbolAsUIntMessage
{
public:
	MsgRefreshOptionFailed(unsigned __int64 symbol) : SymbolAsUIntMessage(M_RESP_REFRESH_OPTION_FAILED, sizeof(MsgRefreshOptionFailed), symbol, 0){}
};

class ReqRefreshOption : public SymbolAsUIntMessage
{
public:
	ReqRefreshOption(unsigned __int64 symbol = 0, unsigned int flags = 0):SymbolAsUIntMessage(M_REQ_REFRESH_OPTION, sizeof(ReqRefreshOption), symbol, flags){}
};

class ReqUnsubscribeOption : public SymbolAsUIntMessage
{
public:
    ReqUnsubscribeOption(unsigned __int64 symbol = 0, unsigned int flags = 0):SymbolAsUIntMessage(M_REQ_FROM_CLIENT_UNSUBSCRIBE_OPTION, sizeof(ReqUnsubscribeOption), symbol, flags){}
};

class MsgOptionUnsubscribed : public SymbolAsUIntMessage
{
public:
	MsgOptionUnsubscribed(unsigned __int64 symbol = 0, unsigned int flags = 0):SymbolAsUIntMessage(M_RESP_TO_CLIENT_OPTION_UNSUBSCRIBED, sizeof(MsgOptionUnsubscribed), symbol, flags){}
};

class MsgUnsubscribeOptionFailed : public SymbolAsUIntMessage
{
public:
	MsgUnsubscribeOptionFailed(unsigned __int64 symbol = 0, unsigned int flags = 0):SymbolAsUIntMessage(M_RESP_TO_CLIENT_UNSUBSCRIBE_OPTION_FAILED, sizeof(MsgUnsubscribeOptionFailed), symbol, flags){}
};

class ReqRefreshUnderlier : public SymbolAsUIntMessage
{
public:
	ReqRefreshUnderlier(unsigned __int64 symbol = 0, unsigned int flags = 0):SymbolAsUIntMessage(M_REQ_REFRESH_UNDERLIER, sizeof(ReqRefreshUnderlier), symbol, flags){}
};

class MsgRefreshUnderlier : public SymbolAsUIntMessage
{
public:
    MsgRefreshUnderlier(unsigned short aSize, unsigned __int64 symbol = 0, unsigned int flags = 0) : SymbolAsUIntMessage(M_RESP_REFRESH_UNDERLIER, aSize, symbol, flags){}
};

class MsgRefreshUnderlierFailed : public SymbolAsUIntMessage
{
public:
	MsgRefreshUnderlierFailed(unsigned __int64 symbol = 0, unsigned int flags = 0) : SymbolAsUIntMessage(M_RESP_REFRESH_UNDERLIER_FAILED, sizeof(MsgRefreshUnderlierFailed), symbol, flags){}
};

//////

class MsgRefreshSymbolFailed : public SymbolMessage
{
public:
	MsgRefreshSymbolFailed(const char* symbol) : SymbolMessage(M_RESP_REFRESH_SYMBOL_FAILED, sizeof(MsgRefreshSymbolFailed), symbol){}
};

class ReqCrossedLocked : public Message
{
public:
	ReqCrossedLocked() : Message(M_REQ_CROSSED_LOCKED, sizeof(ReqCrossedLocked)){}
};

class MsgCrossedLocked : public Message
{
public:
	MsgCrossedLocked(unsigned short aSize) : Message(M_RESP_CROSSED_LOCKED, aSize){}
};

class ReqHeartbeat : public Message
{
public:
	ReqHeartbeat() : Message(M_REQ_HEARTBEAT, sizeof(ReqHeartbeat)){}
};

class MsgHeartbeat : public Message
{
public:
	MsgHeartbeat() : Message(M_RESP_HEARTBEAT){}
};

// Client calculates data and sends as message to transengine
class ReqLogTraderPnL : public Message  
{
public:
	ReqLogTraderPnL() : Message(M_REQ_LOG_TRADER_PNL, sizeof(ReqLogTraderPnL)){}
	enum { MAX_BRANCHIDLEN = 4 };
	char		x_BranchId[MAX_BRANCHIDLEN];
	
	double		x_OpenPnL,
				x_ClosedPnL,
				x_Exposure;
};


class ReqBroadcastMsg : public Message
{
public:
	enum
    {
        TEXTBUFF_SIZE = 1024
    };

    enum
    {
        CLIENT_TO_SERVER,
		SERVER_TO_CLIENT
    };

	char x_TextBuffer[TEXTBUFF_SIZE];
	unsigned int x_Status;


	ReqBroadcastMsg(const char* message = "", unsigned int status = SERVER_TO_CLIENT):
		Message(M_REQ_BROADCAST_MSG, sizeof(ReqBroadcastMsg)),
		x_Status(status)
	{
		FillStrBuffer(x_TextBuffer, sizeof(x_TextBuffer), message);
	}
};

class MsgLogon : public Message
{
public:
	MsgLogon(unsigned int RespFlags, unsigned int Version, unsigned int BuyingPower, unsigned int uMaxOpenPositionSize, 
				  unsigned int uMaxOpenPositionValue, unsigned int uMaxOrderSize, const char* FirmID, 
				  unsigned int dwMulticastIP, unsigned int uMulticastPort):
		Message( M_RESP_LOGON_EX, sizeof(MsgLogon) )
		, x_RespFlags( RespFlags )
		, x_Version( Version )
		, x_BuyingPower( BuyingPower )
		, x_ShortSellMultiplier(1)
		, m_uMaxOpenPositionSize( uMaxOpenPositionSize )
		, m_uMaxOpenPositionValue( uMaxOpenPositionValue )
		, m_uMaxOrderSize( uMaxOrderSize )
		, m_dwMulticastIP( dwMulticastIP )
		, m_uMulticastPort( uMulticastPort )
	{
		FillStrBufferAndPad(m_FirmID, sizeof(m_FirmID), FirmID);
	}

	enum
	{ 
		RESP_INSTITUTIONAL		= 0x00000001,
		RESP_RETAIL				= 0x00000002,
		RESP_RESTRICT_NASDAQ_MD	= 0x00000100,
		RESP_RESTRICT_NASDAQ_OE	= 0x00000200,
		RESP_RESTRICT_NYSE_MD	= 0x00000400,
		RESP_RESTRICT_NYSE_OE	= 0x00000800,
		RESP_RESTRICT_AMEX_MD	= 0x00001000,
		RESP_RESTRICT_AMEX_OE	= 0x00002000,
		RESP_RESTRICT_NYSE_OB	= 0x00004000,
		RESP_FAILED			    = 0x80000000
	};

    enum{ MAX_FIRM = 8 };
	
	unsigned x_RespFlags;
	unsigned x_Version;
	unsigned x_BuyingPower;
	unsigned x_ShortSellMultiplier;
	unsigned m_uMaxOpenPositionSize;
	unsigned m_uMaxOpenPositionValue;
	unsigned m_uMaxOrderSize;
	char	 m_FirmID[MAX_FIRM];
	unsigned m_dwMulticastIP;
	unsigned m_uMulticastPort;
};

class MsgLogonNew : public Message
{
public:
    MsgLogonNew(unsigned int RespFlags1,
        unsigned int Version, 
        unsigned int BuyingPower, 
        unsigned int x_ShortSellMultiplier,
        unsigned int uMaxOpenPositionSize, 
        unsigned int uMaxOpenPositionValue, 
        unsigned int uMaxOrderSize, 
        const char* FirmID, 
        unsigned int dwMulticastIP, 
        unsigned int uMulticastPort,
        unsigned int respFlags2,
        int minShortThreshold,
		unsigned short sizeOfFollowUpBuffer = 0):

		Message(M_RESP_LOGON_EX_1, sizeof(MsgLogonNew) + sizeOfFollowUpBuffer),
		x_RespFlags1(RespFlags1),
		x_Version(Version),
		x_BuyingPower(BuyingPower),
		x_ShortSellMultiplier(1),
		m_uMaxOpenPositionSize(uMaxOpenPositionSize),
		m_uMaxOpenPositionValue(uMaxOpenPositionValue),
		m_uMaxOrderSize(uMaxOrderSize),
		m_dwMulticastIP(dwMulticastIP),
		m_uMulticastPort(uMulticastPort),
		m_respFlags2(respFlags2),
		m_minShortThreshold(minShortThreshold)
	{
		FillStrBufferAndPad(m_FirmID, sizeof(m_FirmID), FirmID);
	}

    enum{ MAX_FIRM = 8 };

    enum TFlags1
    { 
        RESP_INSTITUTIONAL      = 0x00000001,
        RESP_RETAIL             = 0x00000002,
        RESP_RESTRICT_NASDAQ_MD = 0x00000100,
        RESP_RESTRICT_NASDAQ_OE = 0x00000200,
        RESP_RESTRICT_NYSE_MD   = 0x00000400,
        RESP_RESTRICT_NYSE_OE   = 0x00000800,
        RESP_RESTRICT_AMEX_MD   = 0x00001000,
        RESP_RESTRICT_AMEX_OE   = 0x00002000,
        RESP_RESTRICT_NYSE_OB   = 0x00004000,
        RESP_FAILED             = 0x80000000
    };
 
    enum TFlags2
    { 
        RESP_TRADE_DESK_ABLE    = 0x00000001
    };
 
    unsigned int x_RespFlags1;
    unsigned int x_Version;
    unsigned int x_BuyingPower;
    unsigned int x_ShortSellMultiplier;
    unsigned int m_uMaxOpenPositionSize;
    unsigned int m_uMaxOpenPositionValue;
    unsigned int m_uMaxOrderSize;
	char	     m_FirmID[MAX_FIRM];
    unsigned int m_dwMulticastIP;
    unsigned int m_uMulticastPort;
    int m_minShortThreshold;
    unsigned int m_respFlags2;
};

/*
class MsgNW2DatabaseInit : public Message
{
public:
	MsgNW2DatabaseInit():Message( M_NW2_DATABASE_INIT, sizeof(MsgNW2DatabaseInit)){}
	char x_ServiceCode;	// This specifies the database that was initialized.
};
*/
/*
class MsgSelectNet403 : public Message
{
public:
	MsgSelectNet403():Message(M_NW2_SELECTNET403, sizeof(MsgSelectNet403)){}

	// All strings are null terminated.

	char x_ActionCode;
	unsigned int x_ServiceIndex;
	char x_HostKey[ 6+1 ];
	char x_BeepIndicator;
	char x_HighlightQuantityFlag;
	char x_HighlightPriceFlag;
	char x_IDFlag;
	char x_LiabilityIndicatorFlag;
	char x_Side[ 2 ];
	unsigned int x_UnexecutedQuantity;
	char x_Symbol[ LENGTH_SYMBOL ];
	int x_Price;
	char x_NegotiablePriceFlag;
	char x_PartialCode[ 4+1 ];
	char x_OrderEntryFirmID[ 4 ];
	char x_OrderEntryMarketMakerFlag;
	char x_OrderNumber[ 5+1 ];
	char x_EntryTime[ 6+1 ];
	char x_TimeInForce[ 2+1 ];
	unsigned int x_OriginalQuantity;
	char x_Status;
	char x_MessageText[ 10 ];
	char x_NegotiationIndicatorFlag;
};

class MsgSelectNet402 : public Message
{
public:
	MsgSelectNet402():Message( M_NW2_SELECTNET402 , sizeof(MsgSelectNet402)){}

	char x_ActionCode;
	unsigned int x_ServiceIndex;
	char x_Symbol[ LENGTH_SYMBOL ];
	char x_Side;
};
*/
//Level 2 Quote status
const char MM_OK =               ' ';	    // Market maker OK.
const char MM_DELETED =          '1';	    // Market maker deleted.
const char MM_NOINSIDEQUOTE =    '2';    	// No inside bid/ask.
const char MM_INACTIVE =         '3';	    // Market not active.
const char MM_SUSPENDED =        '4';	    // Market maker suspended.
const char MM_NOQUOTE =          'A';	    // No Quote Available.
const char MM_PASSIVE =          'C';	    // Market maker passive.
/*
//ISLD Modify order reason
const char ITCH_MODIFY_REASON_EXECUTION =           'E';
const char ITCH_MODIFY_REASON_CANCEL =              'X';
const char ITCH_MODIFY_REASON_TIMEOUT =             'T';
const char ITCH_MODIFY_REASON_HALT =                'H';
const char ITCH_MODIFY_REASON_SUPERVISORYHALT =     'S';
*/
class MM_QUOTE
{
public:
	char        MarketMakerID[ LENGTH_SYMBOL ];//BMMId
	int		BidPrice;//BPrice
	int		AskPrice;//BPrice
	unsigned int BidSize;
	unsigned int AskSize;
	char		BidPriceInd;
	char		AskPriceInd;
	char		PriceStatus;

	// A bit mask to indicate which fields have changed.
	unsigned char FieldMask;

	//these fields do not require mask and will be set 
	//before they reach stock cache
	unsigned	SeqNumber;
	bool		IsTrusted;

//    char		m_Priority;			// Helps sort MM in the aggregate book.
    char		m_QuoteCondition;			// Quote Condition (replaced m_Priority)
	char		m_PrimaryExchange;

};


class MsgMMQuote : public SymbolMessage
{
public:
	MsgMMQuote(unsigned short aNumberOfQuotes):
		SymbolMessage(M_NW2_MM_QUOTE, (unsigned short)( sizeof( MsgMMQuote ) + aNumberOfQuotes * sizeof(MM_QUOTE) ) )
	{}

	
// Values that the Field Mask can have.
// Since the Market Maker ID is the key field, it will always be reported.
	enum
	{
		FIELD_MASK_BID_PRICE		= 0x02 ,
		FIELD_MASK_BID_SIZE			= 0x04 ,
		FIELD_MASK_ASK_PRICE		= 0x08 ,
		FIELD_MASK_ASK_SIZE			= 0x10 ,
		FIELD_MASK_BID_PRICE_IND	= 0x20 ,
		FIELD_MASK_ASK_PRICE_IND	= 0x40 ,
		FIELD_MASK_PRICE_STATUS		= 0x80 ,
	};

	enum
	{
		FIELD_BID_CHANGED = FIELD_MASK_BID_PRICE | FIELD_MASK_BID_SIZE,// | FIELD_MASK_BID_PRICE_IND,
		FIELD_ASK_CHANGED = FIELD_MASK_ASK_PRICE | FIELD_MASK_ASK_SIZE,// | FIELD_MASK_ASK_PRICE_IND,
    };
	unsigned short	m_numberOfQuotes;		// The number of Market Maker quotes in this message.
	unsigned char	m_isRefreshMessage;	// 1 = This is a Refresh Message, 0 = Update message.

    unsigned int x_TimeOfRequest;
    unsigned int x_SequenceNumber;
	 // Contiguous list of Quotes.
	 // !!! Should be always at the end of this class !!!
	 // struct MM_QUOTE	x_MMQuotes[ 1 ];
};

class MsgInsideQuote : public Message
{
public:
    MsgInsideQuote() : Message(M_NW2_INSIDE_QUOTE, sizeof(MsgInsideQuote)), x_FieldMask(0){}

	unsigned int	m_SeqNumber;
	char			x_Symbol[ LENGTH_SYMBOL ];//BSymbol
	int 			x_USInsideBidPrice;//BPrice
	unsigned int	x_USInsideBidSize;
	int     		x_USInsideAskPrice;//BPrice
	unsigned int	x_USInsideAskSize;
	char			x_USInsideTick;

	// A bit mask to indicate which fields have changed.
	unsigned char x_FieldMask;

	// Values that the Field Mask can have.
	enum
	{
		FIELD_MASK_REFRESH			= 0x01 ,	// This is a refresh message - Otherwise it is an Update message.
		FIELD_MASK_BID_PRICE		= 0x02 ,
		FIELD_MASK_BID_SIZE			= 0x04 ,
		FIELD_MASK_ASK_PRICE		= 0x08 ,
		FIELD_MASK_ASK_SIZE			= 0x10 ,
		FIELD_MASK_TICK				= 0x20 ,
		MASK_UPC_11830				= 0x40 ,
		MASK_HALT					= 0x80
	};

	char			m_PrimaryExchange;
};

class MsgTrade : public Message
{
public:
	// All string fields are null terminated.
	unsigned		m_SeqNumber;
	char			m_Symbol[ LENGTH_SYMBOL ];//BSymbol
	int 			m_price;//BPrice
protected:
	MsgTrade(unsigned short type, unsigned short size) : Message(type, size){}
};

class MsgLastTrade : public MsgTrade
{
public:
	MsgLastTrade() : MsgTrade(M_NW2_LAST_TRADE, sizeof(MsgLastTrade)), x_FieldMask(0){}

	// All string fields are null terminated.
	int 			x_TodaysHighPrice;//BPrice
	int 			x_TodaysLowPrice;//BPrice
	int 			x_PreviousClosePrice;//BPrice
	__int64			x_TodaysVolume;

	// A bit mask to indicate which fields have changed.
	unsigned char	x_FieldMask;

	// Values that the Field Mask can have.
	enum
	{
		FIELD_MASK_REFRESH				= 0x01,	// This is a refresh message - Otherwise it is an Update message.
		FIELD_MASK_LAST_SALE_PRICE		= 0x02,
		FIELD_MASK_LAST_SALE_TIME		= 0x04,
		FIELD_MASK_TODAYS_HIGH_PRICE	= 0x08,
		FIELD_MASK_TODAYS_LOW_PRICE		= 0x10,
		FIELD_MASK_TODAYS_VOLUME		= 0x20,
		FIELD_MASK_PREVIOUS_CLOSE_PRICE = 0x40,
		FIELD_MASK_TRADE_SIZE           = 0x80
	};

	char			m_PrimaryExchange;
	char m_saleCondition;
};


class MsgLevel2Quote : public Message
{
public:
	unsigned	m_SeqNumber;
	char		m_Symbol[ LENGTH_SYMBOL ];
	char		m_MarketMakerID[ LENGTH_SYMBOL ];
	int 		m_BidPrice;
	int 		m_AskPrice;
	unsigned	m_BidSize;
	unsigned	m_AskSize;
	char		m_PriceStatus;

	// A bit mask to indicate which fields have changed.
	unsigned char m_FieldMask;

	//these fields do not require mask and will be set 
	//before they reach stock cache

    char		m_QuoteCondition;			// Quote Condition (replaced m_Priority)
	char		m_PrimaryExchange;

	 // Values that the Field Mask can have.
	 // Since the Market Maker ID is the key field, it will always be reported.
    enum
    {
        FIELD_MASK_BID_PRICE		= 0x01,
        FIELD_MASK_BID_SIZE			= 0x02,
        FIELD_MASK_ASK_PRICE		= 0x04,
        FIELD_MASK_ASK_SIZE			= 0x08,
//      FIELD_MASK_PRICE_STATUS		= 0x10,
//		FIELD_MASK_BID_PRICE_IND	= 0x20,
//		FIELD_MASK_ASK_PRICE_IND	= 0x40,
        FIELD_MASK_BID_CHANGES		= FIELD_MASK_BID_PRICE | FIELD_MASK_BID_SIZE,// | FIELD_MASK_BID_PRICE_IND | FIELD_MASK_PRICE_STATUS,
        FIELD_MASK_ASK_CHANGES		= FIELD_MASK_ASK_PRICE | FIELD_MASK_ASK_SIZE,// | FIELD_MASK_ASK_PRICE_IND | FIELD_MASK_PRICE_STATUS,
    };
protected:
    MsgLevel2Quote(unsigned short type, unsigned short size) : Message(type, size){}
};

class MsgLevel2NotAttributedQuote : public MsgLevel2Quote
{
public:
    MsgLevel2NotAttributedQuote() : MsgLevel2Quote(M_LEVEL2_QUOTE, sizeof(MsgLevel2NotAttributedQuote)){}
};

class MsgLastTradeShort : public MsgTrade
{
public:
	MsgLastTradeShort() : MsgTrade(M_LAST_TRADE_SHORT, sizeof(MsgLastTradeShort)), m_FieldMask(0), m_PrimaryExchange(ANY), m_ExecutionExchange(ExecExch_ANY){}

	// All string fields are null terminated.
	unsigned int m_LastTradeVolume;

	// A bit mask to indicate which fields have changed.
	unsigned char m_FieldMask;

	// Values that the Field Mask can have.
	enum
	{
		FIELD_MASK_REFRESH				= 0x01,	// This is a refresh message - Otherwise it is an Update message.
		FIELD_MASK_LAST_SALE_PRICE		= 0x02,
		FIELD_MASK_LAST_SALE_TIME		= 0x04,
		FIELD_MASK_TRADE_SIZE           = 0x08
	};

	char m_PrimaryExchange;
	char m_ExecutionExchange;
	char m_saleCondition;
};

class MsgLevel1QuoteSingle : public Message
{
public:
	MsgLevel1QuoteSingle():
		Message(M_LEVEL1_QUOTE_SINGLE, sizeof(MsgLevel1QuoteSingle)),
		m_seqNumber(0),
		m_price(0),
		m_size(0),
		m_tick('\0'),
		m_mask(0),
		m_primaryExchange(0),
		m_extraFlag(0)
		{}
	unsigned int m_seqNumber;
	char m_symbol[ LENGTH_SYMBOL ];
	int m_price;
	unsigned int m_size;
	char m_tick;
	enum Mask                                                                                      
	{
		MASK_REFRESH  = 0x01, // This is a refresh message - Otherwise it is an Update message.
		MASK_PRICE    = 0x02,                                                                  
		MASK_SIZE     = 0x04,                                                                  
		MASK_SIDE     = 0x08,                                                                  
		MASK_TICK     = 0x10,                                                                  
		MASK_UPC_11830= 0x20,                                                                  
		MASK_HALT     = 0x40                                                                   
    };                                                                                               
	unsigned char m_mask;
	char m_primaryExchange;
	char m_extraFlag;
};

class MsgLevel2QuoteSingle : public Message
{
public:
	MsgLevel2QuoteSingle() : Message(M_LEVEL2_QUOTE_SINGLE, sizeof(MsgLevel2QuoteSingle)){}

	unsigned	m_SeqNumber;
	char		m_Symbol[ LENGTH_SYMBOL ];
	char		m_MarketMakerID[ LENGTH_SYMBOL ];
	int 		m_Price;
	unsigned	m_Size;
	char		m_PriceStatus;

	// A bit mask to indicate which fields have changed.
	unsigned char m_FieldMask;

	//these fields do not require mask and will be set 
	//before they reach stock cache

    char		m_QuoteCondition;			// Quote Condition (replaced m_Priority)
	char		m_PrimaryExchange;

	 // Values that the Field Mask can have.
	 // Since the Market Maker ID is the key field, it will always be reported.
	enum
	{
		FIELD_MASK_PRICE			= 0x01,
		FIELD_MASK_SIZE				= 0x02,
		FIELD_MASK_SIDE				= 0x04,
		FIELD_MASK_PRICE_STATUS		= 0x08,
	};
};

class MsgTalLastTrade : public MsgTrade
{
public:
	MsgTalLastTrade():MsgTrade(M_TAL_LAST_TRADE, sizeof(MsgTalLastTrade)){}
	unsigned int	x_LastSize;
	int 			x_HighPrice;//BPrice
	int 			x_LowPrice;//BPrice
	int 			x_OpenPrice;//BPrice
	int 			x_ClosePrice;//BPrice
	unsigned __int64			x_SessionVolume;
	char			m_PrimaryExchange;
};

class MsgSoesInfo : public Message
{
public:
	MsgSoesInfo() : Message(M_NW2_SOES_INFO, sizeof(MsgSoesInfo)), x_FieldMask(0){}

	char x_SoesType;
	unsigned int x_SoesTierSize;
	char x_SecurityName[ 20+1 ];

// A bit mask to indicate which fields have changed.
	unsigned char x_FieldMask;

// Values that the Field Mask can have.
	enum
	{
		FIELD_MASK_REFRESH			= 0x01 ,	// This is a refresh message - Otherwise it is an Update message.
		FIELD_MASK_SOES_TYPE		= 0x02 ,
		FIELD_MASK_SOES_TIER_SIZE	= 0x04 ,
		FIELD_MASK_SECURITY_NAME	= 0x08
	};

	char x_Symbol[ LENGTH_SYMBOL ];//BSymbol
};

class MsgIndexDetails : public SymbolMessage
{
public:
	MsgIndexDetails() : SymbolMessage(M_NW2_INDEX_DETAILS, sizeof(MsgIndexDetails)){}

	int			x_IndexValue;//BPrice
	int			x_IndexNetChange;//BPrice
	int			x_IndexHighValue;//BPrice
	int			x_IndexLowValue;//BPrice
};

class MsgMarketImbalance : public SymbolMessage
{
public:
	MsgMarketImbalance():
        SymbolMessage(M_MARKET_IMBALANCE, sizeof(MsgMarketImbalance)),
        m_securityTradingStatus (FLAG_UNDEFINED_IMBALANCE),
        m_buyVolume(0),
        m_sellVolume(0){}

    TImbalanceFlags m_securityTradingStatus;     
    unsigned int m_buyVolume;
    unsigned int m_sellVolume;
};

class MsgNewMarketImbalance : public SymbolMessage
{
public:
    MsgNewMarketImbalance():
        SymbolMessage(M_NEW_MARKET_IMBALANCE, sizeof(MsgNewMarketImbalance)),
        m_ImbalanceSide(FLAG_UNDEFINED_IMBALANCE),
        m_matchedShares(0),
        m_imbalance(0),
        m_CurrentReferencePrice(0),
        m_NearIndicativeClearingPrice(0),
        m_FarIndicativeClearingPrice(0)
    {
    }
 
    TImbalanceFlags m_ImbalanceSide;     
    unsigned int m_matchedShares;     
    unsigned int m_imbalance;     
    int m_CurrentReferencePrice;
    int m_NearIndicativeClearingPrice;
    int m_FarIndicativeClearingPrice;
    char m_noIndicativeClearingPriceFlag;
    char m_priceVariationIndicator;
};

class MsgLRP : public SymbolMessage
{
public:
	int m_price;
//	unsigned int m_flags;
protected:
    MsgLRP(unsigned short type, unsigned short size):SymbolMessage(type, sizeof(size)){}
};

class MsgLRPBid : public MsgLRP
{
public:
    MsgLRPBid():MsgLRP(M_LRP_BID, sizeof(MsgLRPBid)){}
};

class MsgLRPAsk : public MsgLRP
{
public:
    MsgLRPAsk():MsgLRP(M_LRP_ASK, sizeof(MsgLRPAsk)){}
};

class MsgLRPBidAsk : public MsgLRP
{
public:
    MsgLRPBidAsk():MsgLRP(M_LRP_BIDASK, sizeof(MsgLRPBidAsk)){}
	int m_priceAsk;
};

////////////////////////



//Requests


const unsigned int versionDateTime = 706011706;

class ReqLogonNew : public Message
{
public:
    ReqLogonNew(const char* userId,
        const char* password,
        unsigned int logonFlags,
        const char* fileVersion,
        unsigned int numericVersion,
        const char* clientId,
        char allowTrade = 'N',
        unsigned int localIp = 0,
		unsigned int localPort = 0):

		Message(M_REQ_LOGON_EX_1, sizeof(ReqLogonNew)),
		m_logonFlags(logonFlags),
		m_allowTrade(allowTrade),
		m_version(versionDateTime),
		m_localIp(localIp),
		m_localPort(localPort),
		m_numericVersion(numericVersion)
	{
		FillStrBufferAndPad(m_userId, sizeof(m_userId), userId);
		FillStrBufferAndPad(m_password, sizeof(m_password), password);
		FillStrBufferAndPad(m_fileVersion, sizeof(m_fileVersion), fileVersion);
		FillStrBufferAndPad(m_clientId, sizeof(m_clientId), clientId);
	}

	enum
	{ 
		LF_USE_MCAST_MKTDATA	= 0x01,
		LF_REMOTE_SERVER		= 0x02,
		LF_MKTDATA_NONE			= 0x04,
		LF_MKTDATA_NASDAQ		= 0x08,
		LF_MKTDATA_LISTED		= 0x10,
		LF_MKTDATA_ISLAND		= 0x20,
		LF_MKTDATA_ALL			= LF_MKTDATA_NASDAQ | LF_MKTDATA_LISTED | LF_MKTDATA_ISLAND,

        LF_CLIENT_WANTS_COMPRESSED_TRAFFIC              = 0x00000080,
        LF_CLIENT_WANTS_PENDING_ORDERS                  = 0x00000100,
        LF_CLIENT_WANTS_EXECUTED_ORDERS                 = 0x00000200,
        LF_TRANSACTIONS_NONE                            = 0x00000400,
        LF_CLIENT_WANTS_COMPRESSED2_TRAFFIC             = 0x00020000,
        LF_CANCEL_PENDING_ORDERS_ON_DISCONNECT          = 0x00040000,
	};
	
	enum{ MAX_USERID_SIZE = 32 };
	enum{ MAX_PASSWORD_SIZE = 32 };
	enum{ MAX_FILEVERSION_SIZE = 24 };


	char m_userId[MAX_USERID_SIZE];
	char m_password[MAX_PASSWORD_SIZE];
	char m_allowTrade;					// This is a request to allow trades on the client app.
												// 'Y' means Yes, otherwise No - it is in Simulation mode.
	unsigned int m_logonFlags;					// to be defined
	
	unsigned int m_version;						// The version number check must be performed
												// by the server in production mode.
												// The version number is of the format
												// mddyyHHMM. Eg: 928981624 for 09/28.
												// Eg: 1128981624 for 11/28.
    unsigned int m_localIp;
    unsigned int m_localPort;
    unsigned int m_numericVersion;
    char m_clientId[MAX_FILEVERSION_SIZE];

	char m_fileVersion[MAX_FILEVERSION_SIZE];
};

class ReqLogon : public Message
{
public:
	ReqLogon(const char* userId, const char* password, unsigned int logonFlags, const char* fileVersion, char allowTrade = 'N'):
		Message(M_REQ_LOGON_EX, sizeof(ReqLogon)),
		m_logonFlags(logonFlags),
		m_allowTrade(allowTrade),
		m_version(versionDateTime)
	{
		FillStrBufferAndPad(m_userId, sizeof(m_userId), userId);
		FillStrBufferAndPad(m_password, sizeof(m_password), password);
		FillStrBufferAndPad(m_fileVersion, sizeof(m_fileVersion), fileVersion);
	}

	enum
	{ 
		LF_USE_MCAST_MKTDATA	= 0x01,
		LF_REMOTE_SERVER		= 0x02,
		LF_MKTDATA_NONE			= 0x04,
		LF_MKTDATA_NASDAQ		= 0x08,
		LF_MKTDATA_LISTED		= 0x10,
		LF_MKTDATA_ISLAND		= 0x20,
		LF_MKTDATA_ALL			= LF_MKTDATA_NASDAQ | LF_MKTDATA_LISTED | LF_MKTDATA_ISLAND,
        LF_CLIENT_WANTS_COMPRESSED_TRAFFIC              = 0x00000080,
        LF_CLIENT_WANTS_PENDING_ORDERS                  = 0x00000100,
        LF_CLIENT_WANTS_EXECUTED_ORDERS                 = 0x00000200,
        LF_TRANSACTIONS_NONE                            = 0x00000400,
	};

	enum{ MAX_USERID_SIZE = 32 };
	enum{ MAX_PASSWORD_SIZE = 32 };
	enum{ MAX_FILEVERSION_SIZE = 24 };

	char		m_userId[MAX_USERID_SIZE];
	char		m_password[MAX_PASSWORD_SIZE];
	char		m_allowTrade;					// This is a request to allow trades on the client app.
												// 'Y' means Yes, otherwise No - it is in Simulation mode.
	unsigned int	m_logonFlags;					// to be defined

	unsigned int	m_version;						// The version number check must be performed
												// by the server in production mode.
												// The version number is of the format
												// mddyyHHMM. Eg: 928981624 for 09/28.
												// Eg: 1128981624 for 11/28.
	char		m_fileVersion[MAX_FILEVERSION_SIZE];
};


class ReqPopulation : public Message
{
public:
	ReqPopulation() : Message(M_REQ_POPULATION, sizeof(ReqPopulation)){}
};

class ReqPopulateNext : public Message
{
public:
	ReqPopulateNext() : Message(M_REQ_POPULATE_NEXT, sizeof(ReqPopulateNext)){}
};


class MsgTextAscii : public Message
{
public:
    MsgTextAscii(const char* text) : Message(M_TEXT_ASCII, sizeof(MsgTextAscii))
	{
		FillStrBuffer(m_text, sizeof(m_text), text);
	}

    const char* GetText() const{return m_text;}
private:
	char m_text[300 + 1] ;	// Null terminated human readable characters in good Old English language.
};

class MessageAnvilServer : public Message 
{
public:
    MessageAnvilServer(unsigned short anvilServerMessageType, unsigned short size):
		Message(M_ANVIL_SERVER, size),
		m_anvilServerMessageType(anvilServerMessageType)
	{}
	unsigned short m_anvilServerMessageType;
	unsigned short m_reserved;
};

class MsHistoryChart : public SymbolMessage
{
public:
	unsigned int m_from;
	unsigned int m_to;
	unsigned int m_unit;
protected:
	MsHistoryChart(const char* symbol, unsigned short type, unsigned short size, unsigned int from, unsigned int to = 0, unsigned int unit = 0):
		SymbolMessage(type, size, symbol),
		m_from(from),
		m_to(to),
		m_unit(unit)
	{}
};

class ReqMsHistoryChart : public MsHistoryChart
{
public:
	ReqMsHistoryChart(const char* symbol, unsigned int from, unsigned int to = 0, unsigned int unit = 0):
		MsHistoryChart(symbol, MS_REQ_HISTORY_CHART, sizeof(ReqMsHistoryChart), from, to, unit)
	{}
};

class MsgMsHistoryChart : public MsHistoryChart
{
public:
	MsgMsHistoryChart(const char* symbol, unsigned int from, unsigned int to = 0, unsigned int unit = 0):
		MsHistoryChart(symbol, MS_RESP_HISTORY_CHART, sizeof(MsgMsHistoryChart), from, to, unit),
		m_baseSize(sizeof(MsgMsHistoryChart)),
		m_fromFound(0xFFFFFFFF),
		m_toFound(0xFFFFFFFF)
	{}
	unsigned int m_baseSize;
	unsigned int m_fromFound;
	unsigned int m_toFound;
};

class ReqMsRefreshSymbolChart : public SymbolMessage
{
public:
	ReqMsRefreshSymbolChart() : SymbolMessage(MS_REFRESH_SYMBOL_CHART, sizeof(ReqMsRefreshSymbolChart)){}
};
/*
class ReqMsRefreshIndexChart : public SymbolMessage
{
public:
	ReqMsRefreshIndexChart() : SymbolMessage(MS_REFRESH_INDEX_CHART, sizeof(ReqMsRefreshIndexChart)){}
};
*/
class MsgMsRefreshChart : public SymbolMessage
{
public:
    unsigned short m_minutesChartStart;//7 am = 420 minutes
    //Follows : (m_size - sizeof(MsgMsRefreshChart)) / 8   *   (unsigned int price, unsigned int volume)
protected:
	MsgMsRefreshChart(unsigned short type, unsigned short size) : SymbolMessage(type, size){}
};

class MsgMsRefreshSymbolChart : public MsgMsRefreshChart
{
public:
	MsgMsRefreshSymbolChart() : MsgMsRefreshChart(MS_REFRESH_SYMBOL_CHART, sizeof(MsgMsRefreshSymbolChart)){}
};
/*
class MsgMsRefreshIndexChart : public MsgMsRefreshChart
{
public:
	MsgMsRefreshIndexChart() : MsgMsRefreshChart(MS_REFRESH_INDEX_CHART, sizeof(MsgMsRefreshIndexChart)){}
};
*/

class ReqMsPopulateSymbolSortable : public SymbolMessage
{
public:
	bool m_chart;
protected:
	ReqMsPopulateSymbolSortable(unsigned short type, bool chart = false) : SymbolMessage(type, sizeof(ReqMsPopulateSymbolSortable)), m_chart(chart){}
};

class ReqMsPopulateFirstSymbolSortable : public ReqMsPopulateSymbolSortable
{
public:
	ReqMsPopulateFirstSymbolSortable(bool chart = false) : ReqMsPopulateSymbolSortable(MS_REQ_POPULATE_FIRST_SYMBOL_SORTABLE, chart){}
};

class ReqMsPopulateNextSymbolSortable : public ReqMsPopulateSymbolSortable
{
public:
	ReqMsPopulateNextSymbolSortable(bool chart = false) : ReqMsPopulateSymbolSortable(MS_REQ_POPULATE_NEXT_SYMBOL_SORTABLE, chart){}
};

class MsgPrintRemoved : public SymbolMessage
{
public:
	MsgPrintRemoved(const char* symbol):
		SymbolMessage(MS_RESP_SYMBOL_SORTABLE_PRINT_REMOVED, sizeof(MsgPrintRemoved), symbol),
		m_vwap(0),
		m_moneyTraded(0),
		m_priceStart(0),
		m_priceMin(0),
		m_priceMax(0),
		m_priceEnd(0),
		m_volume(0),
		m_ordinal(0),
		m_second(0),
		m_price(0),
		m_size(0),
		m_saleCondition(0),
		m_hidden(0)
	{}
    unsigned int m_vwap;
    __int64 m_moneyTraded;
    unsigned int m_priceStart;
    unsigned int m_priceMin;
    unsigned int m_priceMax;
    unsigned int m_priceEnd;
	unsigned int m_volume;

	unsigned int m_ordinal;
	unsigned int m_second;
	unsigned int m_price;
	unsigned int m_size;
	char m_saleCondition;
	char m_hidden;
};

class SymbolSortableSnapshot
{
public:
	SymbolSortableSnapshot():
        m_chartSize(0),
        m_bid(0),
        m_ask(0),
//        m_minBid(0),
//        m_maxBid(0),
        m_openPrice(0),
        m_closePrice(0),
        m_intradayHigh(0),
        m_intradayLow(0),
        m_dayHigh(0),
        m_dayLow(0),
        m_firstTradePrice(0),
        m_firstPrice(0),
        m_lastTradePrice(0),
        m_lastTradeSize(0),
        m_firstVolume(0),
        m_volume(0),
	    m_primaryExchange(ANY),
	    m_stockAttributes('\0'),
        m_bidTick(' '),
        m_bidSize(0),
        m_askSize(0),
        m_nyseImbalanceType(-1),
        m_nyseBuyVolume(0),
        m_nyseSellVolume(0),
        m_structureLength(sizeof(SymbolSortableSnapshot)),
        m_preMarketIndicatorBid(0),
        m_preMarketIndicatorAsk(0),
        m_preMarketIndicatorTime(0),
        m_preMarketIndicatorOrdinal(0),
        m_todaysClosePrice(0),
        m_yesterdaysVolume(0),
        m_averageVolume(0),
        m_halted('\0'),

        m_nyseBid(0),
        m_nyseAsk(0),
        m_nyseTradePrice(0),
        m_nyseTradeSize(0),
        m_nyseTradeTime(0),
        m_lastTradeTime(0),
        m_nasdaqImbalanceCurrentReferencePrice(0),
        m_nasdaqImbalanceNearIndicativeClearingPrice(0),
        m_nasdaqImbalanceFarIndicativeClearingPrice(0),
        m_nyseImbalanceMatchedShares(0),
        m_regShoPlaceHolder(true),
        m_nyseDayHigh(0),
        m_nyseDayLow(0),
        m_nyseVolume(0),
        m_nysePrevBuyVolume(0),
        m_nysePrevSellVolume(0),
        m_nyseBidSize(0),
        m_nyseAskSize(0),
        m_tradeMoney(0),
        m_msInitialVolume(0),
		m_exchangeOpenPrice(0),
		m_foreignNotTradable(false),
		m_testStock(false),
		m_hybridPlaceHolder(true),
		m_yesterdaysLastExchangePrice(0),
		m_dividend(0),
		m_split('\0'),
		m_bidLRP(0),
		m_askLRP(0),
		m_nysBidCondition('\0'),
		m_nysAskCondition('\0'),
		m_nyseImbalanceTime(0),
		m_nysePreviousImbalanceTime(0),
        m_amexVolume(0),
        m_nasdaqVolume(0),
        m_closeBid(0),
        m_closeAsk(0),

		m_nasdaqBuyVolume(0),
		m_nasdaqSellVolume(0),
		m_nasdaqPrevBuyVolume(0),
		m_nasdaqPrevSellVolume(0),
		m_nasdaqImbalanceMatchedShares(0),
		m_nasdaqImbalanceTime(0),
		m_nasdaqPreviousImbalanceTime(0),
		m_nasdaqImbalanceType(-1)
    {
        *m_symbol = '\0';
        *m_description = '\0';
    }

    char m_symbol[LENGTH_SYMBOL];

    unsigned int m_chartSize;

    unsigned int m_bid;
    unsigned int m_ask;
//    unsigned int m_minBid;
//    unsigned int m_maxBid;
    unsigned int m_openPrice;
    unsigned int m_closePrice;
    unsigned int m_intradayHigh;
    unsigned int m_intradayLow;
    unsigned int m_dayHigh;
    unsigned int m_dayLow;
    unsigned int m_firstTradePrice;
    unsigned int m_firstPrice;
    unsigned int m_lastTradePrice;
    unsigned int m_lastTradeSize;
    unsigned __int64 m_firstVolume;
    unsigned __int64 m_volume;
	char m_primaryExchange;
	char m_stockAttributes;
    char m_bidTick;
    char m_description[LENGTH_SEQURITYNAME];
    unsigned short m_bidSize;
    unsigned short m_askSize;
    char m_nyseImbalanceType;
    unsigned int m_nyseBuyVolume;
    unsigned int m_nyseSellVolume;

    unsigned int m_structureLength;
    int m_preMarketIndicatorBid;
    int m_preMarketIndicatorAsk;
    unsigned int m_preMarketIndicatorTime;
    unsigned int m_preMarketIndicatorOrdinal;
    int m_todaysClosePrice;
    unsigned int m_yesterdaysVolume;
    unsigned int m_averageVolume;
    char m_halted;

    int m_nyseBid;
    int m_nyseAsk;
    int m_nyseTradePrice;
    unsigned int m_nyseTradeSize;
    unsigned int m_nyseTradeTime;

    unsigned int m_lastTradeTime;
    int m_nasdaqImbalanceCurrentReferencePrice;
    int m_nasdaqImbalanceNearIndicativeClearingPrice;
    int m_nasdaqImbalanceFarIndicativeClearingPrice;
    unsigned int m_nyseImbalanceMatchedShares;

    bool m_regShoPlaceHolder;
    unsigned int m_reservedField;
//    char m_reserved[(LENGTH_SEQURITYNAME + 3) % 4];
    unsigned int m_nyseDayHigh;
    unsigned int m_nyseDayLow;
    unsigned __int64 m_nyseVolume;
    unsigned int m_nysePrevBuyVolume;
    unsigned int m_nysePrevSellVolume;

    unsigned int m_nyseBidSize;
    unsigned int m_nyseAskSize;
    unsigned __int64 m_tradeMoney;
    unsigned __int64 m_msInitialVolume;
//    unsigned __int64 m_msNyseInitialVolume;

	int m_exchangeOpenPrice;

	bool m_foreignNotTradable;
    bool m_testStock;
    bool m_hybridPlaceHolder;
	unsigned int m_yesterdaysLastExchangePrice;
	unsigned int m_dividend;
	unsigned char m_split;

	unsigned int m_bidLRP;
	unsigned int m_askLRP;

	char m_nysBidCondition;
	char m_nysAskCondition;

	unsigned int m_nyseImbalanceTime;
	unsigned int m_nysePreviousImbalanceTime;

    unsigned __int64 m_amexVolume;
    unsigned __int64 m_nasdaqVolume;

	unsigned int m_closeBid;
	unsigned int m_closeAsk;

    unsigned int m_nasdaqBuyVolume;
    unsigned int m_nasdaqSellVolume;
    unsigned int m_nasdaqPrevBuyVolume;
    unsigned int m_nasdaqPrevSellVolume;
    unsigned int m_nasdaqImbalanceMatchedShares;
	unsigned int m_nasdaqImbalanceTime;
	unsigned int m_nasdaqPreviousImbalanceTime;
    char m_nasdaqImbalanceType;
};

class MsgMsRefreshSymbolSortablePack : public Message
{
public:
    MsgMsRefreshSymbolSortablePack(unsigned char packSize):Message(MS_RESP_REFRESH_SYMBOL_SORTABLE_PACK, sizeof(MsgMsRefreshSymbolSortablePack)), m_packSize(packSize){}
    unsigned char m_packSize;
};

class MsgMsSymbolSortablePopulationDone : public SymbolMessage
{
public:
	MsgMsSymbolSortablePopulationDone() : SymbolMessage(MS_RESP_SYMBOL_SORTABLE_POPULATION_DONE, sizeof(MsgMsSymbolSortablePopulationDone)){}
};


class ReqMsVersion : public Message
{
public:
	ReqMsVersion(unsigned int version, const char* ip, const char* id, const char* strVersion, bool canReceiveCompressed):
		Message(MS_REQ_VERSION, sizeof(ReqMsVersion)), m_version(version), m_canReceiveCompressed(canReceiveCompressed)
	{
		FillStrBufferAndPad(m_ip, sizeof(m_ip), ip);
		FillStrBufferAndPad(m_id, sizeof(m_id), id);
		FillStrBufferAndPad(m_strVersion, sizeof(m_strVersion), strVersion);
	}
    unsigned int m_version;
    char m_ip[20];
    char m_id[20];
    char m_strVersion[20];
	bool m_canReceiveCompressed;
};

class MsgMsVersion : public Message
{
public:
	MsgMsVersion(unsigned int version,
		const char* msVersion,
		unsigned int parentVersion,
		unsigned int parentIp,
		unsigned short parentPort,
		unsigned short parentApp,
		unsigned char marketStatus):
		Message(MS_RESP_VERSION, sizeof(MsgMsVersion)),
		m_version(version),
		m_parentVersion(parentVersion),
		m_parentIp(parentIp),
		m_parentPort(parentPort),
		m_parentApp(parentApp),
		m_marketStatus(marketStatus)
	{
		FillStrBufferAndPad(m_msVersion, sizeof(m_msVersion), msVersion);
	}
    unsigned int m_version;
    char m_msVersion[16];
	unsigned int m_parentVersion;
	unsigned int m_parentIp;
	unsigned short m_parentPort;
	unsigned short m_parentApp;//0 - HammerServer; 1 - MarketSummary
	unsigned char m_marketStatus;
};

class ReqMsPopulateFirstIndexSortable : public SymbolMessage
{
public:
	ReqMsPopulateFirstIndexSortable() : SymbolMessage(MS_REQ_POPULATE_FIRST_INDEX_SORTABLE, sizeof(ReqMsPopulateFirstIndexSortable)){}
};

class ReqMsPopulateNextIndexSortable : public SymbolMessage
{
public:
	ReqMsPopulateNextIndexSortable() : SymbolMessage(MS_REQ_POPULATE_NEXT_INDEX_SORTABLE, sizeof(ReqMsPopulateNextIndexSortable)){}
};

const int indexInvalidValue = -2147483647;

class MsgMsRefreshIndexSortable : public SymbolMessage
{
public:
	MsgMsRefreshIndexSortable():
        SymbolMessage(MS_RESP_REFRESH_INDEX_SORTABLE, sizeof(MsgMsRefreshIndexSortable)),
        m_closeValue(indexInvalidValue)
    {
        *m_description = '\0';
        Invalidate();
    }
    void Invalidate()
    {
        m_value = indexInvalidValue;
        m_openValue = indexInvalidValue;
        m_intradayHigh = indexInvalidValue;
        m_intradayLow = indexInvalidValue;
        m_dayHigh = indexInvalidValue;
        m_dayLow = indexInvalidValue;
        m_tick = ' ';
    }
    int m_value;
    int m_openValue;
    int m_closeValue;
    int m_intradayHigh;
    int m_intradayLow;
    int m_dayHigh;
    int m_dayLow;
    char m_tick;
    char m_description[LENGTH_SEQURITYNAME];
};

class MsgMsIndexSortablePopulationDone : public SymbolMessage
{
public:
	MsgMsIndexSortablePopulationDone() : SymbolMessage(MS_RESP_INDEX_SORTABLE_POPULATION_DONE, sizeof(MsgMsIndexSortablePopulationDone)){}
};

class MsgMsSymbolSortableLevel1 : public Message
{
public:
    MsgMsSymbolSortableLevel1() : Message(MS_RESP_SYMBOL_SORTABLE_LEVEL1, sizeof(MsgMsSymbolSortableLevel1)){}
    enum
    {
        BID = 1 << 0,
        ASK = 1 << 1,
        BIDSIZE = 1 << 2,
        ASKSIZE = 1 << 3,
        NONE
    };
};

class MsgMsSymbolSortablePreMarketIndicator : public Message
{
public:
    MsgMsSymbolSortablePreMarketIndicator() : Message(MS_RESP_SYMBOL_SORTABLE_PRE_MARKET_INDICATOR, sizeof(MsgMsSymbolSortablePreMarketIndicator)){}
    enum
    {
        BID = 1 << 0,
        ASK = 1 << 1,
        NONE
    };
};

class MsgMsSymbolSortableNysBid : public Message
{
public:
    MsgMsSymbolSortableNysBid() : Message(MS_RESP_SYMBOL_SORTABLE_NYS_BID, sizeof(MsgMsSymbolSortableNysBid)){}
};

class MsgMsSymbolSortableNysAsk : public Message
{
public:
    MsgMsSymbolSortableNysAsk() : Message(MS_RESP_SYMBOL_SORTABLE_NYS_ASK, sizeof(MsgMsSymbolSortableNysAsk)){}
};

class MsgMsSymbolSortableNysBidSize : public Message
{
public:
    MsgMsSymbolSortableNysBidSize() : Message(MS_RESP_SYMBOL_SORTABLE_NYS_BIDSIZE, sizeof(MsgMsSymbolSortableNysBidSize)){}
};

class MsgMsSymbolSortableNysAskSize : public Message
{
public:
    MsgMsSymbolSortableNysAskSize() : Message(MS_RESP_SYMBOL_SORTABLE_NYS_ASKSIZE, sizeof(MsgMsSymbolSortableNysAskSize)){}
};

class MsgMsSymbolSortableNysBidQuote : public Message
{
public:
    MsgMsSymbolSortableNysBidQuote() : Message(MS_RESP_SYMBOL_SORTABLE_NYS_BIDQUOTE, sizeof(MsgMsSymbolSortableNysBidQuote)){}
};

class MsgMsSymbolSortableNysAskQuote : public Message
{
public:
    MsgMsSymbolSortableNysAskQuote() : Message(MS_RESP_SYMBOL_SORTABLE_NYS_ASKQUOTE, sizeof(MsgMsSymbolSortableNysAskQuote)){}
};

class MsgMsSymbolSortableBidLRP : public Message
{
public:
    MsgMsSymbolSortableBidLRP() : Message(MS_RESP_SYMBOL_SORTABLE_BID_LRP, sizeof(MsgMsSymbolSortableBidLRP)){}
};

class MsgMsSymbolSortableAskLRP : public Message
{
public:
    MsgMsSymbolSortableAskLRP() : Message(MS_RESP_SYMBOL_SORTABLE_ASK_LRP, sizeof(MsgMsSymbolSortableAskLRP)){}
};

class MsgMsSymbolSortableBidAskLRP : public Message
{
public:
    MsgMsSymbolSortableBidAskLRP() : Message(MS_RESP_SYMBOL_SORTABLE_BIDASK_LRP, sizeof(MsgMsSymbolSortableBidAskLRP)){}
};

class MsgMsSymbolSortableNysBidCondition : public Message
{
public:
    MsgMsSymbolSortableNysBidCondition() : Message(MS_RESP_SYMBOL_SORTABLE_NYS_BIDCONDITION, sizeof(MsgMsSymbolSortableNysBidCondition)){}
};

class MsgMsSymbolSortableNysAskCondition : public Message
{
public:
    MsgMsSymbolSortableNysAskCondition() : Message(MS_RESP_SYMBOL_SORTABLE_NYS_ASKCONDITION, sizeof(MsgMsSymbolSortableNysAskCondition)){}
};

class MsgMsAdminMessage : public Message
{
public:
	MsgMsAdminMessage(unsigned int time, unsigned short textLen) : Message(MS_RESP_ADMIN_MESSAGE, sizeof(MsgMsAdminMessage) + textLen), m_time(time){}
    unsigned int m_time;
};

class MsgMsAdminMessageDone : public Message
{
public:
	MsgMsAdminMessageDone() : Message(MS_RESP_ADMIN_MESSAGE_DONE, sizeof(MsgMsAdminMessageDone)){}
};


class MsgMsSymbolSortableTrade : public Message
{
public:
	MsgMsSymbolSortableTrade() : Message(MS_RESP_SYMBOL_SORTABLE_TRADE, sizeof(MsgMsSymbolSortableTrade)){}
};

class MsgMsSymbolSortableEcnTrade : public Message
{
public:
	MsgMsSymbolSortableEcnTrade() : Message(MS_RESP_SYMBOL_SORTABLE_ECNTRADE, sizeof(MsgMsSymbolSortableEcnTrade)){}
};
/*
class MsgMsSymbolSortableFyi : public Message
{
public:
	MsgMsSymbolSortableFyi() : Message(MS_RESP_SYMBOL_SORTABLE_FYI, sizeof(MsgMsSymbolSortableFyi)){}
};
*/
class MsgMsSymbolSortableClosePrice : public Message
{
public:
	MsgMsSymbolSortableClosePrice() : Message(MS_RESP_SYMBOL_SORTABLE_CLOSEPRICE, sizeof(MsgMsSymbolSortableClosePrice)){}
};

class MsgMsNextMinute : public Message
{
public:
	MsgMsNextMinute(unsigned short minute = 0):Message(MS_RESP_NEXT_MINUTE, sizeof(MsgMsNextMinute)), m_minute(minute){}
    unsigned short m_minute;
};

class MsgMsInit : public Message
{
public:
	MsgMsInit(unsigned short minute = 0):Message(MS_RESP_INIT, sizeof(MsgMsInit)), m_minute(minute){}
    unsigned short m_minute;
};

class MsgMarketDisconnected : public Message
{
public:
	MsgMarketDisconnected():Message(MS_RESP_MARKET_DISCONNECTED, sizeof(MsgMarketDisconnected)){}
};

class MsgMsSymbolSortableL2LargeQuote : public Message
{
public:
	MsgMsSymbolSortableL2LargeQuote() : Message(MS_RESP_SYMBOL_SORTABLE_L2_LARGE_QUOTE, sizeof(MsgMsSymbolSortableL2LargeQuote)){}
};

class MsgReconnect : public Message
{
public:
	MsgReconnect():Message(M_REQ_RECONNECT, sizeof(MsgReconnect)){}
};

class MsgReceiverTimer : public Message
{
public:
	MsgReceiverTimer():Message(MSGID_RECEIVER_TIMER, sizeof(MsgReceiverTimer)){}
};

class MsgLogout : public Message
{
public:
	MsgLogout():Message(MSGID_LOGOUT, sizeof(MsgLogout)){}
};

class MsgConnectionFailed : public Message
{
public:
	MsgConnectionFailed(int errorCode):Message(MSGID_CONNECTION_FAILED, sizeof(MsgConnectionFailed)), m_errorCode(errorCode){}
    int m_errorCode;
};

class MsgConnectionStarted : public Message
{
public:
	MsgConnectionStarted():Message(MSGID_CONNECTION_STARTED, sizeof(MsgConnectionStarted)){}
};

class MsgDataReceived : public Message
{
public:
	MsgDataReceived(unsigned int dataSize):Message(MSGID_DATA_RECEIVED, sizeof(MsgDataReceived)),m_dataSize(dataSize){}
	unsigned int m_dataSize;
};

class MsgDataProcessed : public Message
{
public:
	MsgDataProcessed():Message(MSGID_DATA_PROCESSED, sizeof(MsgDataProcessed)){}
};

class MsgOrderCancelAI : public SymbolMessage
{
public:
	MsgOrderCancelAI(const char* userId,
        const char* symbol,
        unsigned int quantity,
        unsigned int origOrderId,
		char side):

		SymbolMessage(M_ORDER_CANCEL_AI, sizeof(MsgOrderCancelAI), symbol),
		m_quantity(quantity),
		m_origOrderId(origOrderId),
		m_side(side)
	{
		FillStrBufferAndPad(m_userId, sizeof(m_userId), userId);
	}
	char    m_userId[LENGTH_SYMBOL];
    unsigned int m_quantity;
    unsigned int m_origOrderId;
    char m_side;
};

class MsgMsUpgradeVersion : public Message
{
public:
    unsigned int m_version;
protected:
	MsgMsUpgradeVersion(unsigned int version, unsigned short type, unsigned short size) : Message(type, size), m_version(version){}
};

class MsgUpgradeToVersion : public MsgMsUpgradeVersion
{
public:
	MsgUpgradeToVersion(unsigned int version, char action):MsgMsUpgradeVersion(version, MS_UPGRADE_TO_VERSION, sizeof(MsgUpgradeToVersion)), m_action(action){}
	char m_action;//0 - exact version; 1 - upgrade; 2 - downgrade
};

class MsgLegacyVersion : public MsgMsUpgradeVersion
{
public:
	MsgLegacyVersion(unsigned int version, unsigned int legacyVersion):MsgMsUpgradeVersion(version, MS_LEGACY_VERSION, sizeof(MsgLegacyVersion)), m_legacyVersion(legacyVersion){}
	unsigned int m_legacyVersion;
};

class ReqMsHistoryPrints : public SymbolMessage
{
public:
	ReqMsHistoryPrints(const char* symbol, unsigned short daysAgo, unsigned int printOrdinalFrom):
		SymbolMessage(MS_REQ_HISTORY_PRINTS, sizeof(ReqMsHistoryPrints), symbol),
		m_daysAgo(daysAgo),
		m_printOrdinalFrom(printOrdinalFrom)
	{}
	unsigned short m_daysAgo;
	unsigned int m_printOrdinalFrom;
};

class PrintStructure
{
public:
	PrintStructure(unsigned int price,
		unsigned int size,
//		unsigned short secondsFrom4AM,
//		char primaryExchange,
		char executionExchange,
		char saleCondition,
		char status,
		char hidden):
		m_price(price),
		m_size(size),
//		m_secondsFrom4AM(secondsFrom4AM),
//		m_PrimaryExchange(primaryExchange),
		m_ExecutionExchange(executionExchange),
		m_saleCondition(saleCondition),
		m_status(status),
		m_hidden(hidden)
		{}
	unsigned int m_price;
	unsigned int m_size;
//	unsigned short m_secondsFrom4AM;
//	char m_PrimaryExchange;
	char m_ExecutionExchange;
	char m_saleCondition;
	char m_status;
	char m_hidden;
};

class MsgMsHistoryPrints : public SymbolMessage
{
public:
	MsgMsHistoryPrints(const char* symbol, unsigned  char structureSize):
		SymbolMessage(MS_RESP_HISTORY_PRINTS, sizeof(MsgMsHistoryPrints), symbol),
		m_structureSize(structureSize)
	{}
	unsigned char m_structureSize;
	void SetMessageSize(unsigned short size){m_size = size;}
};

class MsgMsHistoryPrintsLoadingDone : public SymbolMessage
{
public:
	MsgMsHistoryPrintsLoadingDone(const char* symbol):
		SymbolMessage(MS_RESP_HISTORY_PRINTS_LOADING_DONE, sizeof(MsgMsHistoryPrintsLoadingDone), symbol)
	{}
};

#endif