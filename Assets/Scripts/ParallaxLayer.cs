using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] GameObject camera_object = null;

    [SerializeField] Transform background_leftPoint = null, background_rightPoint = null;
    [SerializeField] Transform ground_leftPoint = null, ground_rightPoint = null;
    [SerializeField] Transform camera_leftPoint = null, camera_rightPoint = null;

    private float ground_sideSpace = 0f, background_sideSpace = 0f;

    public float minX;
    public float maxX;

    void Start()
    {
        float camera_width = camera_leftPoint.position.x - camera_rightPoint.position.x;
        ground_sideSpace = ground_rightPoint.position.x - ground_leftPoint.position.x;
        background_sideSpace = background_leftPoint.position.x - background_rightPoint.position.x - camera_width * 0.5f;

        // 제한 설정
        float background_Width = background_rightPoint.position.x - background_leftPoint.position.x;
        minX = ground_leftPoint.position.x + (background_Width / 2f);
        maxX = ground_rightPoint.position.x - (background_Width / 2f);
    }

    void Update()
    {
        SetPosition();
    }

    void SetPosition()
    {
        float background_xPos = camera_object.transform.position.x 
                            + ((camera_object.transform.position.x - ground_leftPoint.position.x) / ground_sideSpace - 0.5f) * background_sideSpace;

        // 양 끝 위치 제한
        background_xPos = Mathf.Clamp(background_xPos, minX, maxX);
        transform.position = new Vector2(background_xPos, transform.position.y);
    }
}
