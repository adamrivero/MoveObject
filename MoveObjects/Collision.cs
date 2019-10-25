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
        public void DoCollision(Figure figure)
        {
            figure.changeColor(Color.Red);
        }
    }
}
