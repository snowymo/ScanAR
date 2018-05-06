// calibration with SVD
#include <iostream>
#include <algorithm>
#include <vector>

#include <Eigen/Dense>

int calibrate(std::vector<Eigen::Vector3f> pointsetA, std::vector<Eigen::Vector3f> pointsetB) {
	//Kabsch algorithm
	int n = std::min(pointsetB.size(), pointsetA.size());
	if (n < 3)
	{
		std::cout << "Need at least three correspondences for calibration." << std::endl;
		return 0;
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
	Eigen::Affine3f transformAtoB;
	transformAtoB.setIdentity();
	transformAtoB.pretranslate(-X_mean);
	Eigen::Affine3f rotation;
	rotation.setIdentity();
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
	for(int i = 0; i < 16; i++)
		std::cout << transformAtoB.data()[i] << "\t";

	float* transform = new float[16 * 3];
	memcpy(transform, transformAtoB.data(), sizeof(float) * 16 * 3);

	delete[] transform;
	transform = NULL;

	//fine registration
// 	Eigen::Affine3f viveControllerTransform = transformSecondaryControllerToDavid;
// 	
// 	currentScan->buildTree();
// 	//currentScan->closestPointRadius = 30;
// 	//viveController->alignTo(*currentScan, 5, 1.0);	
// 	currentScan->closestPointRadius = 10;
// 	viveController->alignTo(*currentScan, 10, 1.0);
// 
// 	transformScannerControllerToDavidSystem = scannerControllerMatrix.inverse() * secondaryControllerMatrix * viveController->transform().inverse();
// 
// 	std::cout << "Controller to Scan registration matrix:" << std::endl << viveController->transform().matrix() << std::endl;
// 
// 	std::ofstream aln("Calibration.aln");
// 	aln << 3 << std::endl;
// 	writeALNPart(aln, "controller.ply", scannerControllerMatrix);
// 	writeALNPart(aln, "controller.ply", secondaryControllerMatrix);
// 	writeALNPart(aln, "scan.ply", scannerControllerMatrix * transformScannerControllerToDavidSystem);
// 	aln.close();
// 
// 	FILE* f = fopen("calibration.bin", "wb");
// 	if (f)
// 	{
// 		fwrite(transformScannerControllerToDavidSystem.data(), sizeof(float), 16, f);
// 
// 		fclose(f);
// 	}
// 
// 	correspondencesSecondaryController.clear();
// 	correspondencesDavidSystem.clear();
	return 0;
}

Eigen::Vector3f pureSVD(Eigen::MatrixXf  meanData) {
	Eigen::JacobiSVD<Eigen::MatrixXf> svd(meanData, Eigen::ComputeFullU | Eigen::ComputeFullV);
	Eigen::Matrix3f vt = svd.matrixV();
	Eigen::Vector3f vv0 = vt.row(0);
	Eigen::Matrix3f vtt = vt.transpose();
	Eigen::Matrix3f vti = vt.inverse();
	//std::cout << "vtt:" << vtt << "\nvti";
	//std::cout << vti << "\n";
	return vt.col(0);
}

int main() {
// 	std::vector<Eigen::Vector3f> pas, pbs;
// 	Eigen::Vector3f v3(3);
// 	v3 << 0.1756, 2.8244, 0.1526;
// 	pas.push_back(v3);
// 	
// 	//v3.resize(0);
// 	v3 << 0.2307, 2.8520, 0.0513;
// 	pas.push_back(v3);
// 
// 	v3 << 0.1928, 2.8307, 0.1067;
// 	pas.push_back(v3);
// 
// 	v3 << 0.2103, 2.8893, 0.1278;
// 	pas.push_back(v3);
// 
// 	v3 << 0.1698, 0.4759, 1.8881;
// 	pbs.push_back(v3);
// 
// 	v3 << 0.0561, 0.4843, 1.8550;
// 	pbs.push_back(v3);
// 
// 	v3 << 0.1236, 0.4714, 1.8712;
// 	pbs.push_back(v3);
// 
// 	v3 << 0.1089, 0.5121, 1.9193;
// 	pbs.push_back(v3);
// 
// 	calibrate(pas, pbs);

	//test
// 	Eigen::MatrixXf m = Eigen::MatrixXf::Random(10, 3);
// 	std::cout << "Here is the matrix m:" << std::endl << m << std::endl;
// 	Eigen::JacobiSVD<Eigen::MatrixXf> svd(m, Eigen::ComputeThinU | Eigen::ComputeThinV);

	Eigen::MatrixXf mean(10,3);
	mean << 0.00437297, 0.04020982, 0.07199235,
		0.01279119, 0.11761607, 0.21058184,
		0.04527895, 0.41634373, 0.74542896,
		-0.0128667, -0.11831038, -0.21182494,
		-0.02001746, -0.18406224, -0.32954819,
		-0.01319987, -0.12137395, -0.21731,
		0.01280345, 0.11772883, 0.21078372,
		-0.01623453, -0.14927783, -0.26726959,
		-0.01769715, -0.16272675, -0.29134876,
		0.00476915, 0.0438527, 0.07851462;
	std::cout << "Here is the matrix m:" << std::endl << mean << std::endl;
	Eigen::Vector3f vv0 = pureSVD(mean);
	//std::cout << "vv:" << vv <<"\n";
	std::cout << vv0 << "\n";
	return 0;
}