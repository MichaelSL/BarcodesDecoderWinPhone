﻿using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Windows.Foundation;
using Windows.Phone.Media.Capture;
using ZXing;
using Microsoft.Devices;
using BarcodesDecoder.Views;
using System.Windows.Navigation;

namespace BarcodesDecoder.ViewModels
{
    public class CaptureViewModel : INotifyPropertyChanged
    {
        private VideoBrush _videoBrush;
        private CompositeTransform _compositeTransform;
        private PhotoCaptureDevice _photoCaptureDevice;
        private Size _captureResolution;
        private Size _previewResolution;
        private ObservableCollection<Result> _results;
        private Result _lastResult;
        private Task<Result> _runningScan;
        private byte[] _previewBuffer;
        private byte[] _rotatedPreviewBuffer;
        private BarcodeReader _barcodeReader;
        private PageOrientation _orientation;
        private bool stop;
        private CameraSensorLocation sensorLocation;

        public CaptureViewModel()
        {
            _orientation = PageOrientation.Portrait;
        }

        #region Methods
        public async void InitializeAndGo()
        {
            sensorLocation = CameraSensorLocation.Front;
            if (PhotoCaptureDevice.AvailableSensorLocations.Contains(CameraSensorLocation.Back))
                sensorLocation = CameraSensorLocation.Back;

            PreviewResolution = GetBestPreviewResolution(sensorLocation);
            CaptureResolution = await GetBestCaptureResolution(sensorLocation, PreviewResolution);
            try
            {
                await InitializePhotoCaptureDevice(sensorLocation, CaptureResolution, PreviewResolution);
            }
            catch (Exception captureDeviceInitException)
            {
                //log
                throw;
            }

            try
            {
                await StartCapturingAsync();
            }
            catch (Exception startCapturingException)
            {
                //log
                throw;
            }

            _previewBuffer = new byte[(int)PreviewResolution.Width * (int)PreviewResolution.Height];
            _rotatedPreviewBuffer = new byte[(int)PreviewResolution.Width * (int)PreviewResolution.Height];
            _barcodeReader = new BarcodeReader();

            PhotoCaptureDevice.PreviewFrameAvailable += PreviewFrame;

            stop = false;
            var focusingTask = new Task(Focusing);
            focusingTask.Start();
        }

        public void Stop()
        {
            stop = true;
        }

        private async void Focusing()
        {
            while (!stop)
            {
                if (PhotoCaptureDevice.IsFocusSupported(sensorLocation))
                    try
                    {
                        await PhotoCaptureDevice.FocusAsync();
                    }
                    catch (Exception ex)
                    {
                        //log it
                    }
                else
                    System.Threading.Thread.Sleep(200);
            }

            PhotoCaptureDevice.PreviewFrameAvailable -= PreviewFrame;
            PhotoCaptureDevice.Dispose();
            PhotoCaptureDevice = null;
        }

        private void PreviewFrame(ICameraCaptureDevice device, object obj)
        {
            if (_runningScan != null)
                return;
            _runningScan = new Task<Result>(AnalysePreview);
            _runningScan.Start();
        }

        private Result AnalysePreview()
        {
            var resultTask = DetectBarcodeAsync();
            resultTask.Wait();
            var result = resultTask.Result;
            if (result != null)
            {
                if (_lastResult == null ||
                    _lastResult.Text != result.Text)
                {
                    _lastResult = result;
                    this.OnBarcodeScanned(result);
                }

            }

            _runningScan = null;

            return result;
        }

        private async Task<Result> DetectBarcodeAsync()
        {
            var width = (int)PreviewResolution.Width;
            var height = (int)PreviewResolution.Height;

            var rotation = PhotoCaptureDevice.SensorRotationInDegrees;
            LuminanceSource luminanceSource = null;

            PhotoCaptureDevice.GetPreviewBufferY(_previewBuffer);

            luminanceSource = new RGBLuminanceSource(_previewBuffer, width, height, RGBLuminanceSource.BitmapFormat.Gray8);
            var result = _barcodeReader.Decode(luminanceSource);
            if (result == null)
            {
                // ok, one try with rotation by 90 degrees
                if ((Orientation & PageOrientation.Portrait) == PageOrientation.Portrait)
                {
                    // if we are in potrait orientation it's better to rotate clockwise
                    // to get it in the right direction
                    luminanceSource = new RGBLuminanceSource(RotateClockwise(_previewBuffer, width, height), height, width, RGBLuminanceSource.BitmapFormat.Gray8);
                }
                else
                {
                    // in landscape we try counter clockwise until we know it better
                    luminanceSource = luminanceSource.rotateCounterClockwise();
                }
                result = _barcodeReader.Decode(luminanceSource);
            }
            return result;
        }

        private byte[] RotateClockwise(byte[] buffer, int width, int height)
        {
            var newWidth = height;
            var newHeight = width;
            for (var yold = 0; yold < height; yold++)
            {
                for (var xold = 0; xold < width; xold++)
                {
                    var xnew = newWidth - yold - 1;
                    var ynew = xold;
                    _rotatedPreviewBuffer[ynew * newWidth + xnew] = buffer[yold * width + xold];
                }
            }

            return _rotatedPreviewBuffer;
        }

        private async Task StartCapturingAsync()
        {
            CameraCaptureSequence sequence = PhotoCaptureDevice.CreateCaptureSequence(1);
            var memoryStream = new MemoryStream();
            sequence.Frames[0].CaptureStream = memoryStream.AsOutputStream();

            try
            {
                PhotoCaptureDevice.SetProperty(KnownCameraPhotoProperties.FlashMode, FlashState.Off);
                PhotoCaptureDevice.SetProperty(KnownCameraPhotoProperties.SceneMode, CameraSceneMode.Macro);
            }
            catch
            {
                // one or more properties not supported
            }

            await PhotoCaptureDevice.PrepareCaptureSequenceAsync(sequence);
        }

        private async Task<Size> GetBestCaptureResolution(CameraSensorLocation sensorLocation, Size previewResolution)
        {
            // The last size in the AvailableCaptureResolutions is the lowest available
            var captureResolutions = PhotoCaptureDevice.GetAvailableCaptureResolutions(sensorLocation);
            var previewResolutions = PhotoCaptureDevice.GetAvailablePreviewResolutions(sensorLocation);

            Size resolution = await Task.Factory.StartNew(() => captureResolutions.LastOrDefault(
                c => (c.Width > 1000.0 || c.Height > 1000.0) && (c.Width / c.Height).Equals(previewResolution.Width / previewResolution.Height)));
            if (resolution == default(Size))
                return previewResolution;
            return resolution;
        }

        private Size GetBestPreviewResolution(CameraSensorLocation sensorLocation)
        {
            var previewResolutions = PhotoCaptureDevice.GetAvailablePreviewResolutions(sensorLocation);
            var result = new Size(640, 480);

            foreach (var previewResolution in previewResolutions)
            {
                if (previewResolution.Width * previewResolution.Height > result.Width * result.Height)
                    result = previewResolution;
            }

            return result;
        }

        private async Task InitializePhotoCaptureDevice(CameraSensorLocation sensorLocation, Size size, Size previewSize)
        {
            PhotoCaptureDevice = await PhotoCaptureDevice.OpenAsync(sensorLocation, size);
            await PhotoCaptureDevice.SetPreviewResolutionAsync(previewSize);

            CompositeTransform = new CompositeTransform();
            CompositeTransform.CenterX = .5;
            CompositeTransform.CenterY = .5;
            CompositeTransform.Rotation = PhotoCaptureDevice.SensorRotationInDegrees
                - (Orientation == PageOrientation.LandscapeLeft ? 90 : 0)
                + (Orientation == PageOrientation.LandscapeRight ? 90 : 0);
            if (sensorLocation == CameraSensorLocation.Front)
            {
                CompositeTransform.ScaleX = -1;
            }

            VideoBrush = new VideoBrush();
            VideoBrush.RelativeTransform = CompositeTransform;
            VideoBrush.Stretch = Stretch.Fill;
            VideoBrush.SetSource(PhotoCaptureDevice);
        }
        #endregion

        #region Properties
        public VideoBrush VideoBrush
        {
            get { return _videoBrush; }
            set
            {
                if (Equals(value, _videoBrush)) return;
                _videoBrush = value;
                OnPropertyChanged();
            }
        }

        public PhotoCaptureDevice PhotoCaptureDevice
        {
            get { return _photoCaptureDevice; }
            set
            {
                if (Equals(value, _photoCaptureDevice)) return;
                _photoCaptureDevice = value;
                OnPropertyChanged();
            }
        }

        public CompositeTransform CompositeTransform
        {
            get { return _compositeTransform; }
            set
            {
                if (Equals(value, _compositeTransform)) return;
                _compositeTransform = value;
                OnPropertyChanged();
            }
        }

        public Size CaptureResolution
        {
            get { return _captureResolution; }
            set
            {
                if (value.Equals(_captureResolution)) return;
                _captureResolution = value;
                OnPropertyChanged();
            }
        }

        public Size PreviewResolution
        {
            get { return _previewResolution; }
            set
            {
                if (value.Equals(_previewResolution)) return;
                _previewResolution = value;
                OnPropertyChanged();
            }
        }

        public PageOrientation Orientation
        {
            get
            {
                return _orientation;
            }
            set
            {
                _orientation = value;
                if (CompositeTransform != null &&
                    PhotoCaptureDevice != null)
                {
                    CompositeTransform.Rotation = PhotoCaptureDevice.SensorRotationInDegrees
                        - (Orientation == PageOrientation.LandscapeLeft ? 90 : 0)
                        + (Orientation == PageOrientation.LandscapeRight ? 90 : 0);

                    if (sensorLocation == CameraSensorLocation.Front)
                    {
                        CompositeTransform.ScaleX = -1;
                    }
                }
            }
        }

        #endregion

        public class ResultEventArgs : EventArgs
        {
            public Result Result { get; set; }
        }

        public event EventHandler<ResultEventArgs> BarcodeScanned;

        private void OnBarcodeScanned(Result barcode)
        {
            var handler = this.BarcodeScanned;
            if (handler != null)
                handler(this, new ResultEventArgs { Result = barcode });
        }

        #region INPC
        // INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;

        //[NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
