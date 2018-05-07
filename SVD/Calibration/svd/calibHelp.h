#pragma once

#include <iostream>
#include <algorithm>
#include <vector>

#include <Eigen/Dense>

Eigen::Affine3f calibrate(std::vector<Eigen::Vector3f> pointsetA, std::vector<Eigen::Vector3f> pointsetB);

Eigen::Vector3f pureSVD(Eigen::MatrixXf  meanData);

Eigen::Vector3f calIntersection(Eigen::MatrixXf pCens, Eigen::MatrixXf ks);

Eigen::Matrix3f calRotation(Eigen::Vector3f tOffset, Eigen::MatrixXf pHeadsets, Eigen::MatrixXf pCmrs);