using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projeto_3D
{
    public class ClsTank : Collider
    {
        Model myModel;
        SphereCollider tankCollider;
        ParticleSystem sp;
        BulletManager bm;
        Game1 gm;
        Range range, rangeToShoot, rangePursuit, rangeRun;

        public enum Player
        {
            P1 = 0, P2 = 1, CPU = 2
        }
        public Player player;

        public enum Movement
        {
            FOLLOW = 0, RUN = 1, NOTMOVING = 2
        }
        public Movement move;

        public Vector3 cannonPos, cannonDir;
        public Vector3 position;
        Vector3 bPos;
        public Vector3 direction;
        Vector3 a;

        public bool moving = false;

        public Matrix rotation;

        public ModelBone[] wheelsBone;
        ModelBone turrentBone;
        ModelBone cannonBone;

        public Matrix[] wheelsTransform;
        public Matrix[] boneTransform;
        Matrix cannonTransform;
        Matrix turrentTransform;

        public float[] wheels;
        public float yaw = 0;
        float scale;
        float health;
        float turrentAngle = MathHelper.ToRadians(0f);// angulo de rotaçao
        float cannonAngle = 0.0f;
        float vel;
        float aMax, velMax;



        public ClsTank(GraphicsDevice device, Model m, Vector3 pos, Player p, Game1 gm)
        {
            sp = new ParticleSystem(device);
            bm = new BulletManager(gm);

            this.gm = gm;

            myModel = m;
            player = p;
            scale = 0.006f;
            position = pos;
            health = 100;

            wheels = new float[2];
            wheelsBone = new ModelBone[4];
            wheelsTransform = new Matrix[4];

            turrentBone = myModel.Bones["turret_geo"];
            cannonBone = myModel.Bones["canon_geo"];
            wheelsBone[0] = myModel.Bones["l_front_wheel_geo"];
            wheelsBone[1] = myModel.Bones["r_front_wheel_geo"];
            wheelsBone[2] = myModel.Bones["l_back_wheel_geo"];
            wheelsBone[3] = myModel.Bones["r_back_wheel_geo"];



            if (p == Player.P1)
            {
                vel = 10;
                yaw = MathHelper.ToRadians(-45f);
                range = new Range(20, position);
                rangeToShoot = new Range(10, position);
            }
            else if (p == Player.CPU || p == Player.P2)
            {
                vel = 5;
                aMax = 3;
                velMax = 8;
                yaw = MathHelper.ToRadians(90f);
                range = new Range(80, position);
                rangeToShoot = new Range(10, position);
                rangePursuit = new Range(15, position);
                rangeRun = new Range(5, position);
            }

            turrentTransform = turrentBone.Transform;
            cannonTransform = cannonBone.Transform;

            wheelsTransform[0] = wheelsBone[0].Transform;
            wheelsTransform[1] = wheelsBone[1].Transform;
            wheelsTransform[2] = wheelsBone[2].Transform;
            wheelsTransform[3] = wheelsBone[3].Transform;

            boneTransform = new Matrix[myModel.Bones.Count];

            tankCollider = new SphereCollider(position, 2f);
            gm.colliders.Add(this as Collider);
        }

        public void Update(KeyboardState kb, MouseState ms, GraphicsDevice gd, GameTime gt, MapGen mp)
        {
            rotation = Matrix.Identity;
            Matrix scl = Matrix.CreateScale(scale);
            Vector2 center = new Vector2(gd.Viewport.Width / 2, gd.Viewport.Height / 2);
            Vector3 dir;
            Vector3 newPos = position;

            float delta_X = ms.X - center.X;
            float delta_Y = ms.Y - center.Y;

            if (player == Player.CPU || player == Player.P2)
            {
                cannonAngle += 2 * delta_Y * MathHelper.ToRadians(2f) * (float)gt.ElapsedGameTime.TotalSeconds;
                turrentAngle += 2 * delta_X * MathHelper.ToRadians(-2f) * (float)gt.ElapsedGameTime.TotalSeconds;
            }

            if (player == Player.P1 && Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                cannonAngle -= 2 * 40f * MathHelper.ToRadians(2f) * (float)gt.ElapsedGameTime.TotalSeconds;
            }
            else if (player == Player.P1 && Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                cannonAngle += 2 * 40f * MathHelper.ToRadians(2f) * (float)gt.ElapsedGameTime.TotalSeconds;
            }

            if (player == Player.P1 && Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                turrentAngle += 2 * 40f * MathHelper.ToRadians(-2f) * (float)gt.ElapsedGameTime.TotalSeconds;
            }
            else if (player == Player.P1 && Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                turrentAngle -= 2 * 40f * MathHelper.ToRadians(-2f) * (float)gt.ElapsedGameTime.TotalSeconds;
            }


            if (MathHelper.ToDegrees(turrentAngle) < -90)
            {
                turrentAngle = MathHelper.ToRadians(-90f);
            }
            else if (MathHelper.ToDegrees(turrentAngle) > 90)
            {
                turrentAngle = MathHelper.ToRadians(90f);
            }
            if (MathHelper.ToDegrees(cannonAngle) > 0)
            {
                cannonAngle = MathHelper.ToRadians(0);
            }
            else if (MathHelper.ToDegrees(cannonAngle) < -90)
            {
                cannonAngle = MathHelper.ToRadians(-90);
            }

            Vector3 direction = Vector3.Transform(Vector3.UnitX, Matrix.CreateFromYawPitchRoll(yaw, 0, 0));

            newPos.Y = mp.Heigth(position.X, position.Z);

            if (player == Player.P1)
            {
                if ((player == Player.P1 && Keyboard.GetState().IsKeyDown(Keys.A)))
                {
                    yaw += MathHelper.ToRadians(1f);
                    wheels[1] += MathHelper.ToRadians(1);
                }
                if ((player == Player.P1 && Keyboard.GetState().IsKeyDown(Keys.D)))
                {
                    yaw -= MathHelper.ToRadians(1f);
                    wheels[1] += MathHelper.ToRadians(1);
                }

                direction = Vector3.Transform(Vector3.UnitX, Matrix.CreateFromYawPitchRoll(yaw, 0, 0));

                if ((player == Player.P1 && Keyboard.GetState().IsKeyDown(Keys.W)))
                {
                    newPos = position + direction * vel * (float)gt.ElapsedGameTime.TotalSeconds;
                    newPos.Y = mp.Heigth(newPos.X, newPos.Z);
                    wheels[0] += 80 * MathHelper.ToRadians(2f) * (float)gt.ElapsedGameTime.TotalSeconds;
                    move = Movement.FOLLOW;
                }

                if ((player == Player.P1 && Keyboard.GetState().IsKeyDown(Keys.S)))
                {
                    newPos = position - direction * vel * (float)gt.ElapsedGameTime.TotalSeconds;
                    newPos.Y = mp.Heigth(newPos.X, newPos.Z);
                    wheels[0] += 80 * MathHelper.ToRadians(-2f) * (float)gt.ElapsedGameTime.TotalSeconds;
                    move = Movement.RUN;
                }
            }
            else if (player == Player.CPU)
            {
                direction = Vector3.Transform(Vector3.UnitX, Matrix.CreateFromYawPitchRoll(yaw, 0, 0));

                if (range.OnRange(gm.tank1.range))
                {
                    Vector3 directionSeek = gm.tank1.position - position;
                    directionSeek.Normalize();
                    Vector3 vseek = directionSeek * velMax;

                    Vector3 v = direction * vel;
                    a = (vseek - v);
                    a.Normalize();
                    a = a * aMax;

                    v = v + a * (float)gt.ElapsedGameTime.TotalSeconds;

                    direction = v;
                    direction.Normalize();

                    vel = v.Length();

                    yaw = (float)Math.Atan2(-direction.Z, direction.X);

                    if (move == Movement.RUN)
                    {
                        newPos = position - direction * vel * (float)gt.ElapsedGameTime.TotalSeconds;
                        newPos.Y = mp.Heigth(newPos.X, newPos.Z);
                    }
                    else if (move == Movement.FOLLOW)
                    {
                        newPos = position + direction * vel * (float)gt.ElapsedGameTime.TotalSeconds;
                        newPos.Y = mp.Heigth(newPos.X, newPos.Z);
                    }
                }
                else
                {
                    newPos = position + direction * vel * (float)gt.ElapsedGameTime.TotalSeconds;
                    newPos.Y = mp.Heigth(newPos.X, newPos.Z);
                }

            }
            else if (player == Player.P2)
            {
                if ((player == Player.P2 && Keyboard.GetState().IsKeyDown(Keys.J)))
                {
                    yaw += MathHelper.ToRadians(1f);
                    wheels[1] += MathHelper.ToRadians(1);
                }
                if ((player == Player.P2 && Keyboard.GetState().IsKeyDown(Keys.L)))
                {
                    yaw -= MathHelper.ToRadians(1f);
                    wheels[1] += MathHelper.ToRadians(1);
                }

                direction = Vector3.Transform(Vector3.UnitX, Matrix.CreateFromYawPitchRoll(yaw, 0, 0));

                if ((player == Player.P2 && Keyboard.GetState().IsKeyDown(Keys.I)))
                {
                    newPos = position + direction * vel * (float)gt.ElapsedGameTime.TotalSeconds;
                    newPos.Y = mp.Heigth(newPos.X, newPos.Z);
                    wheels[0] += 80 * MathHelper.ToRadians(2f) * (float)gt.ElapsedGameTime.TotalSeconds;
                    move = Movement.FOLLOW;
                }

                if ((player == Player.P2 && Keyboard.GetState().IsKeyDown(Keys.K)))
                {
                    newPos = position - direction * vel * (float)gt.ElapsedGameTime.TotalSeconds;
                    newPos.Y = mp.Heigth(newPos.X, newPos.Z);
                    wheels[0] += 80 * MathHelper.ToRadians(-2f) * (float)gt.ElapsedGameTime.TotalSeconds;
                    move = Movement.RUN;
                }
            }

            if (((newPos.X < mp.text.Width - 2 && newPos.X >= 1) && (newPos.Z < mp.text.Height - 2 && newPos.Z >= 1)))
            {
                if (position != newPos)
                {
                    moving = true;
                }
                else
                {
                    moving = false;
                }

                bPos = position;
                position = newPos;
            }
            else
            {
                moving = false;
            }

            range.Update(position);

            if (player == Player.CPU)
            {
                rangeToShoot.Update(position);
                rangePursuit.Update(position);
                rangeRun.Update(position);

                if (!rangePursuit.OnRange(gm.tank1.range))
                {
                    move = Movement.FOLLOW;
                }
                if (rangeToShoot.OnRangePos(gm.tank1.position))
                {
                    move = Movement.NOTMOVING;
                }
                if (rangeRun.OnRangePos(gm.tank1.position))
                {
                    move = Movement.RUN;
                }

            }

            this.direction = -direction;

            Vector3 normal = mp.normalFollow(position.X, position.Z);
            Vector3 right = Vector3.Cross(direction, normal);

            right.Normalize();
            dir = Vector3.Cross(normal, right);
            dir.Normalize();

            rotation.Forward = dir;
            rotation.Right = right;
            rotation.Up = normal;
            rotation.Backward = dir;
            rotation.Left = right;

            Matrix translation = Matrix.CreateTranslation(position);

            myModel.Root.Transform = scl * rotation * translation;
            wheelsBone[0].Transform = Matrix.CreateRotationX(wheels[0]) * wheelsTransform[0];
            wheelsBone[1].Transform = Matrix.CreateRotationX(wheels[0]) * wheelsTransform[1];
            wheelsBone[2].Transform = Matrix.CreateRotationX(wheels[0]) * wheelsTransform[2];
            wheelsBone[3].Transform = Matrix.CreateRotationX(wheels[0]) * wheelsTransform[3];
            turrentBone.Transform = Matrix.CreateRotationY(turrentAngle) * turrentTransform;

            cannonBone.Transform = Matrix.CreateRotationX(cannonAngle) * cannonTransform;
            myModel.CopyAbsoluteBoneTransformsTo(boneTransform);
            Mouse.SetPosition((int)center.X, (int)center.Y);

            cannonPos = boneTransform[cannonBone.Index].Translation;
            cannonDir = boneTransform[cannonBone.Index].Forward;
            cannonDir.Normalize();

            sp.Update2(gt, mp, this);
            bm.Update(gd, gt, this);
            tankCollider.Center = newPos;
        }

        public void Draw(GraphicsDevice device, Camera cam)
        {
            foreach (ModelMesh mesh in myModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = boneTransform[mesh.ParentBone.Index];// saber qual e o bone 
                    effect.View = cam.viewMatrix;
                    effect.Projection = cam.projectionMatrix;
                    effect.EnableDefaultLighting();
                }
                // Draw each mesh of the model
                mesh.Draw();
            }
            bm.Draw(device, cam);
            sp.Draw(device, cam);
        }


        public string Name() { return "Tank"; }

        public void CollisionWith(Collider other)
        {
            if (other.Name() == "Bullet")
            {
                health -= 20;
                Console.WriteLine("Vida:" + health);
                if (health <= 0)
                {
                    health = 100;
                    position = new Vector3(100, gm.mp.Heigth(100, 100), 100);
                    yaw = MathHelper.ToRadians(90f);

                }
            }

            if (other.Name() == "Tank")
            {
                position = bPos;
            }
        }

        public bool CollidesWith(Collider other)
        {
            return tankCollider.CollidesWith(other);
        }

        public Collider GetCollider()
        {
            return tankCollider;
        }
    }
}



