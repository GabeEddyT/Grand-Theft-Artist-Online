#ifndef _LIB_H
#define _LIB_H

//	configuring the DLL
#ifdef MYPLUGIN_EXPORT
#define MYPLUGIN_SYMBOL __declspec(dllexport)
#else 
#ifdef MYPLUGIN_IMPORT
#define MYPLUGIN_IMPORT __declspec(dllimport)
#else
#define	MYPLUGIN_SYMBOL
#endif // MYPLUGIN_IMPORT
#endif
#endif // !LIB_H
