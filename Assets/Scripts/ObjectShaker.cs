using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectShaker: MonoBehaviour {
    private Vector3 originPosition;
    private Quaternion originRotation;
    public float shakeDecay = 0.002f;
    public float shakeIntensity = .3f;

    private float temp_shake_intensity = 0;
    
    public void Shake(){
        originPosition = transform.position;
        originRotation = transform.rotation;
        temp_shake_intensity = shakeIntensity;
    }
    
    void Update (){
        if (temp_shake_intensity > 0){
            transform.position = originPosition + Random.insideUnitSphere * temp_shake_intensity;
            transform.rotation = new Quaternion(
                originRotation.x + Random.Range (-temp_shake_intensity,temp_shake_intensity) * .2f,
                originRotation.y + Random.Range (-temp_shake_intensity,temp_shake_intensity) * .2f,
                originRotation.z + Random.Range (-temp_shake_intensity,temp_shake_intensity) * .2f,
                originRotation.w + Random.Range (-temp_shake_intensity,temp_shake_intensity) * .2f);
            temp_shake_intensity -= shakeDecay;
        }
    }
}
