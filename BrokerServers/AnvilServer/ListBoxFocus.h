#ifndef LISTBOXFOCUSH
#define LISTBOXFOCUSH

class ListBoxFocus : public CListBox
{
public:
    virtual void DrawItem(LPDRAWITEMSTRUCT lpDrawItemStruct);
    virtual COLORREF GetFocusForeColor(LPDRAWITEMSTRUCT lpDrawItemStruct) const{return RGB(255, 255, 255);}
    virtual COLORREF GetFocusBackColor(LPDRAWITEMSTRUCT lpDrawItemStruct) const{return RGB(0, 0, 0);}
    virtual int GetFocusLeftInset() const{return 0;}
    virtual int GetFocusTopInset() const{return 0;}
    virtual int GetFocusRightInset() const{return 0;}
    virtual int GetFocusBottomInset() const{return 0;}
    void InvalidateItem(int index, int left = -1, int right = -1);
    int MoveSelectedItem(unsigned int nPos, int* To = NULL);
    void MoveItem(unsigned int from, unsigned int to);
	virtual int CompareItem(LPCOMPAREITEMSTRUCT lpCompareItemStruct);
    virtual int SearchCompare(const void* item1, const void* item2) const{return 0;}
    int FindItem(const void* item, int approx = 0) const;
    virtual COLORREF GetBackgroundColor() const{return GetSysColor(COLOR_WINDOW);}
    int GetItemAtPoint(const CPoint& point) const;
    virtual int AddSimpleString(const char* str){return -1;}
    int InsertSimpleString(int index, const char* str)
    {
        if(index < 0 || index >= GetCount())
        {
            return AddSimpleString(str);
        }
        else
        {
            return DoInsertSimpleString(index, str);
        }
    }

protected:
    ListBoxFocus();
	DECLARE_MESSAGE_MAP()
	afx_msg BOOL OnEraseBkgnd(CDC* pDC);
	afx_msg void OnPaint();
    virtual int DoInsertSimpleString(int index, const char* str){return -1;}
    void DrawFocus(LPDRAWITEMSTRUCT lpDrawItemStruct);
    virtual void DoDrawItem(LPDRAWITEMSTRUCT lpDrawItemStruct) = 0;
    virtual void DrawBackground(CDC& dc, const CRect& updateRect, const CRect& noItemRect);
};


class ListBoxNoItemColor : public ListBoxFocus
{
public:
    ListBoxNoItemColor(COLORREF textColor, COLORREF bkColor):m_textColor(textColor),m_bkColor(bkColor){}
    ListBoxNoItemColor():m_textColor(GetSysColor(COLOR_WINDOWTEXT)),m_bkColor(GetSysColor(COLOR_WINDOW)){}

    virtual COLORREF GetFocusForeColor(LPDRAWITEMSTRUCT lpDrawItemStruct) const{return m_textColor;}
    virtual COLORREF GetFocusBackColor(LPDRAWITEMSTRUCT lpDrawItemStruct) const{return m_bkColor;}

    void SetTextColor(COLORREF c){m_textColor = c;}
    void SetBkColor(COLORREF c){m_bkColor = c;}
    virtual COLORREF GetBackgroundColor() const{return m_bkColor;}
protected:
    COLORREF m_textColor;
    COLORREF m_bkColor;
};


#endif