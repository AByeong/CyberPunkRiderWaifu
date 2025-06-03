using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
    public Transform Player;

    [SerializeField]
    private float _height;

    private void Update()
    {
        Vector3 cameraPos = Player.position;
        cameraPos.y = _height;
        transform.position = cameraPos;

        float yRotation = Camera.main.transform.eulerAngles.y;
        transform.eulerAngles = new Vector3(90, yRotation, 0);
    }
}
