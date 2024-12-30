namespace Engine;

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;

public sealed class OrthographicCamera
{
    private Vector2 _position;
    public Vector2 Position
    {
        get => _position;
        set => _position = value;
    }

    private Vector2 _newPosition;
    private Matrix _matrix;
    public Matrix Matrix => _matrix;
    private Vector2 _target;
    private float _zoomFactor = 1.0f;
    public float ZoomFactor => _zoomFactor;
    private float _oldZoomFactor = 1.0f;
    private float _cameraSpeed = 0.1f;

    private Screen _screen;

    public OrthographicCamera(Screen screen)
    {
        _screen = screen;
    }

    public void FollowTarget(Vector2 target)
    {
        _target = target;
        _target = target - new Vector2(_screen.EffectiveWidth/(2*_zoomFactor), _screen.EffectiveHeight/(2*_zoomFactor));
        
        _newPosition = Vector2.Lerp(_newPosition, _target, _cameraSpeed);
        
        if (_oldZoomFactor != _zoomFactor)
        {
            Vector2 viewportCenter = new Vector2(_screen.EffectiveWidth / 2, _screen.EffectiveHeight / 2);
            Vector2 worldCenter = _newPosition + viewportCenter / _oldZoomFactor;
            _newPosition = worldCenter - viewportCenter / _zoomFactor;

            // Clamp after zoom to prevent clear color showing
            _newPosition.X = MathHelper.Clamp(_newPosition.X, 0, _screen.TargetWidth - (_screen.EffectiveWidth / _zoomFactor));
            _newPosition.Y = MathHelper.Clamp(_newPosition.Y, 0, _screen.TargetHeight - (_screen.EffectiveHeight / _zoomFactor));
        }
        
        _newPosition.X = MathHelper.Clamp(_newPosition.X, 0, _screen.TargetWidth - (_screen.EffectiveWidth / _zoomFactor));
        _newPosition.Y = MathHelper.Clamp(_newPosition.Y, 0, _screen.TargetHeight - (_screen.EffectiveHeight / _zoomFactor));
       
        _position = _newPosition;
        
        UpdateMatrix(_screen.EffectiveWidth / (2 * _zoomFactor), _screen.EffectiveHeight / (2 * _zoomFactor));
    }

    private void UpdateMatrix(float viewportCenterX, float viewportCenterY)
    {
        var translation = Matrix.CreateTranslation(-viewportCenterX - _position.X, -viewportCenterY - _position.Y, 0);
        var scale = Matrix.CreateScale(new Vector3(_zoomFactor, _zoomFactor, 1));
        var offset = Matrix.CreateTranslation(viewportCenterX * _zoomFactor, viewportCenterY * _zoomFactor, 0);

        _matrix = translation * scale * offset;
    }

    public void Zoom(KeyboardStateExtended keyboardState)
    {
        _newPosition = _position;
        _oldZoomFactor = _zoomFactor;
        
        if (keyboardState.IsKeyDown(Keys.Q))
            _zoomFactor += 0.01f;
        if (keyboardState.IsKeyDown(Keys.E))
            _zoomFactor -= 0.01f;
        
        _zoomFactor = MathHelper.Clamp(_zoomFactor, 1.0f, 4.0f);
        
        Console.WriteLine($"ZoomFactor: {_zoomFactor}");
    }
    
    public Vector2 WorldToScreen(Vector2 worldPosition)
    {
        // Divide by zoom factor before applying matrix
        worldPosition /= _zoomFactor;
        return Vector2.Transform(worldPosition, _matrix);
    }
}