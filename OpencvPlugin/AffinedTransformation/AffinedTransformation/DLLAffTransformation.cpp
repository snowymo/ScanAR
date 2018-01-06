#include "DLLAffTransformation.h"
#include <algorithm>
#include <fstream>

extern "C" {
	void TestSort(int a[], int length) {
		std::sort(a, a + length);
	}

	void EstimateAffTransTT(unsigned char * holopoints, unsigned char * vivepoints, unsigned char * transformation, int typesize[])
	{
		// three points as a set by default
 		cv::Mat holomat = cv::Mat(4, 3, CV_32FC1, holopoints);
 		cv::Mat vivemat = cv::Mat(4, 3, CV_32FC1, vivepoints);
 		cv::Mat transmat = cv::Mat::ones(3, 4, CV_32FC1);
		cv::Mat one43 = cv::Mat::ones(4, 3, CV_32FC1);
		cv::Mat one34 = cv::Mat::ones(3, 4, CV_32FC1);
		std::vector<uchar> inliers;
 		cv::estimateAffine3D(vivemat, holomat, transmat, inliers);
// 		std::fstream file("output.txt");
// 		file << "holom " << holomat
// 			<< "\nvivem " << vivemat 
// 			<< "\ntransmat" << transmat << "\n";

		//transmat = cv::Mat::ones(3, 4, CV_32FC1);
		//int sizeofdata = sizeof one34.data;
		memcpy(transformation, one34.data, sizeof(one34.data) * 3 * 4);
		memcpy(holopoints, one43.data, sizeof(one43.data )* 4 * 3);
		memcpy(vivepoints, one43.data, sizeof(one43.data) * 4 * 3);
		//memcpy(typesize, &sizeofdata, sizeof(int));
		typesize[0] = sizeof(one34.data);
		//transformation = transmat.data;
	}

	void EstimateAffTransSF(unsigned char * holopoints, unsigned char * vivepoints, unsigned char * transformation)
	{
		// three points as a set by default
		 		cv::Mat holomat = cv::Mat(4, 3, CV_64FC1, holopoints);
		 		cv::Mat vivemat = cv::Mat(4, 3, CV_64FC1, vivepoints);
		 		cv::Mat transmat = cv::Mat::ones(3, 4, CV_64FC1);
				cv::Mat one43 = cv::Mat::ones(4, 3, CV_64FC1);
				cv::Mat one34 = cv::Mat::ones(3, 4, CV_64FC1);
		std::vector<uchar> inliers;
		 		cv::estimateAffine3D(vivemat, holomat, transmat, inliers);
		// 		std::fstream file("output.txt");
		// 		file << "holom " << holomat
		// 			<< "\nvivem " << vivemat 
		// 			<< "\ntransmat" << transmat << "\n";

		//transmat = cv::Mat::ones(3, 4, CV_32FC1);
		memcpy(transformation, transmat.data, sizeof one34.data * 3 * 4);
		//memcpy(holopoints, one43.data, sizeof one43.data * 4 * 3);
		//memcpy(vivepoints, one43.data, sizeof one43.data * 4 * 3);
		//transformation = transmat.data;
	}
}