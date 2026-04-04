using UnityEngine;
using UnityEngine.InputSystem;

public class PointCursorBehavior : MonoBehaviour
{
    private Transform player;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
        // get the location of the mouse in world space
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        Vector3 lookDir = mousePos - transform.position;
        float rot = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot);

        if(player.localScale.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, rot * -1f);
        } else if (player.localScale.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, rot * -1f);
        }
    }
}
