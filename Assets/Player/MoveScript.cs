using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MoveScript : MonoBehaviour
{
    // Configuration
    public float speed = 10.0f;

    private float _deltaX = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        _deltaX = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        _deltaX = Mathf.Clamp(_deltaX, -1.5f, 1.5f);

        transform.position = new Vector3(_deltaX, transform.position.y, transform.position.z);
    }

    
}