using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PoorEngine.Interfaces;
using Microsoft.Xna.Framework;
using PoorEngine.Managers;
using PoorEngine.Textures;
using PoorEngine.Helpers;
using Microsoft.Xna.Framework.Graphics;

namespace PoorEngine.SceneObject
{
    public class AntiAirVehicle : GroundVehicle, IPoorWeaponHolder, IPoorEnemy
    {
        private bool _requiredForVictory;
        public bool RequiredForVictory { get { return _requiredForVictory; } }

        private AntiAirCannon _weapon;
        private float _weaponOrientation;
        
        public float Orientation { get { return _weaponOrientation; } }

        private Airplane _target;
        public Airplane Target { get { return _target; } }

        public bool HasTarget { get { return _target != null; } }

        public AntiAirVehicle(int maxHealth, bool requiredForVictory) // Add weapon parameter (obj/str)
            : base(maxHealth, "enemy_antiair_body", "enemy_antiair_destroyed", "antiair")
        {
            _requiredForVictory = requiredForVictory;

            _type = "antiair";
            Scale = new Vector2(0.4f, 0.4f);

            TextureNameWeapon = "enemy_antiair_weapon";
            _weapon = new AntiAirCannon(this);
        }

        public override void Update(GameTime gameTime)
        {
            if (!_destroyed && _health <= 0)
            {
                EngineManager.Score += 3;
            }
            if (!_destroyed) UpdateAI(gameTime);
            base.Update(gameTime);
            if (!RequiredForVictory)
            {
                if (!_destroyed && Position.X < CameraManager.Camera.Pos.X - 2000)
                {
                    SoundFxManager.RemoveFx(_engineFX_id);
                    SoundFxManager.RemoveFx(_fireBulletFX_id);
                    SceneGraphManager.RemoveObject(this);
                    return;
                }
            }

        }

        public override void Draw(GameTime gameTime)
        {
            Texture2D weaponTexture = TextureManager.GetTexture(TextureNameWeapon).BaseTexture as Texture2D;
            Vector2 weaponOrigin = new Vector2(weaponTexture.Width / 2, weaponTexture.Height -5);

            Texture2D bodyTexture = TextureManager.GetTexture(TextureName).BaseTexture as Texture2D;
            Vector2 weaponMountPoint = new Vector2(bodyTexture.Width / 2 * Scale.X, 20f);

            _weapon.Position = new Vector2(bodyTexture.Width / 2 * Scale.X, 20f);

            if (!_destroyed)
            {
                ScreenManager.SpriteBatch.Begin();
                ScreenManager.SpriteBatch.Draw(weaponTexture,
                                                   CameraManager.Camera.Normalize(Position) + _weapon.Position,
                                                   null,
                                                   Color.White,
                                                   (float)CalcHelper.DegreeToRadian((double)_weaponOrientation),
                                                   weaponOrigin,
                                                   (float)(Scale.X),
                                                   SpriteEffects.None,
                                                   0f);
                ScreenManager.SpriteBatch.End();
            }

            base.Draw(gameTime);
        }

        private void UpdateAI(GameTime gameTime)
        {
            SetTarget();
            if (HasTarget)
            {
                _weapon.Angle = GetAngleToTarget(gameTime);
                _weaponOrientation = _weapon.Angle;
                _weapon.Fire();
            }
            
        }

        public void SetTarget()
        {
            if (CalcHelper.DistanceBetween(Position, EngineManager.Player.Position) < 800)
                _target = EngineManager.Player;
            else
                _target = null;
        }

        protected float GetAngleToTarget(GameTime gameTime)
        {
            int distanceMod = (int)CalcHelper.DistanceBetween(Position + _weapon.Position, Target.Position) / 10;
            double orientation = CalcHelper.getAngle(Position + _weapon.Position, CalcHelper.calculatePoint(Target.Position, Target.Orientation, (float)Target.LinearVelocity * distanceMod)) - 90f;

            return (float)CalcHelper.formatAngle(orientation);
        }

        public override void GetPoints()
        {
            EngineManager.Score += 3;
        }

        /// <summary>
        /// The bounding box of this object, used for culling.
        /// </summary>
        public override Rectangle BoundingBox
        {
            get
            {
                IPoorTexture tex = TextureManager.GetTexture(TextureName);
                if (tex == null)
                {
                    return new Rectangle(0, 0, 0, 0);
                }
                else
                {
                    Texture2D texture = tex.BaseTexture as Texture2D;
                    int textureWidth = (int)(texture.Width * Scale.X);
                    int textureHeight = (int)(texture.Height * Scale.Y);
                    return new Rectangle(
                            (int)Position.X,
                            (int)(Position.Y + Scale.X * 45),
                            textureWidth,
                            textureHeight - (int)(Scale.X * 45)
                        );
                }
            }
        } 
    }
}
