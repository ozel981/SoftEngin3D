
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
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Engin3D
{
    public partial class Form1 : Form
    {
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
                new Vector3(-0.5f,-0.5f,0),
                new Vector3(0.5f,-0.5f,0),
                new Vector3(0.5f,0.5f,0),
                new Vector3(-0.5f,0.5f,0),
                new Vector3(-0.5f,-0.5f,1),
                new Vector3(0.5f,-0.5f,1),
                new Vector3(0.5f,0.5f,1),
                new Vector3(-0.5f,0.5f,1),
            };
            Mesh.Mesh block = new Mesh.Mesh("block", vertices);
            block.Position = new Vector3(0, 0, 0);
            Scene.Scene scene = new Scene.Scene();
            scene.AddMesh(block);
            Camera.Camera camera = new Camera.Camera(new Vector3(0,0,2),new Vector3(0,0,0), settings);
            Screen.Screen screen = new Screen.Screen(ref PictureBox);
            Device.Device.Render(scene, camera, screen);
        }
    }
}
