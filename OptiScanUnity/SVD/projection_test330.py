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

    #print(AA.shape)
    #print(BB.shape)
    #print("seperator")
    
    # dot is matrix multiplication for array
    H = np.dot(np.transpose(AA),BB)

    U, S, Vt = np.linalg.svd(H)

    R = Vt.T * U.T

    # special reflection case
    if np.linalg.det(R) < 0:
       print ("Reflection detected")
       Vt[2,:] *= -1
       R = Vt.T * U.T

    #print(np.dot(-R,centroid_A.T))
    #print(centroid_A.T)
    #print(centroid_B.T)
    t = np.dot(-R,centroid_A.T) + centroid_B.T

    print("t")
    print (t)
    print("R")
    print (R)

    return R, t

# figure setup
fig = plt.figure(1)
ax = fig.add_subplot(1,2,1, projection='3d')
bx = fig.add_subplot(133)


def construct_camera_matrix(cmr):
        g = np.squeeze(np.asarray(- np.matmul(cmr, np.array([0,1,0,0]))))[:3].reshape(1,3)
        t = np.squeeze(np.asarray(np.matmul(cmr, np.array([0,0,-1,0]))))[:3].reshape(1,3)
        Mcmr_w = -g/LA.norm(g)
        Mcmr_u = np.cross(t, Mcmr_w)
        Mcmr_v = np.cross(Mcmr_w, Mcmr_u)
        Mcmr_e = np.squeeze(np.asarray(- np.matmul(cmr, np.array([0,0,0,1]))))[:3].reshape(1,3)
        Mcmr0 = np.concatenate((Mcmr_u, Mcmr_v, Mcmr_w, Mcmr_e), axis=0)
        Mcmr1 = Mcmr0.T
        Mcmr2 = np.concatenate((Mcmr1, np.array([0,0,0,1]).reshape(1,4)), axis=0)
        return Mcmr2

def random_test():
    # Test with random data
    amount = 20

    # generate Meye
    # Random rotation and translation
    R = np.mat(np.random.rand(3,3))
    t = np.mat(np.random.rand(3,1))
    t = np.array([0,0,0]) # eye position

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
    gaze_eye = np.squeeze(np.asarray(np.matmul(Meye,np.array([0,0,-1,0]))))[:3].reshape(1,3)
    up_eye = np.squeeze(np.asarray(np.matmul(Meye,np.array([0,1,0,0]))))[:3].reshape(1,3)
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
    pos_obj = np.concatenate((gaze_eye.T * 5,np.array([1]).reshape(1,1)))
    print(pos_obj)
    mtx =  np.matmul(Mproj, Mcmr)
    screen_pos = np.matmul(mtx, pos_obj)
    screen_pos = screen_pos/screen_pos[3]
    print("screen_pos" + str(screen_pos))


    ax.scatter(p_eye[:,0],p_eye[:,1],p_eye[:,2], zdir='z', c= 'red')
    ax.scatter(pos_obj[0],pos_obj[1],pos_obj[2], zdir='z', c= 'blue')

    bx.scatter(screen_pos[0],screen_pos[1], c= 'green')

    plt.show()

def ken_math_test():
    amount = 4
    screen_pos = np.array([0,0,0,1])
    predefine_pos = np.array([0,0.5,0,1])
    Mheadset = np.array([
        [[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]],
        [[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]],
        [[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]],
        [[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]
        ])
    
    Mleft = np.zeros((amount,3))
    Mright = np.zeros((amount,3))
    for i in range(0, amount):
        Mcmr = construct_camera_matrix(Mheadset[i])
        Mleft[i] = screen_pos[:3].reshape(1,3)
        Mright[i] = np.matmul(Mcmr, predefine_pos)[:3].reshape(1,3)

    R,t = rigid_transform_3D(Mleft, Mright)
    print(R)
    print(t)
    return R,t

#def ken_math_valid(R,t,Mheadset_test,screen_pos,predefine_pos):
##    ken_screen_pos = np.array([])
##    m = np.matmul()
##    ken_screen_pos = np.matmul(Mheadset_test, predefine_pos)
    

def hehe_math_test():
    amount = 4
    screen_pos = np.array([0,0,0,1])
    predefine_pos = np.array([0,0.5,0,1])
    Mheadset = np.array([
        [[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]],
        [[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]],
        [[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]],
        [[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]]
        ])
        
#random_test()
ken_math_test()
