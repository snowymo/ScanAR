#pragma once

#include <opencv2/calib3d.hpp>

#define TESTDLLSORT_API __declspec(dllexport) 

extern "C" {
	TESTDLLSORT_API void TestSort(int a[], int length);

	TESTDLLSORT_API void EstimateAffTransTT(unsigned char * holopoints, unsigned char * vivepoints, unsigned char * transformation, int typesize[]);

	TESTDLLSORT_API void EstimateAffTransSF(unsigned char * holopoints, unsigned char * vivepoints, unsigned char * transformation);
}
//int estimateAffTrans(unsigned char * holopoints, unsigned char * vivepoints, unsigned char * transformation);