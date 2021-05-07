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
    class TankBullet : Collider
    {
        Model sphere;
        Game1 gm;
        Vector3 direction, position, vel, a;
        public float t, t0;
        Segment sphereCol;
        Matrix[] boneTransform;
        public bool isColliding = false;

        public TankBullet(GraphicsDevice gd, Game1 gm, Vector3 pos, Vector3 dir, Vector3 v, float ti)
        {
            this.t0 = ti;
            this.vel = v;
            this.position = pos;
            this.direction = dir * 0.5f;
            sphereCol = new Segment(pos,pos);
            this.gm = gm;
            gm.colliders.Add(this as Collider);
            vel = direction;

            sphere = gm.Content.Load<Model>("bulletTank");

            boneTransform = new Matrix[sphere.Bones.Count];
        }

        public bool Update(GameTime gt)
        {
            sphereCol.pos1 = position;
            float y = 0;
            t += (float)gt.ElapsedGameTime.TotalSeconds;
            a = Vector3.Down * 9.8f;

            vel += (direction + a) * (float)gt.ElapsedGameTime.TotalSeconds;
            position += vel * (float)gt.ElapsedGameTime.TotalSeconds;

            Matrix scale = Matrix.CreateScale(0.0010f);
            Matrix trans = Matrix.CreateTranslation(position);

            sphere.Root.Transform = scale * trans;

            sphere.CopyAbsoluteBoneTransformsTo(boneTransform);
            sphereCol.pos2 = position;

            if (((position.X < gm.mp.text.Width - 2 && position.X >= 1) && (position.Z < gm.mp.text.Height - 2 && position.Z >= 1)))
            {
                y = gm.mp.Heigth(position.X, position.Z);
            }

            return (position.Y < 0 || (t - t0 > 4));
        }

        public void Draw(GraphicsDevice gd, Camera camera)
        {
            foreach (ModelMesh mesh in sphere.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = boneTransform[mesh.ParentBone.Index];
                    effect.View = camera.viewMatrix;
                    effect.Projection = camera.projectionMatrix;
                    effect.EnableDefaultLighting();
                }
                // Draw each mesh of the model
                mesh.Draw();
            }
        }

        public string Name() { return "Bullet"; }

        public virtual void CollisionWith(Collider other)
        {
            if (other.Name() == "Tank")
            {
                isColliding = true;
                Console.WriteLine("Colisão");
            }
        }

        public bool CollidesWith(Collider other)
        {
            return sphereCol.CollidesWith(other);
        }

        public Collider GetCollider()
        {
            return sphereCol;
        }
    }
}

