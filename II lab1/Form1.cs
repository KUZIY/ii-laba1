using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;

namespace II_lab1
{
    public partial class Form1 : Form
    {
        private static CascadeClassifier classifierface = new CascadeClassifier("haarcascade_frontalface_alt_tree.xml");
        private static CascadeClassifier classifiereye = new CascadeClassifier("haarcascade_eye.xml");
        public Form1()
        {
            InitializeComponent();
        }

        private void RecognizeButton_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult res = openFileDialog1.ShowDialog();
                if (res == DialogResult.OK)
                {
                    string path = openFileDialog1.FileName;
                    pictureBox1.Image = Image.FromFile(path);
                    Bitmap bitmap = new Bitmap(pictureBox1.Image);
                    Image<Bgr, byte> grayImage = new Image<Bgr, byte>(bitmap);
                    Rectangle[] faces = classifierface.DetectMultiScale(grayImage, 1.01,2);
                    foreach (Rectangle face in faces)
                    {
                        using (Graphics graphics = Graphics.FromImage(bitmap))
                        {
                            using (Pen pen = new Pen(Color.Red, 3))
                            {
                                graphics.DrawRectangle(pen, face);
                            }
                        }
                    }
                    Rectangle[] eyes = classifiereye.DetectMultiScale(grayImage,1.08,13);
                    foreach (Rectangle eye in eyes)
                    {
                        using (Graphics graphics = Graphics.FromImage(bitmap))
                        {
                            using (Pen pen = new Pen(Color.Yellow, 3))
                            {
                                graphics.DrawRectangle(pen, eye);
                            }
                        }
                    }
                    pictureBox1.Image = bitmap;
                }   
                else
                {
                    MessageBox.Show("Изображение не выбрано", "Ошибка");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }
    }
}
