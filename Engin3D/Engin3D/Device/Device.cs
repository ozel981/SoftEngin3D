using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engin3D.Scene;
using Engin3D.Screen;
using Engin3D.Camera;
using System.Drawing;
using System.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Engin3D.Device
{
    class Device
    {

        public Device()
        {

        }

        // This method is called to clear the back buffer with a specific color
        /*public void Clear(Screen.Screen screen)
        {
            
        }*/

        // Once everything is ready, we can flush the back buffer
        // into the front buffer. 
        /*public void Present()
        {
            using (var stream = bmp.PixelBuffer.AsStream())
            {
                // writing our byte[] back buffer into our WriteableBitmap stream
                stream.Write(backBuffer, 0, backBuffer.Length);
            }
            // request a redraw of the entire bitmap
            bmp.Invalidate();
        }*/

        // Project takes some 3D coordinates and transform them
        // in 2D coordinates using the transformation matrix
        /*public Vector2 Project(Vector3 coord, Matrix transMat)
        {
            // transforming the coordinates
            var point = Vector3.TransformCoordinate(coord, transMat);
            // The transformed coordinates will be based on coordinate system
            // starting on the center of the screen. But drawing on screen normally starts
            // from top left. We then need to transform them again to have x:0, y:0 on top left.
            var x = point.X * bmp.PixelWidth + bmp.PixelWidth / 2.0f;
            var y = -point.Y * bmp.PixelHeight + bmp.PixelHeight / 2.0f;
            return (new Vector2(x, y));
        }*/

        // DrawPoint calls PutPixel but does the clipping operation before
        /*public void DrawPoint(Vector2 point)
        {
            // Clipping what's visible on screen
            if (point.X >= 0 && point.Y >= 0 && point.X < bmp.PixelWidth && point.Y < bmp.PixelHeight)
            {
                // Drawing a yellow point
                PutPixel((int)point.X, (int)point.Y, new Color4(1.0f, 1.0f, 0.0f, 1.0f));
            }
        }*/
        
        public static void Render(Scene.Scene scene, Camera.Camera camera, Screen.Screen screen)
        {
            
            // To understand this part, please read the prerequisites resources
            /*var viewMatrix = Matrix.LookAtLH(camera.Position, camera.Target, Vector3.UnitY);
            var projectionMatrix = Matrix.PerspectiveFovRH(0.78f,
                                                           (float)bmp.PixelWidth / bmp.PixelHeight,
                                                           0.01f, 1.0f);*/

            foreach (Mesh.Mesh mesh in scene.Meshes)
            {
                // Beware to apply rotation before translation 
               /* var worldMatrix = Matrix.RotationYawPitchRoll(mesh.Rotation.Y,
                                                              mesh.Rotation.X, mesh.Rotation.Z) *
                                  Matrix.Translation(mesh.Position);

                var transformMatrix = worldMatrix * viewMatrix * projectionMatrix;*/

                //Point[] points = Camera.Project(scene);

                foreach (Vector3 vertice in mesh.Vertices)
                {
                    Matrix<double> worldMatrix = DenseMatrix.OfArray(new double[,]{
                         {1,0,0,mesh.Position.Z},
                         {0,1,0,mesh.Position.X},
                         {0,0,1,mesh.Position.Y},
                         {0,0,0,1}
                        });
                    var point = camera.Project(vertice, worldMatrix);
                    // First, we project the 3D coordinates into the 2D space
                    //var point = Project(vertice, transformMatrix);
                    // Then we can draw on screen
                    screen.PutPixel(point, Color.Black);
                }
            }
        }
    }
}
