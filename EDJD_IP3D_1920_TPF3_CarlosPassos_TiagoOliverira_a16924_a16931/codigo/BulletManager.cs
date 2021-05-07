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
    public class BulletManager
    {
        TankBullet b;
        List<TankBullet> bullet;
        Game1 gm;

        float t;
        int reloadTimer;
        bool isReloading = false;


        public BulletManager(Game1 gm)
        {
            this.gm = gm;
            bullet = new List<TankBullet>();

            reloadTimer = 60;
        }

        public void Update(GraphicsDevice gd, GameTime gt, ClsTank tank)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && tank.player == ClsTank.Player.P1 && isReloading == false)
            {
                bullet.Add(new TankBullet(gd, gm, tank.cannonPos, tank.cannonDir * -80f, new Vector3(0, 10, 0), (float)gt.ElapsedGameTime.TotalSeconds));
                isReloading = true;
            }
            Reloading(gt);

            foreach (TankBullet b in bullet)
            {
                b.Update(gt);
            }

            foreach (TankBullet b in bullet.ToArray())
            {
                if ((b.Update(gt) || b.isColliding) && tank.player == ClsTank.Player.P1)
                {
                    gm.colliders.Remove(b);
                    bullet.Remove(b);
                    Console.WriteLine("Removeu");
                }
            }
        }

        public void Reloading(GameTime gt)
        {
            if (isReloading == true)
            {
                reloadTimer--;
               //Console.WriteLine(reloadTimer);
            }

            if (reloadTimer == 0)
            {
                isReloading = false;
                reloadTimer = 60;
            }
        }

        public void Draw(GraphicsDevice gd, Camera cam)
        {
            foreach (TankBullet b in bullet)
            {
                b.Draw(gd, cam);
            }
        }
    }
}
