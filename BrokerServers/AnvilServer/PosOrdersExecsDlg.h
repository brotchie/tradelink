#if !defined(AFX_POSORDERSEXECSDLG_H__AEB48D33_17B1_4913_BD7B_66AA8D1707C5__INCLUDED_)
#define AFX_POSORDERSEXECSDLG_H__AEB48D33_17B1_4913_BD7B_66AA8D1707C5__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// PosOrdersExecsDlg.h : header file
//

#include "BaseDlg.h"
#include "ListBoxFocus.h"
#include "ObserverApi.h"

class Position;
class Order;
class Execution;
/*
class MyPosition : public Observer
{
public:
    MyPosition(Position* pos):m_position(pos)
protected:
    Position* m_position;
};
*/
class ListBoxPosition : public ListBoxNoItemColor
{
public:
//    void InvalidatePosition(const Position* position);
    virtual int SearchCompare(const void* item1, const void* item2) const;
protected:
    virtual void DoDrawItem(LPDRAWITEMSTRUCT lpDrawItemStruct);
};

class ListBoxOrder : public ListBoxNoItemColor
{
public:
//    void InvalidateOrder(const Order* order);
    virtual int SearchCompare(const void* item1, const void* item2) const;
protected:
    virtual void DoDrawItem(LPDRAWITEMSTRUCT lpDrawItemStruct);
};

class ListBoxExecution : public ListBoxNoItemColor
{
public:
    virtual int SearchCompare(const void* item1, const void* item2) const;
protected:
    virtual void DoDrawItem(LPDRAWITEMSTRUCT lpDrawItemStruct);
};

class WindowLocation
{
public:
    WindowLocation(CWnd* window, int width, int alignment):m_width(width), m_alignment(alignment), m_window(window){}
    const int m_width;
    const int m_alignment;
    CWnd* m_window;
};
/////////////////////////////////////////////////////////////////////////////
// PosOrdersExecsDlg dialog

class PosOrdersExecsDlg : public BaseDlg, public Observer
{
// Construction
public:
	PosOrdersExecsDlg(CWnd* pParent = NULL);   // standard constructor

    typedef std::list<WindowLocation> ControlList;
// Dialog Data
	//{{AFX_DATA(PosOrdersExecsDlg)
	enum { IDD = IDD_POS_ORDERS_EXECS };
	CStatic	m_StaticOrderSide;
	CStatic	m_StaticExecOrderPrice;
	CStatic	m_StaticExecPrice;
	CStatic	m_StaticOrderTime;
	CStatic	m_StaticExecTime;
	CStatic	m_StaticExecOrderId;
	CStatic	m_StaticOrderTif;
	CStatic	m_StaticOrderPrice;
	CStatic	m_StaticOrderExecSize;
	CStatic	m_StaticOrderExecPrice;
	CStatic	m_StaticOrderDestination;
	CStatic	m_StaticPosPendingSellOrderCount;
	CStatic	m_StaticPosPendingBuyOrderCount;
	CStatic	m_StaticPosClosedPnl;
	CStatic	m_StaticPosSymbol;
	CStatic	m_StaticPosSize;
	CStatic	m_StaticPosOpenPnl;
	CStatic	m_StaticOrderSymbol;
	CStatic	m_StaticOrderSize;
	CStatic	m_StaticOrderId;
	CStatic	m_StaticExecSymbol;
	CStatic	m_StaticExecSize;
	CStatic	m_StaticPositionsName;
	CStatic	m_StaticExecutionsName;
	CStatic	m_StaticOrdersName;
	ListBoxPosition m_ListBoxPositions;
	ListBoxOrder m_ListBoxOrders;
	ListBoxExecution m_ListBoxExecutions;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(PosOrdersExecsDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(PosOrdersExecsDlg)
	virtual BOOL OnInitDialog();
	afx_msg void OnSize(UINT nType, int cx, int cy);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
	afx_msg LRESULT OnExitSizeMove(WPARAM, LPARAM);

    virtual void Process(const Message* message, Observable* from, const Message* additionalInfo);
    void Resize(bool vertChanged);
    static void ArrangeTitle(ControlList& controlList, int y);
    static void RepaintTitle(ControlList& controlList);

    int m_staticHeight;
    int m_allStatisHeight;
    int m_lbTitleHeight;
    int m_vertGap;
    CSize m_dimensions;

    Observable* m_account;

    ControlList m_positionTitle;
    ControlList m_orderTitle;
    ControlList m_executionTitle;
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_POSORDERSEXECSDLG_H__AEB48D33_17B1_4913_BD7B_66AA8D1707C5__INCLUDED_)
