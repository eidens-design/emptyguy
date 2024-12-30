using System;
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using OrthographicCamera = Engine.OrthographicCamera;

namespace MinimalReproduction;

public class Game1 : Game
{
    GraphicsDeviceManager graphics;
    SpriteBatch spriteBatch;
    private Texture2D _tileTexture;
    Vector2 playerPosition;
    RenderTarget2D _renderTarget;

    static int scaleFactor = 4;
    int targetWidth = 640 * scaleFactor;
    int targetHeight = 320 * scaleFactor;

    private Texture2D _minimalTileset;
    private Texture2DAtlas _minimalAtlas;
    
    private Screen _screen;
    private OrthographicCamera _camera;

    private Vector2 _playerPosition = Vector2.Zero;

    private Texture2D _pixel;

    public Game1()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
    }

    protected override void Initialize()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);

        _screen = new Screen(
            GraphicsDevice.Viewport.Width,
            GraphicsDevice.Viewport.Height
        );
        _camera = new OrthographicCamera(_screen)
        {
            Position = Vector2.Zero
        };

        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });

        RandomUtils.Initialize(0);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _renderTarget = new RenderTarget2D(GraphicsDevice, targetWidth, targetHeight);
        
        _tileTexture = Content.Load<Texture2D>("tile");
        
        _minimalTileset = Content.Load<Texture2D>("MinimalTileset");
        _minimalAtlas = Texture2DAtlas.Create("Atlas/tileset", _minimalTileset, 16, 16);
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardExtended.Update();
        MouseExtended.Update();
        var keyState = KeyboardExtended.GetState();
        
        Vector2 movement = Vector2.Zero;
        float moveSpeed = 5f;
        
        if (keyState.IsKeyDown(Keys.W) || keyState.IsKeyDown(Keys.Up))
            movement.Y -= moveSpeed;
        if (keyState.IsKeyDown(Keys.S) || keyState.IsKeyDown(Keys.Down))
            movement.Y += moveSpeed;
        if (keyState.IsKeyDown(Keys.A) || keyState.IsKeyDown(Keys.Left))
            movement.X -= moveSpeed;
        if (keyState.IsKeyDown(Keys.D) || keyState.IsKeyDown(Keys.Right))
            movement.X += moveSpeed;
        
        if (movement.Length() > 0)
            movement.Normalize();

        _playerPosition += (movement * moveSpeed);
        
        _camera.Zoom(keyState);

        _screen.CalculateEffectiveResolution(
            GraphicsDevice.Viewport.Width,
            GraphicsDevice.Viewport.Height
        );

        _camera.FollowTarget(_playerPosition);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(_renderTarget);
        GraphicsDevice.Clear(Color.Black);
        
        spriteBatch.Begin(transformMatrix: _camera.Matrix, samplerState: SamplerState.PointClamp, sortMode: SpriteSortMode.FrontToBack);
        
        // Picking the tileTexture only from the atlas
        /*
        for (int x = 0; x < targetWidth/16; x++)
        {
            for (int y = 0; y < targetHeight/16; y++)
            {
                var position = new Vector2(x * 16, y * 16);
                spriteBatch.Draw(_tileTexture,
                    position,
                    _minimalAtlas.GetRegion(1).Bounds,
                    Color.Red,
                    0,
                    Vector2.Zero,
                    Vector2.One,
                    SpriteEffects.None,
                    0f);
            }
        }
        */
        
        // This does not give any weird bleeding lines
        // Rendering the tileTexture as a single texture
        /*
        for (int x = 0; x < targetWidth/16; x++)
        {
            for (int y = 0; y < targetHeight/16; y++)
            {
                var position = new Vector2(x * 16, y * 16);
                spriteBatch.Draw(_tileTexture, position, Color.White);
            }
        }
        */
        
        // Picking the tileTexture only from the atlas
        // and some rounding stuff
        /*
        for (int x = 0; x < targetWidth/16; x++)
        {
            for (int y = 0; y < targetHeight/16; y++)
            {
                var position = new Vector2(
                    (float)Math.Round((decimal)(x * 16)), 
                    (float)Math.Round((decimal)(y * 16))
                );

                // Source rect for the dirt tile in the tileset
                var sourceRect = _minimalAtlas.GetRegion(1).Bounds;
                    
                spriteBatch.Draw(
                    _minimalTileset, 
                    Vector2.Floor(position),
                    sourceRect, 
                    Color.White,
                    0,
                    Vector2.Zero,
                    Vector2.One,
                    SpriteEffects.None,
                    0f
                );
            }
        }
        */
        
        // Just for debugging. Center of camera / "player" position
        spriteBatch.Draw(_pixel,
            _playerPosition,
            new Rectangle((int)_playerPosition.X, (int)playerPosition.Y, 16, 16),
            Color.Red,
            0,
            Vector2.Zero,
            Vector2.One,
            SpriteEffects.None,
            0f);
        
        spriteBatch.End();

        GraphicsDevice.SetRenderTarget(null);

        spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        if (GraphicsDevice.Viewport.Width <= targetWidth && GraphicsDevice.Viewport.Height <= targetHeight)
        {
            spriteBatch.Draw(_renderTarget, new Rectangle(0, 0, targetWidth, targetHeight), Color.White);
        }
        else
        {
            float outputAspect = (float)GraphicsDevice.Viewport.Width / GraphicsDevice.Viewport.Height;
            float preferredAspect = (float)targetWidth / targetHeight;
            Rectangle destinationRectangle;

            if (outputAspect <= preferredAspect)
            {
                int presentHeight = (int)(GraphicsDevice.Viewport.Width / preferredAspect);
                int barHeight = (GraphicsDevice.Viewport.Height - presentHeight) / 2;
                destinationRectangle = new Rectangle(0, barHeight, GraphicsDevice.Viewport.Width, presentHeight);
            }
            else
            {
                int presentWidth = (int)(GraphicsDevice.Viewport.Height * preferredAspect);
                int barWidth = (GraphicsDevice.Viewport.Width - presentWidth) / 2;
                destinationRectangle = new Rectangle(barWidth, 0, presentWidth, GraphicsDevice.Viewport.Height);
            }

            spriteBatch.Draw(_renderTarget, destinationRectangle, Color.White);
        }

        spriteBatch.End();

        base.Draw(gameTime);
    }
}