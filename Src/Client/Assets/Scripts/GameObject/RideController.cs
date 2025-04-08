using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;
using Managers;
using System;

public class RideController : MonoBehaviour
{
    public Transform mountPoint;
    public EntityController rider;
    public Vector3 offset;
    private Animator anim;

    void Start()
    {
        this.anim = this.GetComponent<Animator>();

    }

    void Update()
    {
        if (this.mountPoint == null || this.rider == null)
        {
            return;
        }
        //基于方向来做转向
        this.rider.SetRidePosition(this.mountPoint.position + this.mountPoint.TransformDirection(this.offset));
    }

    public void SetRide(EntityController entityController)
    {
        this.rider = entityController;
    }


    public void OnEntityEvent(EntityEvent entityEvent, int param)
    {
        switch (entityEvent)
        {
            case EntityEvent.Idle:
                anim.SetBool("Move", false);
                anim.SetTrigger("Idle");
                break;
            case EntityEvent.MoveFwd:
                anim.SetBool("Move", true);
                break;
            case EntityEvent.MoveBack:
                anim.SetBool("Move", true);
                break;
            case EntityEvent.Jump:
                anim.SetTrigger("Jump");
                break;
        }
    }

}
