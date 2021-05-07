using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projeto_3D
{
    class Segment : Collider
    {
        public Vector3 pos1 { get; set; }
        public Vector3 pos2 { get; set; }

        public Segment(Vector3 apos, Vector3 bpos)
        {
            pos1 = bpos;
            pos2 = apos;
        }

        virtual public bool CollidesWith(SphereCollider other)
        {
            double a = Distancia(other.Center, pos2);
            double b = Distancia(other.Center, pos1);
            double c = Distancia(pos1, pos2);
            double sp = (a + b + c) / 3;

            double area = Math.Sqrt(sp * (sp - a) * (sp - b) * (sp - c));
            double d = 2 * area / c;

            if (d < other.Radius)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        virtual public bool CollidesWith(Collider other)
        {
            Collider collider = other.GetCollider();

            switch (collider)
            {
                case SphereCollider c:
                    return CollidesWith(c);
                default:
                    return false;
            }
        }

        public string Name() { return "undef"; }

        public void CollisionWith(Collider other) { }

        public Collider GetCollider()
        {
            return this;
        }

        private double Distancia(Vector3 p1, Vector3 p2)
        {
            float xdif = (p1.X - p2.X) * (p1.X - p2.X);
            float ydif = (p1.Y - p2.Y) * (p1.Y - p2.Y);
            float zdif = (p1.Z - p2.Z) * (p1.Z - p2.Z);
            double dist = Math.Sqrt(xdif + ydif + zdif);

            return dist;
        }
    }
}
