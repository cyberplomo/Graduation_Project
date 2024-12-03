using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f; // Hareket hızı
    public float rotationSpeed = 720f; // Dönüş hızı (derece/sn)

    private void Update()
    {
        // Hareket girişi al (WASD veya ok tuşları)
        float horizontal = Input.GetAxis("Horizontal"); // A ve D veya sol/sağ ok tuşları
        float vertical = Input.GetAxis("Vertical"); // W ve S veya yukarı/aşağı ok tuşları

        // Hareket yönünü hesapla
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // Karakteri hareket ettir
            transform.Translate(direction * speed * Time.deltaTime, Space.World);

            // Karakterin bakış yönünü değiştir
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }
    
}