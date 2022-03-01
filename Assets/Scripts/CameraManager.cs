using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    InputMannager inputManager;

    public Transform cameraPivot;
    public Transform targetTransform;
    public Transform cameraTransform;
    public LayerMask collisionLayers;
    private float defaultPosition;      //������ȡpivot������ĵ�������
    private Vector3 cameraFollowVelocity = Vector3.zero;
    private Vector3 cameraVectorPositon;


    [Header("���������Ŀ��Զ��")]
    public float cameraCollisionOffset = 0.2f;
    [Header("�����ڵ����������")]
    public float minimumCollisionOffset = 0.2f;
    [Header("��ⷶΧ")]
    public float cameraCollisionRadius = 2;
    [Header("ˮƽ��ֱ�ı任�ٶ�")]
    public float cameraFollowSpeed = 0.2f;
    public float cameraLookSpeed = 2;
    public float cameraPivotSpeed = 2;


    [Header("�������ӷ�Χ")]
    public float lookAngle;
    public float pivotAngel;
    public float minimumPivotAngle = -35;
    public float maximumPivotAngle = 35;

    private void Awake()
    {
        inputManager = FindObjectOfType<InputMannager>();
        targetTransform = FindObjectOfType<PlayerManager>().transform;
        cameraTransform = Camera.main.transform;
        defaultPosition = cameraTransform.localPosition.z;
    }

    public void HandleAllCameraMovement()
    {
        FollowTarget();
        RotateCamera();
        HandleCameraCollisions();
    }

    private void FollowTarget()
    {
        Vector3 targetPosition = Vector3.SmoothDamp
            (transform.position, targetTransform.position, ref cameraFollowVelocity, cameraFollowSpeed);

        transform.position = targetPosition;
    }

    private void RotateCamera()
    {
        Vector3 rotation;
        Quaternion targetRotation;


        lookAngle = lookAngle + (inputManager.cameraInputX * cameraLookSpeed);
        pivotAngel = pivotAngel - (inputManager.cameraInputY * cameraPivotSpeed);
        pivotAngel = Mathf.Clamp(pivotAngel, minimumPivotAngle, maximumPivotAngle);

        rotation = Vector3.zero;
        rotation.y = lookAngle;
        targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngel;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;
    }

    private void HandleCameraCollisions()
    {
        float targetPositon = defaultPosition;
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();


        //����ڵ���
        if (Physics.SphereCast
            (cameraPivot.transform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetPositon), collisionLayers))
        {
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPositon = -(distance - cameraCollisionOffset);
        }

        if (Mathf.Abs(targetPositon) < minimumCollisionOffset)
        {
            targetPositon = targetPositon - minimumCollisionOffset;
        }

        cameraVectorPositon.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPositon, 0.2f);
        cameraTransform.localPosition = cameraVectorPositon;
    }


}
