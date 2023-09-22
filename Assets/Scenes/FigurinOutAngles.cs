using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FigurinOutAngles : MonoBehaviour
{
    [SerializeField] private GameObject projectile;
    void Update()
    {
        // Rotate to face the mouse's screen position.
        Vector3 mousePos = Input.mousePosition;

        Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);
        mousePos.x -= objectPos.x;
        mousePos.y -= objectPos.y;

        // Get the angle between the points.
        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;

        // Rotate the object to face the mouse.
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        if (Input.GetMouseButtonDown(0))
        {
            // Spawn a projectile at the gun's position and rotation.
            var aProjectile = Instantiate(projectile, transform.position, transform.rotation);
            aProjectile.GetComponent<Rigidbody2D>().velocity = transform.right * 10f;
            Debug.Log("Firing angle: " + transform.right);
        }
    }
}
