﻿using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerManager : Singleton<PlayerManager>
{

    [NonSerialized]
    public bool canMove = false;
    [NonSerialized]
    public bool autoMove = false;
    [Range(0, 50)]
    public float speed = 6f;

    [SerializeField]
    private GameObject player = null;
    private float rotationRate = 360;

    private Animator anim;
    private static Rigidbody rigidBody;

    void Start()
    {
        anim = player.GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();

        Invoke("RandomCry", Random.Range(5, 10));
    }

    private void FixedUpdate()
    {
        if (autoMove) AutoMove();
        if (!GameManager.Instance.enteredGame || !canMove) return;
        Move();
    }

    public Rigidbody GetRigidbody()
    {
        return rigidBody;
    }

    private void Move()
    {
        float hAxis = Input.GetAxis("Horizontal");
        float vAxis = Input.GetAxis("Vertical");

        anim.SetFloat("horizontal", hAxis);
        anim.SetFloat("vertical", vAxis);

        Vector3 movement = new Vector3(0, 0, vAxis) * speed * Time.fixedDeltaTime;
        Vector3 newPosition = rigidBody.position + rigidBody.transform.TransformDirection(movement);
        SetPosition(newPosition);

        Vector3 eulerAngleVelocity = new Vector3(0, hAxis * rotationRate * Time.deltaTime, 0);
        Quaternion newRotation = Quaternion.Euler(eulerAngleVelocity);
        var rotation = rigidBody.rotation * newRotation;
        rigidBody.MoveRotation(rotation);
    }

    private void AutoMove()
    {
        anim.SetFloat("vertical", 1);

        Vector3 movement = Vector3.forward * speed * Time.fixedDeltaTime;
        Vector3 newPosition = rigidBody.position + rigidBody.transform.TransformDirection(movement);
        rigidBody.MovePosition(newPosition);
    }

    public void RotateIntro()
    {
        transform.DORotate(new Vector3(0, 180, 0), 2f)
            .OnPlay(() =>
            {
                autoMove = true;
                CameraManager.Instance.StartTimeline("mainSceneIntroToDefault");
            });
    }

    public void RotateMoutainCorridor()
    {
        transform.DORotate(new Vector3(0, 203, 0), 1f);
    }

    public void ResetPosition()
    {
        SetPosition(new Vector3(0, 0, 0));
    }

    public Vector3 GetPosition()
    {
        return rigidBody.position;
    }

    public void SetPosition(Vector3 position)
    {
        if (rigidBody != null)
        {
            rigidBody.MovePosition(position);
        }
        else
        {
            GetComponent<Rigidbody>().MovePosition(position);
        }
    }

    public void SetRotation(Vector3 rotation)
    {
        if (rigidBody != null)
        {
            rigidBody.MoveRotation(Quaternion.Euler(rotation));
        }
        else
        {
            GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(rotation));
        }
    }

    private void RandomCry()
    {
        PlayerAnimManager.Instance.StartBeakAnim();

        float randomTime = Random.Range(10, 25);
        Invoke("RandomCry", randomTime);
    }

    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("OnTriggerEnter " + collider.name);
        switch (collider.name)
        {
            case "INTERACTION ZONE CREVASSE":
                collider.transform.parent.GetComponent<InteractionCrevasseManager>().PlayerInInteractionZone();
                break;
            case "INTERACTION ZONE CASCADE":
                collider.transform.parent.GetComponent<InteractionCascadeManager>().PlayerInInteractionZone();
                break;
            case "INTERACTION ZONE MOUNTAIN":
                collider.transform.parent.GetComponent<InteractionMountainManager>().PlayerInInteractionZone();
                break;
            case "INTERACTION ZONE FINAL":
                collider.transform.parent.GetComponent<InteractionFinalManager>().PlayerInInteractionZone();
                break;
            case "AURORE CALL COLLIDER":
                EnvironmentManager.Instance.AuroreCall();
                break;
            case "MOUNTAIN CORRIDOR":
                MountainSceneManager.Instance.InCorridor();
                break;
            case "QUOTE 1 COLLIDER":
                UIManager.Instance.ShowQuote1();
                break;
            case "QUOTE 2 COLLIDER":
                UIManager.Instance.ShowQuote2();
                break;
            case "QUOTE 3 COLLIDER":
                UIManager.Instance.ShowQuote3();
                break;
            default:
                break;
        }

        CameraManager.Instance.StartTimeline(collider.name);
    }

    private void OnTriggerExit(Collider collider)
    {
        Debug.Log("OnTriggerExit " + collider.name);
        switch (collider.name)
        {
            case "INTERACTION ZONE CREVASSE":
                collider.transform.parent.GetComponent<InteractionCrevasseManager>().PlayerOutInteractionZone();
                break;
            case "INTERACTION ZONE CASCADE":
                collider.transform.parent.GetComponent<InteractionCascadeManager>().PlayerOutInteractionZone();
                break;
            case "INTERACTION ZONE MOUNTAIN":
                collider.transform.parent.GetComponent<InteractionMountainManager>().PlayerOutInteractionZone();
                break;
            case "MOUNTAIN CORRIDOR":
                MountainSceneManager.Instance.OutCorridor();
                break;
            case "INTERACTION ZONE FINAL":
                collider.transform.parent.GetComponent<InteractionFinalManager>().PlayerOutInteractionZone();
                break;
            default:
                break;
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        switch (collider.name)
        {
            case "MOUNTAIN CORRIDOR":
                MountainSceneManager.Instance.StayInCorridor();
                break;

        }
    }
}