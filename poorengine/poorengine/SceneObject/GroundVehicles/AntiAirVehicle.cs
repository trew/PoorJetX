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
        private AntiAirCannon _weapon;
        private float _weaponOrientation;
        
        public float Orientation { get { return _weaponOrientation; } }

        private Airplane _target;
        public Airplane Target { get { return _target; } }

        public bool IsDead { get { return _destroyed; } }

        public AntiAirVehicle(int maxHealth) // Add weapon parameter (obj/str)
            : base(maxHealth, "enemy_antiair_body", "enemy_antiair_destroyed")
        {
            _type = "battle";
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
        }

        public override void Draw(GameTime gameTime)
        {
            Texture2D weaponTexture = TextureManager.GetTexture(TextureNameWeapon).BaseTexture as Texture2D;
            Vector2 weaponOrigin = new Vector2(weaponTexture.Width / 2, weaponTexture.Height -5);

            Texture2D bodyTexture = TextureManager.GetTexture(TextureName).BaseTexture as Texture2D;
            Vector2 weaponMountPoint = new Vector2(bodyTexture.Width / 2 * Scale.X, 20f);

            _weapon.Position = new Vector2(bodyTexture.Width / 2 * Scale.X, 20f);

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

            base.Draw(gameTime);
        }

        private void UpdateAI(GameTime gameTime)
        {
            SetTarget(gameTime);
            _weapon.Angle = GetAngleToTarget(gameTime);
            _weaponOrientation = _weapon.Angle;

            if(CalcHelper.DistanceBetween(Position, Target.Position) < 800)
            {
                _weapon.Fire();
            }

            
        }

        void SetTarget(GameTime gameTime)
        {
            if (_target == null)
                _target = EngineManager.Player;
        }

        protected float GetAngleToTarget(GameTime gameTime)
        {
            int distanceMod = (int)CalcHelper.DistanceBetween(Position, Target.Position) / 10;
            double orientation = CalcHelper.getAngle(Position, CalcHelper.calculatePoint(Target.Position, Target.Orientation, (float)Target.LinearVelocity * distanceMod)) - 90f;

            return (float)CalcHelper.formatAngle(orientation);
        }
    }
}
