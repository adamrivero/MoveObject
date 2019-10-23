using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace MoveObjects
{
    [Serializable]
    public class Figure
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int dx { get; set; }
        public int dy { get; set; }
        public bool down { get; set; }
        public bool right { get; set; }
        public int speed { get; set; }
        [XmlIgnore]
        public Color color { get; set; }
        [XmlElement("color")]
        public int colorAsArgb
        {
            get { return color.ToArgb(); }
            set { color = Color.FromArgb(value); }
        }
        [NonSerialized()] protected Pen pen;

        public Figure()
        {

        }
        public Figure(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }
        public virtual void Draw(PictureBox pictureBox, Graphics g)
        {
            dx = Width;
            dy = Height;
            down = true;
            right = true;
            color = Color.Black;
            pen = new Pen(color);
            pen.Width = 4.0F;
            speed = 5;
        }
        public virtual void Move(PictureBox pictureBox, Graphics g)
        {
            if (down && right)
            {
                dx += speed;
                dy += speed;
            }
            else if (!down && right)
            {
                dy -= speed;
                dx += speed;
            }
            else if (!down && !right)
            {
                dy -= speed;
                dx -= speed;
            }
            else if (down && !right)
            {
                dy += speed;
                dx -= speed;
            }

            if (dy > pictureBox.Height - 65)
                down = false;
            if (dx > pictureBox.Width - 65)
                right = false;
            if (dy < 0)
                down = true;
            if (dx < 0)
                right = true;
        }
        public void LoadColor()
        {
            pen = new Pen(color);
            pen.Width = 4.0F;
        }
        public void changeColor(Color newColor)
        {
            color = newColor;
            pen.Color = color;
        }
        public void changeSpeed(int speed)
        {
            this.speed = speed;
        }

        public int getSpeed()
        {
            return speed;
        }
    }
    [XmlInclude(typeof(Rectangle))]
    [Serializable]
    public class Rectangle : Figure
    {
        public Rectangle()
        {
        }
        public Rectangle(int width, int height) : base(width, height)
        {
        }
        public override void Draw(PictureBox pictureBox, Graphics g)
        {
            base.Draw(pictureBox, g);
            g = pictureBox.CreateGraphics();
            g.DrawRectangle(pen, Width, Height, 65, 65);
        }

        public override void Move(PictureBox pictureBox, Graphics g)
        {
            base.Move(pictureBox, g);
            g = pictureBox.CreateGraphics();
            g.DrawRectangle(pen, dx, dy, 65, 65);
        }
    }
    [Serializable]
    [XmlInclude(typeof(Circle))]
    public class Circle : Figure
    {
        public Circle()
        {
        }
        public Circle(int width, int height) : base(width, height)
        {
        }
        public override void Draw(PictureBox pictureBox, Graphics g)
        {
            base.Draw(pictureBox, g);
            g = pictureBox.CreateGraphics();
            g.DrawEllipse(pen, Width, Height, 65, 65);
        }

        public override void Move(PictureBox pictureBox, Graphics g)
        {
            base.Move(pictureBox, g);
            g = pictureBox.CreateGraphics();
            g.DrawEllipse(pen, dx, dy, 65, 65);
        }
    }
    [Serializable]
    [XmlInclude(typeof(Triangle))]
    public class Triangle : Figure
    {
        public Triangle()
        {
        }
        public Triangle(int width, int height) : base(width, height)
        {
        }

        public override void Draw(PictureBox pictureBox, Graphics g)
        {
            base.Draw(pictureBox, g);
            Point[] curvePoints =
            {
                new Point(Width, Height),
                new Point(Width, Height + 65), 
                new Point(Width + 65, Height + 65)  
            };
            g = pictureBox.CreateGraphics();
            g.DrawPolygon(pen, curvePoints);
        }

        public override void Move(PictureBox pictureBox, Graphics g)
        {
            base.Move(pictureBox, g);
            Point[] curvePoints =
            {
                new Point(dx, dy), 
                new Point(dx, dy + 65), 
                new Point(dx + 65, dy + 65) 
            };
            g = pictureBox.CreateGraphics();
            g.DrawPolygon(pen, curvePoints);
        }
    }
}
