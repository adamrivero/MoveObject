using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace MoveObjects
{
    [DataContract]
    public class Figure
    {
        [DataMember]
        public int Width { get; set; }
        [DataMember]
        public int Height { get; set; }
        [DataMember]
        public int dx { get; set; }
        [DataMember]
        public int dy { get; set; }
        [DataMember]
        public bool down { get; set; }
        [DataMember]
        public bool right { get; set; }
        [DataMember]
        public int speed { get; set; }
        [DataMember]
        public Color color { get; set; }
        [XmlElement("color")]
        public int colorAsArgb
        {
            get { return color.ToArgb(); }
            set { color = Color.FromArgb(value); }
        }
        protected Pen pen;
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
    [DataContract]
    public class Rectangle : Figure
    {
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
    [DataContract]
    public class Circle : Figure
    {
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
    [DataContract]
    public class Triangle : Figure
    {
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
