using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Projeto_3D
{
    class SphereCollider : Collider
    {
        public Vector3 Center { get; set; }
        public float Radius { get; set; }

        public SphereCollider(Vector3 center, float radius)
        {
            Center = center; Radius = radius;
        }

        virtual public bool CollidesWith(SphereCollider other)
        {
            float dist1 = (Center - other.Center).LengthSquared();
            float dist2 = (float)Math.Pow(Radius + other.Radius, 2f);
            return dist2 >= dist1;
        }
        virtual public bool CollidesWith(Segment other)
        {
           return other.CollidesWith(this);
        }
        virtual public bool CollidesWith(Collider other)
        {
            Collider collider = other.GetCollider();

            switch (collider)
            {
                case SphereCollider c:
                    return CollidesWith(c);
                case Segment s:
                    return CollidesWith(s);
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
 
    }
}
