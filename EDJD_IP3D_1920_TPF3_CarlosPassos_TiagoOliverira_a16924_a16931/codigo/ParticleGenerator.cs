using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projeto_3D
{
    class ParticleGenerator
    {
        Random rnd = new Random();
        Vector3 center = new Vector3(60, 100f, 60);
        float radius;

        public ParticleGenerator()
        {
            radius = 50f;
        }

        public Vector3 Spawn(Vector3 pos)
        {
            //Circulo onde poderá dar spawn cada particula 
            //361 para fazer [0,360]
            pos = center + radius * (float)rnd.NextDouble() * 1.6f * new Vector3((float)Math.Cos(rnd.Next(0, 361)), 0, (float)Math.Sin(rnd.Next(0, 361)));
            return pos;

        }
        public Vector3 Spawn2(Vector3 center, Vector3 right, Matrix rot, Vector3 normal)
        {
            Vector3 pos;
            Vector3 width = new Vector3(-0.6f, 0, -0.8f);
            rot = Matrix.CreateTranslation(width) * rot;
            width = Vector3.Transform(width, rot);

           // centro.Y -= normal.Y;
            pos = ((float)rnd.NextDouble() * 2) * -1 * width;
            pos += center;
            return pos;
        }

    }
}
