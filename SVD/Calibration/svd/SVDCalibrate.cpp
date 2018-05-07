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
			mean(i / 3, i % 3) = a[i];
			OutputDebugStringA(std::to_string(a[i]).c_str());
			OutputDebugStringA("\t");
		}
		OutputDebugStringA("\nmean\n");
		for (int i = 0; i < mean.size(); i++)
			OutputDebugStringA((std::to_string(mean(i/3,i%3)) + "\t").c_str());
		Eigen::Vector3f vvo = pureSVD(mean);
		memcpy(k, vvo.data(), sizeof(float) * 3);
	}

	void CalIntersection(float* cens, float* ks, int len, float* intersection) {
		Eigen::MatrixXf mCens(len, 3);
		Eigen::MatrixXf mKs(len, 3);

		for (int i = 0; i < len * 3; i++) {
			mCens(i / 3, i % 3) = cens[i];
			mKs(i / 3, i % 3) = ks[i];
		}
		Eigen::Vector3f p = calIntersection(mCens, mKs);
		memcpy(intersection, p.data(), sizeof(float) * 3);
	}

	void CalRotation(float* tOffset, float* pHeadsets, float* pCmrs, int lineAmt, int pPerLine, float* rotation) {
		Eigen::Vector3f vOffset;
		vOffset(0) = tOffset[0];
		vOffset(1) = tOffset[1];
		vOffset(2) = tOffset[2];
		Eigen::MatrixXf mHeadsets(lineAmt * pPerLine,3);
		Eigen::MatrixXf mCmrs(lineAmt, 3);
		for (int i = 0; i < lineAmt; i++) {
			for (int j = 0; j < pPerLine; j++) {
				int idx = i*pPerLine + j;
				mHeadsets(idx, 0) = pHeadsets[idx * 3];
				mHeadsets(idx, 1) = pHeadsets[idx * 3+1];
				mHeadsets(idx, 2) = pHeadsets[idx * 3+2];
				//mHeadsets << pHeadsets[idx * 3], pHeadsets[idx * 3 + 1], pHeadsets[idx * 3 + 2];
			}
			mCmrs(i, 0) = pCmrs[i * 3];
			mCmrs(i, 1) = pCmrs[i * 3 + 1];
			mCmrs(i, 2) = pCmrs[i * 3 + 2];
			//mCmrs << pCmrs[i * 3], pCmrs[i * 3 + 1], pCmrs[i * 3 + 2];
		}
		Eigen::Matrix3f rotationOffset = calRotation(vOffset, mHeadsets, mCmrs);
		memcpy(rotation, rotationOffset.data(), sizeof(float) * 3 * 3);
	}
}
