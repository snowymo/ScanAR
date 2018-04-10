import numpy as np
from math import sqrt
import random
import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D
from numpy import linalg as LA
import math as math
import scipy.optimize as optimize

# Input: expects Nx3 matrix of points
# Returns R,t
# R = 3x3 rotation matrix
# t = 3x1 column vector

def rigid_transform_3D(A, B):
    assert len(A) == len(B)

    N = A.shape[0]; # total points

    centroid_A = np.mean(A, axis=0).reshape(1,3)
    centroid_B = np.mean(B, axis=0).reshape(1,3)

    #print(centroid_A.shape)
    #print(centroid_B.shape)
    
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
        t = np.squeeze(np.asarray(np.matmul(cmr, np.array([0,1,0,0]))))[:3].reshape(1,3)
        g = np.squeeze(np.asarray(np.matmul(cmr, np.array([0,0,-1,0]))))[:3].reshape(1,3)
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
    t = np.array([1,1,1]) # eye position

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
    print("\tpos eye:\t" + str(p_eye) + "\n\tgaze:\t" + str(gaze_eye) + "\n\tup:\t" + str(up_eye) + "\np_eye+gaze_eye:\t" + str((p_eye+gaze_eye).shape))
    gazeplot = np.squeeze(np.asarray(np.concatenate((p_eye,p_eye+gaze_eye),axis=0)))
    upplot = np.squeeze(np.asarray(np.concatenate((p_eye,p_eye+up_eye),axis=0)))
    ax.plot(gazeplot[:,0],gazeplot[:,1],gazeplot[:,2],c='y')
    ax.plot(upplot[:,0],upplot[:,1],upplot[:,2],c='b')
    
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
    pos_obj = np.concatenate((gaze_eye.T * 3 + p_eye.T,np.array([1]).reshape(1,1)))
    print(pos_obj)
    mtx =  np.matmul(Mproj, Mcmr)
    screen_pos = np.matmul(mtx, pos_obj)
    screen_pos = screen_pos/screen_pos[3]
    print("screen_pos" + str(screen_pos))


    ax.scatter(p_eye[:,0],p_eye[:,1],p_eye[:,2], zdir='z', c= 'red')
    ax.scatter(pos_obj[0],pos_obj[1],pos_obj[2], zdir='z', c= 'blue')

    bx.scatter(screen_pos[0],screen_pos[1], c= 'green')
    #plt.axis([-4,4,-4,4])
    #plt.show()

def random_test2():
    # Test with random data
    amount = 20

    # generate Meye
    # Random rotation and translation
    R = np.mat(np.random.rand(3,3))
    t = np.mat(np.random.rand(3,1))
    t = np.array([1,1,1]) # eye position

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
    print("\tpos eye:\t" + str(p_eye) + "\n\tgaze:\t" + str(gaze_eye) + "\n\tup:\t" + str(up_eye) + "\np_eye+gaze_eye:\t" + str((p_eye+gaze_eye).shape))
    gazeplot = np.squeeze(np.asarray(np.concatenate((p_eye,p_eye+gaze_eye),axis=0)))
    upplot = np.squeeze(np.asarray(np.concatenate((p_eye,p_eye+up_eye),axis=0)))
    ax.plot(gazeplot[:,0],gazeplot[:,1],gazeplot[:,2],c='y')
    ax.plot(upplot[:,0],upplot[:,1],upplot[:,2],c='b')
    
    Mcmr = LA.inv(Meye)
    #print(Mcmr)

    near = 1
    n = 1
    f = 5
    r = 4
    t = 3
    Mproj = np.array([[n/r,0,0,0],[0,n/t,0,0],[0,0,-(f+n)/(f-n),-2*f*n/(f-n)],[0,0,-1,0]])
    #print(Mproj)

    ##pos_obj = np.array([10,2,-2,1])
    pos_obj = np.concatenate((gaze_eye.T * 3 + p_eye.T,np.array([1]).reshape(1,1)))
    print(pos_obj)
    mtx =  np.matmul(Mproj, Mcmr)
    screen_pos = np.matmul(mtx, pos_obj)
    #screen_pos = screen_pos/screen_pos[3]
    print("screen_pos" + str(screen_pos[0].shape) + str(screen_pos))


    ax.scatter(p_eye[:,0],p_eye[:,1],p_eye[:,2], zdir='z', c= 'red')
    ax.scatter(pos_obj[0],pos_obj[1],pos_obj[2], zdir='z', c= 'blue')

    bx.scatter(np.array(screen_pos[0]),np.array(screen_pos[1]), c= 'green')
#    bx.scatter(0,0, c= 'green')
    #plt.axis([-4,4,-4,4])
    #plt.show()

def ken_math_test(amount, screen_pos, predefine_pos, Mheadset, Mproj):
    Mleft = np.zeros((amount,3))
    Mright = np.zeros((amount,3))
    for i in range(0, amount):
        Mcmr = construct_camera_matrix(Mheadset[i])
        
        Mleft[i] = screen_pos[:3].reshape(1,3)
        Mright[i] = np.matmul(Mproj,np.matmul(LA.inv(Mcmr), predefine_pos[i]))[:3].reshape(1,3)
        print("Mleft:" + str(Mleft[i]))
        print("Mright:" + str(Mright[i]))

        #Meyecmr = construct_camera_matrix(np.matmul(Mheadset[i],Moffset))
        #sp = np.matmul(np.matmul(Mproj, LA.inv(Meyecmr)), predefine_pos[i])
        #bx.scatter(sp[0],sp[1], c= 'green')

    R,t = rigid_transform_3D(Mleft, Mright)

    # test
    MRt34 = np.concatenate((R,t),axis=1)
    #print(MRt34.shape)
    MRt = np.concatenate((MRt34,np.array([0,0,0,1]).reshape(1,4)))
    #print("Mrt " + str(MRt))
    A2 = np.matmul(MRt,np.matmul(Mproj,LA.inv(Mcmr)))
    A3 = np.matmul(A2, predefine_pos)
    #print(A3)
    for i in range(0, amount):
        A3[i] /= A3[i][3][0]
    #print(A3)
    tilepos = np.tile(screen_pos.reshape(4,1),(amount,1,1))
    #print(tilepos)
    err = A3 - tilepos
    #print(err)
    err = np.multiply(err,err)
    #print(err)
    err = np.sum(err)
    #print(err)
    err = sqrt(err/amount)
    print("RMSE:", err)
    
    #print(R)
    #print(t)
    return R,t

#def ken_math_valid(R,t,Mheadset_test,screen_pos,predefine_pos):
##    ken_screen_pos = np.array([])
##    m = np.matmul()
##    ken_screen_pos = np.matmul(Mheadset_test, predefine_pos)
    

def hehe_math_test(amount, screen_pos, predefine_pos, Mheadset, Mproj):
    Mleft = np.zeros((amount,3))
    Mright = np.zeros((amount,3))
    for i in range(0, amount):
        Mleft[i] = np.matmul(LA.inv(Mproj),screen_pos)[:3].reshape(1,3)
        #tmp = np.matmul(LA.inv(Mheadset[i]), predefine_pos[i])
        #print(tmp)
        #print(tmp.shape)
        Mright[i] = np.matmul(LA.inv(Mheadset[i]), predefine_pos[i])[:3].reshape(1,3)
        print("Mleft:" + str(np.matmul(LA.inv(Mproj),screen_pos)))
        print("Mright:" + str(np.matmul(LA.inv(Mheadset[i]), predefine_pos[i])))

        #Meyecmr = construct_camera_matrix(np.matmul(Mheadset[i],Moffset))
        #sp = np.matmul(np.matmul(Mproj, LA.inv(Meyecmr)), predefine_pos[i])
        #bx.scatter(sp[0],sp[1], c= 'green')

    R,t = rigid_transform_3D(Mleft, Mright)

    # test
    MRt34 = np.concatenate((R,t),axis=1)
    #print(MRt34.shape)
    MRt = np.concatenate((MRt34,np.array([0,0,0,1]).reshape(1,4)))
    #print("Mrt " + str(MRt))
    A2 = np.matmul(Mproj,np.matmul(MRt,LA.inv(Mheadset)))
    A3 = np.matmul(A2, predefine_pos)
    print(A3)
    #for i in range(0, amount):
#        A3[i] /= A3[i][3][0]
    #print(A3)
    tilepos = np.tile(screen_pos.reshape(4,1),(amount,1,1))
    #print(tilepos)
    err = A3 - tilepos
    #print(err)
    err = np.multiply(err,err)
    #print(err)
    err = np.sum(err)
    #print(err)
    err = sqrt(err/amount)
    print("RMSE:", err)
    
    #print(R)
    #print(t)
    return R,t

# get one pair of headset mtx and point position
def prepare(Moffset):   # we need to fix the Moffset each time
    yangle = np.random.randint(-90,90)* math.pi/180
    xangle = np.random.randint(-90,90)* math.pi/180
    Mrotx = np.matrix([[1,0,0],
                       [0, math.cos(xangle), -math.sin(xangle)],
                       [0, math.sin(xangle), math.cos(xangle)]])
    Mroty = np.matrix([[math.cos(yangle),0,math.sin(yangle)],
                       [0, 1, 0],
                       [-math.sin(xangle),0, math.cos(xangle)]])
    Pheadset = np.random.rand(3,1) * 1.5
    Mheadset = np.concatenate(
        (np.concatenate(
            (np.matmul(Mrotx,Mroty),Pheadset),axis=1
            ),np.array([0,0,0,1]).reshape(1,4))
        )
    #print("Mrotx:" + str(Mrotx) + "\nMroty:" + str(Mroty) + "\nPheadset:" + str(Pheadset) + "\nMheadset:" + str(Mheadset))
    #print("\nMheadset:" + str(Mheadset))

    Meye = np.matmul(Mheadset, Moffset)
    #print("Meye:\n" + str(Meye))

    gaze = np.squeeze(np.asarray(np.matmul(Meye,np.array([0,0,-1,0])))).reshape(4,1)
    #print("gaze:\n" + str(gaze))

    Pobj = Meye[:,3] + gaze * np.random.rand(1,1) * 1.5
    #print("Meye[:,3]" + str(Meye[:,3]) + "\nPobj:\n" + str(Pobj))

    ax.scatter(Meye[:,3][0],Meye[:,3][1],Meye[:,3][2], zdir='z', c= 'red')
    ax.scatter(Pobj[0],Pobj[1],Pobj[2], zdir='z', c= 'blue')
    Mplot = np.squeeze(np.asarray(np.concatenate((Meye[:,3],Meye[:,3]+gaze),axis=1)))
    ax.plot(Mplot[0],Mplot[1],Mplot[2],c='y')
    #plt.show()

    return Mheadset, Pobj

def func(data, MoffR11, MoffR12, MoffR13, MoffR14, MoffR31, MoffR32, MoffR33, MoffR34): # data is the Pheadset[x,y,z,1] and exapnd the Moffset13 to 8 parameters
    Pcmr0 = MoffR11 * data[:,0,0] + MoffR12 * data[:,1,0] + MoffR13 * data[:,2,0] + MoffR14 * data[:,3,0]
    Pcmr1 = MoffR31 * data[:,0,0] + MoffR32 * data[:,1,0] + MoffR33 * data[:,2,0] + MoffR34 * data[:,3,0]
    return Pcmr0 / Pcmr1

def funcx(data, MoffR11, MoffR12, MoffR13, MoffR14): # data is the Pheadset[x,y,z,1] and exapnd the Moffset13 to 8 parameters
    Pcmr0 = MoffR11 * data[:,0,0] + MoffR12 * data[:,1,0] + MoffR13 * data[:,2,0] + MoffR14 * data[:,3,0]
    return Pcmr0

def funcy(data, MoffR21, MoffR22, MoffR23, MoffR24): # data is the Pheadset[x,y,z,1] and exapnd the Moffset13 to 8 parameters
    Pcmr1 = MoffR21 * data[:,0,0] + MoffR22 * data[:,1,0] + MoffR23 * data[:,2,0] + MoffR24 * data[:,3,0]
    return Pcmr1
    
def nonlinear(amount, Pcmr, Pheadsets):
    res = np.transpose(np.tile(Pcmr[0]/Pcmr[2],amount))
    
    params, pcov = optimize.curve_fit(funcx, Pheadsets, res)
    print(params)

    params2, pcov2 = optimize.curve_fit(funcy, Pheadsets, res)
    print(params)

screen_pos = np.array([0,0,0,1])
near = 1
Mproj = np.array([[1,0,0,0],[0,1,0,0],[0,0,0,0],[0,0,-1/near,0]])

random_test2()
#ken_math_test()


amount = 20 #20 + 4
Mheadsets = np.zeros((amount,4,4),dtype=np.float64)
#print("M headsets:\n" +str(Mheadsets))
Pobjs = np.zeros((amount,4,1),dtype=np.float64)
Pheadsets = np.zeros((amount,4,1),dtype=np.float64)
#print("P objs:\n" +str(Pobjs))

Moffset = np.concatenate((np.random.rand(3,4),np.array([0,0,0,1]).reshape(1,4)))
print("offset:" + str(Moffset))

for i in range(0, amount):
    Mheadset, Pobj = prepare(Moffset)
    Mheadsets[i] = Mheadset
    Pobjs[i] = Pobj
    Pheadsets[i] = np.matmul(LA.inv(Mheadset), Pobj)
    
#print("M headsets:\n" +str(Mheadsets))
#print("P objs:\n" +str(Pobjs))

#R,t = ken_math_test(amount, screen_pos, Pobjs, Mheadsets, Mproj)

n = 1
f = 5
r = 4
t = 3
Mproj = np.array([[n/r,0,0,0],[0,n/t,0,0],[0,0,-(f+n)/(f-n),-2*f*n/(f-n)],[0,0,-1,0]])
screen_pos[2] = -n
#R,t = hehe_math_test(amount, screen_pos, Pobjs, Mheadsets, Mproj)
Pcmr = np.array([0,0,1,1])
nonlinear(amount,Pcmr ,Pheadsets)

plt.show()
