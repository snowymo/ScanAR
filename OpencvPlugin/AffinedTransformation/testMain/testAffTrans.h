#pragma once

#include <opencv2/calib3d.hpp>
#include <iostream>


int estimateAffTrans(unsigned char * holopoints, unsigned char * vivepoints, unsigned char * transformation);

int estimateAffTrans(cv::Mat holopoints, cv::Mat vivepoints, cv::Mat& transformation);

int estimateAffTrans(std::vector<cv::Point3f> holopoints, std::vector<cv::Point3f> vivepoints, cv::Mat& transformation);