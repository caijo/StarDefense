using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Star_Defense
{
    class Enemy
    {
        AnimatedSprite asSprite;

        static int iMapWidth = 1920;
        static int iPlayAreaTop = 30;
        static int iPlayAreaBottom = 630;
        static Random rndGen = new Random();
        int iX = 0;
        int iY = -100;
        int iBackgroundOffset = 0;
        Vector2 v2motion = new Vector2(0f, 0f);
        float fSpeed = 1f;
        float fEnemyMoveCount = 0.0f;
        float fEnemyDelay = 0.01f;
        bool bActive = false;

        public int X
        {
            get { return iX; }
            set { iX = value; }
        }

        public int Y
        {
            get { return iY; }
            set { iY = value; }
        }

        public bool IsActive
        {
            get { return bActive; }
        }

        public Rectangle BoundingBox
        {
            get
            {
                int X = iX - iBackgroundOffset;
                if (X > iMapWidth)
                    X -= iMapWidth;
                if (X < 0)
                    X += iMapWidth;
                return new Rectangle(X, iY, 32, 32);
            }
        }

        public int Offset
        {
            get { return iBackgroundOffset; }
            set { iBackgroundOffset = value; }
        }

        public float Speed
        {
            get { return fSpeed; }
        }

        public Vector2 Motion
        {
            get { return v2motion; }
            set { v2motion = value; }
        }

        public Rectangle CollisionBox
        {
            get
            {
                int X = iX - iBackgroundOffset;
                if (X > iMapWidth)
                    X -= iMapWidth;
                if (X < 0)
                    X += iMapWidth;
                return new Rectangle(X + 2, iY + 2, 28, 28);
            }
        }

        public Enemy(Texture2D texture,
              int X, int Y, int W, int H, int Frames)
        {
            asSprite = new AnimatedSprite(texture, X, Y, W, H, Frames);
        }

        public void Deactivate()
        {
            bActive = false;
        }

        private int GetDrawX()
        {
            int X = iX - iBackgroundOffset;
            if (X > iMapWidth)

                X -= iMapWidth;
            if (X < 0)
                X += iMapWidth;

            return X;
        }

        public void RandomizeMovement()
        {
            v2motion.X = rndGen.Next(-50, 50);
            v2motion.Y = rndGen.Next(-50, 50);
            v2motion.Normalize();
            fSpeed = (float)(rndGen.Next(3, 6));
        }

        public void Generate(int iLocation, int iShipX)
        {
            // Generate a random X location that is NOT 
            // within 200 pixels of the player's ship.
            do
            {
                iX = rndGen.Next(iMapWidth);
                iBackgroundOffset = iLocation;
            } while (Math.Abs(GetDrawX() - iShipX) < 200);

            // Generate a random Y location between iPlayAreaTop 
            // and iPlayAreaBottom (the area of our game screen)

            iY = rndGen.Next(iPlayAreaTop, iPlayAreaBottom);
            RandomizeMovement();
            bActive = true;
        }

        public void Draw(SpriteBatch sb, int iLocation)
        {
            if (bActive)
                asSprite.Draw(sb, GetDrawX(), iY, false);
        }

        public void Update(GameTime gametime, int iOffset)
        {
            iBackgroundOffset = iOffset;

            fEnemyMoveCount += (float)gametime.ElapsedGameTime.TotalSeconds;
            if (fEnemyMoveCount > fEnemyDelay)
            {
                iX += (int)((float)v2motion.X * fSpeed);
                iY += (int)((float)v2motion.Y * fSpeed);

                if (rndGen.Next(200) == 1)
                {
                    RandomizeMovement();
                }

                if (iY < iPlayAreaTop)
                {
                    iY = iPlayAreaTop;
                    RandomizeMovement();
                }

                if (iY > iPlayAreaBottom)
                {
                    iY = iPlayAreaBottom;
                    RandomizeMovement();
                }

                if (iX < 0)
                    iX += iMapWidth;

                if (iX > iMapWidth)
                    iX -= iMapWidth;

                fEnemyMoveCount = 0f;
            }
            asSprite.Update(gametime);
        }
    }
}
