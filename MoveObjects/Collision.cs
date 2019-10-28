using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MoveObjects
{
    public class Collision : EventArgs
    {
        private Point point;
        private Label label;
        public Collision(Label _label)
        {
            label = _label;
        }
        public void DoCollision(Figure figure, Point _point)
        {
            figure.changeColor(Color.Red);
            point = _point;
            label.Text = $"Координаты пересечения: X = {point.X} Y = {point.Y}";
        }
    }
}
