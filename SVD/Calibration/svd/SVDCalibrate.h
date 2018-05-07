#ifdef SVDDLL_EXPORT
#define SVDDLL_API __declspec(dllexport) 
#else
#define SVDDLL_API __declspec(dllimport) 
#endif

#include "calibHelp.h"

extern "C" {
	SVDDLL_API void Calib(float* a, float* b, int len, float* transform);
	SVDDLL_API void SVD(float* a, int len, float* k);
	SVDDLL_API void CalIntersection(float* cens, float* ks, int len, float* intersection);
	SVDDLL_API void CalRotation(float* tOffset, float* pHeadsets, float* pCmrs, int lineAmt, int pPerLine, float* rotation);
}