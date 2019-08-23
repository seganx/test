using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacerPresenter : MonoBehaviour
{
    [System.Serializable]
    public class BluePrint
    {
        public Transform hood = null;
        public Transform roof = null;
        public Transform spoiler = null;
        public Transform frontWheel = null;
        public Transform rearWheel = null;
    }

    public Transform bodyTransform = null;
    public Transform cameraTargetTransform = null;
    public BluePrint bluePrint = new BluePrint();

    [HideInInspector] public BoxCollider boxCollider = null;
    [HideInInspector] public RacerCustomPresenter hood = null;
    [HideInInspector] public RacerCustomPresenter horn = null;
    [HideInInspector] public RacerCustomPresenter roof = null;
    [HideInInspector] public RacerCustomPresenter spoiler = null;
    [HideInInspector] public WheelPresenter[] frontWheels = new WheelPresenter[] { null, null };
    [HideInInspector] public WheelPresenter[] rearWheels = new WheelPresenter[] { null, null };

    private Renderer bodyRenderer = null;
    private RacerMaterial.BaseParam materialParam = null;
    private RacerCustomData data = new RacerCustomData();

    public int Id { get; private set; }
    public int GroupId { get; private set; }
    public bool IsShifting { get; set; }
    public bool AutoSteeringWheel { get; set; }
    public bool AutoWheelRotation { get; set; }
    public Vector3 Size { get; private set; }

    public int Height
    {
        get { return data.Height; }
        set { data.Height = value; }
    }

    public int Hood
    {
        get { return data.Hood; }
        set
        {
            if (data.Hood == value) return;
            data.Hood = value;
            if (hood != null) Destroy(hood.gameObject);
            hood = RacerFactory.Hood.Create(Id, value, bluePrint.hood);
        }
    }

    public int Horn
    {
        get { return data.Horn; }
        set
        {
            if (data.Horn == value) return;
            data.Horn = value;
            if (horn != null) Destroy(horn.gameObject);
            horn = RacerFactory.Hood.Create(Id, value, transform);
        }
    }

    public int Roof
    {
        get { return data.Roof; }
        set
        {
            if (data.Roof == value) return;
            data.Roof = value;
            if (roof != null) Destroy(roof.gameObject);
            roof = RacerFactory.Roof.Create(Id, value, bluePrint.roof);
        }
    }


    public int Spoiler
    {
        get { return data.Spoiler; }
        set
        {
            if (data.Spoiler == value) return;
            data.Spoiler = value;
            if (spoiler != null) Destroy(spoiler.gameObject);
            spoiler = RacerFactory.Spoiler.Create(Id, value, bluePrint.spoiler);
        }
    }

    public int Vinyl
    {
        get { return data.Vinyl; }
        set
        {
            if (data.Vinyl == value) return;
            data.Vinyl = value;
            RacerFactory.Vinyl.SetVinylTexture(Id, value, bodyRenderer.material);
        }
    }

    public int Wheel
    {
        get { return data.Wheel; }
        set
        {
            if (data.Wheel == value) return;
            data.Wheel = value;
            if (frontWheels[0] != null) Destroy(frontWheels[0].gameObject);
            if (frontWheels[1] != null) Destroy(frontWheels[1].gameObject);
            if (rearWheels[0] != null) Destroy(rearWheels[0].gameObject);
            if (rearWheels[1] != null) Destroy(rearWheels[1].gameObject);

            frontWheels[0] = RacerFactory.Wheel.Create(Id, value, transform);
            frontWheels[1] = frontWheels[0].Clone<WheelPresenter>();
            rearWheels[0] = frontWheels[0].Clone<WheelPresenter>();
            rearWheels[1] = frontWheels[0].Clone<WheelPresenter>();

            frontWheels[0].transform.CopyValuesFrom(bluePrint.frontWheel);
            frontWheels[1].transform.CopyValuesFrom(bluePrint.frontWheel);
            rearWheels[0].transform.CopyValuesFrom(bluePrint.rearWheel);
            rearWheels[1].transform.CopyValuesFrom(bluePrint.rearWheel);

            frontWheels[1].transform.ScaleLocalPosition(-1, 1, 1).Scale(-1, 1, 1);
            rearWheels[1].transform.ScaleLocalPosition(-1, 1, 1).Scale(-1, 1, 1);
        }
    }


    public int ColorModel
    {
        get { return data.ColorModel; }
        set
        {
            data.ColorModel = value;
            RacerFactory.Colors.SetModel(value, bodyRenderer.material, materialParam);
            if (hood != null) hood.SetMaterialModel(materialParam, value);
            if (roof != null) roof.SetMaterialModel(materialParam, value);
            if (spoiler != null) spoiler.SetMaterialModel(materialParam, value);

        }
    }

    public int BodyColor
    {
        get { return data.BodyColor; }
        set
        {
            data.BodyColor = value;
            RacerFactory.Colors.SetDiffuseColor(value, bodyRenderer.material, 1, false);
        }
    }

    public int WindowColor
    {
        get { return data.WindowColor; }
        set
        {
            data.WindowColor = value;
            RacerFactory.Colors.SetDiffuseColor(value, bodyRenderer.materials[1], 1, false);
        }
    }

    public int LightsColor
    {
        get { return data.LightsColor; }
        set
        {
            data.LightsColor = value;
            RacerFactory.Colors.SetDiffuseColor(value, bodyRenderer.materials[1], 2, false);
        }
    }

    public int HoodColor
    {
        get { return data.HoodColor; }
        set
        {
            data.HoodColor = value;
            if (hood != null) hood.SetMaterialModel(materialParam, ColorModel).SetColor(value);
        }
    }

    public int RoofColor
    {
        get { return data.RoofColor; }
        set
        {
            data.RoofColor = value;
            if (roof != null) roof.SetMaterialModel(materialParam, ColorModel).SetColor(value);
        }
    }

    public int SpoilerColor
    {
        get { return data.SpoilerColor; }
        set
        {
            data.SpoilerColor = value;
            if (spoiler != null) spoiler.SetMaterialModel(materialParam, ColorModel).SetColor(value);
        }
    }

    public int VinylColor
    {
        get { return data.VinylColor; }
        set
        {
            data.VinylColor = value;
            RacerFactory.Vinyl.SetVinylColor(value, bodyRenderer.material);
        }
    }

    public int RimColor
    {
        get { return data.RimColor; }
        set
        {
            data.RimColor = value;
            if (frontWheels[0] != null)
            {
                frontWheels[0].SetColor(value, 3);
                frontWheels[1].SetColor(value, 3);
                rearWheels[0].SetColor(value, 3);
                rearWheels[1].SetColor(value, 3);
            }
        }
    }


    private void Awake()
    {
        bodyRenderer = bodyTransform.GetComponent<Renderer>(true, true);
        materialParam = RacerMaterial.CreateBaseParam(bodyRenderer.material, 1);

        //  size of the racer
        {
            var boxes = transform.GetComponentsInChildren<BoxCollider>(true);
            boxCollider = boxes.FindMax<BoxCollider>(x => x.bounds.size.magnitude);
            var racersize = boxCollider != null ? boxCollider.bounds.size : Vector3.zero;
            racersize.y = bluePrint.roof.localPosition.y;
            Size = racersize;
        }

        //  force accept any changes
        for (int i = 0; i < data.di.Length; i++)
            data.di[i] = -1;
    }

    public RacerPresenter SetId(int id, int groupId)
    {
        Id = id;
        GroupId = groupId;
        return this;
    }

    public RacerPresenter SetupCustom(RacerCustomData custom)
    {
        Height = custom.Height;
        Hood = custom.Hood;
        Horn = custom.Horn;
        Roof = custom.Roof;
        Spoiler = custom.Spoiler;
        Vinyl = custom.Vinyl;
        Wheel = custom.Wheel;
        BodyColor = custom.BodyColor;
        WindowColor = custom.WindowColor;
        LightsColor = custom.LightsColor;
        HoodColor = custom.HoodColor;
        RoofColor = custom.RoofColor;
        SpoilerColor = custom.SpoilerColor;
        VinylColor = custom.VinylColor;
        RimColor = custom.RimColor;
        ColorModel = custom.ColorModel;
        return this;
    }

    public RacerPresenter SetupCameras(bool enableCameras)
    {
        if (enableCameras)
        {
            var cams = transform.GetComponents<RacerCamera>(true, true);
            foreach (var cam in cams)
            {
                if (cam.mode == RacerCamera.Mode.Front)
                {
                    var leftcam = cam.Clone<RacerCamera>(cam.transform);
                    leftcam.mode = RacerCamera.Mode.SideLeft;
                    leftcam.transform.position = new Vector3(-bluePrint.rearWheel.position.x - 0.75f, cam.transform.position.x + 0.5f, bluePrint.rearWheel.position.z + 0.5f);
                    var rightcam = leftcam.Clone<RacerCamera>();
                    rightcam.mode = RacerCamera.Mode.SideRight;
                    rightcam.transform.position = new Vector3(bluePrint.rearWheel.position.x + 0.75f, cam.transform.position.x + 0.5f, bluePrint.rearWheel.position.z + 0.5f);
                    break;
                }
            }
        }
        else
        {
            var cams = transform.GetComponents<RacerCamera>(true, true);
            foreach (var cam in cams)
                Destroy(cam.gameObject);
        }
        return this;
    }

    public RacerPresenter SetTransparent(bool value)
    {
        var rendrs = transform.GetComponents<Renderer>(true, true);
        foreach (var rndr in rendrs)
            foreach (var mat in rndr.materials)
                RacerMaterial.SetTransparent(mat, value);
        return this;
    }
}
