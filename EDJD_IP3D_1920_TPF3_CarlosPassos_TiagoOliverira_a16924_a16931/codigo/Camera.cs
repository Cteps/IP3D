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
    public class Camera
    {
        public Matrix viewMatrix, projectionMatrix;
        public Vector3 position, normal, direction;
        float farPlane, nearPlane, aspectRatio, fieldOfView;
        Game1 gm;
        float yaw, pitch;

        public enum cametyp
        {
            AERIAL = 0, FIRSTP = 1, SURFACE = 2, SECONDSTP = 3, GOD = 4
        }
        public cametyp cam = cametyp.AERIAL;

        public Camera(Game1 game, string texString, GraphicsDevice tela)
        {
            gm = game;
            position = new Vector3(3f, 10f, 3f);

            direction = new Vector3(1.0f, 0.0f, 1.0f);

            Vector3 target = position + direction;

            normal = Vector3.Up;
            viewMatrix = Matrix.CreateLookAt(position, target, normal);
            yaw = -40.0f;
            pitch = 0.0f;
            fieldOfView = MathHelper.ToRadians(45.0f);
            aspectRatio = (float)tela.Viewport.Width / tela.Viewport.Height;
            nearPlane = 0.1f;
            farPlane = 1000.0f;
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlane, farPlane);
        }

        public void Update(KeyboardState kb, MouseState ms, GameTime gt, Vector3 pos, Vector3 dir, GraphicsDevice tela)
        {
            if (kb.IsKeyDown(Keys.F1))
            {
                cam = cametyp.AERIAL;
            }
            else if (kb.IsKeyDown(Keys.F2))
            {
                cam = cametyp.SURFACE;
            }
            else if (kb.IsKeyDown(Keys.F3))
            {
                cam = cametyp.FIRSTP;
            }
            else if (kb.IsKeyDown(Keys.F4))
            {
                cam = cametyp.SECONDSTP;
            }
            else if (kb.IsKeyDown(Keys.F5))
            {
                cam = cametyp.GOD;
            }

            if (cam == cametyp.FIRSTP || cam == cametyp.SECONDSTP)
            {
                Vector3 novaPos = position;
                Vector3 right = Vector3.Cross(direction, gm.mp.normalFollow(pos.X, pos.Z));
                right.Normalize();

                if (dir != Vector3.Zero)
                {
                    this.position = pos + dir * 20 + 4 * Vector3.UnitY;
                    this.position.Y += 2;
                    direction = -dir;
                }
                Vector3 target = position + direction;

                viewMatrix = Matrix.CreateLookAt(position, target, gm.mp.normalFollow(pos.X, pos.Z));

            }
            else if (cam == cametyp.SURFACE)
            {
                Vector2 center = new Vector2(tela.Viewport.Width / 2, tela.Viewport.Height / 2);

                //Rotaçao da camara perante a posição do rato
                float delta_X = ms.X - center.X;
                float delta_Y = ms.Y - center.Y;
                yaw += 2 * (delta_X) * MathHelper.ToRadians(-2f) * (float)gt.ElapsedGameTime.TotalSeconds;
                pitch += 2 * (delta_Y) * MathHelper.ToRadians(2f) * (float)gt.ElapsedGameTime.TotalSeconds;

                Vector3 defaultDirection = new Vector3(0.0f, 0.0f, -1.0f);
                Matrix cameraRotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0.0f);
                Vector3 direction = Vector3.Transform(defaultDirection, cameraRotation);

                Vector3 newPos = position;
                Vector3 right = Vector3.Cross(direction, Vector3.Up);
                right.Normalize();

                if (kb.IsKeyDown(Keys.NumPad8))
                    newPos = position + direction * 5f * (float)gt.ElapsedGameTime.TotalSeconds;
                if (kb.IsKeyDown(Keys.NumPad5))
                    newPos = position - direction * 5f * (float)gt.ElapsedGameTime.TotalSeconds;
                if (kb.IsKeyDown(Keys.NumPad4))
                    newPos = position - right * 5f * (float)gt.ElapsedGameTime.TotalSeconds;
                if (kb.IsKeyDown(Keys.NumPad6))
                    newPos = position + right * 5f * (float)gt.ElapsedGameTime.TotalSeconds;

                //Extremos do mapa  e seus limites 
                if ((newPos.X > gm.mp.DEMwidth - 1 || newPos.X < 0) || (newPos.Z > gm.mp.DEMheight - 1 || newPos.Z < 0))
                {

                }
                else
                {
                    position = newPos;
                    position.Y = 1.5f + gm.mp.Heigth(position.X, position.Z);
                }

                Vector3 target = position + direction;
                viewMatrix = Matrix.CreateLookAt(position, target, normal);

                Mouse.SetPosition((int)center.X, (int)center.Y);
            }
            else if (cam == cametyp.AERIAL)
            {
                Vector2 center = new Vector2(tela.Viewport.Width / 2, tela.Viewport.Height / 2);
                //Rotaçao da camara perante a posição do rato
                float delta_X = ms.X - center.X;
                float delta_Y = ms.Y - center.Y;
                yaw += 2 * (delta_X) * MathHelper.ToRadians(-2f) * (float)gt.ElapsedGameTime.TotalSeconds;
                pitch += 2 * (delta_Y) * MathHelper.ToRadians(2f) * (float)gt.ElapsedGameTime.TotalSeconds;

                Vector3 defaultDirection = new Vector3(0.0f, 0.0f, -1.0f);
                Matrix cameraRotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0.0f);
                Vector3 direction = Vector3.Transform(defaultDirection, cameraRotation);
                position = new Vector3(2, 60, 2);

                Vector3 target = position + direction;
                viewMatrix = Matrix.CreateLookAt(position, target, normal);
                Mouse.SetPosition((int)center.X, (int)center.Y);
            }
            else if (cam == cametyp.GOD)
            {

                Vector2 center = new Vector2(tela.Viewport.Width / 2, tela.Viewport.Height / 2);
                //Rotaçao da camara perante a posição do rato
                float delta_X = ms.X - center.X;
                float delta_Y = ms.Y - center.Y;
                yaw += 2 * (delta_X) * MathHelper.ToRadians(-2f) * (float)gt.ElapsedGameTime.TotalSeconds;
                pitch += 2 * (delta_Y) * MathHelper.ToRadians(2f) * (float)gt.ElapsedGameTime.TotalSeconds;

                Vector3 defaultDirection = new Vector3(0.0f, 0.0f, -1.0f);
                Matrix cmRot = Matrix.CreateFromYawPitchRoll(yaw, 0f, 0.0f);
                Matrix cameraRotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0.0f);
                Vector3 dirCm = Vector3.Transform(defaultDirection, cameraRotation);
                Vector3 direction = Vector3.Transform(defaultDirection, cmRot);
                Vector3 right = Vector3.Cross(new Vector3(direction.X, 0, direction.Z), Vector3.Up);
                right.Normalize();

                if (kb.IsKeyDown(Keys.NumPad8))
                    position = position + direction * 100f * (float)gt.ElapsedGameTime.TotalSeconds;
                if (kb.IsKeyDown(Keys.NumPad5))
                    position = position - direction * 100f * (float)gt.ElapsedGameTime.TotalSeconds;
                if (kb.IsKeyDown(Keys.NumPad4))
                    position = position - right * 100f * (float)gt.ElapsedGameTime.TotalSeconds;
                if (kb.IsKeyDown(Keys.NumPad6))
                    position = position + right * 100f * (float)gt.ElapsedGameTime.TotalSeconds;
                if (kb.IsKeyDown(Keys.NumPad7))
                    position.Y = position.Y + 100f * (float)gt.ElapsedGameTime.TotalSeconds;
                if (kb.IsKeyDown(Keys.NumPad1))
                    position.Y = position.Y - 100f * (float)gt.ElapsedGameTime.TotalSeconds;

                Vector3 target = position + dirCm;
                viewMatrix = Matrix.CreateLookAt(position, target, normal);
                Mouse.SetPosition((int)center.X, (int)center.Y);
            }
        }
    }
}
