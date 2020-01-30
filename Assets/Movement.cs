using UnityEngine;

public class Movement : MonoBehaviour
{
   Rigidbody2D _rb;
   public float _speed = 2.0f;

   // Start is called before the first frame update
   void Start()
   {
      _rb = GetComponent<Rigidbody2D>();
      _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
   }

   // Update is called once per frame
   void Update()
   {
      if (Input.GetKey(KeyCode.RightArrow))
      {
         _rb.velocity = transform.right * _speed;
      }
      if (Input.GetKey(KeyCode.DownArrow))
      {
         _rb.velocity = -transform.up * _speed;
      }
      if (Input.GetKey(KeyCode.LeftArrow))
      {
         _rb.velocity = -transform.right * _speed;
      }
      if (Input.GetKey(KeyCode.UpArrow))
      {
         _rb.velocity = transform.up * _speed;
      }



      if (_rb.velocity.x > 0)
      {
         _rb.velocity = transform.right * 0.8f;
      }
   }
}