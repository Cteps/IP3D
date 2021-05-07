using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projeto_3D
{
    class ParticleSystem
    {
        List<Particle> lParticles, sPart;
        Random rnd = new Random();
        ParticleGenerator gParticles;
        BasicEffect effect;
        VertexPositionColor[] vertexArray;

        public ParticleSystem(GraphicsDevice gd)
        {
            effect = new BasicEffect(gd);
            lParticles = new List<Particle>();
            gParticles = new ParticleGenerator();

            //perspetiva para ver mais de longe
            float aspectRatio = (float)gd.Viewport.Width / gd.Viewport.Height;
            effect.View = Matrix.CreateLookAt(new Vector3(0.0f, 3.5f, 4.5f), Vector3.Zero, Vector3.Up); ;
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 10.0f);
            effect.VertexColorEnabled = true;

        }

        public void Update(GameTime gt, MapGen mp)
        {
            sPart = new List<Particle>();
            int count = rnd.Next(10, 20);
            int n = 0;

            //criador de 100 a 200 particulas por frame
            while (n < count)
            {
                lParticles.Add(new Particle(gParticles.Spawn(Vector3.Zero), Vector3.Zero));
                n++;
            }

            foreach (Particle p in lParticles)
            {
                if (p.active)
                {
                    //caso a particula esteja "viva" ele faz update e adiciona na lista de particulas vivas
                    p.Update(gt, new Vector3(rnd.Next(-10, 10) * (float)rnd.NextDouble(), -9.8f, rnd.Next(-1, 1) * (float)rnd.NextDouble()), mp);
                    sPart.Add(p);
                }
            }
            //apos adicionar todas as particulas "vivas" , ele limpa a lista antiga e vai adicionar as novas outra vez
            lParticles = new List<Particle>();
            foreach (Particle p in sPart)
            {
                lParticles.Add(p);
            }

            GetVertex(lParticles.Count,Color.DarkBlue);
        }

        public void Update2(GameTime gt, MapGen mp, ClsTank tank)
        {
            sPart = new List<Particle>();
            int count = rnd.Next(1, 10);
            int n = 0;

            if (tank.moving)
            {
                //criador de 1 a 2 particulas por frame
                while (n < count)
                {
                    Matrix rot1 = Matrix.CreateFromAxisAngle(tank.rotation.Right, (float)rnd.NextDouble()*20-10);
                    rot1 = Matrix.CreateFromAxisAngle(tank.rotation.Up, (float)rnd.NextDouble() * 20 - 10);
                    //rotaçao com normal
                    Vector3 dir1 = tank.wheelsTransform[3].Backward;
                    dir1 = Vector3.Transform(dir1, rot1);

                    Matrix rot2 = Matrix.CreateFromAxisAngle(tank.rotation.Right, (float)rnd.NextDouble() * 20 - 10);
                    rot2 = Matrix.CreateFromAxisAngle(tank.rotation.Up, (float)rnd.NextDouble() * 20 - 10);
                    Vector3 dir2 = tank.wheelsTransform[2].Backward;
                    dir2 = Vector3.Transform(dir2, rot2);

                    lParticles.Add(new Particle(gParticles.Spawn2(tank.boneTransform[tank.wheelsBone[3].Index].Translation, tank.wheelsTransform[3].Right, Matrix.CreateRotationY(tank.wheels[0]), tank.rotation.Up), new Vector3(dir1.X, 1, dir1.Z)));
                    lParticles.Add(new Particle(gParticles.Spawn2(tank.boneTransform[tank.wheelsBone[2].Index].Translation, tank.wheelsTransform[2].Right, Matrix.CreateRotationY(tank.wheels[0]), tank.rotation.Up), new Vector3(dir2.X, 1, dir2.Z)));
                    n++;
                }
            }

            foreach (Particle p in lParticles)
            {
                if (p.active)
                {
                    //caso a particula esteja "viva" ele faz update e adiciona na lista de particulas vivas
                    p.Update(gt, new Vector3(rnd.Next(-10, 10) * (float)rnd.NextDouble(), -9.8f, rnd.Next(-1, 1) * (float)rnd.NextDouble()), mp);
                    sPart.Add(p);
                }
            }
            //apos adicionar todas as particulas "vivas" , ele limpa a lista antiga e vai adicionar as novas outra vez
            lParticles = new List<Particle>();
            foreach (Particle p in sPart)
            {
                lParticles.Add(p);
            }

            GetVertex(lParticles.Count,Color.White);
        }

        public void GetVertex(int length,Color color)
        {
            //Criador dos vertices das particulas , neste caso usa a posiçao antiga + a atula para criar  o traço em preto
            vertexArray = new VertexPositionColor[length * 2];
            //A variavel n serve para saber a posição no array que cada vertice da particula tem
            int n = 0;
            foreach (Particle p in lParticles)
            {
                vertexArray[n] = new VertexPositionColor(p.pos, color);
                vertexArray[n + 1] = new VertexPositionColor(p.apos, color);
                //como cada particula tem 2 vertices ele após adicionar os dados soma 2 para serem as seguintes
                n = n + 2;
            }
        }

        public void Draw(GraphicsDevice gd, Camera cm)
        {
            effect.View = cm.viewMatrix;
            effect.Projection = cm.projectionMatrix;
            effect.CurrentTechnique.Passes[0].Apply();

            if (vertexArray != null && lParticles.Count > 0)
            {
                gd.DrawUserPrimitives(PrimitiveType.LineList, vertexArray, 0, lParticles.Count);
            }
        }
    }
}
