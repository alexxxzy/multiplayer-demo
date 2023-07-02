using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CharacterMovement : MonoBehaviour
{
    MouseInput mouseInput;
    [SerializeField]
    Tilemap map;
    [SerializeField]
    float movementSpeed;
    Vector3 destination;
    private void Awake()
    {
        mouseInput = new MouseInput();
    }

    private void OnEnable()
    {
        mouseInput.Enable();
    }

    private void OnDisable()
    {
        mouseInput.Disable();
    }
    // Start is called before the first frame update
    void Start()
    {
        mouseInput.Mouse.MouseClick.performed += _ => MouseClick();
        destination = transform.position;
    }

    void MouseClick()
    {
        Debug.Log("MouseClick");
        Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();
        //to make sure movement inside of the grid
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3Int gridPosition = map.WorldToCell(mousePosition);
        if (map.HasTile(gridPosition))
        {
            destination = mousePosition;
            Debug.Log("mousePosition" + mousePosition);
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, destination) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, movementSpeed * Time.deltaTime);

            //Vector3 direction = (destination - transform.position).normalized;
            //float maxDistance = 0.22f; // Maximum distance for the raycast
            //RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxDistance);

            //// Check if a collider is detected within the specified distance
            //if (hit.collider != null && hit.collider != GetComponent<Collider2D>())
            //{
            //    Debug.Log("hit.distance " + hit.distance);
            //    Debug.Log("hit.collider " + hit.collider);
            //    Debug.Log("STOPPING");
            //    destination = transform.position;
            //}

        }
        else
        {
            //destination = transform.position;

        }
    }
}
