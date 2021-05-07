using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projeto_3D
{
    class ClsCube
    {
        Model myModel;


        public ClsCube(GraphicsDevice device, Model m)
        {
            myModel = m;
        }


        public void Draw(GraphicsDevice device, Camera camara)
        {
            
            foreach (ModelMesh mesh in myModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = Matrix.CreateTranslation(new Vector3(0.0f,0.5f,0.0f));//assentar o cubo no chao
                    effect.View = camara.viewMatrix;
                    effect.Projection = camara.projectionMatrix;
                    effect.EnableDefaultLighting();
                }
                // Draw each mesh of the model
                mesh.Draw();
            }
        }
    }
}



