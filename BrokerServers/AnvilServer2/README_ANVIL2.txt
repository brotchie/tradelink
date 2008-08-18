
This is a object model version of the anvil server, 
that inherits from the same TLS server that powers
the InteractiveBrokers client.

Unfortunately I haven't been able to get it to work with Anvil,
it builds but when I try to load the extension I get 
'specified module not found', which is an error windows gets
when it can't load a dll required in the resource chain.  I've spent 
many hours trying to figure this out on my own, and with Assent developers
with no luck.   Until then Anvil1 works fine, which is built ontop of the
AnvilExtExample project that Assent provides.

Another note... be careful storing your own DLLs inside of the anvil directory.
Anvil seems to crash upon load if you do this while using the '[Extension]'
directive in Anvil.ini.   It will do this even with 3rd party DLLs that are not
anvil extensions.  I have spoken to the head of assent development about this and
he is aware of this 'feature', so it may be fixed in a future release.