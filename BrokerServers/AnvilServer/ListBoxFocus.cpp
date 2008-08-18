#include "stdafx.h"
#include "ListBoxFocus.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif


ListBoxFocus::ListBoxFocus()
{
}

BEGIN_MESSAGE_MAP(ListBoxFocus, CListBox)
	ON_WM_ERASEBKGND()
	ON_WM_PAINT()
END_MESSAGE_MAP()


BOOL ListBoxFocus::OnEraseBkgnd(CDC* pDC) 
{
	return TRUE;
}

void ListBoxFocus::OnPaint() 
{
    CRect updateRect;
    bool toPaint = GetUpdateRect(&updateRect) == TRUE;
    if(toPaint)
    {
        int count = GetCount();
        int top = (count - GetTopIndex()) * GetItemHeight(0);
        if(top > updateRect.top)
        {
            updateRect.top = top;
        }
        if(updateRect.top < updateRect.bottom)
        {
            CClientDC dc(this);
            CRect noItemRect;
            GetClientRect(&noItemRect);
            noItemRect.top = top;
            DrawBackground(dc, updateRect, noItemRect);
        }
    }
    CListBox::OnPaint();
}

void ListBoxFocus::DrawBackground(CDC& dc, const CRect& updateRect, const CRect& noItemRect)
{
    dc.FillSolidRect(&updateRect, GetBackgroundColor());
}

void ListBoxFocus::DrawItem(LPDRAWITEMSTRUCT lpDrawItemStruct)
{
    if((lpDrawItemStruct->itemAction & ODA_FOCUS) != 0)
    {
        DrawFocus(lpDrawItemStruct);
	}
    else
    {
        if(lpDrawItemStruct->itemData != -1)// && lpDrawItemStruct->itemData != 0)
        {
            COLORREF prevBkColor = GetBkColor(lpDrawItemStruct->hDC);
            COLORREF prevTextColor = GetTextColor(lpDrawItemStruct->hDC);
            DoDrawItem(lpDrawItemStruct);

            SetBkColor(lpDrawItemStruct->hDC, prevBkColor);
            SetTextColor(lpDrawItemStruct->hDC, prevTextColor);
        }

        if(lpDrawItemStruct->itemState & ODS_FOCUS)
        {
            DrawFocus(lpDrawItemStruct);
        }

    }
}

void ListBoxFocus::DrawFocus(LPDRAWITEMSTRUCT lpDrawItemStruct)
{
    COLORREF prevTextColor = SetTextColor(lpDrawItemStruct->hDC, GetFocusForeColor(lpDrawItemStruct));
    COLORREF prevBkColor = SetBkColor(lpDrawItemStruct->hDC, GetFocusBackColor(lpDrawItemStruct));
    CRect rect(lpDrawItemStruct->rcItem);
    rect.DeflateRect(GetFocusLeftInset(), GetFocusTopInset(), GetFocusRightInset(), GetFocusBottomInset());
    DrawFocusRect(lpDrawItemStruct->hDC, &rect);
    SetBkColor(lpDrawItemStruct->hDC, prevBkColor);
    SetTextColor(lpDrawItemStruct->hDC, prevTextColor);
}

void ListBoxFocus::InvalidateItem(int index, int left, int right)
{
    int top = GetTopIndex();
    int count = GetCount();
    if(index >= top && index < count)
    {
        CRect r;
        GetClientRect(&r);
        int itemHeight = GetItemHeight(0);

        r.top = (index - top) * itemHeight;
        r.bottom = r.top + itemHeight;
        if(left > 0)
        {
            r.left = left;
        }
        if(right > left)
        {
            r.right = right;
        }
        InvalidateRect(&r, TRUE);
    }
}

void ListBoxFocus::MoveItem(unsigned int from, unsigned int to)
{
    unsigned int count = GetCount();
    if(from < count && to < count && from != to)
    {
        if((GetStyle() & LBS_HASSTRINGS))
        {
            int len = GetTextLen(from);
            char* text = new char[len + 1];
            GetText(from, text);
            DeleteString(from);
            to = InsertSimpleString(to, text);
            if(to >= 0)
            {
                SetCurSel(to);
            }
            delete[] text;
        }
        else
        {
			unsigned int i;
       		void* fromPtr;
	    	void* toPtr;
		    fromPtr = GetItemDataPtr(from);
    		if(from < to)
            {
                for(i = from + 1; i <= to; i++)
                {
                    toPtr = GetItemDataPtr(i);
                    SetItemDataPtr(i - 1, toPtr);
                }
            }
            else
            {
                for(i = from; i > to; i--)
                {
                    toPtr = GetItemDataPtr(i - 1);
                    SetItemDataPtr(i, toPtr);
                }
            }
            SetItemDataPtr(to, fromPtr);

            SetCurSel(to);
	
            int top = GetTopIndex();
            int h = GetItemHeight(0);
            CRect r;
            GetClientRect(&r);
            if(from < to)
            {
                r.top = from * h;
                r.bottom = to * h;
            }
            else
            {
                r.top = to * h;
                r.bottom = from * h;
            }
            r.bottom += h;
            if(top > 0)
            {
                r.OffsetRect(0, -top * h);
            }
		    InvalidateRect(&r);
        }
    }
}

int ListBoxFocus::MoveSelectedItem(unsigned int nPos, int* To)
{
    int ret = -1;
    unsigned int count = GetCount();
    unsigned int from = GetCurSel();
    if(from >= 0 && nPos < count && from < count)
    {
        unsigned int to = count - 1 - nPos;
        if(from == to)
        {
            if(to == 0)
            {
                to = count - 1;
            }
            else if(to == count - 1)
            {
                to = 0;
            }
            ret = -1;
        }
        if(nPos != count - 1 - to)
        {
            ret = count - 1 - to;
        }
        MoveItem(from, to);
        if(To)
        {
            *To = to;
        }
    }
    else
    {
        if(To)
        {
            *To = -1;
        }
    }
    return ret;
}

int ListBoxFocus::CompareItem(LPCOMPAREITEMSTRUCT lpCompareItemStruct)
{
    return SearchCompare((const void*)lpCompareItemStruct->itemData1, (const void*)lpCompareItemStruct->itemData2);
}

int ListBoxFocus::FindItem(const void* item, int approx) const
{
    int count = GetCount();
    if(count == 0)
    {
        return -1;
    }
    if((GetStyle() & LBS_SORT) == 0)//unsorted
    {
        for(int i = 0; i < count; i++)
        {
            if(SearchCompare(item, GetItemDataPtr(i)) == 0)
            {
                return i;
            }
        }
        return -1;
    }
    else
    {
        int from = 0;
        int to = count - 1;
        int middle;
        const void* current;
        int result;
        while(to >= from)
        {
            middle = (from + to) / 2;
            current = GetItemDataPtr(middle);
            result = SearchCompare(item, current);
            if(result == 0)
            {
                for(middle--; middle >= 0; middle--)
                {
                    current = GetItemDataPtr(middle);
                    if(SearchCompare(item, current) != 0)
                    {
                        break;
                    }
                }
                return middle + 1;
            }
            else if(result < 0)
            {
                to = middle - 1;
            }
            else
            {
                from = middle + 1;
            }
        }
        return approx == 0 ? -1 : approx < 0 && to >= 0 ? to : to + 1;
    }
}

int ListBoxFocus::GetItemAtPoint(const CPoint& point) const
{
    int height = GetItemHeight(0);
    int item = point.y / height + GetTopIndex();
    int count = GetCount();
    if(item >= count)
    {
        item = -1;
    }
    return item;
}
