using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum AnimationPlayForce
{
    None,
    Hard
}

public class SpriteAnimator : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public SpriteAnimation[] animations;

    [HideInInspector] public string currentFrameTag = "null";

    public bool debug;
    
    private SpriteAnimation _currentAnimation;
    private int _currentAnimID;
    private float _frameTimer;
    private int _currentFrame;
    private bool _hasAnimationBeenManuallySet;
    private List<IAnimationListener> _listeners;
    private float _frameRateOverride = -1f; // -1 means no override

    private void Start()
    {
        _listeners = new List<IAnimationListener>();
        
        foreach (MonoBehaviour mb in FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (mb is IAnimationListener listener)
                _listeners.Add(listener);
        }
        
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer found on this GameObject.");
            return;
        }

        if (!_hasAnimationBeenManuallySet)
        {
            _currentAnimID = 0;
            _currentAnimation = animations[0];
            if (debug) Debug.Log($"[{name}] Starting with animation: {_currentAnimation.animationName}");
            PlayAnimation(_currentAnimation.animationName);
        }
    }

    private void Update()
    {
        if (animations.Length != 0)
        {
            Animate();
        }
    }

    private void Animate()
    {
        SpriteAnimation spriteAnimation = animations[_currentAnimID];
        _frameTimer += Time.deltaTime;

        // Use override frame rate if set, otherwise use animation's frame rate
        float frameRate = _frameRateOverride > 0 ? _frameRateOverride : spriteAnimation.frameRate;
        float frameDuration = 1f / frameRate;

        if (_frameTimer >= frameDuration)
        {
            _frameTimer = 0f;
            
            // Check if we're about to finish a non-looping animation BEFORE incrementing
            bool isLastFrame = _currentFrame >= spriteAnimation.frames.Length - 1;
            bool shouldEnd = isLastFrame && !spriteAnimation.loop;
            
            if (debug)
                Debug.Log($"[{name}] Frame update - Current: {_currentFrame}, IsLast: {isLastFrame}, Loop: {spriteAnimation.loop}, ShouldEnd: {shouldEnd}");
            
            if (shouldEnd)
            {
                Debug.Log($"[{name}] Animation '{spriteAnimation.animationName}' ENDING - calling NotifyAnimationEnd");
                
                // Call OnAnimationEnd before switching
                NotifyAnimationEnd(spriteAnimation.animationName);
                
                if (debug)
                    Debug.Log($"[{name}] Animation '{spriteAnimation.animationName}' completed. Loop is false, switching to default.");
                
                // Clear frame rate override when animation ends
                _frameRateOverride = -1f;
                
                PlayAnimation(0, true);
            }
            else
            {
                // Normal frame increment
                _currentFrame = (_currentFrame + 1) % spriteAnimation.frames.Length;
                spriteRenderer.sprite = spriteAnimation.frames[_currentFrame];
                
                // Update frame tag based on current frame
                UpdateFrameTag();

                if (debug)
                    Debug.Log($"[{name}] Animation: {spriteAnimation.animationName}, Frame: {_currentFrame}/{spriteAnimation.frames.Length}, Tag: {currentFrameTag}");
            }
        }
    }
    
    private void UpdateFrameTag()
    {
        SpriteAnimation spriteAnimation = animations[_currentAnimID];
        
        // Reset to null
        currentFrameTag = "null";
        
        // Check if current frame has a tag
        if (spriteAnimation.frameTags is { Length: > 0 })
        {
            foreach (FrameTag frameTag in spriteAnimation.frameTags)
            {
                if (frameTag.index == _currentFrame)
                {
                    currentFrameTag = frameTag.tag;
                    break;
                }
            }
        }
    }

    private void NotifyAnimationStart(string animationName)
    {
        Debug.Log($"[{name}] NotifyAnimationStart '{animationName}' - Listeners: {_listeners?.Count ?? 0}");
        
        if (_listeners is { Count: > 0 })
        {
            foreach (var listener in _listeners)
            {
                if (listener != null)
                {
                    Debug.Log($"[{name}] -> Calling OnAnimationStart on {listener.GetType().Name}");
                    listener.OnAnimationStart(animationName);
                }
            }
        }
    }

    private void NotifyAnimationEnd(string animationName)
    {
        Debug.Log($"[{name}] NotifyAnimationEnd '{animationName}' - Listeners: {_listeners?.Count ?? 0}");
        
        if (_listeners is { Count: > 0 })
        {
            foreach (var listener in _listeners)
            {
                if (listener != null)
                {
                    Debug.Log($"[{name}] -> Calling OnAnimationEnd on {listener.GetType().Name}");
                    listener.OnAnimationEnd(animationName);
                }
            }
        }
    }

    public void SetFrameRate(int frameRate)
    {
        // Set a temporary override that doesn't modify the original animation data
        _frameRateOverride = frameRate;
    }
    
    public void ClearFrameRateOverride()
    {
        _frameRateOverride = -1f;
    }

    public void PlayAnimation(int animationID, bool forcePlay = false)
    {
        _hasAnimationBeenManuallySet = true;

        if (_currentAnimation.unstoppable && !forcePlay)
        {
            if (debug)
                Debug.Log($"[{name}] Animation '{_currentAnimation.animationName}' is unstoppable. Ignoring PlayAnimation({animationID}).");
            return;
        }

        if (animationID < 0 || animationID >= animations.Length)
        {
            Debug.LogError($"[{name}] Invalid animation ID: {animationID}");
            return;
        }

        if (_currentAnimID != animationID || forcePlay)
        {
            _currentAnimID = animationID;
            _currentAnimation = animations[animationID];
            _currentFrame = 0;
            _frameTimer = 0f;

            if (spriteRenderer != null && _currentAnimation.frames.Length > 0)
                spriteRenderer.sprite = _currentAnimation.frames[_currentFrame];
            
            // Update frame tag for first frame
            UpdateFrameTag();

            // Notify listeners that animation started
            NotifyAnimationStart(_currentAnimation.animationName);

            if (debug)
                Debug.Log($"[{name}] Playing animation: {_currentAnimation.animationName} (ID: {animationID})");
        }
    }

    public void PlayAnimation(string animationName, AnimationPlayForce forcePlay = AnimationPlayForce.None)
    {
        if (_currentAnimation.unstoppable && forcePlay == AnimationPlayForce.None)
        {
            if (debug)
                Debug.Log($"[{name}] Current animation '{_currentAnimation.animationName}' is unstoppable. Ignoring PlayAnimation(\"{animationName}\").");
            return;
        }

        if (_currentAnimation.animationName == animationName && forcePlay == AnimationPlayForce.Hard)
        {
            if (debug)
                Debug.Log($"[{name}] Already playing animation '{animationName}'.");
            return;
        }

        int index = -1;
        for (int i = 0; i < animations.Length; i++)
        {
            if (animations[i].animationName == animationName)
            {
                index = i;
                break;
            }
        }

        if (index == -1)
        {
            Debug.LogError($"[{name}] Animation with name '{animationName}' not found.");
        }
        else
        {
            PlayAnimation(index);
        }
    }

    public void StopAnimation()
    {
        _currentAnimID = 0;
        _currentAnimation = animations[0];
        spriteRenderer.sprite = null;
        currentFrameTag = "null";

        if (debug)
            Debug.Log($"[{name}] Animation stopped.");
    }

    public int GetCurrentAnimIndex()
    {
        return _currentAnimID;
    }

    public string GetCurrentAnimName()
    {
        return _currentAnimation.animationName;
    }

    public SpriteAnimation GetAnimation(string animationName)
    {
        return animations.FirstOrDefault(x => x.animationName == animationName);
    }
}