#pragma once

#include <metahost.h>
#include <string>

using namespace std;

//
// Parses arguments used to invoke a managed assembly
//
struct ClrLoaderArgs
{
	static const LPCWSTR DELIM;

	ClrLoaderArgs(LPCWSTR command)
	{
		int i = 0;
		wstring s(command);
		wstring* ptrs[] = { &pwzAssemblyPath, &pwzTypeName, &pwzMethodName };

		while (s.find(DELIM) != wstring::npos && i < 3)
		{
			*ptrs[i++] = s.substr(0, s.find(DELIM));
			s.erase(0, s.find(DELIM) + 1);
		}

		if (s.length() > 0)
			pwzArgument = s;
	}

	wstring pwzAssemblyPath;
	wstring pwzTypeName;
	wstring pwzMethodName;
	wstring pwzArgument;
};

const LPCWSTR ClrLoaderArgs::DELIM = L"\t"; // delimiter