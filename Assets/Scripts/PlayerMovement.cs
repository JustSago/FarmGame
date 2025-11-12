using System;
using UnityEngine;

public enum direction
{
    Up,
    Down,
    Left,
    Right,
}

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rigidBody;
    private SpriteAnimator animator;

    private Vector2 _direction;
    public float speed;
    public direction lastDirection = direction.Down;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<SpriteAnimator>();
    }

    private void Update()
    {
        _direction = new Vector2(0, 0);

        if (Input.GetKey(KeyCode.W))
        {
            _direction.y = 1;
            lastDirection = direction.Up;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            _direction.y = -1;
            lastDirection = direction.Down;
        }

        if (Input.GetKey(KeyCode.A))
        {
            _direction.x = -1;
            lastDirection = direction.Left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            _direction.x = 1;
            lastDirection = direction.Right;
        }
        if (_direction != Vector2.zero)
        {
            switch (lastDirection)
            {
                case direction.Up:
                    animator.PlayAnimation("WalkUp");
                    break;

                case direction.Down:
                    animator.PlayAnimation("WalkDown");
                    break;

                case direction.Left:
                    animator.PlayAnimation("WalkLeft");
                    break;

                case direction.Right:
                    animator.PlayAnimation("WalkRight");
                    break;
            }
        }
        else
        {
            switch (lastDirection)
            {
                case direction.Up:
                    animator.PlayAnimation("IdleUp");
                    break;

                case direction.Down:
                    animator.PlayAnimation("IdleDown");
                    break;

                case direction.Left:
                    animator.PlayAnimation("IdleLeft");
                    break;

                case direction.Right:
                    animator.PlayAnimation("IdleRight");
                    break;
            }
        }
    }

    private void FixedUpdate()
    {
        _rigidBody.AddForce(_direction.normalized * (speed * Time.fixedDeltaTime));
    }
}