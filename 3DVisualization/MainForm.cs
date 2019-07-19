using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Diagnostics;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using laszip.net;

namespace _3DVisualization
{
    public partial class MainForm : Form
    {
        #region 全局定义
        bool isResize = false, isBackGround = false;
        bool LeftPressed = false, RightPressed = false;
        int isShowListNum = 0;
        double ratio;
        double scale = 1;
        double LookX = 0, LookY = 0;
        double MouseX = 0, MouseY = 0, MouseZ = 0;
        Stopwatch sw = new Stopwatch();
        Octree octree;
        Point MousePoint;
        List<Point3D> points;
        Point3D choosen;
        Point3D c = new Point3D(0, 0, 1);
        Point3D l = new Point3D(0, 0, 0);
        Point3D h = new Point3D(0, 1, 0);
        Point3D headfront = new Point3D(0, 0, -1);
        Point3D headleft = new Point3D(-1, 0, 0);
        #endregion

        #region 页面
        public MainForm()
        {
            InitializeComponent();
        }
        private void InitialGL()
        {
            GL.ShadeModel(ShadingModel.Smooth);    //  启用平滑渲染。默认
            GL.ClearColor(Color.Black);            //  黑色背景。默认
            GL.ClearDepth(1.0f);                   //  设置深度缓存。默认1
            GL.Enable(EnableCap.DepthTest);        //  启用深度测试。默认关闭
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            SetupViewport();
            isResize = true;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            InitialGL();
            Matrix4d lookat = Matrix4d.LookAt(0, 0, 1, 0, 0, 0, 0, 1, 0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookat);
            PaintBall();
        }
        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            Render();
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (isResize)
            {
                SetupViewport();
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dr = MessageBox.Show("确定要退出这么强大的软件吗？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            //容我不要脸一下
            if (dr == DialogResult.Yes)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }
        #endregion

        #region 观察视角等
        private void SetupLineList()
        {
            double num = 10;
            int iLineListNum = GL.GenLists(1);
            GL.NewList(iLineListNum, ListMode.Compile);
            GL.Begin(PrimitiveType.Lines);
            GL.Enable(EnableCap.LineSmooth);
            GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);
            GL.Color4(Color.Red);
            GL.Vertex3(-num, 0, 0);
            GL.Vertex3(num, 0, 0);
            GL.Color4(Color.Green);
            GL.Vertex3(0, -num, 0);
            GL.Vertex3(0, num, 0);
            GL.Color4(Color.Blue);
            GL.Vertex3(0, 0, -num);
            GL.Vertex3(0, 0, num);
            if (choosen != null)
            {
                GL.Color4(Color.White);
                GL.Vertex3(choosen.X - 1, choosen.Y, choosen.Z);
                GL.Vertex3(choosen.X + 1, choosen.Y, choosen.Z);
                GL.Vertex3(choosen.X, choosen.Y - 1, choosen.Z);
                GL.Vertex3(choosen.X, choosen.Y + 1, choosen.Z);
                GL.Vertex3(choosen.X, choosen.Y, choosen.Z - 1);
                GL.Vertex3(choosen.X, choosen.Y, choosen.Z + 1);
            }
            GL.End();
            GL.EndList();
            GL.CallList(iLineListNum);
        }//坐标轴，鼠标选点显示
        private void SetupViewport()
        {
            int w = glControl1.ClientSize.Width;
            int h = glControl1.ClientSize.Height;
            gluPerspective(90, w / (double)h, 0.01, 100);
            GL.Viewport(0, 0, w, h);
        }
        private void SetupLookAt()
        {
            double x = LookX * 3.14159 / 180, y = LookY * 3.14159 / 180;
            headfront.X = Math.Sin(x) * Math.Cos(y);
            headfront.Y = Math.Sin(y);
            headfront.Z = -Math.Cos(x) * Math.Cos(y);
            l.X = c.X + headfront.X;
            l.Y = c.Y + headfront.Y;
            l.Z = c.Z + headfront.Z;
            h.X = -Math.Sin(x) * Math.Cos(y);
            h.Y = Math.Cos(y);
            h.Z = Math.Cos(x) * Math.Sin(y);
            headleft.X = -Math.Cos(x);
            headleft.Z = -Math.Sin(x);
        }
        private void gluPerspective(double fovy, double aspect, double near, double far)
        {
            const double DEG2RAD = 3.14159265 / 180.0;
            double tangent = Math.Tan(fovy / 2 * DEG2RAD);
            double height = near * tangent;
            double width = height * aspect;
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Frustum(-width, width, -height, height, near, far);
        }
        #endregion

        #region 菜单栏
        private void 打开OToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            LoadLasFile(ofd.FileName);
        }
        private void 图像背景ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("是否添加背景？若是则可能遗失坐标轴", "提示", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                isBackGround = true;
            }
            else
            {
                isBackGround = false;
            }
            glControl1.Invalidate();
        }
        private void 打开兵马俑ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadLasFile("E:\\程序设计实践\\兵马俑.las");
        }
        private void 截图ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            int[] vdata = new int[4];
            GL.GetInteger(GetPName.Viewport, vdata);
            int w = vdata[2];
            int h = vdata[3];
            if ((w % 4) != 0)
                w = (w / 4 + 1) * 4;
            byte[] imgBuffer = new byte[w * h * 3];
            GL.ReadPixels(0, 0, w, h, OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, imgBuffer);
            FlipHeight(imgBuffer, w, h);
            Bitmap bmp = BytesToImg(imgBuffer, w, h);
            SaveFileDialog sfd = new SaveFileDialog();
            string filepath;
            sfd.Filter = "图像文件(*.jpg)|*.jpg";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                filepath = sfd.FileName.ToString();
                bmp.Save(filepath);
                MessageBox.Show("截图成功，已保存至" + filepath);
            }
        }
        private void 帮助HToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("每个菜单栏按钮都可以试试哦" +
                "鼠标左击旋转物体，右击改变视角\n" +
                "鼠标滚轮实现放大缩小\n" +
                "WASD控制上下左右移动\n" +
                "打开LAS文件后可点击八叉树包围盒使其显示包围盒" +
                "打开LAS文件后，鼠标双击会捕捉离鼠标最近的点，捕捉到的点会以三条白线的交点显示，并通过弹窗显示选点耗时和选中点的X,Y,Z的坐标", "提示", MessageBoxButtons.OK);
        }
        private void 退出XToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void 八叉树包围盒ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (octree != null)
            {
                DialogResult dr = MessageBox.Show("是否添加包围盒？", "提示", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {
                    octree.root.BShowBoundingBox = true;
                    glControl1.Invalidate();
                }
                else
                {
                    octree.root.BShowBoundingBox = false;
                    glControl1.Invalidate();
                }
            }
            else
            {
                MessageBox.Show("还没有打开LAS文件构建八叉树哦！");
            }
        }
        private void 换肤ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "皮肤文件(*.ssk)|*.ssk";
            ofd.ShowDialog();
            skinEngine1.SkinFile = ofd.FileName;
        }
        #endregion

        #region 键鼠操作
        private void glControl1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.D:
                    c.X += 0.1 * headleft.X;
                    c.Y += 0.1 * headleft.Y;
                    c.Z += 0.1 * headleft.Z;
                    break;
                case Keys.A:
                    c.X -= 0.1 * headleft.X;
                    c.Y -= 0.1 * headleft.Y;
                    c.Z -= 0.1 * headleft.Z;
                    break;
                case Keys.S:
                    c.X += 0.1 * h.X;
                    c.Y += 0.1 * h.Y;
                    c.Z += 0.1 * h.Z;
                    break;
                case Keys.W:
                    c.X -= 0.1 * h.X;
                    c.Y -= 0.1 * h.Y;
                    c.Z -= 0.1 * h.Z;
                    break;
            }
            SetupLookAt();
            Render();//加这个好像运行更流畅？
            glControl1.Invalidate();
        }
        private void glControl1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                if (!LeftPressed) scale *= 1.25;
                else MouseZ += 3;
            }
            else
            {
                if (!LeftPressed) scale /= 1.25;
                else MouseZ -= 3;
            }
            glControl1.Invalidate();
        }
        private void glControl1_MouseDown(object sender, MouseEventArgs e)
        {
            MousePoint = e.Location;
            if (e.Button == MouseButtons.Left)
            {
                LeftPressed = true;
            }
            else if (e.Button == MouseButtons.Right)
            {
                RightPressed = true;
            }
        }
        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (LeftPressed)
            {
                MouseX += (e.Location.X - MousePoint.X) / 6.0f;
                MouseY += (e.Location.Y - MousePoint.Y) / 6.0f;
                MousePoint = e.Location;
                glControl1.Invalidate();
            }
            else if (RightPressed)
            {
                LookX += (e.Location.X - MousePoint.X) / 6.0f;
                LookY -= (e.Location.Y - MousePoint.Y) / 6.0f;
                MousePoint = e.Location;
                SetupLookAt();
                glControl1.Invalidate();
            }
        }
        private void glControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                LeftPressed = false;
            }
            else if (e.Button == MouseButtons.Right)
            {
                RightPressed = false;
            }
        }
        private void glControl1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (octree != null)
            {
                sw.Start();
                Vector3d winxyz;
                winxyz.X = e.Location.X;
                winxyz.Y = e.Location.Y;
                winxyz.Z = 0.0f;
                Vector3d nearPoint = new Vector3d(0, 0, 0);
                UnProject(winxyz, ref nearPoint);
                winxyz.Z = 1.0f;
                Vector3d farPoint = new Vector3d(0, 0, 0);
                UnProject(winxyz, ref farPoint);
                double length = 1000;
                foreach (var v in points)
                {
                    if (Distance(v, nearPoint, farPoint) < length)
                    {
                        length = Distance(v, nearPoint, farPoint);
                        choosen = v;
                    }
                }
                glControl1.Invalidate();
                sw.Stop();
                MessageBox.Show("选点耗时:" + sw.ElapsedMilliseconds.ToString() + "ms\n"
                    + "选中点坐标为：\nX坐标：" + choosen.X + "\nY坐标：" + choosen.Y + "\nZ坐标：" + choosen.Z);
                sw.Reset();
            }
        }
        #endregion

        #region 绘制函数
        private void PaintBall()
        {
            if (isShowListNum != 0) GL.DeleteLists(isShowListNum, 1);
            isShowListNum = GL.GenLists(1);
            GL.NewList(isShowListNum, ListMode.Compile);
            if (octree == null)
            {
                GL.Begin(PrimitiveType.Points);
                GL.Color4(Color4.SkyBlue);
                const double radius = 0.5;
                const int step = 2;
                int xWidth = 360 / step + 1;
                int zHeight = 180 / step + 1;
                int halfZHeight = (zHeight - 1) / 2;
                double xx, yy, zz;
                for (int z = -halfZHeight; z <= halfZHeight; z++)
                {
                    for (int x = 0; x < xWidth; x++)
                    {
                        xx = radius * Math.Cos(x * step * Math.PI / 180)
                        * Math.Cos(z * step * Math.PI / 180.0);
                        zz = radius * Math.Sin(x * step * Math.PI / 180)
                        * Math.Cos(z * step * Math.PI / 180.0);
                        yy = radius * Math.Sin(z * step * Math.PI / 180);
                        GL.Vertex3(xx, yy, zz);
                    }
                }
            }
            GL.End();
            GL.EndList();
        }
        private void Render()
        {
            glControl1.MakeCurrent(); //后续OpenGL显示操作在当前控件窗口内进行
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            SetupLineList();
            if (isBackGround)
                GradientBackgroudColor();
            if (octree != null)
            {
                octree.render();
            }
            GL.CallList(isShowListNum);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            Matrix4d lookat = Matrix4d.LookAt(c.X, c.Y, c.Z, l.X, l.Y, l.Z, h.X, h.Y, h.Z);
            GL.LoadMatrix(ref lookat);
            GL.Rotate(MouseY, 1, 0, 0);
            GL.Rotate(MouseX, 0, 1, 0);
            GL.Rotate(MouseZ, 0, 0, 1);
            GL.Scale(scale, scale, scale);

            glControl1.SwapBuffers();
            //交换缓冲区。双缓冲绘制时，所有的绘制都是绘制到后台缓冲区里，如果不交换缓冲区，就看不到绘制内容。OpenTK 默认双缓冲
        }
        #endregion

        #region 鼠标选点所需函数
        int UnProject(Vector3d win, ref Vector3d obj)
        {
            Matrix4d modelMatrix;
            GL.GetDouble(GetPName.ModelviewMatrix, out modelMatrix);
            Matrix4d projMatrix;
            GL.GetDouble(GetPName.ProjectionMatrix, out projMatrix);
            int[] viewport = new int[4];
            GL.GetInteger(GetPName.Viewport, viewport);
            return UnProject(win, modelMatrix, projMatrix, viewport, ref obj);
        }
        int UnProject(Vector3d win, Matrix4d modelMatrix, Matrix4d projMatrix, int[] viewport, ref Vector3d obj)
        {
            return like_gluUnProject(win.X, win.Y, win.Z, modelMatrix, projMatrix,
            viewport, ref obj.X, ref obj.Y, ref obj.Z);
        }
        int like_gluUnProject(double winx, double winy, double winz, Matrix4d modelMatrix, Matrix4d projMatrix, int[] viewport, ref double objx, ref double objy, ref double objz)
        {
            Matrix4d finalMatrix;
            Vector4d _in;
            Vector4d _out;
            finalMatrix = Matrix4d.Mult(modelMatrix, projMatrix);
            finalMatrix.Invert();
            _in.X = winx;
            _in.Y = viewport[3] - winy;
            _in.Z = winz;
            _in.W = 1.0f;
            // Map x and y from window coordinates
            _in.X = (_in.X - viewport[0]) / viewport[2];
            _in.Y = (_in.Y - viewport[1]) / viewport[3];
            // Map to range -1 to 1
            _in.X = _in.X * 2 - 1;
            _in.Y = _in.Y * 2 - 1;
            _in.Z = _in.Z * 2 - 1;
            //__gluMultMatrixVecd(finalMatrix, _in, _out); 
            // check if this works:
            _out = Vector4d.Transform(_in, finalMatrix);
            if (_out.W == 0.0)
                return (0);
            _out.X /= _out.W;
            _out.Y /= _out.W;
            _out.Z /= _out.W;
            objx = _out.X;
            objy = _out.Y;
            objz = _out.Z;
            return (1);
        }
        double Distance(Point3D point, Vector3d nearPoint, Vector3d farPoint)
        {
            double distance = 0;
            distance = Math.Sqrt((point.X - nearPoint.X) * (point.X - nearPoint.X)
                + (point.Y - nearPoint.Y) * (point.Y - nearPoint.Y)
                + (point.Z - nearPoint.Z) * (point.Z - nearPoint.Z)) +
                Math.Sqrt((point.X - farPoint.X) * (point.X - farPoint.X)
                + (point.Y - farPoint.Y) * (point.Y - farPoint.Y)
                + (point.Z - farPoint.Z) * (point.Z - farPoint.Z));
            return distance;
        }
        #endregion

        #region 截图所需函数
        private void FlipHeight(byte[] data, int w, int h)
        {
            int wstep = w * 3;
            byte[] temp = new byte[wstep];
            for (int i = 0; i < h / 2; i++)
            {
                Array.Copy(data, wstep * i, temp, 0, wstep);
                Array.Copy(data, wstep * (h - i - 1), data, wstep * i, wstep);
                Array.Copy(temp, 0, data, wstep * (h - i - 1), wstep);
            }
        }
        private Bitmap BytesToImg(byte[] bytes, int w, int h)
        {
            Bitmap bmp = new Bitmap(w, h);
            BitmapData bd = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            IntPtr ptr = bd.Scan0;
            int bmpLen = bd.Stride * bd.Height; Marshal.Copy(bytes, 0, ptr, bmpLen);
            bmp.UnlockBits(bd);
            return bmp;
        }
        #endregion

        #region 背景函数
        private void GradientBackgroudColor()
        {
            GL.Disable(EnableCap.DepthTest);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();
            double left = -1, right = 1, bottom = -1, top = 1, znear = -1, zfar = 1;
            GL.Ortho(left, right, bottom, top, znear, zfar);
            int ishow = GL.GenLists(1);
            GL.NewList(ishow, ListMode.Compile);
            GL.Begin(PrimitiveType.Quads);
            {
                GL.Color3(1.0, 1.0, 1.0);
                GL.Vertex3(left, bottom, zfar);
                GL.Vertex3(right, bottom, zfar);
                GL.Color3(0.0, 0.0, 0.0);
                GL.Vertex3(right, top, zfar);
                GL.Vertex3(left, top, zfar);
            }
            GL.End();
            GL.EndList();
            GL.CallList(ishow);
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();
            GL.Enable(EnableCap.DepthTest);
        }
        #endregion

        #region 打开LAS文件函数
        private void LoadLasFile(string FileName)
        {
            sw.Start();
            points = new List<Point3D>();
            var lasReader = new laszip_dll();
            var compressed = true;
            lasReader.laszip_open_reader(FileName, ref compressed);
            var numberOfPoints = lasReader.header.number_of_point_records;
            double xmid, ymid, zmid;
            ratio = Math.Max(Math.Max(lasReader.header.max_x - lasReader.header.min_x, lasReader.header.max_y - lasReader.header.min_y),
                                  lasReader.header.max_z - lasReader.header.min_z) / 2;
            ratio = (int)ratio + 1;
            xmid = (lasReader.header.max_x + lasReader.header.min_x) / 2;
            ymid = (lasReader.header.max_y + lasReader.header.min_y) / 2;
            zmid = (lasReader.header.max_z + lasReader.header.min_z) / 2;
            int classification = 0;
            var coordArray = new double[3];
            double zmin = (lasReader.header.min_z - zmid) / ratio;
            double zmax = (lasReader.header.max_z - zmid) / ratio;
            for (int pointIndex = 0; pointIndex < numberOfPoints; pointIndex++)//循环读取每个点
            {
                lasReader.laszip_read_point();
                if (pointIndex % 10 == 0)//减少绘制的点数
                {
                    double x, y, z;
                    //  得到每个点坐标值
                    lasReader.laszip_get_coordinates(coordArray);
                    x = (coordArray[0] - xmid) / ratio;
                    y = (coordArray[1] - ymid) / ratio;
                    z = (coordArray[2] - zmid) / ratio;
                    points.Add(new Point3D(x, y, z, zmin, zmax));
                    classification = lasReader.point.classification;
                }
            }
            //  关闭
            lasReader.laszip_close_reader();
            octree = new Octree(points);

            PaintBall();
            MouseX = MouseY = 0;
            scale = 1;
            Render();
            glControl1.Invalidate();
            sw.Stop();
            MessageBox.Show("共读取" + points.Count + "个点\n" +
                "打开该点云文件耗时" + sw.ElapsedMilliseconds + "ms\n");
            sw.Reset();
        }
        #endregion
    }
}
