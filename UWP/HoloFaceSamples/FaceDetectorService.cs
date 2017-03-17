// Copyright(c) 2017 Takahiro Miyaura
// Released under the MIT license
// http://opensource.org/licenses/mit-license.php

using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.Capture;
using Windows.Media.FaceAnalysis;
using Windows.Media.MediaProperties;
using UnityPlayer;

namespace HoloFaceSamples
{
    internal class FaceDetectorService : FaceDetectBase
    {
        public delegate Task SetMediaCaptureObjectAsync(MediaCapture capture);

        private readonly MediaCapture _capture;

        public FaceDetectorService(MediaCapture capture)
        {
            _capture = capture;
        }

        /// <summary>
        ///     Initialize Service and add service.
        /// </summary>
        public static async Task InitizlizeServiceAsync(SetMediaCaptureObjectAsync action)
        {
            var devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            var device = devices[0];
            var capture = new MediaCapture();
            var settings = new MediaCaptureInitializationSettings
            {
                VideoDeviceId = device.Id
            };
            await capture.InitializeAsync(settings);

            await action(capture);

            var service = new FaceDetectorService(capture);

            UWPBridgeServiceManager.Instance.AddService<FaceDetectBase>(service);
        }

        /// <summary>
        ///     Perfoem face detect.
        /// </summary>
        public override void DetectFace()
        {
            AppCallbacks.Instance.InvokeOnUIThread(async () =>
            {
                var properties =
                    _capture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview) as
                        VideoEncodingProperties;
                if (properties == null)
                    return;

                //coution! face detect is only supported 'BitmapPixelFormat.Gray8'.
                var videoFrame = new VideoFrame(BitmapPixelFormat.Gray8, (int) properties.Width, (int) properties.Height);

                this.FrameSizeWidth = (int) properties.Width;
                this.FrameSizeHeight = (int) properties.Height;


                var previewFrame = await _capture.GetPreviewFrameAsync(videoFrame);

                var detector = await FaceDetector.CreateAsync();
                var detectFaces = await detector.DetectFacesAsync(previewFrame.SoftwareBitmap);
                var faceInformations = detectFaces.Select(x => new FaceInformation
                {
                    X = x.FaceBox.X,
                    Y = x.FaceBox.Y,
                    Width = x.FaceBox.Width,
                    Height = x.FaceBox.Height
                }).ToList();
                AppCallbacks.Instance.InvokeOnAppThread(() => { OnDetected(faceInformations); }, false);
                videoFrame.Dispose();
                previewFrame.Dispose();
            }, true);
        }
    }
}