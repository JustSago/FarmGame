using System;
using UnityEngine;

[Serializable]
public struct FrameTag
{
    public int index;
    public string tag;
}

[Serializable]
public struct SpriteAnimation
{
    public string animationName;

    public Sprite[] frames;
    public FrameTag[] frameTags;

    public float frameRate;

    public bool loop;

    public bool unstoppable;
}