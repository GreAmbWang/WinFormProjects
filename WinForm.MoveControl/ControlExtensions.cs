using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WinForm.MoveControl
{
    public static class ControlExtensions
    {
        /// <summary>
        /// 设置控件移动和调整大小
        /// </summary>
        public static void SetMove(this Control control)
        {
            //边框控件
            FrameControl fControl = null;
            //上一个鼠标坐标
            Point lastPoint = new Point();

            #region 绑定事件

            //鼠标键按下时
            control.MouseDown += (sender, e) =>
            {
                //记录鼠标坐标
                lastPoint = Cursor.Position;

                //清除所有控件的边框区域，最主要的是清除上次点击控件的边框，恢复原来状态
                foreach (Control ctrl in control.Parent.Controls)
                    if (ctrl is FrameControl)
                        ctrl.Visible = false;
                if (fControl == null)
                    fControl = new FrameControl(control);
                //设置边框背景色为透明，可以设置其它颜色
                fControl.BackColor = Color.Transparent;
                //把边框控件添加到当前控件的父控件中
                control.Parent.Controls.Add(fControl);
            };

            //鼠标单击时
            control.MouseClick += (sender, e) =>
            {
                control.BringToFront();
            };

            //鼠标在控件上移动时
            control.MouseMove += (sender, e) =>
            {
                Point currentPoint = new Point();
                Cursor.Current = Cursors.SizeAll;
                if (e.Button == MouseButtons.Left)
                {
                    currentPoint = Cursor.Position;
                    control.Location = new Point(control.Location.X + currentPoint.X - lastPoint.X,
                        control.Location.Y + currentPoint.Y - lastPoint.Y);

                    //移动时刷新实线
                    fControl.DrawSolids();

                    control.BringToFront();
                }

                lastPoint = currentPoint;
            };

            //鼠标键释放时
            control.MouseUp += (sender, e) =>
            {
                //设置控件区域
                fControl.SetControlRegion();
            };

            #endregion
        }
    }
}
