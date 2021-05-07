using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Projeto_3D
{

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        ParticleSystem sp;
        public ClsTank tank1, tank2;
        public MapGen mp;
        public Camera cm;
        string MAPNAME = "map";
        public List<Collider> colliders;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }


        protected override void Initialize()
        {
            mp = new MapGen(this, GraphicsDevice, MAPNAME);
            cm = new Camera(this, MAPNAME, GraphicsDevice);
            sp = new ParticleSystem(GraphicsDevice);
            colliders = new List<Collider>();
            tank1 = new ClsTank(GraphicsDevice, Content.Load<Model>("tank"), new Vector3(3, mp.Heigth(3, 3), 3), ClsTank.Player.P1, this);
            tank2 = new ClsTank(GraphicsDevice, Content.Load<Model>("tank"), new Vector3(100, mp.Heigth(100, 100), 100), ClsTank.Player.CPU, this);
            base.Initialize();
        }


        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }


        protected override void UnloadContent()
        {

        }


        protected override void Update(GameTime gameTime)
        {
            KeyboardState kb = Keyboard.GetState();
            MouseState ms = Mouse.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (kb.IsKeyDown(Keys.E))
            {
                tank2.player = ClsTank.Player.CPU;
            }
            else if (kb.IsKeyDown(Keys.Q))
            {
                tank2.player = ClsTank.Player.P2;
            }

            for (int i = 0; i < colliders.Count - 1; i++)
            {
                for (int j = i + 1; j < colliders.Count; j++)
                {
                    if (colliders[i].CollidesWith(colliders[j]))
                    {
                        colliders[i].CollisionWith(colliders[j]);
                        colliders[j].CollisionWith(colliders[i]);
                    }
                }
            }

            sp.Update(gameTime, mp);
            tank1.Update(kb, ms, GraphicsDevice, gameTime, mp);
            tank2.Update(kb, ms, GraphicsDevice, gameTime, mp);

            if (cm.cam == Camera.cametyp.FIRSTP)
            {
                cm.Update(kb, ms, gameTime, tank1.position, tank1.direction, GraphicsDevice);

            }
            else
            {
                cm.Update(kb, ms, gameTime, tank2.position, tank2.direction, GraphicsDevice);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.CornflowerBlue);
            sp.Draw(GraphicsDevice, cm);
            mp.Draw(GraphicsDevice);
            tank1.Draw(GraphicsDevice, cm);
            tank2.Draw(GraphicsDevice, cm);
            base.Draw(gameTime);
        }
     
    }
}
