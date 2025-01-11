using System;
using TMPro;
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
    public float height = 5f;
    private bool isBack;
    public float dragOffsetY = 0.5f;
    private Vector3 screenPoint;
    private Vector3 offset;
    private float initialY;
    private float maxDragHeight = 1.5f;

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

            currentPosition.y = Mathf.Clamp(currentPosition.y, initialY, initialY + maxDragHeight);
            rb.MovePosition(currentPosition);
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
            }
            else if (_sp.CurrentFruit != this && _sp.CurrentFruit.FruitName == this.FruitName)
            {
                rb.isKinematic = false;
                this.gameObject.layer = 6;
                _sp.CurrentFruit.rb.isKinematic = false;
                _sp.CurrentFruit.gameObject.layer = 6;
                _sp.CurrentFruit = null;
                _sp.Score += 5;
                _sp.ScoreText.text = "Score:" + _sp.Score;
                _sp.FruitsCountsText.text = "Fruit Count: " + (_sp.Fruits.childCount - 2).ToString();
            }
            else if (_sp.CurrentFruit != this && _sp.CurrentFruit.FruitName != this.FruitName)
            {
                isBack = true;
                rb.isKinematic = true;
            }
        }

        if (other.transform.name == "Destory Trigger Area")
        {
            Destroy(this.gameObject);
            if (_sp.Fruits.childCount <= 1)
            {
                _sp.ComplatePanel.SetActive(true);
            }
        }

        if (other.transform.name == "WrongFallArea")
        {
            transform.position = fallposition;
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