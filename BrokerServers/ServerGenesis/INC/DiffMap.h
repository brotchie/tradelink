#ifndef __DIFFMAP_H__
#define __DIFFMAP_H__

template< class KEY, class ARG_KEY, class VALUE, class ARG_VALUE >
CMap<KEY, ARG_KEY, VALUE, ARG_VALUE> &Diff(CMap<KEY, ARG_KEY, VALUE, ARG_VALUE> &rst
		, const CMap<KEY, ARG_KEY, VALUE, ARG_VALUE> &dst
		, const CMap<KEY, ARG_KEY, VALUE, ARG_VALUE > &src)
{
	rst.RemoveAll();

	const CMap<KEY, ARG_KEY, VALUE, ARG_VALUE>::CPair *pair = src.PGetFirstAssoc();
	while(pair)
	{
		const CMap<KEY, ARG_KEY, VALUE, ARG_VALUE>::CPair *pDst = dst.PLookup(pair->key);
		if(pDst)
			rst[pair->key] = pDst->value - pair->value;

		pair = src.PGetNextAssoc(pair);
	}

	return rst;
}

#endif//__DIFFMAP_H__
