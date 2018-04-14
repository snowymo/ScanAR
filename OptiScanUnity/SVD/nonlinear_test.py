import numpy as np
from math import sqrt
import random
import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D
from numpy import linalg as LA
import math as math
from scipy.optimize import least_squares
from transforms3d.euler import euler2mat, mat2euler

# figure setup
fig = plt.figure(1)
ax = fig.add_subplot(2,2,1, projection='3d')
bx = fig.add_subplot(2,2,2, projection='3d')
cx = fig.add_subplot(2,2,3, projection='3d')
dx = fig.add_subplot(2,2,4, projection='3d')
ax.set_xlim3d(-2, 2)
ax.set_ylim3d(2,-2)
ax.set_zlim3d(-2, 2)
ax.set_xlabel('X Label' )
ax.set_ylabel('Y Label')
ax.set_zlabel('Z Label')
bx.set_xlim3d(-2, 2)
bx.set_ylim3d(2,-2)
bx.set_zlim3d(-2,2)
bx.set_xlabel('X Label)' )
bx.set_ylabel('Y Label')
bx.set_zlabel('Z Label')
cx.set_xlim3d(-2, 2)
cx.set_ylim3d(2,-2)
cx.set_zlim3d(-2, 2)
cx.set_xlabel('X Label' )
cx.set_ylabel('Y Label')
cx.set_zlabel('Z Label')
dx.set_xlim3d(-2, 2)
dx.set_ylim3d(2,-2)
dx.set_zlim3d(-2, 2)
dx.set_xlabel('X Label' )
dx.set_ylabel('Y Label')
dx.set_zlabel('Z Label')
#cx = fig.add_subplot(1,3,3, projection='3d')

# generate offset matrix based on euler angles
def generate_offset_euler():
    pitchx = np.random.randint(-10,10)* math.pi/180
    rolly = np.random.randint(-10,10)* math.pi/180
    yawz = np.random.randint(-10,10)* math.pi/180

    #test 2d
    rolly = 0
    yawz = 0
    
    R = euler2mat(yawz, rolly, pitchx,'szyx')
    t = np.array([np.random.randint(-10,10)/10,
                  np.random.randint(-10,10)/10,
                  np.random.randint(-10,10)/10]).reshape(3,1)

    #test 2d
    t[0][0] = 0
    t[2][0] = 0
    
    #print(R)
    #print(t)
    #print(zeroone.shape)
    t14 = np.concatenate((t, np.array([1]).reshape(1,1) ))
    print(t14)
    R34 = np.concatenate((R, np.array([0,0,0]).reshape(1,3) ))
    print(R34)
    return np.concatenate((R34,t14 ), axis=1)

# get one pair of headset mtx and point position
def prepare0(Moffset):   # we need to fix the Moffset each time
    pitchx = np.random.randint(-90,90)* math.pi/180
    rolly = np.random.randint(-90,90)* math.pi/180
    yawz = np.random.randint(-90,90)* math.pi/180
    # test 2d first
    rolly = 0
    yawz = 0
    
    Theadset = np.random.rand(3,1) * 1.5
    # test 2d first
    Theadset[0][0] = 0
    Theadset[2][0] = 0

    print(Theadset)
    
    Mheadset = np.concatenate(
        (np.concatenate(
            (euler2mat( yawz, rolly,pitchx, 'szyx'),Theadset),axis=1
            ),np.array([0,0,0,1]).reshape(1,4))
        )
    print(Mheadset)
    Meye = np.matmul(Mheadset, Moffset)

    Poverlap31 = np.random.rand(3,1)
    Poverlap31 = Poverlap31 / LA.norm(Poverlap31)
    Poverlap = np.concatenate((Poverlap31, np.array([[1]]))) #(4,1)
    Pscale_overlap = np.append(Poverlap[0:3,:] * np.random.rand(1,1) * 1.5,1).reshape(4,1)
    
    Pobj = np.matmul(Meye,Pscale_overlap)
    Peye = np.matmul(LA.inv(Meye), Pobj)
    #print("Meye[:,3]" + str(Meye[:,3].reshape(4,1)) + "\nPobj:\n" + str(Pobj))

    ax.scatter(Meye[:,3][0],Meye[:,3][1],Meye[:,3][2], zdir='z', c= 'red')
    ax.scatter(Pobj[0],Pobj[1],Pobj[2], zdir='z', c= 'blue')
    cx.scatter(Peye[0],Peye[1],Peye[2], zdir='z', c= 'green')
    Mplot = np.squeeze(np.asarray(np.concatenate((Meye[:,3].reshape(4,1),Pobj),axis=1)))
    ax.plot(Mplot[0],Mplot[1],Mplot[2],c='y')
    #plt.show()

    return Mheadset, Pobj, Poverlap

# get one pair of headset mtx and point position
def prepare(Moffset, Poverlap):   # we need to fix the Moffset each time
    pitchx = np.random.randint(-90,90)* math.pi/180
    rolly = np.random.randint(-90,90)* math.pi/180
    yawz = np.random.randint(-90,90)* math.pi/180
    # test 2d first
    rolly = 0
    yawz = 0
    
    Theadset = np.random.rand(3,1) * 1.5
    # test 2d first
    Theadset[0][0] = 0
    Theadset[2][0] = 0

    print(Theadset)
    Mheadset = np.concatenate(
        (np.concatenate(
            (euler2mat( yawz, rolly,pitchx, 'szyx'),Theadset),axis=1
            ),np.array([0,0,0,1]).reshape(1,4))
        )
    print(Mheadset)
    Meye = np.matmul(Mheadset, Moffset)
    Pscale_overlap = np.append(Poverlap[0:3,:] * np.random.rand(1,1) * 1.5,1).reshape(4,1)
    #print("check")
    #print(Poverlap)
    #print(Pscale_overlap.shape)
    Pobj = np.matmul(Meye,  Pscale_overlap)

    #print("Meye[:,3]" + str(Meye[:,3].reshape(4,1)) + "\nPobj:\n" + str(Pobj))
    Peye = np.matmul(LA.inv(Meye), Pobj)

    ax.scatter(Meye[:,3][0],Meye[:,3][1],Meye[:,3][2], zdir='z', c= 'red')
    ax.scatter(Pobj[0],Pobj[1],Pobj[2], zdir='z', c= 'blue')
    cx.scatter(Peye[0],Peye[1],Peye[2], zdir='z', c= 'green')
    Mplot = np.squeeze(np.asarray(np.concatenate((Meye[:,3].reshape(4,1),Pobj),axis=1)))
    ax.plot(Mplot[0],Mplot[1],Mplot[2],c='y')
    
    #plt.show()

    return Mheadset, Pobj

# get one pair of headset mtx and point position based on iteration
def prepare(Moffset, Poverlap, i):   # we need to fix the Moffset each time
    pitchx = np.random.randint(-90,90)* math.pi/180
    rolly = np.random.randint(-90,90)* math.pi/180
    yawz = np.random.randint(-90,90)* math.pi/180
    # test 2d first
    rolly = 0
    yawz = 0
    
    #print(Theadset)
    Theadset = np.random.rand(3,1) * 1.5
    # test 2d first
    Theadset[0][0] = 0
    Theadset[2][0] = 0
    Mheadset = np.concatenate(
        (np.concatenate(
            (euler2mat( yawz, rolly, pitchx,'szyx'),Theadset),axis=1
            ),np.array([0,0,0,1]).reshape(1,4))
        )
    testHeadset = np.matmul(Mheadset, np.array([0,0,0,1]).reshape(4,1))
    print("testHeadset:" + str(testHeadset))
    print("Mheadset:" +str(Mheadset))
    Meye = np.matmul(Mheadset, Moffset)
    print("Meye:" +str(Meye))
    testEye = np.matmul(Meye, np.array([0,0,0,1]).reshape(4,1))
    print("testEye:" + str(testEye))
    Pscale_overlap = np.append(Poverlap[0:3,:] * i,1).reshape(4,1)
    #print("check")
    #print(Poverlap)
    #print(Pscale_overlap)
    Pobj = np.matmul(Meye,  Pscale_overlap)

    #print("Meye[:,3]" + str(Meye[:,3].reshape(4,1)) + "\nPobj:\n" + str(Pobj))
    Peye = np.matmul(LA.inv(Meye), Pobj)

    ax.scatter(Meye[:,3][0],Meye[:,3][1],Meye[:,3][2], zdir='z', c= 'red')
    ax.scatter(Pobj[0],Pobj[1],Pobj[2], zdir='z', c= 'blue')
    cx.scatter(Peye[0],Peye[1],Peye[2], zdir='z', c= 'green')
    Mplot = np.squeeze(np.asarray(np.concatenate((Meye[:,3].reshape(4,1),Pobj),axis=1)))
    ax.plot(Mplot[0],Mplot[1],Mplot[2],c='y')
    
    #plt.show()

    return Mheadset, Pobj

# (m * pi) / ||m * pi|| - (po / ||po||)
def func(m, po, pi):
    mpi = np.matmul(pi, m.reshape(12,1))
    #print(mpi.shape)
    #print(po.shape)
    return (mpi / LA.norm(mpi) - po).flatten()

def nllsm(Poverlaps, Pheadsets):
    m = np.array([1.0, 0.0, 0.0,
                 0.0, 1.0, 0.0,
                 0.0, 0.0, 1.0,
                 0.0, 0.0, 0.0])
    # guess the correct one
    correctMoffinv = LA.inv(Moffset)
    #print(correctMoffinv)
    correctMoffinv = correctMoffinv.reshape(1,16).flatten()
    m = np.array([correctMoffinv[0],correctMoffinv[1],correctMoffinv[2],correctMoffinv[4],
                            correctMoffinv[5],correctMoffinv[6],correctMoffinv[8],correctMoffinv[9],
                           correctMoffinv[10],correctMoffinv[3],correctMoffinv[7],correctMoffinv[11]])
    #print(correctMoffinv)
    #print(m)
    print("Pheadsets.shape" + str(Pheadsets.shape))
    res_lsq = least_squares(func, m, args=(Poverlaps, Pheadsets), method = 'lm')
    moffinv = np.array([np.concatenate(    (res_lsq.x[0:3].reshape(1,3),np.array([res_lsq.x[9]]).reshape(1,1)) , axis=1),
                        np.concatenate(    (res_lsq.x[3:6].reshape(1,3),np.array([res_lsq.x[10]]).reshape(1,1)) , axis=1),
                        np.concatenate(    (res_lsq.x[6:9].reshape(1,3),np.array([res_lsq.x[11]]).reshape(1,1)) , axis=1)])
    moffinv = np.array([[res_lsq.x[0],res_lsq.x[1],res_lsq.x[2],res_lsq.x[9]],
                            [res_lsq.x[3],res_lsq.x[4],res_lsq.x[5],res_lsq.x[10]],
                           [res_lsq.x[6],res_lsq.x[7],res_lsq.x[8],res_lsq.x[11]]])
    #print("minv-12:" + str(res_lsq.x))
    #print("minv-34:" + str(moffinv.shape))
    #print(moffinv)
    return moffinv



amount = 20 #20 + 4
Mheadsets = np.zeros((amount,4,4),dtype=np.float64)
Pobjs = np.zeros((amount,4,1),dtype=np.float64)
Pheadsets = np.zeros((amount,4,1),dtype=np.float64)
Pheadsets312 = np.zeros((amount,3,12),dtype=np.float64)
Poverlaps = np.zeros((amount,3,12),dtype=np.float64)
#print("P objs:\n" +str(Pobjs))

#Moffset = np.concatenate((np.random.rand(3,4),np.array([0,0,0,1]).reshape(1,4)))
Moffset = generate_offset_euler()
Poverlap31 = np.random.rand(3,1)
# fix overlap points for testing
Poverlap31 = np.array([0,1,-1]).reshape(3,1)

Poverlap31 = Poverlap31 / LA.norm(Poverlap31)
Poverlap = np.concatenate((Poverlap31, np.array([[1]]))) #(4,1)
#Poverlap312 = np.array([[ Poverlap[0][0], Poverlap[1][0], Poverlap[2][0], 0,0,0, 0,0,0, 1,0,0],
#                        [ 0,0,0, Poverlap[0][0], Poverlap[1][0], Poverlap[2][0], 0,0,0, 0,1,0],
#                        [ 0,0,0, 0,0,0, Poverlap[0][0], Poverlap[1][0], Poverlap[2][0], 0,0,1]])

print("offset:" + str(Moffset))
print("Poverlap:" + str(Poverlap))

for i in range(0, amount):
    #Mheadset, Pobj, Poverlap = prepare0(Moffset)
    Mheadset, Pobj = prepare(Moffset, Poverlap, 0.5+i/20)
    Mheadsets[i] = Mheadset
    #print("pobj:" + str(Pobj))
    Pobjs[i] = Pobj
    Pheadsets[i] = np.matmul(LA.inv(Mheadset), Pobj)
    Pheadsets312[i] = np.array([[ Pheadsets[i][0][0], Pheadsets[i][1][0], Pheadsets[i][2][0], 0,0,0, 0,0,0, 1,0,0],
                                [ 0,0,0, Pheadsets[i][0][0], Pheadsets[i][1][0], Pheadsets[i][2][0], 0,0,0, 0,1,0],
                                [ 0,0,0, 0,0,0, Pheadsets[i][0][0], Pheadsets[i][1][0], Pheadsets[i][2][0], 0,0,1]])
    Poverlaps[i] = np.array([[ Poverlap[0][0], Poverlap[1][0], Poverlap[2][0], 0,0,0, 0,0,0, 1,0,0],
                             [ 0,0,0, Poverlap[0][0], Poverlap[1][0], Poverlap[2][0], 0,0,0, 0,1,0],
                             [ 0,0,0, 0,0,0, Poverlap[0][0], Poverlap[1][0], Poverlap[2][0], 0,0,1]])
    # test func
    #print("Moffset[0:3,:] " + str(Moffset[0:3,:]))
    MoffsetInv = LA.inv(Moffset)
    MoffsetInv12 = np.append(MoffsetInv[0,0:3], [MoffsetInv[1,0:3], MoffsetInv[2,0:3], MoffsetInv[0:3,3]]).reshape(12,1)
    #print("Pheadsets312 " + str(Pheadsets312[i]))
    #print("MoffsetInv12 " + str(MoffsetInv12))
    
    validatePcmr = np.matmul(Pheadsets312[i], MoffsetInv12)
    dx.scatter(validatePcmr[0],validatePcmr[1],validatePcmr[2], zdir='z', c= 'blue')
    validatePcmr2 = np.matmul(MoffsetInv, Pheadsets[i])
    cx.scatter(validatePcmr2[0],validatePcmr2[1],validatePcmr2[2], zdir='z', c= 'purple')
    #print("validatePcmr:" + str(validatePcmr))
    #print("---")
    #print("Pheadsets " + str(Pheadsets[i]))
    #print("MoffsetInv " + str(MoffsetInv))
    #print("validatePcmr2:" + str(validatePcmr2))
    

#Poverlaps = np.tile(Poverlap[0:3,:], (amount,1,1)) #(20,3,12)
#print(Poverlaps.shape)
Moffinv34 = nllsm(Poverlaps, Pheadsets312)
Moffinv44 = np.concatenate((Moffinv34, np.array([0,0,0,1]).reshape(1,4)))
print(Moffinv44)

#validate Pcameras=M * Pheadset
for i in range(0, amount):
    #show original Pcameras to bx
    Pcmr_ori = np.matmul(LA.inv(Moffset), Pheadsets[i])
    bx.scatter(Pcmr_ori[0],Pcmr_ori[1],Pcmr_ori[2], zdir='z', c= 'green')

    Pcmr_cal = np.matmul(Moffinv44, Pheadsets[i])
    bx.scatter(Pcmr_cal[0],Pcmr_cal[1],Pcmr_cal[2], zdir='z', c= 'red')
    Mplot = np.squeeze(np.asarray(np.concatenate((Pcmr_ori,Pcmr_cal),axis=1)))
    bx.plot(Mplot[0],Mplot[1],Mplot[2],c='y')
    
plt.show()
