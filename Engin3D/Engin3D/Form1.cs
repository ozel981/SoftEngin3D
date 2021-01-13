
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Engin3D.Mesh;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Engin3D
{
    public partial class Form1 : Form
    {
        Scene.Scene scene = new Scene.Scene();
        Camera.Camera camera;
        Screen.Screen screen;
        public Form1()
        {
            InitializeComponent();
            Camera.CameraSettings settings = new Camera.CameraSettings
            {
                FieldOfView = 100,
                FarVerge = 100,
                NearVerge = 1,
                AspectRatio = (double)PictureBox.Height / (double)PictureBox.Width
            };
            Vector3[] vertices = new Vector3[]
            {
                new Vector3(-0.5f,-0.5f,-0.5f),
                new Vector3(0.5f,-0.5f,-0.5f),
                new Vector3(0.5f,0.5f,-0.5f),
                new Vector3(-0.5f,0.5f,-0.5f),
                new Vector3(-0.5f,-0.5f,0.5f),
                new Vector3(0.5f,-0.5f,0.5f),
                new Vector3(0.5f,0.5f,0.5f),
                new Vector3(-0.5f,0.5f,0.5f),
            };
            Face[] faces = new Face[]
            {
                new Face {A = 0, B = 1, C = 2},
                new Face {A = 0, B = 2, C = 3},
                new Face {A = 1, B = 5, C = 6},
                new Face {A = 1, B = 6, C = 2},
                new Face {A = 4, B = 0, C = 3},
                new Face {A = 4, B = 3, C = 7},
                new Face {A = 5, B = 7, C = 4},
                new Face {A = 5, B = 7, C = 6},
                new Face {A = 7, B = 2, C = 3},
                new Face {A = 7, B = 2, C = 6},
                new Face {A = 0, B = 5, C = 4},
                new Face {A = 0, B = 0, C = 1}
            };
            Mesh.Mesh block = new Mesh.Mesh("block", vertices, faces);
            block.Position = new Vector3(0, 0, 0);
            block.Rotation = new Vector3(0, 0, 0);
            
            scene.AddMesh(block);
            camera = new Camera.Camera(new Vector3(0,0,2),new Vector3(0,0,0), settings);
            screen = new Screen.Screen(ref PictureBox);
            Device.Device.Render(scene, camera, screen);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            using(Graphics graphics = Graphics.FromImage(PictureBox.Image))
            {
                graphics.Clear(Color.White);

            }
            //camera.Position = new Vector3((((TrackBar)sender).Value)/10, 0, 2);
            //scene.Meshes[0].Rotation = new Vector3((float)(((TrackBar)sender).Value * Math.PI / 180), 0, 0);
            Device.Device.Render(scene, camera, screen);
            PictureBox.Refresh();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        int val = 0;

        private void Timer_Tick(object sender, EventArgs e)
        {
            using (Graphics graphics = Graphics.FromImage(PictureBox.Image))
            {
                graphics.Clear(Color.White);

            }
            val++;
            val %= 360;
            float angle = (float)(val * Math.PI / 180);
            scene.Meshes[0].Rotation = new Vector3(angle, angle, 0);
            Device.Device.Render(scene, camera, screen);
            PictureBox.Refresh();
        }
    }
}
