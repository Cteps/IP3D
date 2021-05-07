using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projeto_3D
{
    class Range
    {
        public float radius;
        public Vector3 center;

        public Range(float r, Vector3 pos)
        {
            radius = r;
            center = pos;
        }

        public void Update(Vector3 position)
        {
            center = position;
        }
        //Descobrir se raio colide com raio
        public bool OnRange(Range range)
        {
            float xdif = (center.X - range.center.X) * (center.X - range.center.X);
            float ydif = (center.Y - range.center.Y) * (center.Y - range.center.Y);
            float zdif = (center.Z - range.center.Z) * (center.Z - range.center.Z);
            double difference = Math.Sqrt(xdif + ydif + zdif);

            return difference < radius;
        }
        //Descobrir se esta dentro do raio usando a posição
        public bool OnRangePos(Vector3 pos) 
        {
            float xdif = (center.X - pos.X) * (center.X - pos.X);
            float ydif = (center.Y - pos.Y) * (center.Y - pos.Y);
            float zdif = (center.Z - pos.Z) * (center.Z - pos.Z);
            double difference = Math.Sqrt(xdif + ydif + zdif);

            return difference < radius;
        }
    }
}
