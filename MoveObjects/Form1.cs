using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Xml.Serialization;
using PersonalLibrary;

namespace MoveObjects
{
    public partial class Form1 : Form
    {
        private Graphics g;
        private List<Figure> _figures = new List<Figure>();
        private List<Figure> currentFigure;
        private static Assembly assembly = Assembly.Load("MoveObjects");
        private static ResourceManager rm = new ResourceManager("MoveObjects.Language.Languages", assembly);
        private static CultureInfo cultureInfo;
        RandomXY rndXY;
        private int counter;

        public Form1()
        {
            InitializeComponent();
            CreateNodes();
        }
        public void CreateNodes()
        {
            treeView_main.Nodes.Add("Квадраты");
            treeView_main.Nodes.Add("Круги");
            treeView_main.Nodes.Add("Треугольники");
        }
        private void buttonSquare_Click(object sender, EventArgs e)
        {
            TreeNode node = new TreeNode();
            rndXY = new RandomXY(pictureBox_Main.Width, pictureBox_Main.Height);
            Rectangle rectangle = new Rectangle(rndXY.GetX(), rndXY.GetY());
            rectangle.Draw(pictureBox_Main, g);
            _figures.Add(rectangle);
            node.Tag = rectangle;
            node.Text = $@"{rm.GetString("square", cultureInfo)} - {counter++}";
            treeView_main.Nodes[0].Nodes.Add(node);
            MainTimer.Enabled = true;
        }

        private void buttonCircle_Click(object sender, EventArgs e)
        {
            TreeNode node = new TreeNode();
            rndXY = new RandomXY(pictureBox_Main.Width, pictureBox_Main.Height);
            Circle circle = new Circle(rndXY.GetX(), rndXY.GetY());
            circle.Draw(pictureBox_Main, g);
            _figures.Add(circle);
            node.Tag = circle;
            node.Text = $@"{rm.GetString("circle", cultureInfo)} - {counter++}";
            treeView_main.Nodes[1].Nodes.Add(node);
            MainTimer.Enabled = true;
        }

        private void buttonTriangle_Click(object sender, EventArgs e)
        {
            TreeNode node = new TreeNode();
            rndXY = new RandomXY(pictureBox_Main.Width, pictureBox_Main.Height);
            Triangle triangle = new Triangle(rndXY.GetX(), rndXY.GetY());
            triangle.Draw(pictureBox_Main, g);
            _figures.Add(triangle);
            node.Tag = triangle;
            node.Text = $@"{rm.GetString("triangle", cultureInfo)} - {counter++}";
            treeView_main.Nodes[2].Nodes.Add(node);
            MainTimer.Enabled = true;
        }

        private void MainTimer_Tick(object sender, EventArgs e)
        {
            Refresh();
            try
            {
                foreach (Figure figure in _figures)
                {
                    currentFigure = null;
                    currentFigure = new List<Figure>(_figures);
                    currentFigure.Remove(figure);
                    foreach(Figure aimingFigure in currentFigure)
                    {
                        figure.Collision(aimingFigure);
                    }
                    figure.Move(pictureBox_Main, g);
                }

                label1.Text = $"{rm.GetString("elements", cultureInfo)}: {_figures.Count}";
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, exception.Source);
            }
        }

        private void treeView_main_AfterSelect(object sender, TreeViewEventArgs e)
        {
        }

        private void treeView_main_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                Figure selectedFigure = (Figure) e.Node.Tag;
                trackBarSpeed.Value = selectedFigure.getSpeed();
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        private void treeView_main_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                _figures.Remove((Figure) treeView_main.SelectedNode.Tag);
                treeView_main.Nodes[e.Node.Level].Nodes.Remove(e.Node);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        private void buttonChangeColor_Click(object sender, EventArgs e)
        {
            Figure selectedFigure = (Figure)treeView_main.SelectedNode.Tag;
            selectedFigure.changeColor(Color.Brown);
        }

        private void trackBarSpeed_Scroll(object sender, EventArgs e)
        {
            try
            {
                Figure selectedFigure = (Figure) treeView_main.SelectedNode.Tag;
                selectedFigure.changeSpeed(trackBarSpeed.Value);
            }
            catch
            {
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
            buttonChangeColor.Text = rm.GetString("color", cultureInfo);
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
    }
}
