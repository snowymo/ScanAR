#include "testAffTrans.h"

int estimateAffTrans(unsigned char * holopoints, unsigned char * vivepoints, unsigned char * transformation)
{
	// three points as a set by default
	cv::Mat holomat = cv::Mat(3, 3, CV_32FC1, holopoints);
	cv::Mat vivemat = cv::Mat(3, 3, CV_32FC1, vivepoints);
	cv::Mat transmat = cv::Mat(3, 4, CV_32FC1, vivepoints);
	std::vector<uchar> inliers;
	cv::estimateAffine3D(vivemat, holomat, transmat, inliers);
	std::cout << "vivemat " << vivemat
		<< "holomat " << holomat
		<< "transmat " << transmat << "\n";
	return 0;
}

int estimateAffTrans(cv::Mat holopoints, cv::Mat vivepoints, cv::Mat& transformation)
{
	std::vector<uchar> inliers;
	int ret = cv::estimateAffine3D(holopoints, vivepoints, transformation, inliers);
	std::cout << "vivemat " << vivepoints
		<< "\nholomat " << holopoints
		<< "\ntransmat " << transformation 
		<< "\nret " << ret;
	return ret;
}

//std::vector<cv::Point3f>
int estimateAffTrans(std::vector<cv::Point3f> holopoints, std::vector<cv::Point3f> vivepoints, cv::Mat& transformation)
{
	std::vector<uchar> inliers;
	int ret = cv::estimateAffine3D(holopoints, vivepoints, transformation, inliers);
	std::cout << "vivemat " << vivepoints
		<< "\nholomat " << holopoints
		<< "\ntransmat " << transformation
		<< "\nret " << ret;
	cv::Mat one = cv::Mat::ones(4, 3, CV_64FC1);
	std::cout << "\none data:" << sizeof one.data << "\n";
	return ret;
}