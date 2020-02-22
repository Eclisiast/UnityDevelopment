using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
   protected const float MinMoveDistance = 0.001f;
   protected const float ShellRadius = 0.01f;

   protected Rigidbody2D Rigidbody2D;
   protected Vector2 Velocity;
   protected Vector2 TargetVelocity;
   protected ContactFilter2D ContactFilter2D;

   protected RaycastHit2D[] HitBuffer = new RaycastHit2D[16];
   protected List<RaycastHit2D> HitBufferList = new List<RaycastHit2D>(16);

   protected bool IsGrounded;
   protected Vector2 GroundNormal;

   public float GravityModifier = 1f;
   public float MinGroundNormalY = .65f;

   private void OnEnable()
   {
      Rigidbody2D = GetComponent<Rigidbody2D>();
   }

   // Start is called before the first frame update
   void Start()
   {
      // Don't check collision against triggers.
      ContactFilter2D.useTriggers = false;

      // Get a layer mask from the project settings for Physics2D - Edit/Poject Settings/Physics 2D
      // Use the Physics2D settings to determine what layers to check collisions against.
      ContactFilter2D.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));

      ContactFilter2D.useLayerMask = true;
   }

   // Update is called once per frame
   void Update()
   {
      // Clear the velocity per frame.
      TargetVelocity = Vector2.zero;
      ComputeVelocity();
   }

   protected virtual void ComputeVelocity() { }

   // Physics related updates
   private void FixedUpdate()
   {
      // Physics2D.gravity is default gravity value of engine.
      Velocity += GravityModifier * Physics2D.gravity * Time.deltaTime;

      // Testing x before y better handles slopes.
      Velocity.x = TargetVelocity.x;

      // Until a collison is registered, grounded should be false.
      IsGrounded = false;

      // Time.deltaTime is the time it took to render the last frame.
      Vector2 deltaPosition = Velocity * Time.deltaTime;

      Vector2 moveAlongGroundDirection = new Vector2(GroundNormal.y, -GroundNormal.x);

      // x Movement
      Vector2 move = moveAlongGroundDirection * deltaPosition.x;
      Movement(move, false);

      // y Movement
      move = Vector2.up * deltaPosition.y;
      Movement(move, true);
   }

   private void Movement(Vector2 move, bool yMovement)
   {
      float distance = move.magnitude;

      // Only check for collisons if the distance the object has moved is greater than the minimum distance.
      // This avoids doing collision detection when an object hasn't moved.
      if (distance > MinMoveDistance)
      {
         // distance + shellRadius gives us a little padding so that we don't get stuck in another object.
         // Cast checks if any of the attached colliders are going to overlap in the next frame.
         int count = Rigidbody2D.Cast(move, ContactFilter2D, HitBuffer, distance + ShellRadius);

         // List of current active contacts. These overlap with the physics collider
         HitBufferList.Clear();
         for (int i = 0; i < count - 1; i++)
         {
            HitBufferList.Add(HitBuffer[i]);
         }

         // Check the normal of each raycast hit to determine the angle of the collison.
         foreach (RaycastHit2D raycastHit2D in HitBufferList)
         {
            Vector2 currentNormal = raycastHit2D.normal;

            // Is the player grounded or not. 
            if (currentNormal.y > MinGroundNormalY)
            {
               IsGrounded = true;

               if (yMovement)
               {
                  GroundNormal = currentNormal;
                  currentNormal.x = 0;
               }
            }

            // The difference between the velocity and the current normal. Allow player to hit head and scrape along ceiling.
            float projection = Vector2.Dot(Velocity, currentNormal);
            if (projection < 0)
            {
               // Cancel out the part of the velocity that would be stopped by the collision.
               Velocity -= projection * currentNormal;
            }

            float modifiedDistance = raycastHit2D.distance - ShellRadius;
            distance = modifiedDistance < distance ? modifiedDistance : distance;
         }
      }

      Rigidbody2D.position += move.normalized * distance;
   }
}
