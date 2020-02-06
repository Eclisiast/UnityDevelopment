using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Movement : MonoBehaviour
{
   Rigidbody2D _rb;

   public float _speed = 2.0f;
   private Dictionary<string, string> textList = new Dictionary<string, string>();
   bool canJump = true;

   // Start is called before the first frame update
   void Start()
   {
      _rb = GetComponent<Rigidbody2D>();
      _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
      textList.Add("x", "0");
      textList.Add("y", "0");
      textList.Add("canJump", "");
   }

   //Draw debug text
   void OnDrawGizmos()
   {
      float leftCorner = Camera.main.transform.position.x; //- (Camera.main.scaledPixelWidth * 0.5f);
      float topCorner = Camera.main.transform.position.y; // - (Camera.main.scaledPixelHeight * 0.5f);
      Vector3 topLeft = new Vector3(Camera.main.transform.position.x - (Camera.main.pixelWidth * 0.1f),
         Camera.main.transform.position.y - (Camera.main.pixelHeight * 0.1f));
      int i = 0;
      foreach (string key in textList.Keys)
      {
         //Handles.Label(Camera.main.transform.position + new Vector3(leftCorner + 20f, topCorner + (30f * i)), key + textList[key].ToString());
         topLeft = new Vector3(Camera.main.transform.position.x -3, Camera.main.transform.position.y + i);
         Handles.Label(topLeft, key + textList[key].ToString());
         i++;
      }

      //Camera.main.transform.position
   }

   private void OnCollisionEnter2D(Collision2D collision)
   {
      foreach(ContactPoint2D c in collision.contacts)
      {
         Vector2 collisionDirection = c.point - _rb.position;
         if (collisionDirection.y < 0)
         {
            canJump = true;
         }
      }
   }

   private void OnCollisionExit2D(Collision2D collision)
   {
      foreach (ContactPoint2D c in collision.contacts)
      {
         Vector2 collisionDirection = c.point - _rb.position;
         if (collisionDirection.y > 0)
         {
            canJump = false;
         }
      }
      canJump = false;
   }

   // Update is called once per frame
   void Update()
   {
      if (Input.GetKey(KeyCode.RightArrow))
      {
         _rb.MovePosition(new Vector2(_rb.position.x + 0.2f, _rb.position.y));
         //_rb.velocity = transform.right * _speed;
      }
      if (Input.GetKey(KeyCode.DownArrow))
      {
         _rb.velocity = -transform.up * _speed;
      }
      if (Input.GetKey(KeyCode.LeftArrow))
      {
         //_rb.MovePosition(new Vector2(_rb.position.x - 0.2f, _rb.position.y));
         //_rb.transform.right = new Vector3(3f,0);
         _rb.velocity = -transform.right * _speed;
      }
      if (Input.GetKeyUp(KeyCode.UpArrow))
      {
         jump();
      }

      textList["x"] = _rb.transform.position.x.ToString();//_rb.transform.position.x;
      textList["y"] = _rb.transform.position.y.ToString();
      textList["canJump"] = canJump.ToString();



      if (_rb.velocity.x > 0)
      {
         _rb.velocity = transform.right * 0.8f;
      }
   }

   void jump()
   {
      if (canJump)
      {
         _rb.velocity = transform.up * _speed * 3;
      }
   }
}