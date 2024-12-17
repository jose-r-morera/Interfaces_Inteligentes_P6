using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ejercicio2 : MonoBehaviour
{
    public Transform samurai;  // Referencia al objeto del samurái
    public float movementSpeed = 2f;  // Velocidad base
    public float smoothing = 0.1f;    // Suavizado para Slerp
    public float latitudeMin = -90f;  // Rango permitido de latitud
    public float latitudeMax = 90f;
    public float longitudeMin = -180f;
    public float longitudeMax = 180f;

    private Transform rotacion_deseada;
    private bool gpsReady = false;       // GPS listo para usarse
    private Vector3 gravity;             // Vector de gravedad desde el giroscopio
    private LocationInfo location;       // Datos del GPS
    
    void Start()
    {
        // Habilitar el giroscopio
        Input.gyro.enabled = true;

        // Iniciar los servicios de ubicación
        StartCoroutine(StartLocationService());

        // Inicializar transform
        rotacion_deseada = transform;
    }

    void Update()
    {
        // Aplicar orientación al norte
        rotacion_deseada.rotation =  Quaternion.Inverse(Input.gyro.attitude);

        // rotacion_deseada.rotation *= Quaternion.AngleAxis(-Input.compass.trueHeading, Vector3.up); 

       // Corregir la orientación en el eje Z
        rotacion_deseada.Rotate(0f, 0f, 180f, Space.Self);  

        // Alinear el sistema de coordenadas del dispositivo con el de Unity
        rotacion_deseada.Rotate(90f, 180f, 0f, Space.World);

        // Suavizar la rotación final con interpolación Slerp
        samurai.rotation = Quaternion.Slerp(samurai.rotation, rotacion_deseada.rotation, smoothing);

        // Detener al samurái si está fuera del rango permitido
        bool move = true;
        if (Input.location.status == LocationServiceStatus.Running)
        {
            location = Input.location.lastData;
            if (!IsWithinBounds(location.latitude, location.longitude))
            {
                Debug.Log("El samurái está fuera del rango permitido. Deteniendo...");
                movementSpeed = 0f;  // Detener el movimiento
                move = false;
            }
        }
        
        if (move) {
            //  Movimiento proporcional a la aceleración
            Vector3 acceleration = Input.acceleration;
            float forwardMovement = -acceleration.z;  // Invertir el eje z para alinearlo con la vista del dispositivo
            Vector3 movement = samurai.forward * forwardMovement * movementSpeed * Time.deltaTime;
            transform.position += movement;
        }
    }

    private IEnumerator StartLocationService()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogError("El usuario no ha habilitado los servicios de ubicación.");
            yield break;
        }

        Input.location.Start();

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait <= 0 || Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("No se puede inicializar el servicio de ubicación.");
            yield break;
        }

        gpsReady = true;
    }

    private bool IsWithinBounds(float latitude, float longitude)
    {
        return latitude >= latitudeMin && latitude <= latitudeMax &&
               longitude >= longitudeMin && longitude <= longitudeMax;
    }
}

