#ifndef OBSERVER_API_H
#define OBSERVER_API_H


#ifdef OBSERVER_EXPORTS
#define OBSERVER_API __declspec(dllexport)
#else
#define OBSERVER_API __declspec(dllimport)
#endif

const char* const ObserverHeaderVersion = "2.7.5.4";

class Observable;
class Observer;

class OBSERVER_API Message
{
public:
//	virtual ~Message(){}
    typedef unsigned short MsgLength;
    MsgLength GetSize() const{return m_size;}
    unsigned short GetType() const{return m_type;}
protected:
	Message(unsigned short type = 0, MsgLength size = 0) : m_type(type), m_size(size){}
    MsgLength m_size;
    unsigned short m_type;
};

class OBSERVER_API Observable
{
public:
	friend class Observer;
    friend class ObserverManager;
    friend class ObserverNotification;
	Observable();
    Observable(const Observable& o);
	virtual ~Observable();
    const Observable& operator=(const Observable& o){return *this;}
	typedef std::set<Observer*> Observers;
	virtual void Add(Observer*);
	virtual void Remove(Observer*);
//	void RemoveDelayed(Observer*);
//	void DestroyDelayed(Observer*);
	void Notify(const Message* message, const Message* additionalInfo = NULL, Observable* from = NULL);
	void Clear();
//    static void NotifyObservers(Observers& m_observers, Observable* from, const Message* message, const Message* additionalInfo = NULL);

    void* CreateObserverIterator();
    void StartIteration(void* observerIterator);
    Observer* GetNextObserver(void* observerIterator);
    void DestroyObserverIterator(void* observerIterator);

    void AfterObserverAdded();
    void BeforeObserverRemoved(Observer*);
    void AfterObserverRemoved();
    void AfterObserversCleared();

	unsigned int GetObserverCount() const{return (unsigned int)m_observerCount;}//m_observers.size();}
	bool HasObservers() const{return m_observerCount != 0;}//!m_observers.empty();}
    bool isObserverAttached(Observer* o) const{return m_observers.find(o) != m_observers.end();}
	bool hasIdleJob() const;
    bool HasOnlyThisObserverOrNone(Observer* o) const{return m_observers.empty() || m_observers.size() == 1 && isObserverAttached(o);}
protected:
	volatile LONG m_observerCount;
private:
//	virtual void EraseObserver(Observer* o);
	Observers m_observers;
    std::list<ObserverNotification*> m_notifications;
//	Observers toRemove;
//	Observers toDestroy;
};

class OBSERVER_API Observer
{
public:
	friend class Observable;
    friend class ObserverManager;
	typedef std::set<Observable*> Observables;
	virtual ~Observer();
    const Observer& operator=(const Observer& o){return *this;}
    virtual std::string GetObserverName() const{return std::string();}
	void Clear();
	virtual void Process(const Message* message, Observable* from, const Message* additionalInfo) = 0;
	virtual bool hasIdleJob() const{return false;}
//    void RemoveSafely();
//    void DestroySafely();

    void* CreateObservableIterator();
    void StartIteration(void* observableIterator);
    Observable* GetNextObservable(void* observableIterator);
    void DestroyObservableIterator(void* observableIterator);
    size_t GetObservableCount() const{return m_observables.size();}
    bool HasObservables() const{return !m_observables.empty();}
    bool isObservableAttached(Observable* o) const{return m_observables.find(o) != m_observables.end();}
protected:
	Observer();
    Observer(const Observer& o);
private:
	Observables m_observables;
//    unsigned char m_status;
};

class OBSERVER_API ObservableObserver : public Observable, public Observer
{
protected:
    ObservableObserver(){}
};

#ifdef __cplusplus
extern "C" {
#endif

void WINAPI O_Terminate();
const char* WINAPI O_GetDllVersion(bool externally);
const char* WINAPI O_GetDllPath();
const char* WINAPI O_GetDllBuild();
unsigned int WINAPI O_GetPlatform();
bool WINAPI O_IsDebug();
const char* WINAPI O_GetHeaderVersion();

#ifdef __cplusplus
} //extern "C"
#endif

#endif