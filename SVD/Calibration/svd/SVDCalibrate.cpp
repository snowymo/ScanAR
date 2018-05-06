#include "SVDCalibrate.h"
#include <Windows.h>

extern "C" {
	void Calib(float* a, float* b, int len, float* transform)
	{
		std::vector<Eigen::Vector3f> pointsetA, pointsetB;
		int setsAmount = len / 3;
		for (int i = 0; i < setsAmount; i++) {
			Eigen::Vector3f va(3);
			va << a[i * 3], a[i * 3 + 1], a[i * 3 + 2];
			pointsetA.push_back(va);

			Eigen::Vector3f vb(3);
			vb << b[i * 3], b[i * 3 + 1], b[i * 3 + 2];
			pointsetB.push_back(vb);
		}

		Eigen::Affine3f mtx = calibrate(pointsetA, pointsetB);

		OutputDebugStringA(std::to_string(transform[0]).c_str());
		//transform = new float[16*3];
		memcpy(transform, mtx.data(), sizeof(float) * 16);
		OutputDebugStringA(std::to_string(transform[0]).c_str());
	}

	void SVD(float* a, int len, float* k)
	{
		Eigen::MatrixXf mean(len, 3);
		for (int i = 0; i < len * 3; i++) {
			mean << a[i];
		}
		Eigen::Vector3f vvo = pureSVD(mean);
		memcpy(k, vvo.data(), sizeof(float) * 3);
	}
}
