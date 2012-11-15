using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Star_Defense
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        AnimatedSprite Explosion;
        Background background;

        Player player;
        public int iPlayAreaTop = 30;
        public int iPlayAreaBottom = 630;
        int iMaxHorizontalSpeed = 8;
        float fBoardUpdateDelay = 0f;
        float fBoardUpdateInterval = 0.01f;
        int iBulletVerticalOffset = 12;
        int[] iBulletFacingOffsets = new int[2] { 70, 0 };
        static int iMaxBullets = 40;
        Bullet[] bullets = new Bullet[iMaxBullets];
        float fBulletDelayTimer = 0.0f;
        float fFireDelay = 0.15f;

        int iMaxEnemies = 9;
        int iActiveEnemies = 9;
        static int iTotalMaxEnemies = 30;
        Enemy[] Enemies = new Enemy[iTotalMaxEnemies];
        Texture2D t2dEnemyShip;

        Random rndGen = new Random();
        Texture2D t2dExplosionSheet;
        Explosion[] Explosions = new Explosion[iTotalMaxEnemies + 1];

        int iGameStarted = 0;
        Texture2D t2dTitleScreen;

        int iProcessEvents = 1;
        int iLivesLeft = 3;
        int iGameWave = 0;
        int iPlayerScore = 0;
        float fPlayerRespawnTimer = 4f;
        float fPlayerRespawnCount = 0f;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.ApplyChanges();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            background = new Background(
                Content,
                @"Textures\PrimaryBackground",
                @"Textures\ParallaxStars");

            player = new Player(Content.Load<Texture2D>(@"Textures\PlayerShip"));

            bullets[0] = new Bullet(Content.Load<Texture2D>(@"Textures\PlayerBullet"));

            for (int x = 1; x < iMaxBullets; x++)
                bullets[x] = new Bullet();

            t2dEnemyShip = Content.Load<Texture2D>(@"Textures\enemy");

            for (int i = 0; i < iTotalMaxEnemies; i++)
            {
                Enemies[i] = new Enemy(t2dEnemyShip, 0, 0, 32, 32, 1);
            }

            t2dExplosionSheet = Content.Load<Texture2D>(@"Textures\Explosions");
            for (int i = 0; i < iTotalMaxEnemies + 1; i++)
            {
                Explosions[i] = new Explosion(t2dExplosionSheet,
                                0, rndGen.Next(8) * 64, 64, 64, 16);
            }

            t2dTitleScreen = Content.Load<Texture2D>(@"Textures\TitleScreen");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected void GenerateEnemies()
        {
            if (iMaxEnemies < iTotalMaxEnemies)
                iMaxEnemies++;

            iActiveEnemies = 0;

            for (int x = 0; x < iMaxEnemies; x++)
            {
                Enemies[x].Generate(background.BackgroundOffset,
                                    player.X);
                iActiveEnemies += 1;
            }
        }

        protected void PlayerKilled()
        {
            // Reset the iGameWave and iMaxEnemies, since they will 
            // both be bumped automatically when the new wave is 
            // generated.
            iGameWave--;
            iMaxEnemies--;

            // Stop the player's ship
            player.ScrollRate = 0;
        }

        protected void StartNewWave()
        {
            iProcessEvents = 1;
            iGameStarted = 1;
            iGameWave++;
            GenerateEnemies();
        }

        protected void StartNewGame()
        {
            iGameStarted = 1;
            iProcessEvents = 1;
            iLivesLeft = 3;
            player.ScrollRate = 0;
            iPlayerScore = 0;
            iGameWave = 0;
            iMaxEnemies = 9;

            StartNewWave();
        }

        protected void UpdateBullets(GameTime gameTime)
        {
            // Updates the location of all of thell active player bullets. 
            for (int x = 0; x < iMaxBullets; x++)
            {
                if (bullets[x].IsActive)
                    bullets[x].Update(gameTime);
            }
        }

        protected void FireBullet(int iVerticalOffset)
        {
            // Find and fire a free bullet
            for (int x = 0; x < iMaxBullets; x++)
            {
                if (!bullets[x].IsActive)
                {
                    bullets[x].Fire(player.X + iBulletFacingOffsets[player.Facing],
                                             player.Y + iBulletVerticalOffset + iVerticalOffset,
                                             player.Facing);
                    break;

                }
            }
        }

        protected void CheckOtherKeys(KeyboardState ksKeys, GamePadState gsPad)
        {

            // Space Bar or Game Pad A button fire the 
            // player's weapon.  The weapon has it's
            // own regulating delay (fBulletDelayTimer) 
            // to pace the firing of the player's weapon.
            if ((ksKeys.IsKeyDown(Keys.Space)) ||
                (gsPad.Buttons.A == ButtonState.Pressed))
            {
                if (fBulletDelayTimer >= fFireDelay)
                {
                    FireBullet(0);
                    fBulletDelayTimer = 0.0f;
                }
            }
        }
        protected void CheckHorizontalMovementKeys(KeyboardState ksKeys,
                                   GamePadState gsPad)
        {
            bool bResetTimer = false;

            player.Thrusting = false;
            if ((ksKeys.IsKeyDown(Keys.Right)) ||
                (gsPad.ThumbSticks.Left.X > 0))
            {
                if (player.ScrollRate < iMaxHorizontalSpeed)
                {
                    player.ScrollRate += player.AccelerationRate;
                    if (player.ScrollRate > iMaxHorizontalSpeed)
                        player.ScrollRate = iMaxHorizontalSpeed;
                    bResetTimer = true;
                }
                player.Thrusting = true;
                player.Facing = 0;
            }

            if ((ksKeys.IsKeyDown(Keys.Left)) ||
                (gsPad.ThumbSticks.Left.X < 0))
            {
                if (player.ScrollRate > -iMaxHorizontalSpeed)
                {
                    player.ScrollRate -= player.AccelerationRate;
                    if (player.ScrollRate < -iMaxHorizontalSpeed)
                        player.ScrollRate = -iMaxHorizontalSpeed;
                    bResetTimer = true;
                }
                player.Thrusting = true;
                player.Facing = 1;
            }

            if (bResetTimer)
                player.SpeedChangeCount = 0.0f;
        }

        protected void CheckVerticalMovementKeys(KeyboardState ksKeys,
                                 GamePadState gsPad)
        {

            bool bResetTimer = false;

            if ((ksKeys.IsKeyDown(Keys.Up)) ||
                (gsPad.ThumbSticks.Left.Y > 0))
            {
                if (player.Y > iPlayAreaTop)
                {
                    player.Y -= player.VerticalMovementRate;
                    bResetTimer = true;
                }
            }

            if ((ksKeys.IsKeyDown(Keys.Down)) ||
                (gsPad.ThumbSticks.Left.Y < 0))
            {
                if (player.Y < iPlayAreaBottom)
                {
                    player.Y += player.VerticalMovementRate;
                    bResetTimer = true;
                }
            }

            if (bResetTimer)
                player.VerticalChangeCount = 0f;
        }

        public void UpdateBoard()
        {
            background.BackgroundOffset += player.ScrollRate;
            background.ParallaxOffset += player.ScrollRate * 2;
        }

        protected bool Intersects(Rectangle rectA, Rectangle rectB)
        {
            // Returns True if rectA and rectB contain any overlapping points
            return (rectA.Right > rectB.Left && rectA.Left < rectB.Right &&
                    rectA.Bottom > rectB.Top && rectA.Top < rectB.Bottom);
        }

        protected void DestroyEnemy(int iEnemy)
        {
            Enemies[iEnemy].Deactivate();
            Explosions[iEnemy].Activate(
                Enemies[iEnemy].X - 16,
                Enemies[iEnemy].Y - 16,
                Enemies[iEnemy].Motion,
                Enemies[iEnemy].Speed / 2,
                Enemies[iEnemy].Offset);
            
            iActiveEnemies--;
        }

        protected void RemoveBullet(int iBullet)
        {
            bullets[iBullet].IsActive = false;
        }

        protected void CheckBulletHits()
        {
            // Check to see of any of the players bullets have 
            // impacted any of the enemies.
            for (int i = 0; i < iMaxBullets; i++)
            {
                if (bullets[i].IsActive)
                    for (int x = 0; x < iTotalMaxEnemies; x++)
                        if (Enemies[x].IsActive)
                            if (Intersects(bullets[i].BoundingBox,
                                           Enemies[x].CollisionBox))
                            {
                                DestroyEnemy(x);
                                RemoveBullet(i);
                            }
           }

            // If we have run out of active enemies, generate new ones
            if (iActiveEnemies < 1)
                StartNewWave();
        }

        protected void CheckPlayerHits()
        {
            for (int x = 0; x < iTotalMaxEnemies; x++)
            {
                if (Enemies[x].IsActive)
                {
                    // If the enemy and ship sprites  collide...
                    if (Intersects(player.BoundingBox, Enemies[x].CollisionBox))
                    {
                        // Stop event processing
                        iProcessEvents = 0;

                        // Set up the ship's explosion
                        Explosions[iTotalMaxEnemies].Activate(
                            player.X - 16,
                            player.Y - 16,
                            Vector2.Zero,
                            0f,
                            background.BackgroundOffset);

                        fPlayerRespawnCount = 0.0f;

                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Store values for the Keyboard and GamePad so we aren't
            // Querying them multiple times per Update
            KeyboardState keystate = Keyboard.GetState();
            GamePadState gamepadstate = GamePad.GetState(PlayerIndex.One);

            // If the Escape Key is pressed, or the user presses
            // the "Back" button on the game pad, exit the game.
            if ((keystate.IsKeyDown(Keys.Escape) ||
                 gamepadstate.Buttons.Back == ButtonState.Pressed))
                this.Exit();

            // Get elapsed game time since last call to Update
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (iGameStarted == 1)
            {
                #region GamePlay Mode (iGameStarted==1)

                if (iProcessEvents == 1)
                {
                    #region Processing Events (iProcessEvents==1)
                    //Accumulate time since the last bullet was fired
                    fBulletDelayTimer += elapsed;

                    // Accumulate time since the player's speed changed
                    player.SpeedChangeCount += elapsed;

                    // If enough time has passed that the player can change
                    // speed again, call CheckHorizontalMovementKeys
                    if (player.SpeedChangeCount > player.SpeedChangeDelay)
                    {
                        CheckHorizontalMovementKeys(keystate, gamepadstate);
                    }

                    // Accumulate time since the player moved vertically
                    player.VerticalChangeCount += elapsed;

                    // If enough time has passed, call CheckVerticalMovementKeys
                    if (player.VerticalChangeCount > player.VerticalChangeDelay)
                    {
                        CheckVerticalMovementKeys(keystate, gamepadstate);
                    }

                    // Check any other key presses
                    CheckOtherKeys(keystate, gamepadstate);

                    // Update all enemies and explosions
                    for (int i = 0; i < iTotalMaxEnemies; i++)
                    {
                        if (Enemies[i].IsActive)
                            Enemies[i].Update(gameTime,
                              background.BackgroundOffset);

                        if (Explosions[i].IsActive)
                            Explosions[i].Update(gameTime,
                              background.BackgroundOffset);

                    }

                    // Update the player's star fighter
                    player.Update(gameTime);

                    // Move any active bullets
                    UpdateBullets(gameTime);

                    // See if any active bullets hit any active enemies
                    CheckBulletHits();

                    // Check to see if the player has collided with any enemies
                    CheckPlayerHits();

                    // Accumulate time since the game board was last updated
                    // This reflects the actual movement rate of the screen
                    // as opposed to speed changes by the player
                    fBoardUpdateDelay += elapsed;

                    // If enough time has elapsed, update the game board.
                    if (fBoardUpdateDelay > fBoardUpdateInterval)
                    {
                        fBoardUpdateDelay = 0f;
                        UpdateBoard();
                    }
                    #endregion
                }
                else
                {
                    #region Not Processing Events (iProcessEvents==0)

                    if (Explosions[iTotalMaxEnemies].IsActive)
                        Explosions[iTotalMaxEnemies].Update(gameTime,
                            background.BackgroundOffset);

                    fPlayerRespawnCount += elapsed;

                    if (fPlayerRespawnCount > fPlayerRespawnTimer)
                    {
                        iLivesLeft -= 1;
                        if (iLivesLeft > 0)
                        {
                            PlayerKilled();
                            StartNewWave();
                        }
                        else
                        {
                            iGameStarted = 0;
                            iProcessEvents = 1;
                        }
                    }
                    #endregion
                }
                #endregion
            }
            else
            {
                #region Title Screen Mode (iGameStarted==0)
                if ((keystate.IsKeyDown(Keys.Space)) ||
                   (gamepadstate.Buttons.Start == ButtonState.Pressed))
                {
                    StartNewGame();
                }
                #endregion
            }
            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Clear the Graphics Device
            graphics.GraphicsDevice.Clear(Color.Black);

            // Start a SpriteBatch.Begin which will be used
            // by all of our drawing code.
            spriteBatch.Begin();

            if (iGameStarted == 1)
            {
                #region Game Play Mode (iGameStarted==1)

                // Draw the Background object
                background.Draw(spriteBatch);

                // Draw the Player's Star Fighter
                if (iProcessEvents == 1)
                {
                    player.Draw(spriteBatch);
                }

                // Draw any active bullets on the screen
                for (int i = 0; i < iMaxBullets; i++)
                {
                    if (bullets[i].IsActive)
                    {
                        bullets[i].Draw(spriteBatch);
                    }
                }

                // Draw any active enemies and explosions
                for (int i = 0; i < iMaxEnemies; i++)
                {
                    if (Enemies[i].IsActive)
                    {
                        Enemies[i].Draw(spriteBatch,
                          background.BackgroundOffset);
                    }

                    if (Explosions[i].IsActive)
                    {
                        Explosions[i].Draw(spriteBatch, false);
                    }
                }

                // Draw the player's explosion if it is happening
                if (Explosions[iTotalMaxEnemies].IsActive)
                    Explosions[iTotalMaxEnemies].Draw(spriteBatch, true);
                #endregion
            }
            else
            {
                #region Title Screen Mode (iGameStarted==0)
                spriteBatch.Draw(t2dTitleScreen, new Rectangle(0, 0, 1280, 720), Color.White);
                #endregion
            }
            // Close the SpriteBatch
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
