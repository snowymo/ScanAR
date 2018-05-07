from  matplotlib import pyplot as plt
import  numpy as np
from mpl_toolkits.mplot3d import Axes3D

fig = plt.figure()
ax = Axes3D(fig)

#列出实验数据
point=[[2,3,48],[4,5,50],[5,7,51],[8,9,55],[9,12,56]]
#point = [[0.5,-0.8,0.86],[0.53,0.12,1.1]]
plt.xlabel("X1")
plt.ylabel("X2")

#表示矩阵中的值
ISum = 0.0
X1Sum = 0.0
X2Sum = 0.0
X1_2Sum = 0.0
X1X2Sum = 0.0
X2_2Sum = 0.0
YSum = 0.0
X1YSum = 0.0
X2YSum = 0.0

#在图中显示各点的位置
for i in range(0,len(point)):

    x1i=point[i][0]
    x2i=point[i][1]
    yi=point[i][2]
    ax.scatter(x1i, x2i, yi, color="red")
    show_point = "["+ str(x1i) +","+ str(x2i)+","+str(yi) + "]"
    ax.text(x1i,x2i,yi,show_point)

    ISum = ISum+1
    X1Sum = X1Sum+x1i
    X2Sum = X2Sum+x2i
    X1_2Sum = X1_2Sum+x1i**2
    X1X2Sum = X1X2Sum+x1i*x2i
    X2_2Sum = X2_2Sum+x2i**2
    YSum = YSum+yi
    X1YSum = X1YSum+x1i*yi
    X2YSum = X2YSum+x2i*yi

nppoint = np.array(point)
print(nppoint[:,0])
sigma = np.sum(nppoint, axis=0)
sigmax = sigma[0]
sigmay = sigma[1]
sigmaz = sigma[2]
sq = np.multiply(nppoint, nppoint)
sigmasq = np.sum(sq, axis=0)
sigmasqx = sigmasq[0]
sigmasqy = sigmasq[1]
xy = np.multiply(nppoint[:,0],nppoint[:,1])
sigmaxy = np.sum(xy, axis=0)
xz = np.multiply(nppoint[:,0],nppoint[:,2])
sigmaxz = np.sum(xz, axis=0)
yz = np.multiply(nppoint[:,1],nppoint[:,2])
sigmayz = np.sum(yz, axis=0)

# 进行矩阵运算
# _mat1 设为 mat1 的逆矩阵
m1=[[ISum,sigmax,sigmay],[sigmax,sigmasqx,sigmaxy],[sigmay,sigmaxy,sigmasqy]]
mat1 = np.matrix(m1)
m2=[[sigmaz],[sigmaxz],[sigmayz]]
mat2 = np.matrix(m2)
_mat1 =mat1.getI()
mat3 = _mat1*mat2

# 用list来提取矩阵数据
m3=mat3.tolist()
a0 = m3[0][0]
a1 = m3[1][0]
a2 = m3[2][0]

# 绘制回归线
x1 = np.linspace(0,9)
x2 = np.linspace(0,12)
y = a0+a1*x1+a2*x2
ax.plot(x1,x2,y)
show_line = "y="+str(a0)+"+"+str(a1)+"x1"+"+"+str(a2)+"x2"
plt.title(show_line)
plt.show()
