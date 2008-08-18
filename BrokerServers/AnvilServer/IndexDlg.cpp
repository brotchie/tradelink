// IndexDlg.cpp : implementation file
//

#include "stdafx.h"
#include "Resource.h"
#include "IndexDlg.h"
#include "ExtFrame.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// IndexDlg dialog


IndexDlg::IndexDlg(CWnd* pParent):
	BaseDlg(IndexDlg::IDD, pParent),
    m_index(NULL)
{
	//{{AFX_DATA_INIT(IndexDlg)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
}


void IndexDlg::DoDataExchange(CDataExchange* pDX)
{
	BaseDlg::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(IndexDlg)
	DDX_Control(pDX, IDB_DYNAMIC_UPDATE, m_CheckBoxDynamicUpdate);
	DDX_Control(pDX, IDS_INDEX_SYMBOL, m_StaticSymbol);
	DDX_Control(pDX, IDS_VALUE, m_StaticValue);
	DDX_Control(pDX, IDS_TICK, m_StaticTick);
	DDX_Control(pDX, IDS_OPEN, m_StaticOpen);
	DDX_Control(pDX, IDS_NETCHANGE, m_StaticNetChange);
	DDX_Control(pDX, IDS_MARKETOPEN, m_StaticMarketOpen);
	DDX_Control(pDX, IDS_LOW, m_StaticLow);
	DDX_Control(pDX, IDS_HIGH, m_StaticHigh);
	DDX_Control(pDX, IDS_CLOSE, m_StaticClose);
	DDX_Control(pDX, IDOK, m_ButtonOk);
	DDX_Control(pDX, IDE_INDEX_SYMBOL, m_EditSymbol);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(IndexDlg, BaseDlg)
	//{{AFX_MSG_MAP(IndexDlg)
	ON_EN_CHANGE(IDE_INDEX_SYMBOL, OnChangeIndexSymbol)
	ON_BN_CLICKED(IDB_DYNAMIC_UPDATE, OnDynamicUpdate)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// IndexDlg message handlers

void IndexDlg::OnChangeIndexSymbol() 
{
    m_ButtonOk.EnableWindow(m_EditSymbol.GetWindowTextLength() == 0 ? FALSE : TRUE);
}
/*
void IndexDlg::OnCancel()
{
    ExtFrame::GetInstance()->DestroyIndexDlg(this);
}
*/
void IndexDlg::FillInfo()
{
    m_StaticSymbol.SetWindowText(m_index->GetSymbol());
    CString str;
    ExtFrame::FormatMoney(str, m_index->GetValue());
	m_StaticValue.SetWindowText(str);
    str = "";
    ExtFrame::FormatMoney(str, m_index->GetNetChange());
	m_StaticNetChange.SetWindowText(str);
    str = "";
    ExtFrame::FormatMoney(str, m_index->GetOpenValue());
	m_StaticOpen.SetWindowText(str);
    str = "";
    ExtFrame::FormatMoney(str, m_index->GetMarketOpenValue());
	m_StaticMarketOpen.SetWindowText(str);
    str = "";
    ExtFrame::FormatMoney(str, m_index->GetLow());
	m_StaticLow.SetWindowText(str);
    str = "";
    ExtFrame::FormatMoney(str, m_index->GetHigh());
	m_StaticHigh.SetWindowText(str);
    str = "";
    ExtFrame::FormatMoney(str, m_index->GetCloseValue());
	m_StaticClose.SetWindowText(str);
    int tick = m_index->GetTickDirection();
    m_StaticTick.SetWindowText(tick > 0 ? "U" : tick < 0 ? "D" : "");
}

void IndexDlg::OnOK()
{
    CString symbol;
    m_EditSymbol.GetWindowText(symbol);
    if(symbol.GetLength() != 0)
    {
        MarketIndex* index = B_FindIndex(symbol);
        if(m_index != index)
        {
            if(m_index)
            {
                m_index->Remove(this);
            }
            m_index = index;
            if(m_index)
            {
                if(m_CheckBoxDynamicUpdate.GetCheck() == BST_CHECKED)
                {
                    m_index->Add(this);
                }
                FillInfo();
            }
            else
            {
                m_StaticSymbol.SetWindowText("");
	            m_StaticValue.SetWindowText("");
	            m_StaticNetChange.SetWindowText("");
	            m_StaticOpen.SetWindowText("");
	            m_StaticMarketOpen.SetWindowText("");
	            m_StaticLow.SetWindowText("");
	            m_StaticHigh.SetWindowText("");
	            m_StaticClose.SetWindowText("");
                m_StaticTick.SetWindowText("");
            }
        }
    }
}

void IndexDlg::Process(const Message* message, Observable* from, const Message* additionalInfo)
{
    switch(message->GetType())
    {
        case M_NW2_INDEX_DETAILS:
        FillInfo();
        break;
    }
}

void IndexDlg::OnDynamicUpdate() 
{
    if(m_index)
    {
        if(m_CheckBoxDynamicUpdate.GetCheck() == BST_CHECKED)
        {
            FillInfo();
            m_index->Add(this);
        }
        else
        {
            m_index->Remove(this);
        }
    }
}
