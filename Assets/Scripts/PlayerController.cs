using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera _cam;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _aimSpeed = 5f;
    [SerializeField] private GameObject _rockPrefab;
    

    [Range(-180, 180)]
    [SerializeField] private float _minYAim;
    [Range(-180, 180)]
    [SerializeField] private float _maxYAim;

    [SerializeField] private bool _clampX;
    [Range(-180, 180)]
    [SerializeField] private float _minXAim;
    [Range(-180, 180)]
    [SerializeField] private float _maxXAim;

    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private int _rocksStock=10;
    [SerializeField] private UnityEvent<int> _onRocksStockChanged;

    [SerializeField] private RockPicker _rockPicker;

    [Header("Shooting")]
    [SerializeField] private float _maxHoldTime = 1f;
    [Tooltip("Rock throw force when the player holds the shoot button for the maximum time")]
    [SerializeField] private float _rockThrowForce;

    //The half height of the player's collider
    private float _halfHeight;
    private float _hitboxRadius;

    private float _yAim=0;
    private float _xAim=0;

    private Vector2 _moveInput;
    private Vector2 _aimInput;

    private Rigidbody _rb;

    public int RocksStock
    {
        get => _rocksStock; set
        {
            _rocksStock = value;
            _onRocksStockChanged?.Invoke(value);
        }
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    MyInputs inputs;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _halfHeight = GetComponent<Collider>().bounds.extents.y;
        _hitboxRadius = GetComponent<Collider>().bounds.extents.x;
        _onRocksStockChanged?.Invoke(_rocksStock);

        inputs = new MyInputs();
        inputs.Player.Enable();
        inputs.Player.Move.performed += OnMove;
        inputs.Player.Jump.performed += ctx => OnJump();
        inputs.Player.Aim.performed += OnAim;
    }

    private void OnAim(InputAction.CallbackContext value)
    {
        _aimInput = value.ReadValue<Vector2>();
    }

    private void OnMove(InputAction.CallbackContext value)
    {
        _moveInput = value.ReadValue<Vector2>();
    }

    private void OnJump()
    {
        if (!IsGrounded()) return;
        _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
    }

    private void OnShoot(InputValue inputValue)
    {
        if(inputValue.isPressed)
        {
            OnShootHoldStart();
            return;
        }
        OnShootHoldEnd();
    }

    private DateTime _startHoldTime;
    private void OnShootHoldStart()
    {       
        if (RocksStock <= 0) return;
        _startHoldTime = DateTime.Now;
    }

    private void OnShootHoldEnd()
    {
        if (RocksStock <= 0) return;
        float holdTime = Mathf.Clamp((float)(DateTime.Now-_startHoldTime).TotalSeconds, 0, _maxHoldTime);
        float holdPercentage=holdTime / _maxHoldTime;
        GameObject rock = Instantiate(_rockPrefab, transform.position + _cam.transform.forward, Quaternion.identity);
        rock.GetComponent<Rigidbody>().AddForce(_cam.transform.forward * _rockThrowForce*holdPercentage, ForceMode.Impulse);
        RocksStock--;
    }

    private void OnPick()
    {
        _rockPicker.Pick();
    }

    private void FixedUpdate()
    {
        //We remove the y component of the camera's forward vector to make sure the player doesn't move up or down
        Vector3 lookDirection = _cam.transform.forward;
        lookDirection.y = 0;
        lookDirection = lookDirection.normalized;

        Vector3 rightLookDirection = Quaternion.Euler(0, 90, 0) * lookDirection;

        Vector3 move = rightLookDirection * _moveInput.x + lookDirection * _moveInput.y;
        _rb.AddForce(move * _speed, ForceMode.VelocityChange);
    }

    private void Update()
    {      
        _yAim+=-_aimInput.y * _aimSpeed * Time.deltaTime;
        _yAim = Mathf.Clamp(_yAim, _minYAim, _maxYAim);

        _xAim += _aimInput.x * _aimSpeed * Time.deltaTime;
        if (_clampX)
        {
            _xAim = Mathf.Clamp(_xAim, _minXAim, _maxXAim);
        }
        
        Vector3 currentRotation = new Vector3(_yAim, _xAim, 0);

        _cam.transform.rotation = Quaternion.Euler(currentRotation);
    }

    private bool IsGrounded()
    {
        RaycastHit raycastHit;
        return Physics.SphereCast(transform.position, _hitboxRadius, transform.TransformDirection(Vector3.down), out raycastHit, _halfHeight+0.1f, _groundLayer);
    }
}
