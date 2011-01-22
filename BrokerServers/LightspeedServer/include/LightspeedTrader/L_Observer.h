#pragma once
#if !defined(LS_L_OBSERVER_H)
#define LS_L_OBSERVER_H

// Copyright © 2001-2011 Lightspeed Financial, Inc. All rights reserved.


namespace LightspeedTrader
{

class L_Message;
class L_ObserverImpl;
class L_Observer;
extern "C" void L_CreateObserver(L_Observer *);
extern "C" void L_DestroyObserver(L_Observer *);

class __declspec(novtable) L_Observer
{
public:
	virtual void HandleMessage(L_Message const *message) = 0;

	L_Observer() : impl(0)
	{
		L_CreateObserver(this);
	}

	L_Observer(L_Observer const &rhs) : impl(0)
	{
		L_CreateObserver(this);
	}

	~L_Observer()
	{
		L_DestroyObserver(this);
	}
	L_ObserverImpl *impl;
};

class __declspec(novtable) L_Observable
{
public:
	virtual void L_Attach(L_Observer *observer) = 0;
	virtual void L_Detach(L_Observer *observer) = 0;
};

} // namespace LightspeedTrader


#endif // !defined(LS_L_OBSERVER_H)

