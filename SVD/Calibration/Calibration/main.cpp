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
	Eigen::Matrix3f v = svd.matrixV();
	//Eigen::Vector3f vv0 = vt.row(0);
	//Eigen::Matrix3f vtt = vt.transpose();
	//Eigen::Matrix3f vti = vt.inverse();
	std::cout << "v:" << v << "\n";
	//std::cout << vti << "\n";
	return v.col(0);
}

Eigen::Vector3f calIntersection(Eigen::MatrixXf pCens, Eigen::MatrixXf ks)
{
	Eigen::Matrix3f sigmaR = Eigen::Matrix3f::Zero();
	Eigen::Vector3f sigmaQ = Eigen::Vector3f::Zero();
	//sigmaQ << 0, 0, 0;
	for (int i = 0; i < pCens.rows(); i++) {
 		Eigen::Matrix3f Ri = Eigen::Matrix3f::Identity() - ks.row(i).transpose() * ks.row(i);
		std::cout << "nnT\t" << ks.row(i).transpose() * ks.row(i) << "\n";
		std::cout << "Ri\t" << Ri << "\n";
 		sigmaR += Ri;
 		Eigen::Vector3f Qi = Ri * pCens.row(i).transpose();
		std::cout << "Qi\t" << Qi << "\n";
 		sigmaQ += Qi;
	}
	std::cout << "sigmaR\t" << sigmaR << "\n";
	std::cout << "sigmaQ\t" << sigmaQ << "\n";
	Eigen::Vector3f p = sigmaR.inverse() * sigmaQ;
	return p;
	//return Eigen::Vector3f::Zero();
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
	Eigen::Matrix3f C = pLocalNorm2.transpose() * pCmrsNormDup;
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
	Eigen::Matrix3f mid = Eigen::Matrix3f::Identity();
	mid(2, 2) = d;
	Eigen::Matrix3f rotationOffset = v * mid * u.transpose();
	std::cout << "rotation offset\n" << rotationOffset;
	return rotationOffset;
}

void calRotation2(Eigen::Vector3f tOffset, Eigen::MatrixXf pCens, Eigen::MatrixXf pCmrs)
{
	Eigen::MatrixXf pLocal(pCens.rows(), pCens.cols());
	pLocal = pCens.rowwise() - tOffset.transpose();
	//Eigen::MatrixXf pLocalNorm(pCens.rows(), pCens.cols());
	Eigen::MatrixXf pLocalNorm2(pCens.rows(), pCens.cols());
	//pLocalNorm = pLocal.rowwise() / pLocal.rowwise().norm;
	pLocalNorm2 = pLocal.rowwise().normalized();

	Eigen::MatrixXf pCmrsNorm(pCmrs.rows(), pCmrs.cols());
	pCmrsNorm = pCmrs.rowwise().normalized();

	int lineAmt = pCmrs.rows();
	int ppl = pCens.rows() / lineAmt;
	Eigen::MatrixXf pCmrsNormDup(pCens.rows(), pCens.cols());
	std::cout << pCmrsNorm << "\n\n";
	for (int i = 0; i < lineAmt; i++) {
		for (int j = 0; j < ppl; j++){
			pCmrsNormDup.row(i*ppl+j) = pCmrsNorm.row(i);
			//std::cout << pCmrsNormDup << "\n";
		}
			
	}
	Eigen::Matrix3f C = pLocalNorm2.transpose() * pCmrsNormDup;
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
	Eigen::Matrix3f mid = Eigen::Matrix3f::Identity();
	mid(2, 2) = d;
	Eigen::Matrix3f rotationOffset = v * mid * u.transpose();
	std::cout << "rotation offset\n" << rotationOffset;
	
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

	// test svd
	Eigen::MatrixXf mean(5,3);
	mean << 0, 10.5, 0,
		0, 4.5, 0,
		0, 0, 0,
		-0, -10.5, -0,
		-0, -4.5, -0;
	std::cout << "Here is the matrix m:" << std::endl << mean << std::endl;
	Eigen::Vector3f vv0 = pureSVD(mean);
	//std::cout << "vv:" << vv <<"\n";
	std::cout << vv0 << "\n";

	// test intersection point
// 	Eigen::MatrixXf ks(5, 3);
// 	Eigen::MatrixXf cens(5, 3);
// 	ks << -0.04113644, - 0.36959511, - 0.92828188,
// 	0.05671096 ,- 0.79801888, - 0.5999581,
// 	-0.15241208  ,0.9869512 ,- 0.0519412,
// 	0.0753808   ,0.17666255 , 0.9813807,
// 	-0.01718273,  0.64612344 , 0.76303948;
// 	cens << 
// 		0.73217485,  0.18907871, - 0.27394459,
// 		0.66269672,  0.42492002, - 0.60536019,
// 		0.58139666,  0.66802117, - 1.04041936,
// 		0.77262614,  0.07020671, - 0.0544822,
// 		0.68662335,  0.40300336, - 0.40597818;
// 	Eigen::Vector3f p = calIntersection(cens, ks);
// 	std::cout << "p" << p << "\n";

	// test rotation matrix
// 	Eigen::Vector3f tOffset;
// 	tOffset << -0.2,0.6,-0.3;
// 	Eigen::MatrixXf pHeadsets(10*5,3);
// 	pHeadsets <<
// 		-2.06263177e-01,  7.16667066e-01, - 1.42908451e-01,
// 		-2.19833408e-01,  9.69445994e-01,  1.97456957e-01,
// 	-2.34261163e-01,  1.23819841e+00,  5.59330580e-01,
// 	-2.22224849e-01,  1.01399247e+00,  2.57438532e-01,
// 	-2.25087494e-01,  1.06731628e+00,  3.29238741e-01,
// 	-2.33845017e-01,  1.23044666e+00,  5.48892895e-01,
// 	-2.11889607e-01,  8.21473175e-01, - 1.78761349e-03,
// 	-2.07490008e-01,  7.39519826e-01, - 1.12137337e-01,
// 	-2.10232690e-01,  7.90609017e-01, - 4.33460293e-02,
// 	-2.02606498e-01,  6.48552443e-01, - 2.34624409e-01,
// 
// 		-2.68673181e-01,  1.45045182e+00, - 2.40375211e-03,
// 		-2.42417613e-01,  1.12530166e+00, - 1.16182645e-01,
// 	-2.49184930e-01,  1.20910842e+00, - 8.68563774e-02,
// 	-2.74478625e-01,  1.52234670e+00,  2.27542258e-02,
// 	-2.55659923e-01,  1.28929503e+00, - 5.87968987e-02,
// 	-2.86938980e-01,  1.67665631e+00,  7.67513587e-02,
// 	-2.99811425e-01,  1.83606925e+00,  1.32534288e-01,
// 	-2.28867754e-01,  9.57499583e-01, - 1.74901161e-01,
// 	-2.24446379e-01,  9.02745081e-01, - 1.94061253e-01,
// 	-2.59606370e-01,  1.33816802e+00, - 4.16949110e-02,
// 
// 	-2.01797401e-01,  6.86551032e-01, - 7.85545187e-02,
// 	-2.03081442e-01,  7.48381966e-01,  7.96432591e-02,
// 	-2.06816026e-01,  9.28214970e-01,  5.39755691e-01,
// 	-2.01085315e-01,  6.52261615e-01, - 1.66285840e-01,
// 	-2.00337035e-01,  6.16229387e-01, - 2.58476239e-01,
// 	-2.09372916e-01,  1.05133800e+00,  8.54772605e-01,
// 	-2.10312603e-01,  1.09658713e+00,  9.70544952e-01,
// 	-2.05873978e-01,  8.82852169e-01,  4.23692522e-01,
// 	-2.06735121e-01,  9.24319113e-01,  5.29787933e-01,
// 	-2.06284138e-01,  9.02602761e-01,  4.74225474e-01,
// 
// 	-2.62953466e-01,  1.51526096e+00,  4.23579581e-01,
// 	-2.24554884e-01,  9.56995862e-01, - 1.77691091e-02,
// 	-2.71299196e-01,  1.63659695e+00,  5.19504401e-01,
// 	-2.21512048e-01,  9.12757011e-01, - 5.27431011e-02,
// 	-2.74793915e-01,  1.68740559e+00,  5.59672282e-01,
// 	-2.75604157e-01,  1.69918544e+00,  5.68985106e-01,
// 	-2.31180774e-01,  1.05332763e+00,  5.83880795e-02,
// 	-2.55020815e-01,  1.39993061e+00,  3.32402650e-01,
// 	-2.69038662e-01,  1.60373176e+00,  4.93522105e-01,
// 	-2.80491085e-01,  1.77023498e+00,  6.25154870e-01,
// 
// 	-2.35832663e-01,  1.20445122e+00,  3.89316242e-01,
// 	-2.20071954e-01,  9.38588197e-01,  8.61260226e-02,
// 	-2.10448009e-01,  7.76244562e-01, - 9.90106795e-02,
// 	-2.49559078e-01,  1.43599828e+00,  6.53372550e-01,
// 	-2.23259730e-01,  9.92361913e-01,  1.47449576e-01,
// 	-2.03188282e-01,  6.53782250e-01, - 2.38666715e-01,
// 	-2.15374833e-01,  8.59353777e-01, - 4.23291956e-03,
// 	-2.10347807e-01,  7.74554275e-01, - 1.00938283e-01,
// 	-2.56006440e-01,  1.54475704e+00,  7.77401045e-01,
// 	- 2.14522903e-01,  8.44982802e-01, - 2.06215812e-02;
// 	Eigen::MatrixXf pCmrs(5, 3);
// 	pCmrs<<
// 		0,
// 		0.66612297,
// 	0.74584193,
// 
// 	0,
// 	0.96996311,
// 	0.24325205,
// 
// 	0,
// 	0.44550206,
// 	0.89528091,
// 
// 	0,
// 	0.83750362,
// 	0.54643178,
// 
// 	0,
// 	0.72453428,
// 	0.68923877;
// 	Eigen::Matrix3f  rotationOffset = calRotation(tOffset, pHeadsets, pCmrs);

	return 0;
}