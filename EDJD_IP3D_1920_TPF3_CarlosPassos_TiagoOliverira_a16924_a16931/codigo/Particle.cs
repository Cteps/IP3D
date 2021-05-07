using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projeto_3D
{
    class Particle
    {
        public Vector3 apos, pos, vel;
        public bool active = true;

        public Particle(Vector3 Position, Vector3 Velocidade)
        {
            pos = Position;
            vel = Velocidade;
        }

        public void Update(GameTime g, Vector3 gravidade, MapGen mp)
        {
            //apos =  posição anterior
            apos = pos;
            //a particula aumenta sempre a velocidade conforme a velocidade e o tempo  v = Vi + a*t
            vel += gravidade * (float)g.ElapsedGameTime.TotalSeconds;
            //A posição para a qual a particula vai depende da velocidade que quem + a sua posição  p = Pi + v * t 
            pos += vel * (float)g.ElapsedGameTime.TotalSeconds;

            if (pos.Y <= 0)
            {
                //caso a posição seja igual ou inferior a 0 , a particula "morre"
                active = false;
            }
        }
    }
}
