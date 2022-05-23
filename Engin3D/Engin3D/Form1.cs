
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
using System.Text.Json;
using System.Text.Json.Serialization;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Xamarin.Forms.PlatformConfiguration;
using System.IO;
using Engin3D.Lighting;

namespace Engin3D
{
    public partial class Form1 : Form
    {
        PhongLightingModel PhongLightingModel = new PhongLightingModel();
        Scene.Scene scene = new Scene.Scene();
        Shading shading = Shading.CONSTANT;
        Camera.CameraSettings settings;
        Screen.Screen screen;
        public Form1()
        {
            InitializeComponent();
            PictureBox.Image = new Bitmap(PictureBox.Width, PictureBox.Height);
            screen = new Screen.Screen(ref PictureBox);
            settings = new Camera.CameraSettings
            {
                FieldOfView = 100,
                FarVerge = 100,
                NearVerge = 1,
                AspectRatio = (double)PictureBox.Height / (double)PictureBox.Width
            };
            scene.CamerasOrientation.Add(new Scene.CameraOrientation
            {
                Position = new Vector3D(0, 0, 3),
                Targer = new Vector3D(0, 0, 0),
                CameraSettings = settings
            });
            LoadStartMesh();
            
        }


        int val = 0;
        int avgFPS = 0;
        int iter = 0;
        int wayStage = -1;
        float roudStage = 0;

        DateTime timeNow = DateTime.Now;

        private void Timer_Tick(object sender, EventArgs e)
        {
            if(FogCheckBox.Checked)
            {
                PhongLightingModel.Fog = true;
                if(PhongLightingModel.FogIntencity > 4)
                {
                    PhongLightingModel.FogIntencity -= 0.2;
                }
            }
            else
            {
                if (PhongLightingModel.FogIntencity < 10)
                {
                    PhongLightingModel.FogIntencity += 0.2;
                }
                else
                {
                    PhongLightingModel.Fog = false;
                }
            }
            using (Graphics graphics = Graphics.FromImage(PictureBox.Image))
            {
                graphics.Clear(Color.White);

            }
            if (scene.Meshes.Count > 9)
            {
                switch (wayStage)
                {
                    case -1:
                        {
                            scene.Meshes[9].Position = new Vector3D(1, 0, 1);
                            scene.Meshes[9].Rotation = new Vector3D(0, 0, 0);
                            roudStage = 0;
                            wayStage = 0;
                        }
                        break;
                    case 0:
                        {
                            scene.Meshes[9].Position = new Vector3D(1, 0, scene.Meshes[9].Position.Z - 0.1);
                            if (scene.Meshes[9].Position.Z <= -1) wayStage = 1;
                        }
                        break;
                    case 1:
                        {
                            scene.Meshes[9].Rotation = new Vector3D(roudStage * Math.PI / 180, 0, 0);
                            roudStage += 9;
                            if (roudStage > 90) wayStage = 2;
                        }
                        break;
                    case 2:
                        {
                            scene.Meshes[9].Position = new Vector3D(scene.Meshes[9].Position.X - 0.1, 0, -1);
                            if (scene.Meshes[9].Position.X <= -1) wayStage = 3;
                        }
                        break;
                    case 3:
                        {
                            scene.Meshes[9].Rotation = new Vector3D(roudStage * Math.PI / 180, 0, 0);
                            roudStage += 9;
                            if (roudStage > 180) wayStage = 4;
                        }
                        break;
                    case 4:
                        {
                            scene.Meshes[9].Position = new Vector3D(-1, 0, scene.Meshes[9].Position.Z + 0.1);
                            if (scene.Meshes[9].Position.Z >= 1) wayStage = 5;
                        }
                        break;
                    case 5:
                        {
                            scene.Meshes[9].Rotation = new Vector3D(roudStage * Math.PI / 180, 0, 0);
                            roudStage += 9;
                            if (roudStage > 270) wayStage = 6;
                        }
                        break;
                    case 6:
                        {
                            scene.Meshes[9].Position = new Vector3D(scene.Meshes[9].Position.X + 0.1, 0, 1);
                            if (scene.Meshes[9].Position.X >= 1) wayStage = 7;
                        }
                        break;
                    case 7:
                        {
                            scene.Meshes[9].Rotation = new Vector3D(roudStage * Math.PI / 180, 0, 0);
                            roudStage += 9;
                            if (roudStage > 360) wayStage = -1;
                        }
                        break;
                }
                Device.Device.Render(scene, screen, shading, PhongLightingModel);               
                timeNow = DateTime.Now;
                Device.Device.Render(scene, screen, shading, PhongLightingModel);
                TimeSpan timeItTook = DateTime.Now - timeNow;
                if (timeItTook.Milliseconds == 0) this.Text = $"RealEngine3D | FPS:[oo]";
                else
                {
                    if (iter > 1000000)
                    {
                        avgFPS = 0;
                        iter = 0;
                    }
                    avgFPS += (int)(1000 / timeItTook.Milliseconds);
                    iter++;
                    this.Text = $"RealEngine3D | FPS:[{avgFPS/ iter}]";
                }
                PictureBox.Refresh();
            };

        }

        private async void LoadMeshButton_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                string path = Application.StartupPath;
                openFileDialog.InitialDirectory = path.Substring(0, path.Length - 9);
                openFileDialog.Filter = "all files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (FileStream openStream = File.OpenRead(openFileDialog.FileName))
                    {
                        MeshesCollection meshesCollection = await JsonSerializer.DeserializeAsync<MeshesCollection>(openStream);

                        AddMeshes(meshesCollection.meshes);
                    }
                }
            }
        }

        private async void LoadStartMesh()
        {
            string path = Application.StartupPath;
            int x = path.LastIndexOf("\\");
            path = path.Remove(path.LastIndexOf("\\"),path.Length - x);
            x = path.LastIndexOf("\\");
            path = path.Remove(path.LastIndexOf("\\"), path.Length - x);
            x = path.LastIndexOf("\\");
            path = path.Remove(path.LastIndexOf("\\"), path.Length - x);
            path += "\\Objects\\MyScene";
            using (FileStream openStream = File.OpenRead(path))
            {
                try
                {
                    scene = await JsonSerializer.DeserializeAsync<Scene.Scene>(openStream);
                    Device.Device.Render(scene, screen, shading, PhongLightingModel);
                    PictureBox.Refresh();
                }
                catch
                {
                    MessageBox.Show("Nie udało się załadować");
                }
            }
        }


        private void AddMeshes(MeshData[] meshes)
        {
            foreach (MeshData mesh in meshes)
            {
                List<Vertex> vectors = new List<Vertex>();
                Dictionary<Vector3D, Vector3D> newVectors = new Dictionary<Vector3D, Vector3D>();
                Dictionary<int, int> indexes = new Dictionary<int, int>();
                Dictionary<Vector3D, int> vectorIndexes = new Dictionary<Vector3D, int>();
                List<Vector3D> Normals = new List<Vector3D>();
                int index = 0;
                for (int i = 3; i <= mesh.positions.Length; i += 3)
                {
                    Normals.Add(new Vector3D(mesh.normals[i - 3], mesh.normals[i - 2], mesh.normals[i - 1]));
                    Vector3D newCoordinates = new Vector3D(mesh.positions[i - 3], mesh.positions[i - 2], mesh.positions[i - 1]);

                    if (newVectors.ContainsKey(newCoordinates))
                    {
                        newVectors[newCoordinates] += new Vector3D(mesh.normals[i - 3], mesh.normals[i - 2], mesh.normals[i - 1]);
                    }
                    else
                    {
                        vectorIndexes.Add(newCoordinates, index);
                        index++;
                        newVectors.Add(newCoordinates, new Vector3D(mesh.normals[i - 3], mesh.normals[i - 2], mesh.normals[i - 1]));
                    }
                    indexes.Add(i / 3 - 1, vectorIndexes[newCoordinates]);
                }
                foreach (var vector in newVectors)
                {
                    vectors.Add(
                        new Vertex
                        {
                            Coordinates = vector.Key,
                            Normal = vector.Value
                        });
                }
                List<Face> faces = new List<Face>();
                for (int i = 0; i < mesh.indices.Length; i += 3)
                {
                    faces.Add(new Face { A = indexes[mesh.indices[i]], B = indexes[mesh.indices[i + 1]], C = indexes[mesh.indices[i + 2]],
                    Vector = (Normals[mesh.indices[i]] + Normals[mesh.indices[i + 1]] + Normals[mesh.indices[i + 2]])/3});
                }           
                Mesh.Mesh newMesh = new Mesh.Mesh($"mesh", vectors.ToArray(), faces.ToArray());
                newMesh.Rotation = new Vector3D(0, 0, 0);
                newMesh.Position = new Vector3D(0, 0, 0);
                scene.Meshes.Clear();
                scene.AddMesh(newMesh);
                scene.CamerasOrientation.Clear();
                scene.Reflectors.Clear();
                scene.CamerasOrientation.Add(new Scene.CameraOrientation
                {
                    CameraSettings = settings,
                    Position = new Vector3D(0,3,3),
                    Targer = new Vector3D(0,0,0)
                });
                using (Graphics graphics = Graphics.FromImage(PictureBox.Image))
                {
                    graphics.Clear(Color.White);

                }
                Device.Device.Render(scene, screen, shading, PhongLightingModel);
                PictureBox.Refresh();
            }
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            using (Graphics graphics = Graphics.FromImage(PictureBox.Image))
            {
                graphics.Clear(Color.White);

            }
            var reflectorOrientation = scene.Reflectors[0];
            reflectorOrientation.Reflector = new Reflector
            {
                Color = reflectorOrientation.Reflector.Color,
                Position = reflectorOrientation.Reflector.Position,
                Target = new Vector3D(0, ((float)((TrackBar)sender).Value / 10), 0)
            };
            scene.Reflectors[0] = reflectorOrientation;
            Device.Device.Render(scene, screen, shading, PhongLightingModel);
            PictureBox.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Timer.Enabled = !Timer.Enabled;
            if (((Button)sender).Text == "Stop") ((Button)sender).Text = "Start";
            else ((Button)sender).Text = "Stop";
        }

        private void ConstantRadioButton_CheckedChanged(object sender, EventArgs e)
        { shading = Shading.CONSTANT; avgFPS = 0; iter = 0; }
        private void GouraudRadioButton_CheckedChanged(object sender, EventArgs e)
        { shading = Shading.GOURAUD; avgFPS = 0; iter = 0; }
        private void PhongRadioButton_CheckedChanged(object sender, EventArgs e)
        { shading = Shading.PHONG; avgFPS = 0; iter = 0; }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            Stream newStream;
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "all files (*.*)|*.*";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if ((newStream = saveFileDialog.OpenFile()) != null)
                {
                    try
                    {
                        var serivalizedScene = JsonSerializer.Serialize(scene);
                        StreamWriter streamWriter = new StreamWriter(newStream);
                        streamWriter.Write(serivalizedScene);
                        streamWriter.Close();
                        MessageBox.Show("Zapisano pomyślnie");
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Nie udało się zapisać");
                        return;
                    }
                    newStream.Close();
                }
            }
        }

        private async void LoadButton_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                string path = Application.StartupPath;
                int x = path.LastIndexOf("\\");
                path = path.Remove(path.LastIndexOf("\\"), path.Length - x);
                x = path.LastIndexOf("\\");
                path = path.Remove(path.LastIndexOf("\\"), path.Length - x);
                x = path.LastIndexOf("\\");
                path = path.Remove(path.LastIndexOf("\\"), path.Length - x);
                path += "\\Objects\\MyScene";
                openFileDialog.Filter = "all files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (FileStream openStream = File.OpenRead(openFileDialog.FileName))
                    {
                        try
                        {
                            scene = await JsonSerializer.DeserializeAsync<Scene.Scene>(openStream);
                        }
                        catch
                        {
                            MessageBox.Show("Nie udało się załadować");
                        }
                    }
                }
            }
        }


        float rotX = 0;
        private void ActionButton_Click(object sender, EventArgs e)
        {
            scene.Reflectors.Add(new Scene.ReflectorOrientation
            {
                Reflector = new Reflector
                {
                    Position = new Vector3D(0, 3, 0),
                    Target = new Vector3D(0, 3, 0),
                },
                StickPositionToMesh = 9,
                StickTargetToMesh = 9,
            });
            /*scene.Reflectors.Add(new Scene.ReflectorOrientation
            {
                Reflector = new Reflector
                {
                    Position = new Vector3D(2, 0, 0),
                    Target = new Vector3D(0, 0, 0),
                },
            });*/
            scene.PointLights.Add(new Scene.PointLightOrientation
            {
                PointLight = new PointLight
                {
                    Position = new Vector3D(-10, 10, 10)
                }
            });
            /*scene.Meshes[0].Color = Color.Gray;
            scene.Meshes[1].Color = Color.Purple;
            scene.Meshes[2].Color = Color.Blue;
            scene.Meshes[3].Color = Color.Yellow;
            scene.Meshes[4].Color = Color.Green;
            scene.Meshes[5].Color = Color.Orange;
            scene.Meshes[6].Color = Color.Brown;
            scene.Meshes[7].Color = Color.LightGray;
            scene.Meshes[8].Color = Color.Gold;
            scene.Meshes[9].Color = Color.Red;
            scene.Meshes[10].Color = Color.LightBlue;*/
        }

        private void StaticRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            scene.ActiveCameraIndex = 0;
        }

        private void FollowRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            scene.ActiveCameraIndex = 1;
        }

        private void StickRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            scene.ActiveCameraIndex = 2;
        }
    }
}
