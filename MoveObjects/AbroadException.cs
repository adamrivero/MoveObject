using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveObjects
{
    class AbroadException : Exception
    {
        public Point point { get; set; }
        public DateTime date { get; set; }
        public Figure figure { get; set; }
        public AbroadException(string message, Point point, Figure figure)
            : base(message)
        {
            this.point = point;
            date = DateTime.Now;
            this.figure = figure;
        }
    }
}
