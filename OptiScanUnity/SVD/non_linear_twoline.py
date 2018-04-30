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
#bx.set_ylim3d(2,-2)
#bx.set_zlim3d(-2,2)
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
#dx.set_ylim3d(2,-2)
#dx.set_zlim3d(-2, 2)
dx.set_xlabel('X Label' )
dx.set_ylabel('Y Label')
dx.set_zlabel('Z Label')
#cx = fig.add_subplot(1,3,3, projection='3d')

# generate offset matrix based on euler angles
def generate_offset_euler(twod = False):
    pitchx = np.random.randint(-10,10)* math.pi/180
    rolly = np.random.randint(-10,10)* math.pi/180
    yawz = np.random.randint(-10,10)* math.pi/180

    #test 2d
    if twod:
        rolly = 0
        yawz = 0
    
    R = euler2mat(yawz, rolly, pitchx,'szyx')
    t = np.array([np.random.randint(-10,10)/10,
                  np.random.randint(-10,10)/10,
                  np.random.randint(-10,10)/10]).reshape(3,1)

    #test 2d
    if twod:
        t[0][0] = 0
    
    #print(R)
    #print(t)
    #print(zeroone.shape)
    t14 = np.concatenate((t, np.array([1]).reshape(1,1) ))
    #print(t14)
    R34 = np.concatenate((R, np.array([0,0,0]).reshape(1,3) ))
    #print(R34)
    return np.concatenate((R34,t14 ), axis=1)

# get one pair of headset mtx and point position
def prepare1(Moffset, Poverlap, twod = False):   # we need to fix the Moffset each time
    pitchx = np.random.randint(-90,90)* math.pi/180
    rolly = np.random.randint(-90,90)* math.pi/180
    yawz = np.random.randint(-90,90)* math.pi/180
    # test 2d first
    if twod:
        rolly = 0
        yawz = 0
    
    Theadset = np.random.rand(3,1) * 1.5
    # test 2d first
    if twod:
        Theadset[0][0] = 0
        #Theadset[2][0] = 0

    #print(Theadset)
    Mheadset = np.concatenate(
        (np.concatenate(
            (euler2mat( yawz, rolly,pitchx, 'szyx'),Theadset),axis=1
            ),np.array([0,0,0,1]).reshape(1,4))
        )
    #print(Mheadset)
    Meye = np.matmul(Mheadset, Moffset)
    Pscale_overlap = np.append(Poverlap[0:3,:] * np.random.rand(1,1) * 1.5,1).reshape(4,1)
    #print("check")
    #print(Poverlap)
    #print(Pscale_overlap)
    Pobj = np.matmul(Meye,  Pscale_overlap)

    #print("Meye[:,3]" + str(Meye[:,3].reshape(4,1)) + "\nPobj:\n" + str(Pobj))
    Peye = np.matmul(LA.inv(Meye), Pobj)

    ax.scatter(Mheadset[:,3][0],Mheadset[:,3][1],Mheadset[:,3][2], zdir='z', c= 'purple')
    ax.scatter(Meye[:,3][0],Meye[:,3][1],Meye[:,3][2], zdir='z', c= 'red')
    ax.scatter(Pobj[0],Pobj[1],Pobj[2], zdir='z', c= 'green')
    Mplot = np.squeeze(np.asarray(np.concatenate((Meye[:,3].reshape(4,1),Pobj),axis=1)))
    ax.plot(Mplot[0],Mplot[1],Mplot[2],c='y')
    Mplot = np.squeeze(np.asarray(np.concatenate((Meye[:,3].reshape(4,1),Mheadset[:,3].reshape(4,1)),axis=1)))
    ax.plot(Mplot[0],Mplot[1],Mplot[2],c='y')

    cx.scatter(Peye[0],Peye[1],Peye[2], zdir='z', c= 'green')
    
    #plt.show()

    return Mheadset, Pobj

# (m * pi) / ||m * pi|| - (po / ||po||)
#(m * pi) / ||m * pi||
def func(m, po, pi):
    mpi = np.matmul(pi, m.reshape(12,1))
    mpi0 = mpi[:,0:3,:]
    mpi1 = mpi[:,3:6,:]
    
    delta = np.concatenate( (mpi0 / LA.norm(mpi0),mpi1 / LA.norm(mpi1)), axis=1)# - po
    #delta = mpi / LA.norm(mpi) - po
    #print(mpi.shape)
    #print(po.shape)
    return delta.flatten()

def validate(Moff, Poverlap, Pheadset312):
    print("---validate---")
    mpi = np.matmul(Pheadset312, Moff.reshape(12,1))
    # = np.matmul(LA.inv(Moff), Pheadset)[0:3,:]
    #print("mpi:" + str(mpi))
    # do normalization for each three lines
    print(Poverlap)
    mpi0 = mpi[0:3,:]
    mpi1 = mpi[3:6,:]
    print("mpi:" + str(mpi0.flatten()) + "\t" + str(mpi1.flatten()))
    delta = np.concatenate( (mpi0 / LA.norm(mpi0),mpi1 / LA.norm(mpi1)), axis=0) - Poverlap
    print("delta:" + str(delta.flatten()))

def nllsm(Poverlaps, Pheadsets, med):
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

    #print("Poverlaps[0] in nllsm:\n" + str(Poverlaps[0]))
    #print("Pheadsets[0] in nllsm:\n" + str(Pheadsets[0]))
    validate(m, Poverlaps[2], Pheadsets[2])
    m = np.array([1.0, 0.0, 0.0,
                 0.0, 1.0, 0.0,
                 0.0, 0.0, 1.0,
                 0.0, 0.0, 0.0])
    
    #print(correctMoffinv)
    #print("Poverlaps:" + str(Poverlaps))
    #print("Pheadsets.shape" + str(Pheadsets.shape))
    if med != "lm":
        res_lsq = least_squares(func, m, args=(Poverlaps, Pheadsets), verbose = 1,
                                bounds=([-1,-1,-1,-1,
                                         -1,-1,-1,-1,
                                         -1,-np.inf,-np.inf,-np.inf],
                                        [1,1,1,1,
                                         1,1,1,1,
                                         1,np.inf,np.inf,np.inf]))
    else:
        res_lsq = least_squares(func, m, args=(Poverlaps, Pheadsets), method = 'lm', verbose = 1
                                )

    
    #print("res_lsq:" + str(res_lsq))
    moffinv = np.array([[res_lsq.x[0],res_lsq.x[1],res_lsq.x[2],res_lsq.x[9]],
                            [res_lsq.x[3],res_lsq.x[4],res_lsq.x[5],res_lsq.x[10]],
                           [res_lsq.x[6],res_lsq.x[7],res_lsq.x[8],res_lsq.x[11]],
                        [0,0,0,1]])
    #print("minv-12:" + str(res_lsq.x))
    #print("minv-34:" + str(moffinv.shape))
    #print(moffinv)
    #moff = LA.inv(np.concatenate(( noffinv, np.array([0,0,0,1]) )) )
    #print("calculated offset:" + str(moff))
    return moffinv

# prepare one predefined point series
def prepare_one_line (amount, Moffset):
    Poverlap31 = np.random.rand(3,1).reshape(3,1)
    Poverlap31[0][0] = 0
    Poverlap31 = Poverlap31 / LA.norm(Poverlap31)
    Poverlap = np.concatenate((Poverlap31, np.array([[1]]))) #(4,1)
    print("Poverlap:" + str(Poverlap.flatten()))

    # prepare the return vars
    Pobjs = np.zeros((amount,4,1),dtype=np.float64)
    Poverlaps = np.zeros((amount,3,1),dtype=np.float64)
    Pheadsets312 = np.zeros((amount,3,12),dtype=np.float64)
    Pheadsets = np.zeros((amount,4,1),dtype=np.float64)
    
    for i in range(0, amount):
        #Mheadset, Pobj, Poverlap = prepare0(Moffset)
        Mheadset, Pobj = prepare1(Moffset, Poverlap, True)
        #print(Mheadset)
        #print("pobj:" + str(Pobj))
        Pobjs[i] = Pobj
        Pheadsets[i] = np.matmul(LA.inv(Mheadset), Pobj)
        Pheadsets312[i] = np.array([[ Pheadsets[i][0][0], Pheadsets[i][1][0], Pheadsets[i][2][0], 0,0,0, 0,0,0, 1,0,0],
                                    [ 0,0,0, Pheadsets[i][0][0], Pheadsets[i][1][0], Pheadsets[i][2][0], 0,0,0, 0,1,0],
                                    [ 0,0,0, 0,0,0, Pheadsets[i][0][0], Pheadsets[i][1][0], Pheadsets[i][2][0], 0,0,1]])
        Poverlaps[i] = Poverlap31

    #print (Poverlaps.shape)
    #print (Pheadsets312)
    return Poverlaps, Pheadsets312, Pheadsets
        

# initialize the var
amount = 20 #20 + 4

# randomize the offset
Moffset = generate_offset_euler(True)

print("offset:" + str(Moffset))
print("offset inv:" + str(LA.inv(Moffset)))
#print("Poverlap:" + str(Poverlap))

Poverlaps1, Pheadsets3121, Pheadsets1 = prepare_one_line (amount, Moffset)
Poverlaps2, Pheadsets3122, Pheadsets2 = prepare_one_line (amount, Moffset)

Poverlaps = np.concatenate((Poverlaps1, Poverlaps2), axis = 1)
Pheadsets312 = np.concatenate((Pheadsets3121, Pheadsets3122), axis = 1)
Pheadsets = np.concatenate((Pheadsets1, Pheadsets2), axis = 1)

print(Poverlaps.shape)
print(Pheadsets312.shape)
print(Pheadsets.shape)
    
#Poverlaps = np.tile(Poverlap[0:3,:], (amount,1,1)) #(20,3,12)
#print(Poverlaps.shape)
print("---lm---")
Moffinv44 = nllsm(Poverlaps, Pheadsets312, "lm")
print("Moffinv44:" + str(Moffinv44))
Moff44 = LA.inv(Moffinv44)
print("Moff44:" + str(Moff44))

#validate Pcameras=M * Pheadset
#calculate the Toffset
Toffset = np.zeros((4,1),dtype=np.float64)
for i in range(0, amount):
    #show original Pcameras to bx
    Pcmr_ori1 = np.matmul(LA.inv(Moffset), Pheadsets1[i])
    Pcmr_ori2 = np.matmul(LA.inv(Moffset), Pheadsets2[i])
    cx.scatter(Pcmr_ori1[0],Pcmr_ori1[1],Pcmr_ori1[2], zdir='z', c= 'green')
    cx.scatter(Pcmr_ori2[0],Pcmr_ori2[1],Pcmr_ori2[2], zdir='z', c= 'green')
    
    cx.scatter(Pheadsets[i][0][0],Pheadsets[i][1][0],Pheadsets[i][2][0], zdir='z', c= 'blue')
    cx.scatter(Pheadsets[i][4][0],Pheadsets[i][5][0],Pheadsets[i][6][0], zdir='z', c= 'blue')
    
for i in range(0, amount):
    Pcmr_cal1 = np.matmul(Moffinv44, Pheadsets1[i])# + Toffset
    Pcmr_cal2 = np.matmul(Moffinv44, Pheadsets2[i])

    bx.scatter(Pcmr_cal1[0],Pcmr_cal1[1],Pcmr_cal1[2], zdir='z', c= 'red')
    bx.scatter(Pcmr_cal2[0],Pcmr_cal2[1],Pcmr_cal2[2], zdir='z', c= 'pink')
    #bx.scatter(Pcmr_ori[0],Pcmr_ori[1],Pcmr_ori[2], zdir='z', c= 'green')
    Mplot = np.squeeze(np.asarray(np.concatenate((Pcmr_ori1,Pcmr_cal1),axis=1)))
    #bx.plot([Pcmr_ori1[0],Pcmr_cal1[0]],Mplot[1],Mplot[2],c='y')
    Mplot = np.squeeze(np.asarray(np.concatenate((Pcmr_ori2,Pcmr_cal2),axis=1)))
    #bx.plot([Pcmr_ori2[0],Pcmr_cal2[0]],Mplot[1],Mplot[2],c='y')

print("---default---")
Moffinv44 = nllsm(Poverlaps, Pheadsets312, "other")
print("Moffinv44:" + str(Moffinv44))
Moff44 = LA.inv(Moffinv44)
print("Moff44:" + str(Moff44))

for i in range(0, amount):
    Pcmr_cal1 = np.matmul(Moffinv44, Pheadsets1[i])# + Toffset
    Pcmr_cal2 = np.matmul(Moffinv44, Pheadsets2[i])# + Toffset

    dx.scatter(Pcmr_cal1[0],Pcmr_cal1[1],Pcmr_cal1[2], zdir='z', c= 'red')
    dx.scatter(Pcmr_cal2[0],Pcmr_cal2[1],Pcmr_cal2[2], zdir='z', c= 'pink')
    #dx.scatter(Pcmr_ori[0],Pcmr_ori[1],Pcmr_ori[2], zdir='z', c= 'green')
    Mplot = np.squeeze(np.asarray(np.concatenate((Pcmr_ori1,Pcmr_cal1),axis=1)))
    #dx.plot([Pcmr_ori1[0],Pcmr_cal1[0]],Mplot[1],Mplot[2],c='y')
    Mplot = np.squeeze(np.asarray(np.concatenate((Pcmr_ori2,Pcmr_cal2),axis=1)))
    #dx.plot([Pcmr_ori2[0],Pcmr_cal2[0]],Mplot[1],Mplot[2],c='y')
    
plt.show()
