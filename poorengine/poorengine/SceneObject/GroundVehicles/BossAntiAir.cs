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
    public class BossAntiAir : GroundVehicle, IPoorWeaponHolder, IPoorEnemy
    {
        private AntiAirCannon _weapon1;
        private AntiAirCannon _weapon2;
        private float _weaponOrientation1;
        private float _weaponOrientation2;
        
        public float Orientation { get { return _weaponOrientation1; } }

        private Airplane _target;
        public Airplane Target { get { return _target; } }
        Vector2 WeaponScale;

        public bool RequiredForVictory { get; set; }

        public BossAntiAir(int maxHealth, bool requiredForVictory) // Add weapon parameter (obj/str)
            : base(maxHealth, "enemy_antiair_body", "enemy_antiair_destroyed", "bossantiair")
        {
            Scale = new Vector2(1.3f, 1.3f);
            WeaponScale = new Vector2(1f, 1f);
            TextureNameWeapon = "enemy_antiair_weapon";
            _weapon1 = new AntiAirCannon(this);
            _weapon1.Position = new Vector2(60, 110);
            _weapon1.setGunLength(100);

            _weapon2 = new AntiAirCannon(this);
            _weapon2.Position = new Vector2(180, 65);
            _weapon2.setGunLength(100);

            RequiredForVictory = requiredForVictory;
        }

        public override void Update(GameTime gameTime)
        {
            if (!_destroyed && _health <= 0)
            {
                EngineManager.Score += 20;
            }
            if (!_destroyed) UpdateAI(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Texture2D weaponTexture = TextureManager.GetTexture(TextureNameWeapon).BaseTexture as Texture2D;
            Vector2 weaponOrigin = new Vector2(weaponTexture.Width / 2, weaponTexture.Height -5);

            Texture2D bodyTexture = TextureManager.GetTexture(TextureName).BaseTexture as Texture2D;

            if (!_destroyed)
            {
                ScreenManager.SpriteBatch.Begin();
                ScreenManager.SpriteBatch.Draw(weaponTexture,
                                                   CameraManager.Camera.Normalize(Position) + _weapon1.Position,
                                                   null,
                                                   Color.White,
                                                   (float)CalcHelper.DegreeToRadian((double)_weaponOrientation1),
                                                   weaponOrigin,
                                                   (float)(WeaponScale.X),
                                                   SpriteEffects.None,
                                                   0f);

                ScreenManager.SpriteBatch.Draw(weaponTexture,
                                   CameraManager.Camera.Normalize(Position) + _weapon2.Position,
                                   null,
                                   Color.White,
                                   (float)CalcHelper.DegreeToRadian((double)_weaponOrientation2),
                                   weaponOrigin,
                                   (float)(WeaponScale.X),
                                   SpriteEffects.None,
                                   0f);

                ScreenManager.SpriteBatch.End();
            }
            base.Draw(gameTime);
            if (RequiredForVictory && !IsDead)
                DrawArrow("arrow", true);
        }

        private void UpdateAI(GameTime gameTime)
        {
            SetTarget(gameTime);

            int distanceMod = (int)CalcHelper.DistanceBetween(Position, Target.Position) / 10;

            _weapon1.Angle = (float)CalcHelper.getAngle(Position + _weapon1.Position, CalcHelper.calculatePoint(Target.Position, Target.Orientation, (float)Target.LinearVelocity * distanceMod)) - 90f;
            _weaponOrientation1 = _weapon1.Angle;

            _weapon2.Angle = (float)CalcHelper.getAngle(Position + _weapon2.Position, CalcHelper.calculatePoint(Target.Position, Target.Orientation, (float)Target.LinearVelocity * distanceMod)) - 90f;
            _weaponOrientation2 = _weapon2.Angle;

            if (CalcHelper.DistanceBetween(Position, Target.Position) < 800 && (!_target.IsCrashing || !_target.IsDead))
            {
                _weapon1.Fire();
                _weapon2.Fire();
            }
        }

        public override void GetPoints()
        {
            EngineManager.Score += 30;
        }

        void SetTarget(GameTime gameTime)
        {
            if (_target == null)
                _target = EngineManager.Player;
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
