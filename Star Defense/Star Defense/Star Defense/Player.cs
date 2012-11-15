using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Star_Defense
{
    class Player
    {
        AnimatedSprite asSprite;
        int iX = 604;
        int iY = 260;
        int iFacing = 0;
        bool bThrusting = false;
        int iScrollRate = 0;
        int iShipAccelerationRate = 1;
        int iShipVerticalMoveRate = 3;
        float fSpeedChangeCount = 0.0f;
        float fSpeedChangeDelay = 0.1f;
        float fVerticalChangeCount = 0.0f;
        float fVerticalChangeDelay = 0.01f;

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

        public int Facing
        {
            get { return iFacing; }
            set { iFacing = value; }
        }

        public bool Thrusting
        {
            get { return bThrusting; }
            set { bThrusting = value; }
        }

        public int ScrollRate
        {
            get { return iScrollRate; }
            set { iScrollRate = value; }
        }

        public int AccelerationRate
        {
            get { return iShipAccelerationRate; }
            set { iShipAccelerationRate = value; }
        }

        public int VerticalMovementRate
        {
            get { return iShipVerticalMoveRate; }
            set { iShipVerticalMoveRate = value; }
        }

        public float SpeedChangeCount
        {
            get { return fSpeedChangeCount; }
            set { fSpeedChangeCount = value; }
        }

        public float SpeedChangeDelay
        {
            get { return fSpeedChangeDelay; }
            set { fSpeedChangeDelay = value; }
        }

        public float VerticalChangeCount
        {
            get { return fVerticalChangeCount; }
            set { fVerticalChangeCount = value; }
        }

        public float VerticalChangeDelay
        {
            get { return fVerticalChangeDelay; }
            set { fVerticalChangeDelay = value; }
        }

        public Rectangle BoundingBox
        {
            get { return new Rectangle(iX, iY, 72, 16); }
        }

        public Player(Texture2D texture)
        {
            asSprite = new AnimatedSprite(texture, 0, 0, 72, 16, 4);
            asSprite.IsAnimating = false;
        }

        public void Draw(SpriteBatch sb)
        {
            asSprite.Draw(sb, iX, iY, false);
        }

        public void Update(GameTime gametime)
        {
            if (iFacing == 0)
            {
                if (bThrusting)
                    asSprite.Frame = 1;
                else
                    asSprite.Frame = 0;
            }
            else
            {
                if (bThrusting)
                    asSprite.Frame = 3;
                else
                    asSprite.Frame = 2;
            }
        }
    }
}
