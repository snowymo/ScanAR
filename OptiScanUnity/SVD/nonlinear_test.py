import numpy as np
from math import sqrt
import random
import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D
from numpy import linalg as LA
import math as math
from scipy.optimize import least_squares
from transforms3d.euler import euler2mat, mat2euler

# generate offset matrix based on euler angles
def generate_offset_euler():
    pitchx = np.random.randint(-10,10)* math.pi/180
    rolly = np.random.randint(-10,10)* math.pi/180
    yawz = np.random.randint(-10,10)* math.pi/180
    R = euler2mat(pitchx, yawz, rolly, 'szxy')
    t = np.array([np.random.randint(-10,10)/10,
                  np.random.randint(-10,10)/10,
                  np.random.randint(-10,10)/10]).reshape(3,1)

    #print(R)
    #print(t)
    #print(zeroone.shape)
    t14 = np.concatenate((t, np.array([1]).reshape(1,1) ))
    #print(t41)
    R34 = np.concatenate((R, np.array([0,0,0]).reshape(1,3) ))
    #print(R34)
    return np.concatenate((R34,t14 ), axis=1)

# get one pair of headset mtx and point position
def prepare(Moffset):   # we need to fix the Moffset each time
    pitchx = np.random.randint(-90,90)* math.pi/180
    rolly = np.random.randint(-90,90)* math.pi/180
    yawz = np.random.randint(-90,90)* math.pi/180
    Theadset = np.random.rand(3,1) * 1.5
    Mheadset = np.concatenate(
        (np.concatenate(
            (euler2mat(pitchx, yawz, rolly, 'szxy'),Theadset),axis=1
            ),np.array([0,0,0,1]).reshape(1,4))
        )
    Meye = np.matmul(Mheadset, Moffset)

    Poverlap31 = np.random.rand(3,1)
    Poverlap31 = Poverlap31 / LA.norm(Poverlap31)
    Poverlap = np.concatenate((Poverlap31, np.array([[1]]))) #(4,1)

    Pobj = Meye[:,3].reshape(4,1) + Poverlap * np.random.rand(1,1) * 1.5
    #print("Meye[:,3]" + str(Meye[:,3].reshape(4,1)) + "\nPobj:\n" + str(Pobj))

    ax.scatter(Meye[:,3][0],Meye[:,3][1],Meye[:,3][2], zdir='z', c= 'red')
    ax.scatter(Pobj[0],Pobj[1],Pobj[2], zdir='z', c= 'blue')
    Mplot = np.squeeze(np.asarray(np.concatenate((Meye[:,3].reshape(4,1),Meye[:,3].reshape(4,1)+Poverlap*2),axis=1)))
    ax.plot(Mplot[0],Mplot[1],Mplot[2],c='y')
    #plt.show()

    return Mheadset, Pobj, Poverlap

# get one pair of headset mtx and point position
def prepare(Moffset, Poverlap):   # we need to fix the Moffset each time
    pitchx = np.random.randint(-90,90)* math.pi/180
    rolly = np.random.randint(-90,90)* math.pi/180
    yawz = np.random.randint(-90,90)* math.pi/180
    Theadset = np.random.rand(3,1) * 1.5
    Mheadset = np.concatenate(
        (np.concatenate(
            (euler2mat(pitchx, yawz, rolly, 'szxy'),Theadset),axis=1
            ),np.array([0,0,0,1]).reshape(1,4))
        )
    Meye = np.matmul(Mheadset, Moffset)

    Pobj = np.matmul(Meye,Poverlap * np.random.rand(1,1) * 1.5)
    #print("Meye[:,3]" + str(Meye[:,3].reshape(4,1)) + "\nPobj:\n" + str(Pobj))
    Peye = np.matmul(LA.inv(Meye), Pobj)

    ax.scatter(Meye[:,3][0],Meye[:,3][1],Meye[:,3][2], zdir='z', c= 'red')
    ax.scatter(Pobj[0],Pobj[1],Pobj[2], zdir='z', c= 'blue')
    ax.scatter(Peye[0],Peye[1],Peye[2], zdir='z', c= 'green')
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
    res_lsq = least_squares(func, m, args=(Poverlaps, Pheadsets), method = 'lm')
    
    print(res_lsq.x.reshape(3,4))
    return res_lsq.x.reshape(3,4)

# figure setup
fig = plt.figure(1)
ax = fig.add_subplot(1,2,1, projection='3d')
bx = fig.add_subplot(1,2,2, projection='3d')
ax.set_xlim3d(0, 2)
ax.set_ylim3d(2,0)
ax.set_zlim3d(0,2)
ax.set_xlabel('X Label' )
ax.set_ylabel('Y Label')
ax.set_zlabel('Z Label')
bx.set_xlim3d(0, 2)
bx.set_ylim3d(2,0)
bx.set_zlim3d(0,2)
bx.set_xlabel('X Label)' )
bx.set_ylabel('Y Label')
bx.set_zlabel('Z Label')
#cx = fig.add_subplot(1,3,3, projection='3d')

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
Poverlap31 = Poverlap31 / LA.norm(Poverlap31)
Poverlap = np.concatenate((Poverlap31, np.array([[1]]))) #(4,1)
#Poverlap312 = np.array([[ Poverlap[0][0], Poverlap[1][0], Poverlap[2][0], 0,0,0, 0,0,0, 1,0,0],
#                        [ 0,0,0, Poverlap[0][0], Poverlap[1][0], Poverlap[2][0], 0,0,0, 0,1,0],
#                        [ 0,0,0, 0,0,0, Poverlap[0][0], Poverlap[1][0], Poverlap[2][0], 0,0,1]])

print("offset:" + str(Moffset))
print("Poverlap:" + str(Poverlap))

for i in range(0, amount):
    Mheadset, Pobj = prepare(Moffset, Poverlap)
    Mheadsets[i] = Mheadset
    Pobjs[i] = Pobj
    Pheadsets[i] = np.matmul(LA.inv(Mheadset), Pobj)
    Pheadsets312[i] = np.array([[ Pheadsets[i][0][0], Pheadsets[i][1][0], Pheadsets[i][2][0], 0,0,0, 0,0,0, 1,0,0],
                                [ 0,0,0, Pheadsets[i][0][0], Pheadsets[i][1][0], Pheadsets[i][2][0], 0,0,0, 0,1,0],
                                [ 0,0,0, 0,0,0, Pheadsets[i][0][0], Pheadsets[i][1][0], Pheadsets[i][2][0], 0,0,1]])
    Poverlaps[i] = np.array([[ Poverlap[0][0], Poverlap[1][0], Poverlap[2][0], 0,0,0, 0,0,0, 1,0,0],
                             [ 0,0,0, Poverlap[0][0], Poverlap[1][0], Poverlap[2][0], 0,0,0, 0,1,0],
                             [ 0,0,0, 0,0,0, Poverlap[0][0], Poverlap[1][0], Poverlap[2][0], 0,0,1]])

    

#Poverlaps = np.tile(Poverlap[0:3,:], (amount,1,1)) #(20,3,12)
#print(Poverlaps.shape)
Moffinv34 = nllsm(Poverlaps, Pheadsets312)
Moffinv44 = np.concatenate((Moffinv34, np.array([0,0,0,1]).reshape(1,4)))
print(Moffinv44)

#validate Pcameras=M * Pheadset to cx
for i in range(0, amount):
    #show original Pcameras to bx
    Pcmr_ori = np.matmul(LA.inv(Moffset), Pheadsets[i])
    bx.scatter(Pcmr_ori[0],Pcmr_ori[1],Pcmr_ori[2], zdir='z', c= 'green')

    Pcmr_cal = np.matmul(Moffinv44, Pheadsets[i])
    bx.scatter(Pcmr_cal[0],Pcmr_cal[1],Pcmr_cal[2], zdir='z', c= 'red')
    Mplot = np.squeeze(np.asarray(np.concatenate((Pcmr_ori,Pcmr_cal),axis=1)))
    bx.plot(Mplot[0],Mplot[1],Mplot[2],c='y')
    
plt.show()
