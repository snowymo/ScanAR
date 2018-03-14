from numpy import *
from math import sqrt

# Input: expects Nx3 matrix of points
# Returns R,t
# R = 3x3 rotation matrix
# t = 3x1 column vector

def rigid_transform_3D(A, B):
    assert len(A) == len(B)

    N = A.shape[0]; # total points

    centroid_A = mean(A, axis=0)
    centroid_B = mean(B, axis=0)
    
    # centre the points
    AA = A - tile(centroid_A, (N, 1))
    BB = B - tile(centroid_B, (N, 1))

    # dot is matrix multiplication for array
    H = transpose(AA) * BB

    U, S, Vt = linalg.svd(H)

    R = Vt.T * U.T

    # special reflection case
    if linalg.det(R) < 0:
       print ("Reflection detected")
       Vt[2,:] *= -1
       R = Vt.T * U.T

    t = -R*centroid_A.T + centroid_B.T

    print (t)

    return R, t

# Test with random data

# Random rotation and translation
R = mat(random.rand(3,3))
t = mat(random.rand(3,1))

# make R a proper rotation matrix, force orthonormal
U, S, Vt = linalg.svd(R)
R = U*Vt

# remove reflection
if linalg.det(R) < 0:
   Vt[2,:] *= -1
   R = U*Vt

# number of points
n = 4

# A = mat(random.rand(n,3));
A = array([[0.1756, 2.8244, 0.1526],
              [0.2307, 2.8520, 0.0513],
              [0.1928, 2.8307, 0.1067],
              [0.2103, 2.8893, 0.1278]]);
A = asmatrix(A);
# B = R*A.T + tile(t, (1, n))
# B = B.T;
B = array([[0.1698, 0.4759, 1.8881],
              [0.0561, 0.4843, 1.8550],
              [0.1236, 0.4714, 1.8712],
              [0.1089, 0.5121, 1.9193]]);
B = asmatrix(B);

# recover the transformation
ret_R, ret_t = rigid_transform_3D(A, B)

A2 = (ret_R*A.T) + tile(ret_t, (1, n))
A2 = A2.T

# Find the error
err = A2 - B

err = multiply(err, err)
err = sum(err)
rmse = sqrt(err/n);

print ("Points A")
print (A)
print ("")

print ("Points B")
print (B)
print ("")

print ("Rotation")
print (ret_R)
print ("")

print ("Translation")
print (ret_t)
print ("")

print ("RMSE:", rmse)
print ("If RMSE is near zero, the function is correct!")
