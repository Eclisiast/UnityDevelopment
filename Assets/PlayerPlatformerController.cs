using UnityEngine;

public class PlayerPlatformerController : PhysicsObject
{
   private SpriteRenderer _spriteRenderer;
   private Animator _animator;

   public float MaxSpeed = 7;
   public float JumpTakeOffSpeed = 7;

   // Start is called before the first frame update
   void Awake()
   {
      _spriteRenderer = GetComponent<SpriteRenderer>();
      _animator = GetComponent<Animator>();
   }

   protected override void ComputeVelocity()
   {
      Vector2 move = Vector2.zero;

      move.x = Input.GetAxis("Horizontal");

      if (Input.GetButtonDown("Jump") && IsGrounded)
      {
         Velocity.y = JumpTakeOffSpeed;
      }
      else if (Input.GetButtonUp("Jump"))
      {
         if (Velocity.y > 0)
         {
            // Reduce velocity when the player releases the jump button.
            Velocity.y *= .5f;
         }
      }

      bool shouldFlipSprite = (_spriteRenderer.flipX ? (move.x > 0.01f) : (move.x < 0.01f));
      if (shouldFlipSprite)
      {
         _spriteRenderer.flipX = !_spriteRenderer.flipX;
      }

      _animator.SetBool("grounded", IsGrounded);
      _animator.SetFloat("velocityX", Mathf.Abs(Velocity.x) / MaxSpeed);

      TargetVelocity = move * MaxSpeed;
   }
}
