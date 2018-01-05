#include "testAffTrans.h"
#include "opencv2\core\mat.hpp"

int main() {

	std::vector<cv::Point3f> holom, vivem;

	for (int i = 0; i < 4; i++)
	{
		holom.push_back(cv::Point3f(i, i % 3, 1));
		vivem.push_back(cv::Point3f(i, i % 3, 1));
	}

	std::cout << "holom " << holom
		<< "\nvivem " << vivem << "\n";

	cv::Mat transm = cv::Mat::zeros(3, 4, CV_32FC1);
	estimateAffTrans(holom, vivem, transm);
	return 0;
}