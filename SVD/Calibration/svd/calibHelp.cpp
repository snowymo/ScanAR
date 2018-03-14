#include "calibHelp.h"
//#include <WinBase.h>
#include <Windows.h>

Eigen::Affine3f calibrate(std::vector<Eigen::Vector3f> pointsetA, std::vector<Eigen::Vector3f> pointsetB) {
	Eigen::Affine3f transformAtoB, rotation;
	transformAtoB.setIdentity();
	rotation.setIdentity();

	//Kabsch algorithm
	int n = min(pointsetB.size(), pointsetA.size());
	if (n < 3)
	{
		std::cout << "Need at least three correspondences for calibration." << std::endl;
		return transformAtoB;
	}


	Eigen::Vector3f X_mean, Y_mean;
	X_mean.setZero(); Y_mean.setZero();
	for (int i = 0; i < n; ++i)
	{
		X_mean += pointsetA[i];
		Y_mean += pointsetB[i];
	}
	X_mean /= n;
	Y_mean /= n;

	/// Compute transformation
	Eigen::Matrix3f sigma; sigma.setConstant(0.0f);
	for (int i = 0; i < n; ++i)
		sigma += (pointsetA[i] - X_mean) * (pointsetB[i] - Y_mean).transpose();
	Eigen::JacobiSVD<Eigen::Matrix3f> svd(sigma, Eigen::ComputeFullU | Eigen::ComputeFullV);
	
	transformAtoB.pretranslate(-X_mean);
	if (svd.matrixU().determinant()*svd.matrixV().determinant() < 0.0) {
		Eigen::Vector3f S = Eigen::Vector3f::Ones(); S(2) = -1.0;
		rotation.linear().noalias() = svd.matrixV()*S.asDiagonal()*svd.matrixU().transpose();
	}
	else {
		rotation.linear().noalias() = svd.matrixV()*svd.matrixU().transpose();
	}
	transformAtoB = rotation * transformAtoB;
	transformAtoB.pretranslate(Y_mean);

	float error = 0;
	for (int i = 0; i < n; ++i)
		error += (pointsetB[i] - transformAtoB * pointsetA[i]).norm();
	error /= n;

	std::cout << "Coarse calibration error: " << error << "." << std::endl;
	//Eigen::IOFormat CleanFmt(4, 0, ", ", "\n", "[", "]");
	for (int i = 0; i < 16; i++) {
		std::cout << transformAtoB.data()[i] << "\t";
		OutputDebugStringA(std::to_string(transformAtoB.data()[i]).c_str());
		OutputDebugStringA("\t");
	}
		

	
	return transformAtoB;
}