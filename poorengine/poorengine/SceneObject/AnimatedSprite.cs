using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoorEngine.Interfaces;
using PoorEngine.Managers;
using PoorEngine.Textures;

namespace PoorEngine.SceneObject
{
    public class AnimatedSprite : PoorSceneObject, IPoorDrawable, IPoorUpdateable
    {
        private string texName;
        private int framesPerSecond;
        private bool repeat;

        private Point frameSize;
        private Point currentFrame = new Point(0, 0);
        private Point sheetSize;

        private TimeSpan FrameTime;
        private TimeSpan NextUpdate;
        private float alpha;
        private float rotation;

        public AnimatedSprite(string texName, Point frameSize, Point sheetSize,Vector2 position, float rotation, Vector2 scale, float alpha, int framesPerSecond, bool repeat, float Z)
            :base(texName)
        {
            this.alpha = alpha;
            this.Z = Z;
            this.frameSize = frameSize;
            this.sheetSize = sheetSize;
            this.Scale = scale;
            this.texName = texName;
            this.Position = position;
            this.rotation = rotation;
            this.framesPerSecond = framesPerSecond;
            this.repeat = repeat;
            Z = 1;

            FrameTime = new TimeSpan(0, 0, 0, 0, 1000 / framesPerSecond);
            NextUpdate = new TimeSpan(0, 0, 0, 0, FrameTime.Milliseconds);
        }

        public void Update(GameTime gameTime)
        {
            if (gameTime.TotalGameTime > NextUpdate)
            {
                NextUpdate = gameTime.TotalGameTime + FrameTime;

                currentFrame.X++;
                if (currentFrame.X > sheetSize.X)
                {
                    if (repeat)
                    {
                        currentFrame.X = 0;
                    }
                    else
                    {
                        SceneGraphManager.RemoveObject(this);
                    }
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            Texture2D texture = TextureManager.GetTexture(texName).BaseTexture as Texture2D;
            Vector2 textureOrigin = new Vector2(frameSize.X / 2, frameSize.Y / 2);
            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(texture, Position - CameraManager.Camera.Pos, new Rectangle(
                                frameSize.X * currentFrame.X,
                                frameSize.Y * currentFrame.Y,
                                frameSize.X,
                                frameSize.Y),
                                Color.White * alpha, rotation, textureOrigin, Scale, SpriteEffects.None, 0f);
            ScreenManager.SpriteBatch.End();
        }
    }
}
