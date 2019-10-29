using System;
using System.Drawing;
using System.Windows.Forms;

namespace MoveObjects
{
    [Serializable]
    public class Figure
    {
        protected int Width { get; set; }
        protected int Height { get; set; }
        protected Point point;
        protected int dx { get; set; }
        protected int dy { get; set; }
        protected bool down { get; set; }
        protected bool right { get; set; }
        protected int speed { get; set; }
        protected Color color { get; set; }
        public delegate void MethodContainer(Figure figure, Point point);
        private event MethodContainer onCount;
        [NonSerialized()] protected Pen pen;
        public event MethodContainer OnCount
        {
            add { onCount += value; }
            remove { onCount -= value; }
        }
        public Figure(int width, int height)
        {
            point = new Point(width, height);
            this.Width = width;
            this.Height = height;
        }
        public virtual void Draw(PictureBox pictureBox, Graphics g)
        {
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
                point.X += speed;
                point.Y += speed;
            }
            else if (!down && right)
            {
                point.Y -= speed;
                point.X += speed;
            }
            else if (!down && !right)
            {
                point.Y -= speed;
                point.X -= speed;
            }
            else if (down && !right)
            {
                point.Y += speed;
                point.X -= speed;
            }

            if (point.Y > pictureBox.Height - 65)
                down = false;
            if (point.X > pictureBox.Width - 65)
                right = false;
            if (point.Y < 0)
                down = true;
            if (point.X < 0)
                right = true;
            if (point.Y > pictureBox.Height)
                throw new AbroadException("Фигура находится вне границ объекта PictureBox", point, this);
            if (point.X > pictureBox.Width)
                throw new AbroadException("Фигура находится вне границ объекта PictureBox", point, this);
            if (point.Y < -5)
                throw new AbroadException("Фигура находится вне границ объекта PictureBox", point, this);
            if (point.X < -5)
                throw new AbroadException("Фигура находится вне границ объекта PictureBox", point, this);
        }
        public void BackInBoard(PictureBox pictureBox)
        {
            if (pictureBox.Width - 65 < point.X)
                point.X = pictureBox.Width - 65;
            if (pictureBox.Height - 65 < point.Y)
                point.Y = pictureBox.Height - 65;
        }
        public void Collision(Figure figure)
        {
            int X1, X2, Y1, Y2;
            X1 = point.X; X2 = figure.GetPoint().X; Y1 = point.Y; Y2 = figure.GetPoint().Y;

            if (((X2 <= X1 + 65 && Y2 <= Y1 + 65) && (X2 >= X1 && Y2 >= Y1)) || (X2 >= X1 && X2 <= X1 + 65) && (Y2 + 65 > Y1 && Y2 + 65 <= Y1 + 65))
            {
                if (figure.GetType() == this.GetType())
                {
                    onCount?.Invoke(this, figure.GetPoint());
                }
            }
            else if (((X1 <= X2 + 65 && Y1 <= Y2 + 65) && (X1 >= X2 && Y1 >= Y2)) || (X1 >= X2 && X1 <= X2 + 65) && (Y1 + 65 > Y2 && Y1 + 65 <= Y2 + 65))
            {
                if (figure.GetType() == this.GetType())
                {
                    onCount?.Invoke(this, point);
                }
            }
            else
            {
                changeColor(Color.Black);
                figure.changeColor(Color.Black);
            }
        }
        public Point GetPoint()
        {
            return point;
        }
        public void LoadColor()
        {
            pen = new Pen(color);
            pen.Width = 4.0F;
        }
        public void SetBorder(float width)
        {
            pen.Width = width;
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
    [Serializable]
    public class Rectangle : Figure
    {
        public Rectangle(int width, int height) : base(width, height)
        {
        }
        public override void Draw(PictureBox pictureBox, Graphics g)
        {
            base.Draw(pictureBox, g);
            g = pictureBox.CreateGraphics();
            g.DrawRectangle(pen, point.X, point.Y, 65, 65);
        }

        public override void Move(PictureBox pictureBox, Graphics g)
        {
            base.Move(pictureBox, g);
            g = pictureBox.CreateGraphics();
            g.DrawRectangle(pen, point.X, point.Y, 65, 65);
        }
    }
    [Serializable]
    public class Circle : Figure
    {
        public Circle(int width, int height) : base(width, height)
        {
        }
        public override void Draw(PictureBox pictureBox, Graphics g)
        {
            base.Draw(pictureBox, g);
            g = pictureBox.CreateGraphics();
            g.DrawEllipse(pen, point.X, point.Y, 65, 65);
        }
        public override void Move(PictureBox pictureBox, Graphics g)
        {
            base.Move(pictureBox, g);
            g = pictureBox.CreateGraphics();
            g.DrawEllipse(pen, point.X, point.Y, 65, 65);
        }
    }
    [Serializable]
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
                new Point(point.X, point.Y),
                new Point(point.X, point.Y + 65),
                new Point(point.X + 65, point.Y + 65)
            };
            g = pictureBox.CreateGraphics();
            g.DrawPolygon(pen, curvePoints);
        }
        public override void Move(PictureBox pictureBox, Graphics g)
        {
            base.Move(pictureBox, g);
            Point[] curvePoints =
            {
                new Point(point.X, point.Y),
                new Point(point.X, point.Y + 65),
                new Point(point.X + 65, point.Y + 65)
            };
            g = pictureBox.CreateGraphics();
            g.DrawPolygon(pen, curvePoints);
        }
    }
}
