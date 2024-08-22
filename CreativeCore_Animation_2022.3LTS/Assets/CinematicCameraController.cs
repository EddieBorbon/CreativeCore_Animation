using UnityEngine;
using Cinemachine;

public class CinematicCameraController : MonoBehaviour
{
    public Transform target; // El objeto alrededor del cual la cámara se moverá (la plataforma)

    // Parámetros para los rangos de oscilación
    public float radiusMin = 8.0f;
    public float radiusMax = 17.0f;
    public float speedMin = -0.5f;
    public float speedMax = 0.5f;
    public float heightMin = 2.6f;
    public float heightMax = 14.0f;

    // Parámetro para controlar la tasa de cambio del Lerp del speed
    public float speedLerpRate = 0.1f;

    private CinemachineVirtualCamera virtualCamera;
    private float angle = 0f;
    private float time;
    private float currentSpeed;
    private Vector3 initialCameraPosition;

    // Tiempo para iniciar el movimiento de la cámara
    private float initialWaitTime = 10f;
    private bool transitionStarted = false;
    private float transitionDuration = 3f; // Duración de la transición suave
    private float transitionTime = 0f;

    void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        if (virtualCamera == null)
        {
            Debug.LogError("CinemachineVirtualCamera component not found.");
        }

        currentSpeed = speedMin;
        initialCameraPosition = virtualCamera.transform.position;
    }

    void Update()
    {
        if (target != null && virtualCamera != null)
        {
            time += Time.deltaTime;

            if (time >= initialWaitTime)
            {
                if (!transitionStarted)
                {
                    // Iniciar la transición
                    transitionStarted = true;
                    transitionTime = 0f;
                }

                transitionTime += Time.deltaTime;
                float t = Mathf.Clamp01(transitionTime / transitionDuration);

                // Oscilar los valores usando funciones seno
                float radius = Mathf.Lerp(radiusMin, radiusMax, (Mathf.Sin(time - initialWaitTime) + 1.0f) / 2.0f);
                float height = Mathf.Lerp(heightMin, heightMax, (Mathf.Sin((time - initialWaitTime) * 0.5f) + 1.0f) / 2.0f);

                // Lerp suavizado para la velocidad
                float targetSpeed = Mathf.Lerp(speedMin, speedMax, (Mathf.Sin((time - initialWaitTime) * 0.3f) + 1.0f) / 2.0f);
                currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, speedLerpRate * Time.deltaTime);

                // Actualizar el ángulo con la velocidad
                angle += currentSpeed * Time.deltaTime;

                // Calcular la nueva posición de la cámara
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;
                Vector3 offset = new Vector3(x, height, z);
                Vector3 newPosition = target.position + offset;

                // Interpolación suave entre la posición inicial y la nueva posición
                virtualCamera.transform.position = Vector3.Lerp(initialCameraPosition, newPosition, t);
                virtualCamera.transform.LookAt(target);
            }
        }
    }
}
