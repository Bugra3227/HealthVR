using System.Collections.Generic;
using System.IO; // Dosya işlemleri için gerekli.
using UnityEngine;

public class PlayerIdleTracker : MonoBehaviour
{
    public PathTracking pathTracking;
    public Transform playerHead; // OVRCameraRig'in CenterEyeAnchor veya benzeri baş pozisyonu.
    public float idleThreshold = 0.01f; // Hareket algılama eşiği.


    private Vector3 lastPosition; // Oyuncunun bir önceki pozisyonu.
    private float idleTime = 0f; // Şu anki duraksama süresi.
    private float totalIdleTime = 0f; // Toplam duraksama süresi.
    private bool isIdle = false; // Oyuncu duraksıyor mu?
    private int idleTimeCount;
    void Start()
    {
        // İlk pozisyonu kaydet.
        lastPosition = playerHead.position;
    }

    void Update()
    {
        if (!pathTracking.HasStarted)
            return;
        // Oyuncunun mevcut pozisyonunu al.
        Vector3 currentPosition = playerHead.position;

        // Hareket miktarını hesapla.
        float movement = Vector3.Distance(currentPosition, lastPosition);

        // Eğer hareket eşiğin altındaysa oyuncu duraksıyor.
        if (movement < idleThreshold)
        {
            if (!isIdle)
            {
                isIdle = true;
                Debug.Log("Player duraksamaya başladı.");
            }

            // Duraksama süresini artır.
            idleTime += Time.deltaTime;
        }
        else
        {
            if (isIdle)
            {
                isIdle = false;

                // Toplam duraksama süresine ekle.
                totalIdleTime += idleTime;
                if (idleTime >= 3)
                    idleTimeCount++;
               
                Debug.Log($"Player duraksamayı bitirdi. Bu duraksama süresi: {idleTime} saniye.");
            }

            // Duraksama süresini sıfırla.
            idleTime = 0f;
        }

        // Mevcut pozisyonu bir sonraki çerçeve için kaydet.
        lastPosition = currentPosition;
    }


    public string ReturnTotalIdleTime()
    {
      
        string totalEntry = totalIdleTime.ToString("F2");
        return totalEntry;
    }

    public string IdleTimeCount()
    {
        return idleTimeCount.ToString();
    }

}