#ifndef OBSERVERH
#define OBSERVERH

#include "ObserverApi.h"

class ObserverManager
{
public:
	ObserverManager(HMODULE hInstance);
	~ObserverManager();
/*
	void AddObserver(Observer* o);
	void RemoveObserver(Observer* o);
	void AddObservable(Observable* o);
	void RemoveObservable(Observable* o);
	void Clear();
*/
	const char* GetDllPath() const{return m_dllPath.c_str();}
protected:
/*
	Observer::Observables m_observables;
	Observable::Observers m_observers;
*/
	std::string m_dllPath;
};

#endif