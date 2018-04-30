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
    
def setup_figure(boundforresult = False):
    ax.set_xlim3d(-2, 2)
    ax.set_ylim3d(2,-2)
    ax.set_zlim3d(-2, 2)
    ax.set_xlabel('X Label' )
    ax.set_ylabel('Y Label')
    ax.set_zlabel('Z Label')
    bx.set_xlim3d(-2, 2)
    if boundforresult:
        bx.set_ylim3d(2,-2)
        bx.set_zlim3d(-2,2)
    bx.set_xlabel('X Label)' )
    bx.set_ylabel('Y Label')
    bx.set_zlabel('Z Label')
    cx.set_xlim3d(-2, 2)
    if boundforresult:
        cx.set_ylim3d(2,-2)
        cx.set_zlim3d(-2, 2)
    cx.set_xlabel('X Label' )
    cx.set_ylabel('Y Label')
    cx.set_zlabel('Z Label')
    dx.set_xlim3d(-2, 2)
    if boundforresult:
        dx.set_ylim3d(2,-2)
        dx.set_zlim3d(-2, 2)
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

    return Mheadset, Pobj, Peye

# prepare one predefined point series
def prepare_one_line (amount, Moffset):
    Poverlap31 = np.random.rand(3,1).reshape(3,1)
    Poverlap31[0][0] = 0
    Poverlap31 = Poverlap31 / LA.norm(Poverlap31)
    Poverlap = np.concatenate((Poverlap31, np.array([[1]]))) #(4,1)
    print("Poverlap:" + str(Poverlap.flatten()))

    # prepare the return vars
    Peyes = np.zeros((amount,4,1),dtype=np.float64)
    #Poverlaps = np.zeros((amount,3,1),dtype=np.float64)
    #Pheadsets312 = np.zeros((amount,3,12),dtype=np.float64)
    Pheadsets = np.zeros((amount,4,1),dtype=np.float64)
    
    for i in range(0, amount):
        #Mheadset, Pobj, Poverlap = prepare0(Moffset)
        Mheadset, Pobj, Peye = prepare1(Moffset, Poverlap, True)
        #print(Mheadset)
        #print("pobj:" + str(Pobj))
        Peyes[i] = Peye
        Pheadsets[i] = np.matmul(LA.inv(Mheadset), Pobj)
        #Pheadsets312[i] = np.array([[ Pheadsets[i][0][0], Pheadsets[i][1][0], Pheadsets[i][2][0], 0,0,0, 0,0,0, 1,0,0],
                                    #[ 0,0,0, Pheadsets[i][0][0], Pheadsets[i][1][0], Pheadsets[i][2][0], 0,0,0, 0,1,0],
                                    #[ 0,0,0, 0,0,0, Pheadsets[i][0][0], Pheadsets[i][1][0], Pheadsets[i][2][0], 0,0,1]])
        #Poverlaps[i] = Poverlap31

    #print (Poverlaps.shape)
    #print (Pheadsets312)
    return Poverlap31, Pheadsets, Peyes
        

# initialize the var
amount = 4 #20 + 4

# set up the figure
setup_figure(True)

# randomize the offset
Moffset = generate_offset_euler(True)

print("offset:" + str(Moffset))
print("offset inv:" + str(LA.inv(Moffset)))
#print("Poverlap:" + str(Poverlap))

# Let's try two lines at the beginning and it should be many lines later

line_amount = 1
Dirs = np.zeros((line_amount,3,1),dtype=np.float64)
Pheadsetss = np.zeros((line_amount, amount,4,1),dtype=np.float64)
Peyess = np.zeros((line_amount, amount,4,1),dtype=np.float64)
for i in range(0, line_amount):
    Dirs[i], Pheadsetss[i], Peyess[i] = prepare_one_line (amount, Moffset)
    # solve each line equation with svd
    Pcentroid = np.mean(Pheadsetss[i], axis=0)[0:3,:]
    Ph_minus_c = (Pheadsetss[i][:,0:3,:]-Pcentroid)
    print(Ph_minus_c)
    Psigma =  np.zeros((amount,3,3),dtype=np.float64)
    for j in range(0, amount):
        Psigma[j] = np.matmul(Ph_minus_c[j],Ph_minus_c[j].T)
    C = np.sum(Psigma, axis=0)
    #print(Pheadsetss[i])
    #print(Pcentroid)
    print(C)
    U, S, Vt = LA.svd(C)
    # let us form the line, use x=0, 0.5, 1, 1.5 to calculate y
    # totally does not work, sigh, try the chinese paper
    points = np.zeros((2,3,1), dtype=np.float64)
    points[0] = np.matmul(U, np.array([[0],[-2],[-2]])) + Pcentroid
    points[1] = np.matmul(U, np.array([[0],[2],[2]])) + Pcentroid
    
    print("U" + str(U))
    print("Vt" + str(Vt))
    print(points)
    cx.scatter(Pcentroid[0],Pcentroid[1],Pcentroid[2], zdir='z', c= 'purple')
    cx.plot([points[0][0][0],points[1][0][0]],[points[0][1][0],points[1][1][0]],
            [points[0][2][0],points[1][2][0]],c='red')

#print(Dirs)
#print(Pheadsetss)
#Poverlaps2, Pheadsets3122, Pheadsets2 = prepare_one_line (amount, Moffset)

##Poverlaps = np.concatenate((Poverlaps1, Poverlaps2), axis = 1)
##Pheadsets312 = np.concatenate((Pheadsets3121, Pheadsets3122), axis = 1)
##Pheadsets = np.concatenate((Pheadsets1, Pheadsets2), axis = 1)
##
##print(Poverlaps.shape)
##print(Pheadsets312.shape)
##print(Pheadsets.shape)
    
#Poverlaps = np.tile(Poverlap[0:3,:], (amount,1,1)) #(20,3,12)
#print(Poverlaps.shape)
##print("---lm---")
##Moffinv44 = nllsm(Poverlaps, Pheadsets312, "lm")
##print("Moffinv44:" + str(Moffinv44))
##Moff44 = LA.inv(Moffinv44)
##print("Moff44:" + str(Moff44))

#validate Pcameras=M * Pheadset
#calculate the Toffset
#Toffset = np.zeros((4,1),dtype=np.float64)
for line_i in range(0, line_amount):
    for i in range(0, amount):
        #show original Pcameras to bx
        #Pcmr_ori1 = np.matmul(LA.inv(Moffset), Pheadsets1[i])
        #Pcmr_ori2 = np.matmul(LA.inv(Moffset), Pheadsets2[i])
        cx.scatter(Pheadsetss[line_i][i][0],Pheadsetss[line_i][i][1],Pheadsetss[line_i][i][2], zdir='z', c= 'blue')
        cx.scatter(Peyess[line_i][i][0],Peyess[line_i][i][1],Peyess[line_i][i][2], zdir='z', c= 'green')
        
        #cx.scatter(Pheadsets[i][0][0],Pheadsets[i][1][0],Pheadsets[i][2][0], zdir='z', c= 'blue')
        #cx.scatter(Pheadsets[i][4][0],Pheadsets[i][5][0],Pheadsets[i][6][0], zdir='z', c= 'blue')
    cx.plot([0,Dirs[line_i][0]],[0,Dirs[line_i][1]], [0,Dirs[line_i][2]],c='y')
    
##for i in range(0, amount):
##    Pcmr_cal1 = np.matmul(Moffinv44, Pheadsets1[i])# + Toffset
##    Pcmr_cal2 = np.matmul(Moffinv44, Pheadsets2[i])
##
##    bx.scatter(Pcmr_cal1[0],Pcmr_cal1[1],Pcmr_cal1[2], zdir='z', c= 'red')
##    bx.scatter(Pcmr_cal2[0],Pcmr_cal2[1],Pcmr_cal2[2], zdir='z', c= 'pink')
##    #bx.scatter(Pcmr_ori[0],Pcmr_ori[1],Pcmr_ori[2], zdir='z', c= 'green')
##    Mplot = np.squeeze(np.asarray(np.concatenate((Pcmr_ori1,Pcmr_cal1),axis=1)))
##    #bx.plot([Pcmr_ori1[0],Pcmr_cal1[0]],Mplot[1],Mplot[2],c='y')
##    Mplot = np.squeeze(np.asarray(np.concatenate((Pcmr_ori2,Pcmr_cal2),axis=1)))
##    #bx.plot([Pcmr_ori2[0],Pcmr_cal2[0]],Mplot[1],Mplot[2],c='y')
##
##print("---default---")
##Moffinv44 = nllsm(Poverlaps, Pheadsets312, "other")
##print("Moffinv44:" + str(Moffinv44))
##Moff44 = LA.inv(Moffinv44)
##print("Moff44:" + str(Moff44))
##
##for i in range(0, amount):
##    Pcmr_cal1 = np.matmul(Moffinv44, Pheadsets1[i])# + Toffset
##    Pcmr_cal2 = np.matmul(Moffinv44, Pheadsets2[i])# + Toffset
##
##    dx.scatter(Pcmr_cal1[0],Pcmr_cal1[1],Pcmr_cal1[2], zdir='z', c= 'red')
##    dx.scatter(Pcmr_cal2[0],Pcmr_cal2[1],Pcmr_cal2[2], zdir='z', c= 'pink')
##    #dx.scatter(Pcmr_ori[0],Pcmr_ori[1],Pcmr_ori[2], zdir='z', c= 'green')
##    Mplot = np.squeeze(np.asarray(np.concatenate((Pcmr_ori1,Pcmr_cal1),axis=1)))
##    #dx.plot([Pcmr_ori1[0],Pcmr_cal1[0]],Mplot[1],Mplot[2],c='y')
##    Mplot = np.squeeze(np.asarray(np.concatenate((Pcmr_ori2,Pcmr_cal2),axis=1)))
##    #dx.plot([Pcmr_ori2[0],Pcmr_cal2[0]],Mplot[1],Mplot[2],c='y')
    
plt.show()
