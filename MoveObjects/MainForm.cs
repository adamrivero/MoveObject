using PersonalLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;

namespace MoveObjects
{
    public partial class MainForm : Form
    {
        private Graphics g;
        private List<Figure> _figures = new List<Figure>();
        private List<Figure> currentFigure;
        private static Assembly assembly = Assembly.Load("MoveObjects");
        private static ResourceManager rm = new ResourceManager("MoveObjects.Language.Languages", assembly);
        private static CultureInfo cultureInfo;
        RandomXY rndXY;
        Collision collision;
        private int counter;
        public MainForm()
        {
            InitializeComponent();
            CreateNodes();
            collision = new Collision(collisionLabel);
            rndXY = new RandomXY(pictureBox_Main.Width, pictureBox_Main.Height);
        }
        public void CreateNodes()
        {
            treeView_main.Nodes.Add("Квадраты");
            treeView_main.Nodes.Add("Круги");
            treeView_main.Nodes.Add("Треугольники");
        }
        private void DrawFigure(Figure figure, string figureName, int nodeLevel)
        {
            TreeNode node = new TreeNode();
            figure.Draw(pictureBox_Main, g);
            _figures.Add(figure);
            node.Tag = figure;
            node.Text = $@"{rm.GetString(figureName, cultureInfo)} - {counter++}";
            treeView_main.Nodes[nodeLevel].Nodes.Add(node);
            MainTimer.Enabled = true;
        }
        private void buttonSquare_Click(object sender, EventArgs e)
        {
            Rectangle rectangle = new Rectangle(rndXY.GetX(), rndXY.GetY());
            DrawFigure(rectangle, "square", 0);
        }
        private void buttonCircle_Click(object sender, EventArgs e)
        {
            Circle circle = new Circle(rndXY.GetX(), rndXY.GetY());
            DrawFigure(circle, "circle", 1);
        }
        private void buttonTriangle_Click(object sender, EventArgs e)
        {
            Triangle triangle = new Triangle(rndXY.GetX(), rndXY.GetY());
            DrawFigure(triangle, "triangle", 2);
        }
        private void MainTimer_Tick(object sender, EventArgs e)
        {
            StreamWriter sw = new StreamWriter(@"..\..\Log\ExceptionReport.log", true, Encoding.UTF8);
            Refresh();
            try
            {
                foreach (Figure figure in _figures)
                {
                    currentFigure = null;
                    currentFigure = new List<Figure>(_figures);
                    currentFigure.Remove(figure);
                    foreach (Figure aimingFigure in currentFigure)
                    {
                        figure.Collision(aimingFigure);
                    }
                }
                foreach (Figure figure in _figures)
                {
                    figure.Move(pictureBox_Main, g);
                }
                label1.Text = $"{rm.GetString("elements", cultureInfo)}: {_figures.Count}";
            }
            catch (AbroadException ex)
            {
                sw.WriteLine(ex.Message + "\n" + "X: " + ex.point.X + " Y: " + ex.point.Y + "\n" + "Дата: " + ex.date);
                ex.figure.BackInBoard(pictureBox_Main);
                sw.Flush();
            }
            finally
            {
                if (sw != null)
                    sw.Close();
            }
        }
        private void treeView_main_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                Figure selectedFigure = (Figure)e.Node.Tag;
                trackBarSpeed.Value = selectedFigure.getSpeed();
                selectedFigure.SetBorder(8.0F);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void treeView_main_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                if (treeView_main.SelectedNode.IsSelected && treeView_main.SelectedNode.Level > 0)
                {
                    _figures.Remove((Figure)treeView_main.SelectedNode.Tag);
                    treeView_main.Nodes[e.Node.Level].Nodes.Remove(e.Node);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }
        private void trackBarSpeed_Scroll(object sender, EventArgs e)
        {
            if (treeView_main.SelectedNode.IsSelected && treeView_main.SelectedNode.Level > 0)
            {
                Figure selectedFigure = (Figure)treeView_main.SelectedNode.Tag;
                selectedFigure.changeSpeed(trackBarSpeed.Value);
            }
        }
        private void английскийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeLanguage("eu-US");
        }
        private void русскийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeLanguage("ru");
        }
        private void ChangeLanguage(string language)
        {
            cultureInfo = new CultureInfo(language);
            buttonSquare.Text = rm.GetString("square", cultureInfo);
            buttonCircle.Text = rm.GetString("circle", cultureInfo);
            buttonTriangle.Text = rm.GetString("triangle", cultureInfo);
            английскийToolStripMenuItem.Text = rm.GetString("eng", cultureInfo);
            русскийToolStripMenuItem.Text = rm.GetString("rus", cultureInfo);
            языкToolStripMenuItem.Text = rm.GetString("lang", cultureInfo);
            файлToolStripMenuItem.Text = rm.GetString("file", cultureInfo);
            сохранитьToolStripMenuItem.Text = rm.GetString("save", cultureInfo);
            загрузитьToolStripMenuItem.Text = rm.GetString("load", cultureInfo);
            label1.Text = rm.GetString("elements", cultureInfo);
        }
        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // BinaryFormatter formatter = new BinaryFormatter();
            BinaryFormatter formatter = new BinaryFormatter();
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "BIN file (*.bin)|*.bin";
                dialog.RestoreDirectory = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    using (Stream stream = dialog.OpenFile())
                    {
                        formatter.Serialize(stream, _figures);
                    }
                }
            }
        }
        private void загрузитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainTimer.Enabled = false;
            _figures = null;
            treeView_main.Nodes.Clear();
            CreateNodes();
            BinaryFormatter formatter = new BinaryFormatter();
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "Binary file (*.bin)|*.bin";
                dialog.RestoreDirectory = true;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    using (Stream stream = dialog.OpenFile())
                    {
                        _figures = (List<Figure>)formatter.Deserialize(stream);
                    }
                }
            }
            foreach (Figure figure in _figures)
            {
                if (figure.GetType() == typeof(Rectangle))
                {
                    TreeNode node = new TreeNode();
                    node.Tag = figure;
                    node.Text = $@"{rm.GetString("square", cultureInfo)} - {counter++}";
                    treeView_main.Nodes[0].Nodes.Add(node);
                }
                else if (figure.GetType() == typeof(Circle))
                {
                    TreeNode node = new TreeNode();
                    node.Tag = figure;
                    node.Text = $@"{rm.GetString("circle", cultureInfo)} - {counter++}";
                    treeView_main.Nodes[1].Nodes.Add(node);
                }
                else
                {
                    TreeNode node = new TreeNode();
                    node.Tag = figure;
                    node.Text = $@"{rm.GetString("triangle", cultureInfo)} - {counter++}";
                    treeView_main.Nodes[2].Nodes.Add(node);
                }
                figure.LoadColor();
            }
            MainTimer.Enabled = true;
        }
        private void buttonCollision_Click(object sender, EventArgs e)
        {
            if (treeView_main.SelectedNode.IsSelected && treeView_main.SelectedNode.Level > 0)
            {
                Figure selectedFigure = (Figure)treeView_main.SelectedNode.Tag;
                selectedFigure.OnCount += collision.DoCollision;
            }
            else MessageBox.Show("Что-бы добавить событие столкновения фигуре\nнеобходимо выбрать ее в дереве слева", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void buttonCollisionOff_Click(object sender, EventArgs e)
        {
            if (treeView_main.SelectedNode.IsSelected && treeView_main.SelectedNode.Level > 0)
            {
                Figure selectedFigure = (Figure)treeView_main.SelectedNode.Tag;
                selectedFigure.OnCount -= collision.DoCollision;
                selectedFigure.changeColor(Color.Black);
            }
            else MessageBox.Show("Что-бы удалить событие столкновения фигуре\nнеобходимо выбрать ее в дереве слева", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
