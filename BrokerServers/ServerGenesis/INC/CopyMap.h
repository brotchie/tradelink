#ifndef __COPYMAP_H__
#define __COPYMAP_H__

template< class KEY, class ARG_KEY, class VALUE, class ARG_VALUE >
CMap<KEY, ARG_KEY, VALUE, ARG_VALUE> &Append(CMap<KEY, ARG_KEY, VALUE, ARG_VALUE> &dst, const CMap<KEY, ARG_KEY, VALUE, ARG_VALUE > &src)
{
	for(const CMap<KEY, ARG_KEY, VALUE, ARG_VALUE>::CPair *pair = src.PGetFirstAssoc()
		; pair != NULL
		; pair = src.PGetNextAssoc(pair))
		dst[pair->key] = pair->value;	

	return dst;
}

template< class KEY, class ARG_KEY, class VALUE, class ARG_VALUE >
CMap<KEY, ARG_KEY, VALUE, ARG_VALUE> &Resize(CMap<KEY, ARG_KEY, VALUE, ARG_VALUE> &hash, UINT nNewHashTableSize)
{
	if(hash.GetHashTableSize() == nNewHashTableSize)
		return hash;

	CMap<KEY, ARG_KEY, VALUE, ARG_VALUE> temp;
	temp.InitHashTable(nNewHashTableSize);
	Append(temp, hash);

	hash.RemoveAll();
	hash.InitHashTable(nNewHashTableSize);
	Append(hash, temp);

	return hash;
}

#endif//__COPYMAP_H__
