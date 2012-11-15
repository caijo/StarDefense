using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Star_Defense
{
    class Bullet
    {
        static AnimatedSprite asLeft;
        static AnimatedSprite asRight;

        int iX;
        int iY;
        bool bActive;
        int iFacing = 0;
        float fElapsed = 0f;
        float fUpdateInterval = 0.015f;
        public int iSpeed = 12;

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
            set { bActive = value; }
        }

        public int Facing
        {
            get { return iFacing; }
            set { iFacing = value; }
        }

        public int Speed
        {
            get { return iSpeed; }
            set { iSpeed = value; }
        }

        public Rectangle BoundingBox
        {
            get { return new Rectangle(iX, iY, 16, 1); }
        }

        public Bullet(Texture2D texture)
        {
            asRight = new AnimatedSprite(texture, 0, 0, 16, 1, 1);
            asRight.IsAnimating = false;
            asLeft = new AnimatedSprite(texture, 16, 0, 16, 1, 1);
            asRight.IsAnimating = false;
            iFacing = 0;
            iX = 0;
            iY = 0;
            bActive = false;
        }

        public Bullet()
        {
            iFacing = 0;
            iX = 0;
            iY = 0;
            bActive = false;
        }

        public void Fire(int X, int Y, int Facing)
        {
            iX = X;
            iY = Y;
            iFacing = Facing;
            bActive = true;
        }

        public void Update(GameTime gameTime)
        {
            if (bActive)
            {
                fElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (fElapsed > fUpdateInterval)
                {
                    fElapsed = 0f;
                    if (iFacing == 0)
                    {
                        iX += iSpeed;
                    }
                    else
                    {
                        iX -= iSpeed;
                    }

                    // If the bullet has moved off of the screen, 
                    // set it to inactive
                    if ((iX > 1280) || (iX < 0))
                    {
                        bActive = false;
                    }
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if (bActive)
            {
                if (iFacing == 0)
                {
                    asRight.Draw(sb, iX, iY, false);
                }
                else
                {
                    asLeft.Draw(sb, iX, iY, false);
                }
            }
        }
    }
}
