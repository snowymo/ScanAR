import numpy as np
from math import sqrt
import random
import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D
from numpy import linalg as LA

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

fig = plt.figure(1)
ax = fig.add_subplot(1,2,1, projection='3d')
bx = fig.add_subplot(133)

# Test with random data

amount = 20

# generate Meye
# Random rotation and translation
R = np.mat(np.random.rand(3,3))
t = np.mat(np.random.rand(3,1))
t = np.array([0,0,0])

# make R a proper rotation matrix, force orthonormal
U, S, Vt = LA.svd(R)
R = U*Vt

# remove reflection
if LA.det(R) < 0:
   Vt[2,:] *= -1
   R = U*Vt
print(R)
#R = np.array([[1,0,0],[0,1,0],[0,0,1]])
print(t)
Meye = np.concatenate(
    (np.concatenate((R.T,t.reshape(1,3))).T, np.array([0,0,0,1]).reshape(1,4)))
print(Meye)

p_eye0 = np.matmul(Meye,np.array([0,0,0,1]))
p_eye = np.squeeze(np.asarray(p_eye0))[:3].reshape(1,3)
gaze_eye = np.squeeze(np.asarray(np.matmul(Meye,np.array([0,0,-1,1]))))[:3].reshape(1,3)
up_eye = np.squeeze(np.asarray(np.matmul(Meye,np.array([0,1,0,1]))))[:3].reshape(1,3)
print("\tpos eye:\t" + str(p_eye) + "\n\tgaze:\t" + str(gaze_eye) + "\n\tup:\t" + str(up_eye))
ax.plot([0,gaze_eye[0][0]],[0,gaze_eye[0][1]],[0,gaze_eye[0][2]],c='y')
ax.plot([0,up_eye[0][0]],[0,up_eye[0][1]],[0,up_eye[0][2]],c='b')
# change gaze and up every frame
w_eye = -gaze_eye/LA.norm(gaze_eye)
u_eye = np.cross(up_eye, w_eye)
u_eye = u_eye/LA.norm(u_eye)
v_eye = np.cross(w_eye, u_eye).reshape(1,3)
w_eye = w_eye.reshape(1,3)
u_eye = u_eye.reshape(1,3)
##print(w_eye)
##print(u_eye)
##print(v_eye)
print("\tw_eye:\t" + str(w_eye) + "\n\tu_eye:\t" + str(u_eye) + "\n\tv_eye:\t" + str(v_eye))
Mcmr0 = np.concatenate((u_eye, v_eye, w_eye, p_eye), axis=0)
#print(Mcmr0)
Mcmr1 = Mcmr0.T
#print(Mcmr1)
Mcmr2 = np.concatenate((Mcmr1, np.array([0,0,0,1]).reshape(1,4)), axis=0)
#print(Mcmr2)
Mcmr = LA.inv(Mcmr2)
#print(Mcmr)

near = 1
Mproj = np.array([[1,0,0,0],[0,1,0,0],[0,0,0,0],[0,0,-1/near,0]])
#print(Mproj)

##pos_obj = np.array([10,2,-2,1])
pos_obj = np.concatenate((gaze_eye.T,np.array([1]).reshape(1,1)))
print(pos_obj)
mtx =  np.matmul(Mproj, Mcmr)
screen_pos = np.matmul(mtx, pos_obj)
screen_pos = screen_pos/screen_pos[3]
print("screen_pos" + str(screen_pos))


ax.scatter(p_eye[:,0],p_eye[:,1],p_eye[:,2], zdir='z', c= 'red')
ax.scatter(pos_obj[0],pos_obj[1],pos_obj[2], zdir='z', c= 'blue')

bx.scatter(screen_pos[0],screen_pos[1], c= 'green')

##f, axarr = plt.subplots(2)
##axarr[0].scatter(p_eye[:,0],p_eye[:,1],p_eye[:,2], zdir='z', c= 'red')
##axarr[0].scatter(pos_obj[0],pos_obj[1],pos_obj[2], zdir='z', c= 'blue')
##axarr[0].set_title('3D view')
##axarr[1].scatter(screen_pos[0],screen_pos[1],screen_pos[2], zdir='z', c= 'green')
##axarr[1].set_title('screen view')
plt.show()
##d_fixed = 0.1   # a point showed up in front of the eye
##pos_eye = np.array([0,0,0])
##C = np.empty((0,3), dtype=float)
##D = np.empty((0,3), dtype=float)
##
##fig = plt.figure()
##ax = fig.add_subplot(111, projection='3d')
##
##for i in range(0, amount):
##    #print ("iteration: %d" % i)
##    d_random = random.random() * random.randrange(2) + 0.3    # random distance btw eye and random point
##    view_random = np.array([random.random(),random.random(),random.random()])    # random view direction of the eye
##    pos_random = d_random * view_random
##    pos_fixed = d_fixed * view_random
##    C = np.row_stack((C, [pos_fixed]))
##    D = np.row_stack((D, [pos_random]))
##    ax.plot([pos_random[0], pos_eye[0]],[pos_random[1], pos_eye[1]],[pos_random[2], pos_eye[2]], c='yellow')
##    #print("screen:")
##    #print(pos_fixed)
##    #print("random:")
##    #print(pos_random)
##
###print(C)
##print(D)
##
### recover the transformation
##ret_R, ret_t = rigid_transform_3D(C, D)
##
##A2 = (np.dot(ret_R,C.T)) + np.tile(ret_t, (1, amount))
##A2 = A2.T
##
###print(np.tile(ret_t, (1, amount)))
###print(np.tile(ret_t, (1, amount)).shape)
##
### Find the error
### amount delta position
##err = A2 - D
####print(err)
####err = np.multiply(err, err)
####print(err.shape)
####err = np.sum(err, axis=1)
##### amount dis
####err = np.sqrt(err);
####print(err)
##### sd
####err = np.multiply(err, err)
####err = sum(err)/amount
####print(err)
####rmse = np.sqrt(err);
##err = np.multiply(err, err)
##err = np.sum(err)
##rmse = np.sqrt(err/amount);
##
##print ("Points C")
##print (C)
##print ("")
##
##print ("Points D")
##print (D)
##print ("")
##
##print ("Rotation")
##print (ret_R)
##print ("")
##
##print ("Translation")
##print (ret_t)
##print ("")
##
##print ("RMSE:", rmse)
##print ("If RMSE is near zero, the function is correct!")
##
##ax.scatter(C[:,0],C[:,1],C[:,2], zdir='z', c= 'red')
##ax.scatter(D[:,0],D[:,1],D[:,2], zdir='z', c= 'blue')
##plt.show()
