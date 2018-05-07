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

Eigen::Vector3f pureSVD(Eigen::MatrixXf  meanData) {
	Eigen::JacobiSVD<Eigen::MatrixXf> svd(meanData, Eigen::ComputeFullU | Eigen::ComputeFullV);
	Eigen::Matrix3f v = svd.matrixV();
	OutputDebugStringA("\nv\n");
	for (int i = 0; i < v.size(); i++)
		OutputDebugStringA((std::to_string(v.data()[i]) + "\t").c_str());
	return v.col(0);
}

Eigen::Vector3f calIntersection(Eigen::MatrixXf pCens, Eigen::MatrixXf ks)
{
	Eigen::Matrix3f sigmaR = Eigen::Matrix3f::Zero();
	Eigen::Vector3f sigmaQ = Eigen::Vector3f::Zero();
	for (int i = 0; i < pCens.rows(); i++) {
		Eigen::Matrix3f Ri = Eigen::Matrix3f::Identity() - ks.row(i).transpose() * ks.row(i);
		sigmaR += Ri;
		Eigen::Vector3f Qi = Ri * pCens.row(i).transpose();
		sigmaQ += Qi;
	}
	Eigen::Vector3f p = sigmaR.inverse() * sigmaQ;
	return p;
}

Eigen::Matrix3f calRotation(Eigen::Vector3f tOffset, Eigen::MatrixXf pHeadsets, Eigen::MatrixXf pCmrs)
{
	Eigen::MatrixXf pLocal(pHeadsets.rows(), pHeadsets.cols());
	pLocal = pHeadsets.rowwise() - tOffset.transpose();
	//Eigen::MatrixXf pLocalNorm(pCens.rows(), pCens.cols());
	Eigen::MatrixXf pLocalNorm2(pHeadsets.rows(), pHeadsets.cols());
	//pLocalNorm = pLocal.rowwise() / pLocal.rowwise().norm;
	pLocalNorm2 = pLocal.rowwise().normalized();

	Eigen::MatrixXf pCmrsNorm(pCmrs.rows(), pCmrs.cols());
	pCmrsNorm = pCmrs.rowwise().normalized();

	int lineAmt = pCmrs.rows();
	int ppl = pHeadsets.rows() / lineAmt;
	Eigen::MatrixXf pCmrsNormDup(pHeadsets.rows(), pHeadsets.cols());
	std::cout << pCmrsNorm << "\n\n";
	for (int i = 0; i < lineAmt; i++) {
		for (int j = 0; j < ppl; j++) {
			pCmrsNormDup.row(i*ppl + j) = pCmrsNorm.row(i);
			//std::cout << pCmrsNormDup << "\n";
		}

	}
	OutputDebugStringA("\npLocalNorm2\n");
	for(int i = 0; i < pLocalNorm2.size(); i++)
		OutputDebugStringA((std::to_string(pLocalNorm2.data()[i]) + "\t").c_str());
	OutputDebugStringA("\npCmrsNorm\n");
	for (int i = 0; i < pCmrsNorm.size(); i++)
		OutputDebugStringA((std::to_string(pCmrsNorm.data()[i]) + "\t").c_str());
	Eigen::Matrix3f C = pLocalNorm2.transpose() * pCmrsNormDup;
	OutputDebugStringA("\nC\n");
	for (int i = 0; i < C.size(); i++)
		OutputDebugStringA((std::to_string(C.data()[i]) + "\t").c_str());
	std::cout << "C\n" << C << "\n";
	Eigen::JacobiSVD<Eigen::MatrixXf> svd(C, Eigen::ComputeFullU | Eigen::ComputeFullV);
	Eigen::Matrix3f v = svd.matrixV();
	Eigen::Matrix3f u = svd.matrixU();
	Eigen::Matrix3f VUT = v * u.transpose();
	//Eigen::Matrix3f VUT = u.transpose() * vt.transpose();
	//Eigen::Matrix3f VUT = u.transpose() * vt.transpose();
	//Eigen::Matrix3f VUT = u.transpose() * vt.transpose();
	std::cout << "vt\n" << v << "\n";
	std::cout << "u\n" << u << "\n";
	std::cout << "VUT\n" << VUT << "\n";
	float d = VUT.determinant();
	OutputDebugStringA("\nd:\n");
	OutputDebugStringA((std::to_string(d) + "\t").c_str());
	Eigen::Matrix3f mid = Eigen::Matrix3f::Identity();
	mid(2, 2) = d;
	Eigen::Matrix3f rotationOffset = v * mid * u.transpose();
	std::cout << "rotation offset\n" << rotationOffset;
	return rotationOffset;
}
