#include "DLLAffTransformation.h"
#include <algorithm>
#include <fstream>

extern "C" {
	void TestSort(int a[], int length) {
		std::sort(a, a + length);
	}

	void EstimateAffTrans(unsigned char * holopoints, unsigned char * vivepoints, unsigned char * transformation)
	{
		// three points as a set by default
		cv::Mat holomat = cv::Mat(3, 3, CV_32FC1, holopoints);
		cv::Mat vivemat = cv::Mat(3, 3, CV_32FC1, vivepoints);
		cv::Mat transmat = cv::Mat(3, 4, CV_32FC1, vivepoints);
		std::vector<uchar> inliers;
		cv::estimateAffine3D(vivemat, holomat, transmat, inliers);
		std::fstream file("output.txt");
		file << "holom " << holomat
			<< "\nvivem " << vivemat 
			<< "\ntransmat" << transmat << "\n";

		//transmat = cv::Mat::ones(3, 4, CV_32FC1);
		transformation = transmat.data;
	}
}