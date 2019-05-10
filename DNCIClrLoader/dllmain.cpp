// Main Module - Contains DLLMain and the Export Function to Load .NET Assembly into Memory

/*****************************************************************************************************
	WARNING
	
	Try to Load .NET Framework inside of DllMain ATTACH could/maybe/will result in loader lock.
	Reference: http://msdn.microsoft.com/en-us/library/ms172219.aspx
*****************************************************************************************************/


#include "pch.h"

#include <metahost.h>
#include <string>

#pragma comment(lib, "mscoree.lib")

// Import CLR Loader Args Interface
# include "ClrLoaderArgs.h"

using namespace std;

// Import mscorlib.tlb from .NET Subsystem
#import "mscorlib.tlb" raw_interfaces_only				\
    high_property_prefixes("_get","_put","_putref")		\
    rename("ReportEvent", "InteropServices_ReportEvent")

using namespace mscorlib;


//
// LoadDNA - Load Dot Net Assembly Function
//
__declspec(dllexport) HRESULT LoadDNA(_In_ LPCTSTR lpCommand)
{
	HRESULT hr;
	ICLRMetaHost* pMetaHost = NULL;
	ICLRRuntimeInfo* pRuntimeInfo = NULL;
	ICLRRuntimeHost* pClrRuntimeHost = NULL;

	// Load .NET Runtime
	hr = CLRCreateInstance(CLSID_CLRMetaHost, IID_PPV_ARGS(&pMetaHost));
	hr = pMetaHost->GetRuntime(L"v4.0.30319", IID_PPV_ARGS(&pRuntimeInfo));
	hr = pRuntimeInfo->GetInterface(CLSID_CLRRuntimeHost, IID_PPV_ARGS(&pClrRuntimeHost));

	// Start Runtime
	hr = pClrRuntimeHost->Start();

	// Parse Arguments
	ClrLoaderArgs args(lpCommand);

	// Execute Loaded .NET Code
	DWORD pReturnValue;
	hr = pClrRuntimeHost->ExecuteInDefaultAppDomain(
		args.pwzAssemblyPath.c_str(),
		args.pwzTypeName.c_str(),
		args.pwzMethodName.c_str(),
		args.pwzArgument.c_str(),
		&pReturnValue);

	// (optional) unload the .net runtime; note it cannot be restarted if stopped without restarting the process
	//hr = pClrRuntimeHost->Stop();

	// Release and Free Resources
	pMetaHost->Release();
	pRuntimeInfo->Release();
	pClrRuntimeHost->Release();

	// Return .NET Code Result
	return hr;
}


//
// Main C++ Dll Entry Point
//
BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{

	// Do Nothing. Just exists for Entry Point
	return TRUE;
}
