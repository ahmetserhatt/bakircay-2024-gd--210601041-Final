using System;
using System.Collections;
using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    private Vector3 startposition;
    private Vector3 fallposition;
    public string FruitName;
    public float backduration = 3f;
    private Camera mainCamera;
    public Rigidbody rb;
    private bool isDragging = false;
    public SimplePlatform _sp;
    private float elapsedTime = 0;
    public float height = 3f;
    private bool isBack;
    public float dragOffsetY = 0.5f;
    private Vector3 screenPoint;
    private Vector3 offset;
    private float initialY;
    private float maxDragHeight = 1f;

    void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
        fallposition = transform.position;
        initialY = transform.position.y;
    }

    void Update()
    {
        if (isBack)
        {
            if (elapsedTime < backduration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / backduration;
                Vector3 horizontalPosition = Vector3.Lerp(transform.position, startposition, t);
                float arcHeight = Mathf.Sin(t * Mathf.PI) * height;
                transform.position = new Vector3(horizontalPosition.x, horizontalPosition.y + arcHeight, horizontalPosition.z);
            }
            else
            {
                isBack = false;
                rb.isKinematic = false;
                elapsedTime = 0;
            }
        }
    }

    void OnMouseDown()
    {
        isDragging = true;
        rb.isKinematic = true;
        startposition = transform.position;

        screenPoint = mainCamera.WorldToScreenPoint(transform.position);
        offset = transform.position - mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 currentScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
            Vector3 currentPosition = mainCamera.ScreenToWorldPoint(currentScreenPoint) + offset;

            // 🎯 **Hassasiyet ayarı ekle**
            float dragSpeed = 10f; // **Hassasiyet faktörü (Bunu artırarak hassasiyeti artırabilirsin)**
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, currentPosition, dragSpeed * Time.deltaTime);

            // **Yükseklik sınırlamasını daha hassas yap**
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, initialY, initialY + maxDragHeight);

            // **Doğrudan Transform değiştirerek hassasiyeti artır**
            transform.position = smoothedPosition;
        }
    }


    void OnMouseUp()
    {
        isDragging = false;
        rb.isKinematic = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Placement Area")
        {
            _sp = other.GetComponent<SimplePlatform>();

            if (_sp.CurrentFruit == null)
            {
                _sp.CurrentFruit = this;
                FreezeOnPlatform();
            }
            else if (_sp.CurrentFruit != this && _sp.CurrentFruit.FruitName == this.FruitName)
            {
                rb.isKinematic = false;
                this.gameObject.layer = 6;
                _sp.CurrentFruit.rb.isKinematic = false;
                _sp.CurrentFruit.gameObject.layer = 6;

                // 🎆 SimplePlatform'daki efekti çağır!
                _sp.PlayMatchEffect(transform.position);

                // 🏆 Skor arttırma işlemi (2X skor desteği eklendi)
                _sp.UpdateScore(5);

                // **Eşleşme sayısını artır ve oyunu kontrol et**
                _sp.IncreaseMatchCount();

                // 🛠 İki meyvenin de yok edilmesini sağla
                GameObject matchedFruit = _sp.CurrentFruit.gameObject;
                _sp.CurrentFruit = null;

                // **Meyve sayısını 2 azalt**
                _sp.FruitsCountsText.text = "Fruit Count: " + (_sp.Fruits.childCount - 2).ToString();

                // 🛠 Destroy işlemini geciktirerek çökmeyi önle
                StartCoroutine(DestroyMatchedFruits(matchedFruit));
            }
            else if (_sp.CurrentFruit != this && _sp.CurrentFruit.FruitName != this.FruitName)
            {
                isBack = true;
                rb.isKinematic = true;
            }
        }
    }

    // **Coroutine ile iki meyveyi de gecikmeli yok et**
    private IEnumerator DestroyMatchedFruits(GameObject matchedFruit)
    {
        yield return new WaitForSeconds(0.2f); // 0.2 saniye bekle
        Destroy(matchedFruit); // İlk meyveyi sil
        Destroy(gameObject);   // İkinci meyveyi sil
    }



    private void FreezeOnPlatform()
    {
        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_sp.CurrentFruit == this)
        {
            _sp.CurrentFruit = null;
        }
    }
}
