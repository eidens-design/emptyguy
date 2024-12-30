namespace Engine;

public sealed class Screen
{
    private float _effectiveWidth;
    public float EffectiveWidth => _effectiveWidth;
    private float _effectiveHeight;
    public float EffectiveHeight => _effectiveHeight;

    private static int _scaleFactor = 4;
    private int _targetWidth = 640 * _scaleFactor;
    public int TargetWidth => _targetWidth;
    private int _targetHeight = 320 * _scaleFactor;
    public int TargetHeight => _targetHeight;

    private int _viewportWidth;
    public int ViewportWidth => _viewportWidth;
    private int _viewportHeight;
    public int ViewportHeight => _viewportHeight;
    
    
    public Screen(int viewportWidth, int viewportHeight)
    {
        _viewportWidth = viewportWidth;
        _viewportHeight = viewportHeight;
    }

    public void CalculateEffectiveResolution(int viewportWidth, int viewportHeight)
    {
        _viewportWidth = viewportWidth;
        _viewportHeight = viewportHeight;
        
        bool isLargerThanTarget = _viewportWidth > _targetWidth || _viewportHeight > _targetHeight;
        float effectiveWidth, effectiveHeight;

        if (isLargerThanTarget)
        {
            float outputAspect = (float)_viewportWidth / _viewportHeight;
            float preferredAspect = (float)_targetWidth / _targetHeight;
            effectiveWidth = outputAspect <= preferredAspect ? _viewportWidth : (_viewportHeight * preferredAspect);
            effectiveHeight = outputAspect <= preferredAspect ? (_viewportWidth / preferredAspect) : _viewportHeight;
        }
        else
        {
            effectiveWidth = _viewportWidth;
            effectiveHeight = _viewportHeight;
        }
        
        _effectiveWidth = effectiveWidth;
        _effectiveHeight = effectiveHeight;
    }
}