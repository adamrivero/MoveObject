using System;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;

namespace MoveObjects
{
    public class Collision : EventArgs
    {
        private Point point;
        private Label label;
        public Collision(Label label)
        {
            this.label = label;
        }
        public void DoCollision(Figure figure, Point point)
        {
            figure.changeColor(Color.Red);
            this.point = point;
            label.Text = $"Координаты пересечения: X = {point.X} Y = {point.Y}";
        }
    }
}
