using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projeto_3D
{
    public class MapGen
    {
        Game1 gm;
        public Texture2D text, texture;
        BasicEffect effect;
        VertexPositionNormalTexture[] VertexArray;
        short[] ind;
        Color[] cores;
        VertexBuffer verteBuffer;
        IndexBuffer indexBuffer;
        Vector3[] normalMap;
        public float DEMwidth, DEMheight;
        public float[] HeightMap;

        public MapGen(Game1 gm, GraphicsDevice tela, string txt)
        {
            this.gm = gm;
            texture = gm.Content.Load<Texture2D>("pedra");
            effect = new BasicEffect(tela);
            float aspectRatio = (float)tela.Viewport.Width /
       tela.Viewport.Height;
            effect.View = Matrix.CreateLookAt(
            new Vector3(1.0f, 2.0f, 2.0f),          //para mudar de visao
            Vector3.Zero, Vector3.Up);
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 0.1f, 1000.0f);

            // Iluminação
            effect.VertexColorEnabled = false;
            effect.TextureEnabled = true;
            effect.Texture = texture;
            effect.LightingEnabled = true;

            // Set the ambient color
            effect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f);

            effect.DirectionalLight0.Enabled = true;
            effect.DirectionalLight0.DiffuseColor = new Vector3(0.7f, 0.7f, 0.7f);
            effect.DirectionalLight0.Direction = new Vector3(-1f, -1f, -1f);
            effect.DirectionalLight0.Direction.Normalize();
            effect.DirectionalLight0.SpecularColor = new Vector3(1f, 1f, 1f);

            effect.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
            //effect.DiffuseColor = Color.White.ToVector3();
            effect.SpecularColor = new Vector3(1.0f, 1.0f, 1.0f);
            effect.SpecularPower = 127f;

            inicialize(txt, tela);
        }

        public void inicialize(string txt, GraphicsDevice tela)
        {

            text = gm.Content.Load<Texture2D>(txt);
            DEMheight = text.Height;
            DEMwidth = text.Width;
            VertexArray = new VertexPositionNormalTexture[text.Width * text.Height];
            cores = new Color[text.Width * text.Height];
            text.GetData<Color>(cores);
            ind = new short[text.Height * (text.Width - 1) * 2];

            normalMap = new Vector3[(int)(DEMheight * DEMwidth)];
            HeightMap = new float[(int)(DEMwidth * DEMheight)];
            float esc = 0.05f;
            //Recolha dos valores do array de cores vezes a constante para "Suavizar" as altitudes 
            for (int c = 0; c < text.Height * text.Width; c++)
            {
                HeightMap[c] = cores[c].R * esc;
            }

            //Distribuição das alturas para cada ponto do mapa
            for (int z = 0; z < text.Height; z++)
            {
                for (int x = 0; x < text.Width; x++)
                {
                    VertexArray[z * text.Width + x] = new VertexPositionNormalTexture(new Vector3(x, HeightMap[z * (int)DEMwidth + x], z), Vector3.Zero, new Vector2(x % 2, z % 2));
                }
            }
            CreateNormals();
            verteBuffer = new VertexBuffer(tela, typeof(VertexPositionNormalTexture), text.Width * text.Height, BufferUsage.None);
            verteBuffer.SetData<VertexPositionNormalTexture>(VertexArray);
            //Indices de ordem vertical ,Coluna por Coluna 
            for (int x = 0; x < text.Width - 1; x++)
            {
                for (int z = 0; z < text.Height; z++)
                {
                    ind[2 * z + 0 + x * text.Width * 2] = (short)(z * text.Width + x);
                    ind[2 * z + 1 + x * text.Width * 2] = (short)(z * text.Width + 1 + x);
                }
            }
            indexBuffer = new IndexBuffer(tela, typeof(short), (text.Width - 1) * text.Height * 2, BufferUsage.None);
            indexBuffer.SetData<short>(ind);
        }


        public float Heigth(float x, float z)   //Calculo da altura a que se encontra
        {
            //Recolha dos dados para calcular as distancia dos pontos 
            int xa = (int)x;
            int za = (int)z;
            float ya = HeightMap[za * text.Width + xa];

            int xb = xa + 1;
            int zb = za;
            float yb = HeightMap[zb * text.Width + xb];

            int xc = xa;
            int zc = za + 1;
            float yc = HeightMap[zc * text.Width + xc];

            int xd = xa + 1;
            int zd = za + 1;
            float yd = HeightMap[zd * text.Width + xd];

            float da = x - xa;
            float db = xb - x;
            float dc = x - xc;
            float dd = xd - x;

            float dab = z - za;
            float dcd = zc - z;

            //Distancia do Y entre os extremos e a nossa coordenada 
            float yab = da * yb + db * ya;
            float ycd = dc * yd + dd * yc;


            return dab * ycd + dcd * yab;
        }

        public void Draw(GraphicsDevice tela)
        {
            effect.View = gm.cm.viewMatrix;
            effect.Projection = gm.cm.projectionMatrix;

            effect.World = Matrix.Identity;
            effect.CurrentTechnique.Passes[0].Apply();

            tela.SetVertexBuffer(verteBuffer);
            tela.Indices = indexBuffer;

            //Desenho de coluna por coluna
            for (int x = 0; x < text.Width - 1; x++)
            {
                tela.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, x * text.Height * 2, text.Height * 2 - 2);

            }
        }

        public void CreateNormals()
        {
            Vector3[] aroundPos = new Vector3[8];
            Vector3[] crossPos = new Vector3[8];
            Vector3 crossNorm;
            Vector3 media = Vector3.Zero;
            Vector3 center;

            // middle

            for (int z = 1; z < DEMheight - 1; z++)
            {
                for (int x = 1; x < DEMwidth - 1; x++)
                {
                    media = Vector3.Zero;
                    aroundPos = new Vector3[8];
                    crossPos = new Vector3[8];
                    center = VertexArray[z * (int)DEMwidth + x].Position;

                    aroundPos[0] = VertexArray[(z - 1) * (int)DEMwidth + (x)].Position - center;
                    aroundPos[1] = VertexArray[(z - 1) * (int)DEMwidth + (x - 1)].Position - center;
                    aroundPos[2] = VertexArray[(z) * (int)DEMwidth + (x - 1)].Position - center;
                    aroundPos[3] = VertexArray[(z + 1) * (int)DEMwidth + (x - 1)].Position - center;
                    aroundPos[4] = VertexArray[(z + 1) * (int)DEMwidth + (x)].Position - center;
                    aroundPos[5] = VertexArray[(z + 1) * (int)DEMwidth + (x + 1)].Position - center;
                    aroundPos[6] = VertexArray[(z) * (int)DEMwidth + (x + 1)].Position - center;
                    aroundPos[7] = VertexArray[(z - 1) * (int)DEMwidth + (x + 1)].Position - center;


                    for (int i = 0; i < 8; i++)
                    {

                        if (i == 7)
                        {
                            crossPos[i] = Vector3.Cross(aroundPos[7], aroundPos[0]);
                        }
                        else
                        {
                            crossPos[i] = Vector3.Cross(aroundPos[i], aroundPos[i + 1]);
                        }


                        media += crossPos[i];
                    }
                    media = media / 8;

                    VertexArray[z * (int)DEMwidth + x].Normal = media;
                    normalMap[z * (int)DEMwidth + x] = media;
                }
            }

            // top edge
            for (int x = 1, z = 0; x < DEMwidth - 1; x++)
            {
                media = Vector3.Zero;
                aroundPos = new Vector3[5];
                crossPos = new Vector3[5];
                center = VertexArray[z * (int)DEMwidth + x].Position;


                aroundPos[0] = VertexArray[(z) * (int)DEMwidth + (x - 1)].Position - center;
                aroundPos[1] = VertexArray[(z + 1) * (int)DEMwidth + (x - 1)].Position - center;
                aroundPos[2] = VertexArray[(z + 1) * (int)DEMwidth + (x)].Position - center;
                aroundPos[3] = VertexArray[(z + 1) * (int)DEMwidth + (x + 1)].Position - center;
                aroundPos[4] = VertexArray[(z) * (int)DEMwidth + (x + 1)].Position - center;

                for (int i = 0; i < 4; i++)
                {
                    crossPos[i] = Vector3.Cross(aroundPos[i], aroundPos[i + 1]);

                    media += crossPos[i];
                }
                media = media / 4;

                VertexArray[z * (int)DEMwidth + x].Normal = media;
                normalMap[z * (int)DEMwidth + x] = media;
            }

            // bottom edge
            for (int x = 1, z = (int)DEMheight - 1; x < DEMwidth - 1; x++)
            {
                media = Vector3.Zero;
                aroundPos = new Vector3[5];
                crossPos = new Vector3[5];
                center = VertexArray[z * (int)DEMwidth + x].Position;

                aroundPos[0] = VertexArray[(z) * (int)DEMwidth + (x + 1)].Position - center;
                aroundPos[1] = VertexArray[(z - 1) * (int)DEMwidth + (x + 1)].Position - center;
                aroundPos[2] = VertexArray[(z - 1) * (int)DEMwidth + (x)].Position - center;
                aroundPos[3] = VertexArray[(z - 1) * (int)DEMwidth + (x - 1)].Position - center;
                aroundPos[4] = VertexArray[(z) * (int)DEMwidth + (x - 1)].Position - center;

                for (int i = 0; i < 4; i++)
                {
                    crossPos[i] = Vector3.Cross(aroundPos[i], aroundPos[i + 1]);

                    media += crossPos[i];
                }
                media = media / 4;
                VertexArray[z * (int)DEMwidth + x].Normal = media;
                normalMap[z * (int)DEMwidth + x] = media;
            }

            // left edge
            for (int x = 0, z = 1; z < DEMheight - 1; z++)
            {
                media = Vector3.Zero;
                aroundPos = new Vector3[5];
                crossPos = new Vector3[5];
                center = VertexArray[z * (int)DEMwidth + x].Position;

                aroundPos[0] = VertexArray[(z + 1) * (int)DEMwidth + (x)].Position - center;
                aroundPos[1] = VertexArray[(z + 1) * (int)DEMwidth + (x + 1)].Position - center;
                aroundPos[2] = VertexArray[(z) * (int)DEMwidth + (x + 1)].Position - center;
                aroundPos[3] = VertexArray[(z - 1) * (int)DEMwidth + (x + 1)].Position - center;
                aroundPos[4] = VertexArray[(z - 1) * (int)DEMwidth + (x)].Position - center;

                for (int i = 0; i < 4; i++)
                {
                    crossPos[i] = Vector3.Cross(aroundPos[i], aroundPos[i + 1]);

                    media += crossPos[i];
                }
                media = media / 4;
                VertexArray[z * (int)DEMwidth + x].Normal = media;
                normalMap[z * (int)DEMwidth + x] = media;
            }

            // right edge
            for (int x = (int)DEMwidth - 1, z = 1; z < DEMheight - 1; z++)
            {
                media = Vector3.Zero;
                aroundPos = new Vector3[5];
                crossPos = new Vector3[5];
                center = VertexArray[z * (int)DEMwidth + x].Position;

                aroundPos[0] = VertexArray[(z - 1) * (int)DEMwidth + (x)].Position - center;
                aroundPos[1] = VertexArray[(z - 1) * (int)DEMwidth + (x - 1)].Position - center;
                aroundPos[2] = VertexArray[(z) * (int)DEMwidth + (x - 1)].Position - center;
                aroundPos[3] = VertexArray[(z + 1) * (int)DEMwidth + (x - 1)].Position - center;
                aroundPos[4] = VertexArray[(z + 1) * (int)DEMwidth + (x)].Position - center;

                for (int i = 0; i < 4; i++)
                {
                    crossPos[i] = Vector3.Cross(aroundPos[i], aroundPos[i + 1]);

                    media += crossPos[i];
                }
                media = media / 4;
                VertexArray[z * (int)DEMwidth + x].Normal = media;
                normalMap[z * (int)DEMwidth + x] = media;
            }


            // Corners
            //top left

            aroundPos = new Vector3[3];
            crossPos = new Vector3[3];
            crossNorm = Vector3.Zero;

            aroundPos[0] = VertexArray[(int)DEMwidth].Position - VertexArray[0].Position;
            aroundPos[1] = VertexArray[(int)DEMwidth + 1].Position - VertexArray[0].Position;
            aroundPos[2] = VertexArray[1].Position - VertexArray[0].Position;

            crossPos[0] = Vector3.Cross(aroundPos[0], aroundPos[1]);
            crossPos[1] = Vector3.Cross(aroundPos[1], aroundPos[2]);
            crossNorm = (crossPos[0] + crossPos[1]) / 2;

            VertexArray[0].Normal = crossNorm;
            normalMap[0] = crossNorm;

            //top right
            aroundPos = new Vector3[3];
            crossPos = new Vector3[3];
            crossNorm = Vector3.Zero;
            aroundPos[0] = VertexArray[(int)DEMwidth - 2].Position - VertexArray[(int)DEMwidth - 1].Position;
            aroundPos[1] = VertexArray[((int)DEMwidth - 1) * 2].Position - VertexArray[(int)DEMwidth - 1].Position;
            aroundPos[2] = VertexArray[((int)DEMwidth - 1) * 2 + 1].Position - VertexArray[(int)DEMwidth - 1].Position;

            crossPos[0] = Vector3.Cross(aroundPos[0], aroundPos[1]);
            crossPos[1] = Vector3.Cross(aroundPos[1], aroundPos[2]);
            crossNorm = (crossPos[0] + crossPos[1]) / 2;

            VertexArray[(int)DEMwidth - 1].Normal = crossNorm;
            normalMap[(int)DEMwidth - 1] = crossNorm;

            //bottom left
            aroundPos = new Vector3[3];
            crossPos = new Vector3[3];
            crossNorm = Vector3.Zero;
            aroundPos[0] = VertexArray[(int)(DEMheight * DEMwidth) - (int)DEMwidth + 1].Position - VertexArray[(int)((DEMheight - 1) * DEMwidth)].Position;
            aroundPos[1] = VertexArray[(int)((DEMheight - 2) * DEMwidth) + 1].Position - VertexArray[(int)((DEMheight - 1) * DEMwidth)].Position;
            aroundPos[2] = VertexArray[(int)((DEMheight - 2) * DEMwidth)].Position - VertexArray[(int)((DEMheight - 1) * DEMwidth)].Position;

            crossPos[0] = Vector3.Cross(aroundPos[0], aroundPos[1]);
            crossPos[1] = Vector3.Cross(aroundPos[1], aroundPos[2]);
            crossNorm = (crossPos[0] + crossPos[1]) / 2;

            VertexArray[(int)((DEMheight - 1) * DEMwidth)].Normal = crossNorm;
            normalMap[(int)((DEMheight - 1) * DEMwidth)] = crossNorm;

            //bottom right
            aroundPos = new Vector3[3];
            crossPos = new Vector3[3];
            crossNorm = Vector3.Zero;
            aroundPos[0] = VertexArray[(int)(DEMheight * (DEMwidth - 1)) - 1].Position - VertexArray[(int)(DEMheight * DEMwidth) - 1].Position;
            aroundPos[1] = VertexArray[(int)((DEMheight) * (DEMwidth - 1)) - 2].Position - VertexArray[(int)(DEMheight * DEMwidth) - 1].Position;
            aroundPos[2] = VertexArray[(int)(DEMheight * DEMwidth) - 2].Position - VertexArray[(int)(DEMheight * DEMwidth) - 1].Position;
            crossPos[0] = Vector3.Cross(aroundPos[0], aroundPos[1]);
            crossPos[1] = Vector3.Cross(aroundPos[1], aroundPos[2]);
            crossNorm = (crossPos[0] + crossPos[1]) / 2;

            VertexArray[(int)(DEMheight * DEMwidth) - 1].Normal = crossNorm;
            normalMap[(int)((DEMheight * DEMwidth) - 1)] = crossNorm;
        }

        public Vector3 normalFollow(float x, float z)
        {
            int xa = (int)x;
            int za = (int)z;
            Vector3 ya = normalMap[za * (int)DEMwidth + xa];
            ya.Normalize();

            int xb = xa + 1;
            int zb = za;
            Vector3 yb = normalMap[zb * (int)DEMwidth + xb];
            yb.Normalize();

            int xc = xa;
            int zc = za + 1;
            Vector3 yc = normalMap[zc * (int)DEMwidth + xc];
            yc.Normalize();

            int xd = xb;
            int zd = zc;
            Vector3 yd = normalMap[zd * (int)DEMwidth + xd];
            yd.Normalize();

            float da = x - xa;
            float db = xb - x;
            float dc = da;
            float dd = db;

            float dab = z - za;
            float dcd = zc - z;

            Vector3 yab = da * yb + db * ya;
            Vector3 ycd = dc * yd + dd * yc;

            Vector3 ret = dab * ycd + dcd * yab;
            ret.Normalize();
            return ret;
        }
    }
}
