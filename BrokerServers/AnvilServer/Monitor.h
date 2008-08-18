
#pragma once
#include "ObserverApi.h"
#include <vector>
#include "BusinessApi.h"

unsigned int AnvilId(unsigned int TLOrderId);

class Monitor : public Observer
{
public:
	Monitor();
	~Monitor();
	unsigned int cacheOrder(Order* o);
	bool hasOrder(unsigned int id);
	unsigned int AnvilId(unsigned int TLOrderId);
	virtual void Process(const Message* message, Observable* from, const Message* additionalInfo);
private:
	std::vector<Observable*> accounts;
};