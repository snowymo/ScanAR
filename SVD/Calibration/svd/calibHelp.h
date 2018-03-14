#pragma once

#include <iostream>
#include <algorithm>
#include <vector>

#include <Eigen/Dense>

Eigen::Affine3f calibrate(std::vector<Eigen::Vector3f> pointsetA, std::vector<Eigen::Vector3f> pointsetB);