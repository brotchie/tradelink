// BaseDlg.cpp : implementation file
//

#include "stdafx.h"
#include "BaseDlg.h"
#include "ExtFrame.h"
#include "BusinessApi.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// BaseDlg dialog


BaseDlg::BaseDlg(unsigned int id, CWnd* pParent):
    CDialog(id, pParent),
    m_dialogId(id),
    m_parentWindow(pParent)
{
	//{{AFX_DATA_INIT(BaseDlg)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
}

BOOL BaseDlg::CreateTradeDialog()
{
    return Create(m_dialogId, m_parentWindow);
}

void BaseDlg::OnCancel()
{
//    Clear();
    DoCancel();
    ExtFrame::GetInstance()->DestroyDlg(this);
}


void BaseDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(BaseDlg)
		// NOTE: the ClassWizard will add DDX and DDV calls here
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(BaseDlg, CDialog)
	//{{AFX_MSG_MAP(BaseDlg)
		// NOTE: the ClassWizard will add message map macros here
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// BaseDlg message handlers
void BaseDlg::Fill(BaseDlg::QuoteControls& quoteControls, void* iterator)
{
    const BookEntry* be;
    unsigned int entries = (unsigned int)quoteControls.size();
    unsigned int count = 0;
    char num[33];
    CString str;        
    B_StartIteration(iterator);
    while((be = B_GetNextBookEntry(iterator)) && count < entries)
    {
        str = be->GetMmid();
        str += "  ";
//        ExtFrame::FormatDollarsAndCents(str, be->GetPriceWhole(), be->GetPriceThousandsFraction());
        ExtFrame::FormatMoney(str, *be);
        str += "       ";
		_itoa_s(be->GetSize(), num, sizeof(num), 10);
        str += num;
        quoteControls[count]->SetWindowText(str);
        count++;
    }
    for(; count < entries; count++)
    {
        quoteControls[count]->SetWindowText("");
    }
}

