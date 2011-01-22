#pragma once


// MainDlg dialog

class MainDlg : public CDialog, public L_Observer
{
	DECLARE_DYNAMIC(MainDlg)

public:
	MainDlg(CWnd* pParent = NULL);   // standard constructor
	virtual ~MainDlg();

// Dialog Data
	enum { IDD = IDD_MAINDLG };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

	DECLARE_MESSAGE_MAP()

protected:
	virtual void OnOK() {}
	virtual void OnCancel() {}
	virtual BOOL OnInitDialog();
	afx_msg void OnNcDestroy();
	afx_msg void OnClose();
	afx_msg void OnBnClickedSendOrder();

// L_Observer
	virtual void HandleMessage(L_Message const *msg);

private:
	L_Account *account;
	L_Summary *summary;
};
