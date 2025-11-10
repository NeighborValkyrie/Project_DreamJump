using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float cameraRotationSpeed = 10f;
    [SerializeField] private float jumpForce = 10f;
    
    private Transform _cameraParentTr;
    private Transform _bodyTr;
    private Rigidbody _rb;
    
    private  Vector3 _dir;
    private  Vector3 _cameraRot;
    
    private bool _isLife;
    
    private Vector3 savePos = new Vector3(0, 5, 0); 
    
    // Start is called before the first frame update
    void Start()
    {
        _cameraParentTr = transform.Find("CameraParentTr");
        _bodyTr = transform.Find("Body");
        _rb = GetComponent<Rigidbody>();
        Physics.gravity = new Vector3(0, -20f, 0);
        _isLife = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isLife)
        {
            return;
        }
        
        Move();
        CameraRotation();
        Jump();
    }

    private void LateUpdate()
    {
        if (transform.position.y <= -15 && _isLife)
        {
            GameOver();
        }
    }

    public  void SetSavePos(Vector3 pos)
    {
        savePos = pos;
    }
    
    public void GameOver()
    {
        if (!_isLife)
        {
            return;
        }
        _isLife = false;
        StartCoroutine(Restart());
    }
    
    IEnumerator Restart()
    {
        ParticleSystem part = gameObject.GetComponentInChildren<ParticleSystem>();
        _rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        part.Play();
        _bodyTr.gameObject.SetActive(false);
        yield return new WaitForSeconds(3f);
        part.Stop();
        part.Clear();
        _bodyTr.gameObject.SetActive(true);
        _rb.constraints = RigidbodyConstraints.FreezeRotation;
        transform.position = savePos;
        _isLife = true;
    }

    private void Move()
    {
        _dir = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        
        if (_dir != Vector3.zero)
        {
            _dir = speed * Time.deltaTime * _dir.normalized ;
            _dir = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * _dir;
            _bodyTr.forward = _dir;

            if (transform.parent)
            {
                transform.position += speed * Time.deltaTime * _bodyTr.forward;
            }
            else
            {
                _rb.MovePosition( speed * Time.deltaTime * _bodyTr.forward + _rb.position);
            }
        }
    }

    private void CameraRotation()
    {
        Vector3 angle = _cameraParentTr.eulerAngles;
        _cameraRot = new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0f);
        angle += cameraRotationSpeed * Time.deltaTime * _cameraRot;
        if (angle.x > 180)
        {
            angle.x -= 360;
        }
        angle.x = Mathf.Clamp(angle.x, -10f, 60f);
        _cameraParentTr.eulerAngles = angle;
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RaycastHit[] hits = Physics.RaycastAll(transform.position + Vector3.up, Vector3.down,1.3f);
            foreach (RaycastHit hit in hits)
            {
                if (!hit.collider.gameObject.CompareTag("Player"))
                {
                    _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                    break;
                }
            }
        }
    }

}
