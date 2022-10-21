using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu;
using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.Structure;
using Emgu.Util;
using DirectShowLib;

namespace II_lab1
{
    public partial class Form1 : Form
    {
        private VideoCapture capture = null;
        private DsDevice[] webCams = null;
        private int selectedCameraid = 0;
        private static CascadeClassifier classifierface = new CascadeClassifier("haarcascade_frontalface_alt_tree.xml");
        private static CascadeClassifier classifiereye = new CascadeClassifier("haarcascade_eye.xml");
        public Form1()
        {
            InitializeComponent();
        }
        private Bitmap Find(Bitmap image)
        {
            Bitmap bitmap = new Bitmap(image);
            Image<Bgr, byte> grayImage = new Image<Bgr, byte>(bitmap);

            Rectangle[] faces = classifierface.DetectMultiScale(grayImage, 1.01, 2);
            foreach (Rectangle face in faces)
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    using (Pen pen = new Pen(Color.Blue, 3))
                    {
                        graphics.DrawRectangle(pen, face);
                    }
                }
            }

            Rectangle[] eyes = classifiereye.DetectMultiScale(grayImage, 1.08, 13);
            //int recCount = 9;
            //Rectangle[] eyes = classifiereye.DetectMultiScale(grayImage, 1.08, recCount); ;
            //while (eyes.Count() % 2 != 0 || eyes.Count() > 20)
            //{
            //    eyes = classifiereye.DetectMultiScale(grayImage, 1.08, recCount);
            //    recCount++;
            //}

            foreach (Rectangle eye in eyes)
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    using (Pen pen = new Pen(Color.White, 3))
                    {
                        graphics.DrawRectangle(pen, eye);
                    }
                }
            }
            return bitmap;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            webCams = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice); 
            for (int i =0; i < webCams.Length; i++)
            {
                toolStripComboBox1.Items.Add(webCams[i].Name);
            }
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (webCams.Length == 0) throw new Exception("Нет доступных камер!");
                else if (toolStripComboBox1.SelectedItem == null) throw new Exception("Необходимо выбрать камеру!!");
                else if (capture != null) capture.Start();
                else
                {
                    capture = new VideoCapture(selectedCameraid);
                    capture.ImageGrabbed += Capture_ImageGrabbed;
                    capture.Start();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Capture_ImageGrabbed(object sender, EventArgs e)
        {
            try
            {
                Mat m = new Mat();
                capture.Retrieve(m);
                Bitmap outImage = Find(m.ToImage<Bgr, byte>().Flip(Emgu.CV.CvEnum.FlipType.Horizontal).Bitmap);
                pictureBox1.Image = outImage;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PauseButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (capture != null)
                {
                    capture.Pause();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (capture != null)
                {
                    capture.Pause();
                    capture.Dispose();
                    capture = null;
                    pictureBox1.Image.Dispose();
                    pictureBox1.Image = null;
                    selectedCameraid = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedCameraid = toolStripComboBox1.SelectedIndex;
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        Point LastPoint;
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - LastPoint.X;
                this.Top += e.Y - LastPoint.Y;
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            LastPoint = new Point(e.X, e.Y);
        }

        //private void RecognizeButton_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        DialogResult res = openFileDialog1.ShowDialog();
        //        if (res == DialogResult.OK)
        //        {
        //            string path = openFileDialog1.FileName;
        //            pictureBox1.Image = Image.FromFile(path);
        //            Bitmap bitmap = new Bitmap(pictureBox1.Image);
        //            Image<Bgr, byte> grayImage = new Image<Bgr, byte>(bitmap);
        //            Rectangle[] faces = classifierface.DetectMultiScale(grayImage, 1.01,2);
        //            foreach (Rectangle face in faces)
        //            {
        //                using (Graphics graphics = Graphics.FromImage(bitmap))
        //                {
        //                    using (Pen pen = new Pen(Color.Red, 3))
        //                    {
        //                        graphics.DrawRectangle(pen, face);
        //                    }
        //                }
        //            }
        //            Rectangle[] eyes = classifiereye.DetectMultiScale(grayImage,1.08,13);
        //            foreach (Rectangle eye in eyes)
        //            {
        //                using (Graphics graphics = Graphics.FromImage(bitmap))
        //                {
        //                    using (Pen pen = new Pen(Color.Yellow, 3))
        //                    {
        //                        graphics.DrawRectangle(pen, eye);
        //                    }
        //                }
        //            }
        //            pictureBox1.Image = bitmap;
        //        }   
        //        else
        //        {
        //            MessageBox.Show("Изображение не выбрано", "Ошибка");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "Ошибка");
        //    }
        //}
    }
}
