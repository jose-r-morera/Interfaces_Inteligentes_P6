using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ejercicio1 : MonoBehaviour
{
    public TMP_Text angularVelocityText;
    public TMP_Text accelerationText;
    public TMP_Text altitudeText;
    public TMP_Text gravityText;
    public TMP_Text latitudeText;
    public TMP_Text longitudeText;

    private Gyroscope gyro;
    private Vector3 acceleration;
    private LocationInfo location;

    // Start is called before the first frame update
    void Start()
    {
        // Giroscopio
        gyro = Input.gyro;
        gyro.enabled = true;

        StartCoroutine(StartLocationService());
    }

    // Update is called once per frame
    void Update()
    {
        // Obtener velocidad angular y gravedad de la brújula
        if (gyro != null)
        {
            angularVelocityText.text = "Velocidad Angular: " + gyro.rotationRate.ToString();
            gravityText.text = "Gravedad: " + gyro.gravity.ToString();
        }

        acceleration = Input.acceleration;
        accelerationText.text = "Aceleración: " + acceleration.ToString();

        // Actualizar datos de ubicación
        if (Input.location.status == LocationServiceStatus.Running)
        {
            location = Input.location.lastData;
            latitudeText.text = "Latitud: " + location.latitude.ToString();
            longitudeText.text = "Longitud: " + location.longitude.ToString();
            altitudeText.text = "Altitud: " + location.altitude.ToString();
        }
        else
        {
            latitudeText.text = "Latitud: ---";
            longitudeText.text = "Longitud: ---";
            altitudeText.text = "Altitud: ---";
        }
    }

    private IEnumerator StartLocationService() {
        // Starts the location service.
        Input.location.Start();

        // Waits until the location service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // If the service didn't initialize in 20 seconds this cancels location service use.
        if (maxWait < 1)
        {
            Debug.Log("Timed out");
            yield break;
        }
    }
}
