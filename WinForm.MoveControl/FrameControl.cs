using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WinForm.MoveControl
{
    public partial class FrameControl : UserControl
    {
        public FrameControl(Control control)
        {
            this.control = control;

            #region 绑定事件

            //鼠标按下时
            MouseDown += (sender, e) =>
            {
                //记录鼠标位置
                lastPoint = Cursor.Position;

                //改变鼠标状态
                SetCursor(e.X, e.Y);
            };

            //鼠标移动时，即在调整控件大小
            MouseMove += (sender, e) =>
            {
                //鼠标左键
                if (e.Button == MouseButtons.Left)
                {
                    //移动时刷新实线
                    DrawSolids();

                    //调整控件大小
                    ReCtrlSize();
                }
                //不是鼠标左键，则表示只是在控件上移动
                else
                {
                    //改变鼠标状态
                    SetCursor(e.X, e.Y);
                }
            };

            //鼠标键释放时
            MouseUp += (sender, e) =>
            {
                control.Refresh();

                //显示虚线
                SetControlRegion();
            };

            ////鼠标离开控件隐藏边框
            //MouseLeave += (sender, e) =>
            //{
            //    this.Visible = false;
            //};

            //大小改变时
            SizeChanged += (sender, e) =>
            {
                isSizeChanged = true;
            };

            #endregion
        }

        /// <summary>
        /// 是否已经改变大小，默认大小（150,150）不运行OnPaint方法，避免出现异常绘制
        /// </summary>
        bool isSizeChanged = false;

        /// <summary>
        /// 设置控件支持透明
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x20;
                return cp;
            }
        }

        /// <summary>
        /// 绘制虚线：4条虚线和8个小矩形
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            //默认大小，去除绘制，避免出现异常绘制
            if (isSizeChanged == false) return;

            DrawDottedLines(e.Graphics);
        }

        /// <summary>
        /// 当前控件
        /// </summary>
        Control control;

        /// <summary>
        /// 上一个鼠标坐标
        /// </summary>
        Point lastPoint = new Point();

        /// <summary>
        /// 包括边框的局域
        /// </summary>
        Rectangle controlRect;
        /// <summary>
        /// 4条底边的集合
        /// </summary>
        Rectangle[] borderRects = new Rectangle[4];

        /// <summary>
        /// 组成4条虚线的点
        /// </summary>
        Point[] linePoints = new Point[5];

        /// <summary>
        /// 8个小矩形大小
        /// </summary>
        const int size = 6;
        /// <summary>
        /// 8个小矩形的大小
        /// </summary>
        Size smallRectSize = new Size(size, size);
        /// <summary>
        /// 8个小矩形
        /// </summary>
        Rectangle[] smallRects = new Rectangle[8];

        #region 设置控件区域，绘制虚线，绘制实线

        /// <summary>
        /// 设置控件区域：4条底边，点击控件后显示
        /// </summary>
        public void SetControlRegion()
        {
            //显示边框并置顶
            Visible = true;
            BringToFront();

            #region 设置控件区域

            int x = control.Bounds.X - smallRectSize.Width;
            int y = control.Bounds.Y - smallRectSize.Height;
            int width = control.Bounds.Width + (smallRectSize.Width * 2);
            int height = control.Bounds.Height + (smallRectSize.Height * 2);
            Bounds = new Rectangle(x, y, width, height);

            //包括边框的局域
            controlRect = new Rectangle(new Point(0, 0), Bounds.Size);

            #endregion

            #region 4条底边

            GraphicsPath path = new GraphicsPath();

            //上底边
            borderRects[0] = new Rectangle(0, 0, Width + size * 2, smallRectSize.Height + 1);
            //左底边
            borderRects[1] = new Rectangle(0, size + 1, smallRectSize.Width + 1, Height - size * 2 - 2);
            //下底边
            borderRects[2] = new Rectangle(0, Height - size - 1, Width + size * 2, smallRectSize.Height + 1);
            //右底边
            borderRects[3] = new Rectangle(Width - size - 1, size + 1, smallRectSize.Width + 1, Height - size * 2 - 2);
            path.AddRectangle(borderRects[0]);
            path.AddRectangle(borderRects[1]);
            path.AddRectangle(borderRects[2]);
            path.AddRectangle(borderRects[3]);

            Region = new Region(path);

            #endregion
        }

        /// <summary>
        /// 绘制虚线：4条虚线和8个小矩形
        /// </summary>
        public void DrawDottedLines(Graphics g)
        {
            #region 4条虚线

            //左上
            linePoints[0] = new Point(3, 3);
            //右上
            linePoints[1] = new Point(Width - 3 - 1, 3);
            //右下
            linePoints[2] = new Point(Width - 3 - 1, Height - 3 - 1);
            //左下
            linePoints[3] = new Point(3, Height - 3 - 1);
            //左上
            linePoints[4] = new Point(3, 3);

            Pen pen = new Pen(Color.Black, 1) { DashStyle = DashStyle.Dot };

            g.DrawLines(pen, linePoints);

            #endregion

            #region  8个小矩形

            //左上
            smallRects[0] = new Rectangle(new Point(0, 0), smallRectSize);
            //右上
            smallRects[1] = new Rectangle(new Point(Width - size - 1, 0), smallRectSize);
            //左下
            smallRects[2] = new Rectangle(new Point(0, Height - size - 1), smallRectSize);
            //右下
            smallRects[3] = new Rectangle(new Point(Width - size - 1, Height - size - 1), smallRectSize);
            //上中
            smallRects[4] = new Rectangle(new Point(Width / 2 - 1, 0), smallRectSize);
            //下中
            smallRects[5] = new Rectangle(new Point(Width / 2 - 1, Height - size - 1), smallRectSize);
            //左中
            smallRects[6] = new Rectangle(new Point(0, Height / 2 - size / 2), smallRectSize);
            //右中
            smallRects[7] = new Rectangle(new Point(Width - size - 1, Height / 2 - size / 2), smallRectSize);

            //填充矩形内部为白色
            g.FillRectangles(Brushes.White, smallRects);
            //绘制矩形
            g.DrawRectangles(Pens.Black, smallRects);

            #endregion
        }

        /// <summary>
        /// 绘制实线：4条实线，移动控件和调整控件大小时显示
        /// </summary>
        public void DrawSolids()
        {
            //隐藏边框
            Visible = false;

            Graphics g = control.CreateGraphics();
            int width = control.Width;
            int height = control.Height;
            Point[] points = new Point[5] { new Point(0,0),new Point(width - 1,0),
                    new Point(width - 1,height-1),new Point(0,height-1),new Point(0,0)};
            g.DrawLines(new Pen(Color.Gray, 3), points);
        }

        #endregion

        #region 调整控件大小

        /// <summary>
        /// 鼠标在控件中的位置
        /// </summary>
        enum MousePos
        {
            None,
            Top,
            Right,
            Bottom,
            Left,
            LeftTop,
            LeftBottom,
            RightTop,
            RightBottom
        }

        /// <summary>
        /// 鼠标在控件中的位置
        /// </summary>
        MousePos mousePos;

        /// <summary>
        /// 改变鼠标状态，鼠标在控件上，下，左，右，左上，右上，左下，右下，向上不同的光标
        /// </summary>
        private void SetCursor(int x, int y)
        {
            Point point = new Point(x, y);

            if (!controlRect.Contains(point))//不在边框局域内直接退出
            {
                Cursor.Current = Cursors.Arrow;
                return;
            }
            else if (smallRects[0].Contains(point))//左上
            {
                Cursor.Current = Cursors.SizeNWSE;
                mousePos = MousePos.LeftTop;
            }
            else if (smallRects[1].Contains(point))//右上
            {
                Cursor.Current = Cursors.SizeNESW;
                mousePos = MousePos.RightTop;
            }
            else if (smallRects[2].Contains(point))//左下
            {
                Cursor.Current = Cursors.SizeNESW;
                mousePos = MousePos.LeftBottom;
            }
            else if (smallRects[3].Contains(point))//右下
            {
                Cursor.Current = Cursors.SizeNWSE;
                mousePos = MousePos.RightBottom;
            }

            else if (borderRects[0].Contains(point))//上
            {
                Cursor.Current = Cursors.SizeNS;
                mousePos = MousePos.Top;
            }
            else if (borderRects[1].Contains(point))//左
            {
                Cursor.Current = Cursors.SizeWE;
                mousePos = MousePos.Left;
            }
            else if (borderRects[2].Contains(point))//下
            {
                Cursor.Current = Cursors.SizeNS;
                mousePos = MousePos.Bottom;
            }
            else if (borderRects[3].Contains(point))//右
            {
                Cursor.Current = Cursors.SizeWE;
                mousePos = MousePos.Right;
            }
            else
            {
                Cursor.Current = Cursors.Arrow;
            }
        }

        /// <summary>
        /// 控件的最小高度
        /// </summary>
        const int MinHeight = 10;
        /// <summary>
        /// 控件的最小宽度
        /// </summary>
        const int MinWeight = 10;

        /// <summary>
        /// 调整控件大小：鼠标在控件中的不同位置，调整控件大小
        /// </summary>
        private void ReCtrlSize()
        {
            //获取当前鼠标位置
            Point currentPoint = Cursor.Position;
            int x = currentPoint.X - lastPoint.X;
            int y = currentPoint.Y - lastPoint.Y;
            switch (mousePos)
            {
                case MousePos.None:
                    break;
                case MousePos.Top://上，调整
                    if (control.Height - y > MinHeight)
                    {
                        control.Top += y;
                        control.Height -= y;
                    }
                    break;
                case MousePos.Right:
                    if (control.Width + x > MinWeight)
                    {
                        control.Width += x;
                    }
                    break;
                case MousePos.Bottom:
                    if (control.Height + y > MinHeight)
                    {
                        control.Height += y;
                    }
                    break;
                case MousePos.Left:
                    if (control.Width - x > MinWeight)
                    {
                        control.Left += x;
                        control.Width -= x;
                    }
                    break;
                case MousePos.LeftTop://左上
                    if (control.Width - x > MinWeight)
                    {
                        control.Left += x;
                        control.Width -= x;
                    }
                    if (control.Height - y > MinHeight)
                    {
                        control.Top += y;
                        control.Height -= y;
                    }
                    break;
                case MousePos.LeftBottom:
                    if (control.Width - x > MinWeight)
                    {
                        control.Left += x;
                        control.Width -= x;
                    }
                    if (control.Height + y > MinHeight)
                    {
                        control.Height += y;
                    }
                    break;
                case MousePos.RightTop:
                    if (control.Width + x > MinWeight)
                    {
                        control.Width += x;
                    }
                    if (control.Height - y > MinHeight)
                    {
                        control.Top += y;
                        control.Height -= y;
                    }
                    break;
                case MousePos.RightBottom:
                    if (control.Width + x > MinWeight)
                    {
                        control.Width += x;
                    }
                    if (control.Height + y > MinHeight)
                    {
                        control.Height += y;
                    }
                    break;
                default:
                    break;
            }
            lastPoint = Cursor.Position;
        }

        #endregion

    }
}
