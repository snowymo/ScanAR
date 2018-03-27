import numpy as np
from math import sqrt
import random
import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D

# Input: expects Nx3 matrix of points
# Returns R,t
# R = 3x3 rotation matrix
# t = 3x1 column vector

def rigid_transform_3D(A, B):
    assert len(A) == len(B)

    N = A.shape[0]; # total points

    centroid_A = np.mean(A, axis=0).reshape(1,3)
    centroid_B = np.mean(B, axis=0).reshape(1,3)

    print(centroid_A.shape)
    print(centroid_B.shape)
    
    # centre the points
    AA = A - np.tile(centroid_A, (N, 1))
    BB = B - np.tile(centroid_B, (N, 1))

    print(AA.shape)
    print(BB.shape)
    print("seperator")
    
    # dot is matrix multiplication for array
    H = np.dot(np.transpose(AA),BB)

    U, S, Vt = np.linalg.svd(H)

    R = Vt.T * U.T

    # special reflection case
    if np.linalg.det(R) < 0:
       print ("Reflection detected")
       Vt[2,:] *= -1
       R = Vt.T * U.T

    print(np.dot(-R,centroid_A.T))
    #print(centroid_A.T)
    print(centroid_B.T)
    t = np.dot(-R,centroid_A.T) + centroid_B.T

    print("t")
    print (t)
    print("R")
    print (R)

    return R, t

# Test with random data

amount = 20
d_fixed = 0.1   # a point showed up in front of the eye
pos_eye = np.array([0,0,0])
C = np.empty((0,3), dtype=float)
D = np.empty((0,3), dtype=float)

fig = plt.figure()
ax = fig.add_subplot(111, projection='3d')

for i in range(0, amount):
    #print ("iteration: %d" % i)
    d_random = random.random() * random.randrange(2) + 0.3    # random distance btw eye and random point
    view_random = np.array([random.random(),random.random(),random.random()])    # random view direction of the eye
    pos_random = d_random * view_random
    pos_fixed = d_fixed * view_random
    C = np.row_stack((C, [pos_fixed]))
    D = np.row_stack((D, [pos_random]))
    ax.plot([pos_random[0], pos_eye[0]],[pos_random[1], pos_eye[1]],[pos_random[2], pos_eye[2]], c='yellow')
    #print("screen:")
    #print(pos_fixed)
    #print("random:")
    #print(pos_random)

#print(C)
print(D)

# recover the transformation
ret_R, ret_t = rigid_transform_3D(C, D)

A2 = (np.dot(ret_R,C.T)) + np.tile(ret_t, (1, amount))
A2 = A2.T

#print(np.tile(ret_t, (1, amount)))
#print(np.tile(ret_t, (1, amount)).shape)

# Find the error
# amount delta position
err = A2 - D
##print(err)
##err = np.multiply(err, err)
##print(err.shape)
##err = np.sum(err, axis=1)
### amount dis
##err = np.sqrt(err);
##print(err)
### sd
##err = np.multiply(err, err)
##err = sum(err)/amount
##print(err)
##rmse = np.sqrt(err);
err = np.multiply(err, err)
err = np.sum(err)
rmse = np.sqrt(err/amount);

print ("Points C")
print (C)
print ("")

print ("Points D")
print (D)
print ("")

print ("Rotation")
print (ret_R)
print ("")

print ("Translation")
print (ret_t)
print ("")

print ("RMSE:", rmse)
print ("If RMSE is near zero, the function is correct!")

ax.scatter(C[:,0],C[:,1],C[:,2], zdir='z', c= 'red')
ax.scatter(D[:,0],D[:,1],D[:,2], zdir='z', c= 'blue')
plt.show()
