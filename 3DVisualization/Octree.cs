using OpenTK;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using System;
using OpenTK.Graphics;

namespace _3DVisualization
{
    class OctreeNode
    {
        List<Point3D>[] l=new List<Point3D>[8];
        double sumx=0,sumy=0,sumz=0;
        double[,] mFrustum = new double[6, 4];
        private static readonly int MAX_POINTS = 5000;
        private List<Point3D> data; //点数据
        public OctreeNode[] child; //8个子结点
        public Vector3d minc=new Vector3d(), maxc=new Vector3d(), gCenter=new Vector3d();
        private int iShowListNum,ishow;
        public static bool bShowBoundingBox = false;
        public OctreeNode() { }

        public OctreeNode(List<Point3D> d)
        {
            foreach (var v in d)
            {
                sumx += v.X;
                sumy += v.Y;
                sumz += v.Z;
                if (minc.X > v.X)
                    minc.X=v.X;
                if (minc.Y > v.Y)
                    minc.Y = v.Y;
                if (minc.Z > v.Z)
                    minc.Z = v.Z;
                if (maxc.X < v.X)
                    maxc.X = v.X;
                if (maxc.Y < v.Y)
                    maxc.Y = v.Y;
                if (maxc.Z < v.Z)
                    maxc.Z = v.Z;
            }
            gCenter.X = sumx / d.Count;
            gCenter.Y = sumy / d.Count;
            gCenter.Z = sumz / d.Count;

            if (d.Count <= MAX_POINTS) //把数据放入该结点的数组里
            {
                data = d;
                iShowListNum = GL.GenLists(1);
                GL.NewList(iShowListNum,ListMode.Compile);
                GL.Begin(PrimitiveType.Points);
                foreach (var v in data)
                {
                    GL.Color3(v.PointColor);
                    GL.Vertex3(v.X,v.Y,v.Z);
                }
                GL.End();
                GL.EndList();
            }
            else //把数据放到子结点里
            {
                for(int i=0;i<8;i++)
                    l[i] = new List<Point3D>();
                for (int i = 0; i < d.Count; i++)
                {
                    if (d[i].X < gCenter.X && d[i].Y < gCenter.Y && d[i].Z < gCenter.Z)
                        l[0].Add(d[i]); 
                    else if (d[i].X < gCenter.X && d[i].Y > gCenter.Y && d[i].Z < gCenter.Z)
                        l[1].Add(d[i]);
                    else if (d[i].X > gCenter.X && d[i].Y < gCenter.Y && d[i].Z < gCenter.Z)
                        l[2].Add(d[i]);
                    else if (d[i].X > gCenter.X && d[i].Y > gCenter.Y && d[i].Z < gCenter.Z)
                        l[3].Add(d[i]);
                    else if (d[i].X < gCenter.X && d[i].Y < gCenter.Y && d[i].Z > gCenter.Z)
                        l[4].Add(d[i]);
                    else if (d[i].X < gCenter.X && d[i].Y > gCenter.Y && d[i].Z > gCenter.Z)
                        l[5].Add(d[i]);
                    else if (d[i].X > gCenter.X && d[i].Y < gCenter.Y && d[i].Z > gCenter.Z)
                        l[6].Add(d[i]);
                    else
                        l[7].Add(d[i]);
                }
                child = new OctreeNode[8];
                for (int i = 0; i < 8; i++)
                    child[i] = new OctreeNode(l[i]);
            }
        }

        #region 视景体相关函数
        public void CalculateFrustum()
        {
            Matrix4 projectionMatrix = new Matrix4();
            GL.GetFloat(GetPName.ProjectionMatrix, out projectionMatrix);
            Matrix4 modelViewMatrix = new Matrix4();
            GL.GetFloat(GetPName.ModelviewMatrix, out modelViewMatrix);

            float[] _clipMatrix = new float[16];
            const int RIGHT = 0, LEFT = 1, BOTTOM = 2, TOP = 3, BACK = 4, FRONT = 5;

            _clipMatrix[0] = (modelViewMatrix.M11 * projectionMatrix.M11) + (modelViewMatrix.M12 * projectionMatrix.M21) + (modelViewMatrix.M13 * projectionMatrix.M31) + (modelViewMatrix.M14 * projectionMatrix.M41);
            _clipMatrix[1] = (modelViewMatrix.M11 * projectionMatrix.M12) + (modelViewMatrix.M12 * projectionMatrix.M22) + (modelViewMatrix.M13 * projectionMatrix.M32) + (modelViewMatrix.M14 * projectionMatrix.M42);
            _clipMatrix[2] = (modelViewMatrix.M11 * projectionMatrix.M13) + (modelViewMatrix.M12 * projectionMatrix.M23) + (modelViewMatrix.M13 * projectionMatrix.M33) + (modelViewMatrix.M14 * projectionMatrix.M43);
            _clipMatrix[3] = (modelViewMatrix.M11 * projectionMatrix.M14) + (modelViewMatrix.M12 * projectionMatrix.M24) + (modelViewMatrix.M13 * projectionMatrix.M34) + (modelViewMatrix.M14 * projectionMatrix.M44);

            _clipMatrix[4] = (modelViewMatrix.M21 * projectionMatrix.M11) + (modelViewMatrix.M22 * projectionMatrix.M21) + (modelViewMatrix.M23 * projectionMatrix.M31) + (modelViewMatrix.M24 * projectionMatrix.M41);
            _clipMatrix[5] = (modelViewMatrix.M21 * projectionMatrix.M12) + (modelViewMatrix.M22 * projectionMatrix.M22) + (modelViewMatrix.M23 * projectionMatrix.M32) + (modelViewMatrix.M24 * projectionMatrix.M42);
            _clipMatrix[6] = (modelViewMatrix.M21 * projectionMatrix.M13) + (modelViewMatrix.M22 * projectionMatrix.M23) + (modelViewMatrix.M23 * projectionMatrix.M33) + (modelViewMatrix.M24 * projectionMatrix.M43);
            _clipMatrix[7] = (modelViewMatrix.M21 * projectionMatrix.M14) + (modelViewMatrix.M22 * projectionMatrix.M24) + (modelViewMatrix.M23 * projectionMatrix.M34) + (modelViewMatrix.M24 * projectionMatrix.M44);

            _clipMatrix[8] = (modelViewMatrix.M31 * projectionMatrix.M11) + (modelViewMatrix.M32 * projectionMatrix.M21) + (modelViewMatrix.M33 * projectionMatrix.M31) + (modelViewMatrix.M34 * projectionMatrix.M41);
            _clipMatrix[9] = (modelViewMatrix.M31 * projectionMatrix.M12) + (modelViewMatrix.M32 * projectionMatrix.M22) + (modelViewMatrix.M33 * projectionMatrix.M32) + (modelViewMatrix.M34 * projectionMatrix.M42);
            _clipMatrix[10] = (modelViewMatrix.M31 * projectionMatrix.M13) + (modelViewMatrix.M32 * projectionMatrix.M23) + (modelViewMatrix.M33 * projectionMatrix.M33) + (modelViewMatrix.M34 * projectionMatrix.M43);
            _clipMatrix[11] = (modelViewMatrix.M31 * projectionMatrix.M14) + (modelViewMatrix.M32 * projectionMatrix.M24) + (modelViewMatrix.M33 * projectionMatrix.M34) + (modelViewMatrix.M34 * projectionMatrix.M44);

            _clipMatrix[12] = (modelViewMatrix.M41 * projectionMatrix.M11) + (modelViewMatrix.M42 * projectionMatrix.M21) + (modelViewMatrix.M43 * projectionMatrix.M31) + (modelViewMatrix.M44 * projectionMatrix.M41);
            _clipMatrix[13] = (modelViewMatrix.M41 * projectionMatrix.M12) + (modelViewMatrix.M42 * projectionMatrix.M22) + (modelViewMatrix.M43 * projectionMatrix.M32) + (modelViewMatrix.M44 * projectionMatrix.M42);
            _clipMatrix[14] = (modelViewMatrix.M41 * projectionMatrix.M13) + (modelViewMatrix.M42 * projectionMatrix.M23) + (modelViewMatrix.M43 * projectionMatrix.M33) + (modelViewMatrix.M44 * projectionMatrix.M43);
            _clipMatrix[15] = (modelViewMatrix.M41 * projectionMatrix.M14) + (modelViewMatrix.M42 * projectionMatrix.M24) + (modelViewMatrix.M43 * projectionMatrix.M34) + (modelViewMatrix.M44 * projectionMatrix.M44);

            mFrustum[RIGHT, 0] = _clipMatrix[3] - _clipMatrix[0];
            mFrustum[RIGHT, 1] = _clipMatrix[7] - _clipMatrix[4];
            mFrustum[RIGHT, 2] = _clipMatrix[11] - _clipMatrix[8];
            mFrustum[RIGHT, 3] = _clipMatrix[15] - _clipMatrix[12];
            NormalizePlane(mFrustum, RIGHT);
            mFrustum[LEFT, 0] = _clipMatrix[3] + _clipMatrix[0];
            mFrustum[LEFT, 1] = _clipMatrix[7] + _clipMatrix[4];
            mFrustum[LEFT, 2] = _clipMatrix[11] + _clipMatrix[8];
            mFrustum[LEFT, 3] = _clipMatrix[15] + _clipMatrix[12];
            NormalizePlane(mFrustum, LEFT);
            mFrustum[BOTTOM, 0] = _clipMatrix[3] + _clipMatrix[1];
            mFrustum[BOTTOM, 1] = _clipMatrix[7] + _clipMatrix[5];
            mFrustum[BOTTOM, 2] = _clipMatrix[11] + _clipMatrix[9];
            mFrustum[BOTTOM, 3] = _clipMatrix[15] + _clipMatrix[13];
            NormalizePlane(mFrustum, BOTTOM);
            mFrustum[TOP, 0] = _clipMatrix[3] - _clipMatrix[1];
            mFrustum[TOP, 1] = _clipMatrix[7] - _clipMatrix[5];
            mFrustum[TOP, 2] = _clipMatrix[11] - _clipMatrix[9];
            mFrustum[TOP, 3] = _clipMatrix[15] - _clipMatrix[13];
            NormalizePlane(mFrustum, TOP);
            mFrustum[BACK, 0] = _clipMatrix[3] - _clipMatrix[2];
            mFrustum[BACK, 1] = _clipMatrix[7] - _clipMatrix[6];
            mFrustum[BACK, 2] = _clipMatrix[11] - _clipMatrix[10];
            mFrustum[BACK, 3] = _clipMatrix[15] - _clipMatrix[14];
            NormalizePlane(mFrustum, BACK);
            mFrustum[FRONT, 0] = _clipMatrix[3] + _clipMatrix[2];
            mFrustum[FRONT, 1] = _clipMatrix[7] + _clipMatrix[6];
            mFrustum[FRONT, 2] = _clipMatrix[11] + _clipMatrix[10];
            mFrustum[FRONT, 3] = _clipMatrix[15] + _clipMatrix[14];
            NormalizePlane(mFrustum, FRONT);
        }
        bool VoxelWithinFrustum(double[,] ftum)
        {
            double x1 = this.minc.X, y1 = this.minc.Y, z1 = this.minc.Z;
            double x2 = this.maxc.X, y2 = this.maxc.Y, z2 = this.maxc.Z;
            for (int i = 0; i < 6; i++)
            {
                if ((ftum[i, 0] * x1 + ftum[i, 1] * y1 + ftum[i, 2] * z1 + ftum[i, 3] <= 0.0F) &&
                    (ftum[i, 0] * x2 + ftum[i, 1] * y1 + ftum[i, 2] * z1 + ftum[i, 3] <= 0.0F) &&
                    (ftum[i, 0] * x1 + ftum[i, 1] * y2 + ftum[i, 2] * z1 + ftum[i, 3] <= 0.0F) &&
                    (ftum[i, 0] * x2 + ftum[i, 1] * y2 + ftum[i, 2] * z1 + ftum[i, 3] <= 0.0F) &&
                    (ftum[i, 0] * x1 + ftum[i, 1] * y1 + ftum[i, 2] * z2 + ftum[i, 3] <= 0.0F) &&
                    (ftum[i, 0] * x2 + ftum[i, 1] * y1 + ftum[i, 2] * z2 + ftum[i, 3] <= 0.0F) &&
                    (ftum[i, 0] * x1 + ftum[i, 1] * y2 + ftum[i, 2] * z2 + ftum[i, 3] <= 0.0F) &&
                    (ftum[i, 0] * x2 + ftum[i, 1] * y2 + ftum[i, 2] * z2 + ftum[i, 3] <= 0.0F))
                {
                    return false;
                }
            }
            return true;
        }
        private void NormalizePlane(double[,] frustum, int side)
        {
            double magnitude = Math.Sqrt((frustum[side, 0] * frustum[side, 0]) +
       (frustum[side, 1] * frustum[side, 1]) + (frustum[side, 2] * frustum[side, 2]));
            frustum[side, 0] /= magnitude;
            frustum[side, 1] /= magnitude;
            frustum[side, 2] /= magnitude;
            frustum[side, 3] /= magnitude;
        }
        #endregion

        public bool BShowBoundingBox
        {
            get { return bShowBoundingBox; }
            set { bShowBoundingBox = value; }
        }//判断是否添加包围盒
        public void renderBox()
        {
            ishow = GL.GenLists(1);
            GL.NewList(ishow,ListMode.Compile);
            GL.Color4(Color4.Yellow);
            GL.LineWidth(2);
            GL.Begin(PrimitiveType.LineLoop);
            GL.Vertex3(minc.X, minc.Y, maxc.Z);
            GL.Vertex3(minc.X, maxc.Y, maxc.Z);
            GL.Vertex3(maxc.X, maxc.Y, maxc.Z);
            GL.Vertex3(maxc.X, minc.Y, maxc.Z);
            GL.End();
            //下表面
            GL.Begin(PrimitiveType.LineLoop);
            GL.Vertex3(minc.X, minc.Y, minc.Z);
            GL.Vertex3(minc.X, maxc.Y, minc.Z);
            GL.Vertex3(maxc.X, maxc.Y, minc.Z);
            GL.Vertex3(maxc.X, minc.Y, minc.Z);
            GL.End();
            //柱子
            GL.Begin(PrimitiveType.LineStrip);
            GL.Vertex3(minc.X, minc.Y, minc.Z);
            GL.Vertex3(minc.X, minc.Y, maxc.Z);
            GL.End();
            GL.Begin(PrimitiveType.LineStrip);
            GL.Vertex3(minc.X, maxc.Y, minc.Z);
            GL.Vertex3(minc.X, maxc.Y, maxc.Z);
            GL.End();
            GL.Begin(PrimitiveType.LineStrip);
            GL.Vertex3(maxc.X, maxc.Y, minc.Z);
            GL.Vertex3(maxc.X, maxc.Y, maxc.Z);
            GL.End();             
            GL.Begin(PrimitiveType.LineStrip);
            GL.Vertex3(maxc.X, minc.Y, minc.Z);
            GL.Vertex3(maxc.X, minc.Y, maxc.Z);
            GL.End();
            GL.EndList();
        }//包围盒
        public void render()
        {
            if (child == null)
            {
                if (bShowBoundingBox)
                {
                    renderBox();
                    GL.CallList(ishow);
                }
                CalculateFrustum();
                if (VoxelWithinFrustum(mFrustum))
                    GL.CallList(iShowListNum);
            }
            else
            {
                foreach (var v in child)
                    v.render();
            } 
        }
    }
    class Octree
    {
        public OctreeNode root;
        public Octree()
        {
            root = null;
        }
        public Octree(List<Point3D> d)
        {
            root = new OctreeNode(d);
        }
        public void render()
        {
            root.render();
        }
    }
}