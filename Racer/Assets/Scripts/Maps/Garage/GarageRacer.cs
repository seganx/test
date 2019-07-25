﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeganX.Effects;

public class GarageRacer : Base
{
    [SerializeField] private float waterSpeed = 0.0001f;
    [SerializeField] private Transform water = null;


    private void Update()
    {
        var t = System.DateTime.Now;
        water.localEulerAngles = Vector3.up * (t.Minute * 60 + t.Second) * waterSpeed;


        if (racer == null) return;
        if (racer.transform.position.y > 0)
            racer.transform.position = Vector3.MoveTowards(racer.transform.position, Vector3.back * 5, Time.deltaTime * 30);
        else if (Mathf.Abs(racer.transform.position.z) > 0)
            racer.transform.position = Vector3.MoveTowards(racer.transform.position, Vector3.zero, Time.deltaTime * 5);
    }


    ////////////////////////////////////////////////////////
    /// STATIC MEMBER
    ////////////////////////////////////////////////////////
    public static RacerPresenter racer = null;

    public static void LoadRacer(int id)
    {
        if (racer != null && racer.Id == id) return;
        if (racer != null) Destroy(racer.gameObject);

        var config = RacerFactory.Racer.AllConfigs.Find(x => x.Id == id);
        if (config == null) return;

        racer = RacerFactory.Racer.Create(id, GameMap.Current.transform);
        if (racer == null) return;

        var profile = Profile.GetRacer(id);
        racer.SetupCustom(profile == null ? config.DefaultRacerCustom : profile.custom).SetupCameras(false);
        racer.transform.SetLocalPositionAndRotation(Vector3.back * 10 + Vector3.up * 10, Quaternion.identity);
        racer.AutoWheelRotation = true;

        GarageCamera.radiusFactor = racer.Size.magnitude / 4.41f;
    }

    public static void SetRacerWheelsSpeed(float wheelSpeed)
    {
        racer.AutoWheelRotation = false;
        foreach (var wheel in racer.frontWheels)
            wheel.RotationSpeed = wheelSpeed;
        foreach (var wheel in racer.rearWheels)
            wheel.RotationSpeed = wheelSpeed;
    }

    public static void SetRacerWheelsAngle(float wheelAngle)
    {
        racer.AutoSteeringWheel = false;
        foreach (var wheel in racer.frontWheels)
            wheel.transform.DoRotateToward(0, wheelAngle, 0, 10, Space.Self);
    }
}
