// Copyright(c) 2017 Takahiro Miyaura
// Released under the MIT license
// http://opensource.org/licenses/mit-license.php

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraScript : MonoBehaviour
{
    /// <summary>
    ///     detect interval.
    /// </summary>
    private static int FRAME_INTERVAL = 20;

    /// <summary>
    ///     face Detect object list.
    /// </summary>
    private readonly List<Image> _faceObjects = new List<Image>();
    
    /// <summary>
    ///     Canvas Object
    /// </summary>
    public GameObject Canvas;

    /// <summary>
    ///     Tempalte Image Object.
    /// </summary>
    public Image FaceObject;

    /// <summary>
    ///     Text object for Inidicate Detected Data
    /// </summary>
    public Text TextData;
    

    /// <summary>
    ///     FaceDetect object.
    /// </summary>
    private FaceDetectBase Service { get; set; }


    // Update is called once per frame
    private void Start()
    {
    }

    // Use this for initialization
    private void Update()
    {

        if (Time.frameCount % FRAME_INTERVAL == 0)
        {
            if (Service == null)
            {
#if UNITY_EDITOR
                // For Debug.when this application execute by unity,call this. 
                Service = new FaceDetectStub();
#else
// execute For HoloLens. 
                Service = UWPBridgeServiceManager.Instance.GetService<FaceDetectBase>();
                TextData.text = "Service Initialized.";
#endif

                Service.OnDetected = SetFaceObject;
            }

            Service.DetectFace();
        }
    }

    /// <summary>
    ///     when detected faces in screenshot,Sets Face's marker on canvas..
    /// </summary>
    /// <param name="list"></param>
    public void SetFaceObject(List<FaceInformation> list)
    {
        var dif = _faceObjects.Count - list.Count;
        if (dif > 0)
            for (var i = 0; i < dif; i++)
            {
                Destroy(_faceObjects[0]);
                _faceObjects[0] = null;
                _faceObjects.RemoveAt(0);
            }
        else if (dif < 0)
            for (var i = 0; i < -1 * dif; i++)
            {
                var instantiate = Instantiate(FaceObject);
                _faceObjects.Add(instantiate);
            }
        TextData.text = "";
        for (var i = 0; i < _faceObjects.Count; i++)
        {
            var faceObject = _faceObjects[i];

            var faceDetectedImageRectTransform = faceObject.GetComponent(typeof(RectTransform)) as RectTransform;

            var canvasRectTransform = Canvas.GetComponent(typeof(RectTransform)) as RectTransform;
            if (canvasRectTransform == null)
                return;
            var w = canvasRectTransform.sizeDelta.x / Service.FrameSizeWidth;
            var h = canvasRectTransform.sizeDelta.y / Service.FrameSizeHeight;

            if (faceDetectedImageRectTransform == null)
                return;
            faceDetectedImageRectTransform.transform.parent = Canvas.transform;
            faceDetectedImageRectTransform.sizeDelta = new Vector2(list[i].Width, list[i].Height);

            //Sets face's maeker position.
            faceDetectedImageRectTransform.position =
                Canvas.transform.TransformPoint(
                    list[i].X*w  - canvasRectTransform.sizeDelta.x/2,
                    -list[i].Y*h  + canvasRectTransform.sizeDelta.y/2,
                    0f);
            faceObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
            faceObject.transform.localScale = new Vector3(1f, 1f, 1f);
            TextData.text += string.Format("X=[{0}],Y=[{1}],Width=[{2}],Height=[{3}]\n", list[i].X, list[i].Y,
                list[i].Width, list[i].Height);
        }
    }
}

/// <summary>
///     when this application is Debug on Unity,Sets StubData.
/// </summary>
internal class FaceDetectStub : FaceDetectBase
{

    public FaceDetectStub()
    {
        FrameSizeWidth = 1920;
        FrameSizeHeight = 1200;
    }

    public override void DetectFace()
    {
        var faceInformations = new List<FaceInformation>();
        faceInformations.Add(new FaceInformation
        {
            X = 827,
            Y = 510,
            Width = 37, //(float) (300/1920*0.44),
            Height = 37 //(float) (500/1200*0.24)
        });
        faceInformations.Add(new FaceInformation
        {
            X = 746,
            Y = 660,
            Width = 59, //(float) (300/1920*0.44),
            Height = 37 //(float) (500/1200*0.24)
        });
        OnDetected(faceInformations);
    }
}

/// <summary>
///     Represents a class that face detect processing.
/// </summary>
public abstract class FaceDetectBase : IUWPBridgeService
{
    public delegate void SetFaceObject(List<FaceInformation> list);

    /// <summary>
    ///  Gets or Sets width of screenshot  size.
    /// </summary>
    public int FrameSizeWidth;

    /// <summary>
    ///  Gets or Sets height of screenshot  size.
    /// </summary>
    public int FrameSizeHeight;


    /// <summary>
    /// Gets or sets the action to be performed after face detect processing.
    /// </summary>
    public SetFaceObject OnDetected;

    /// <summary>
    /// Perform face detect. 
    /// </summary>
    public abstract void DetectFace();
}

/// <summary>
///     Represents a class that face detected data.
/// </summary>
public class FaceInformation
{
    /// <summary>
    ///     Set and Get face detect Height.
    /// </summary>
    public float Height;

    /// <summary>
    ///     Set and Get face detect Width.
    /// </summary>
    public float Width;

    /// <summary>
    ///     Set and Get face detect posiotion X.
    /// </summary>
    public float X;

    /// <summary>
    ///     Set and Get face detect posiotion Y.
    /// </summary>
    public float Y;
}