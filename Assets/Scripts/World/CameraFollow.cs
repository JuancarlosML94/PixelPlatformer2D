using UnityEngine;

// Sigue al personaje sin enseñar nada fuera del dibujo del escenario.
public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Objeto que sigue la cámara.")]
    private Transform _target;

    [SerializeField]
    [Tooltip("Esquina inferior izquierda del escenario (en unidades).")]
    private Vector2 _levelMin = new Vector2(-5.12f, -2.79f);

    [SerializeField]
    [Tooltip("Esquina superior derecha del escenario (en unidades).")]
    private Vector2 _levelMax = new Vector2(5.12f, 2.79f);

    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        // LateUpdate se ejecuta después de mover al personaje.
        if (_target == null)
        {
            return;
        }

        // Media pantalla en unidades: el alto sale del Size y el ancho de la proporción de la pantalla.
        float halfHeight = _camera.orthographicSize;
        float halfWidth = halfHeight * _camera.aspect;

        float cameraX = Mathf.Clamp(_target.position.x, _levelMin.x + halfWidth, _levelMax.x - halfWidth);
        float cameraY = Mathf.Clamp(_target.position.y, _levelMin.y + halfHeight, _levelMax.y - halfHeight);
        transform.position = new Vector3(cameraX, cameraY, transform.position.z);
    }
}
