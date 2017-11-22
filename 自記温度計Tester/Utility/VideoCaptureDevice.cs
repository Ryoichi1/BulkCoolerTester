// AForge Direct Show Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2009-2013
// contacts@aforgenet.com
//

namespace AForge.Video.DirectShow
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;
    using System.Threading;
    using AForge.Video;
    using AForge.Video.DirectShow.Internals;

    /// <summary>
    /// Video source for local video capture device (for example USB webcam).
    /// </summary>
    ///
    /// <remarks><para>This video source class captures video data from local video capture device,
    /// like USB web camera (or internal), frame grabber, capture board - anything which
    /// supports <b>DirectShow</b> interface. For devices which has a shutter button or
    /// support external software triggering, the class also allows to do snapshots. Both
    /// video size and snapshot size can be configured.</para>
    ///
    /// <para>Sample usage:</para>
    /// <code>
    /// // enumerate video devices
    /// videoDevices = new FilterInfoCollection( FilterCategory.VideoInputDevice );
    /// // create video source
    /// VideoCaptureDevice videoSource = new VideoCaptureDevice( videoDevices[0].MonikerString );
    /// // set NewFrame event handler
    /// videoSource.NewFrame += new NewFrameEventHandler( video_NewFrame );
    /// // start the video source
    /// videoSource.Start( );
    /// // ...
    /// // signal to stop when you no longer need capturing
    /// videoSource.SignalToStop( );
    /// // ...
    ///
    /// private void video_NewFrame( object sender, NewFrameEventArgs eventArgs )
    /// {
    ///     // get new frame
    ///     Bitmap bitmap = eventArgs.Frame;
    ///     // process the frame
    /// }
    /// </code>
    /// </remarks>
    ///
    public class VideoCaptureDevice2 //: IVideoSource
    {
        // moniker string of video capture device
        private string deviceMoniker;

        // received frames count
        private int framesReceived;

        // recieved byte count
        private long bytesReceived;

        // video and snapshot resolutions to set
        private VideoCapabilities videoResolution = null;

        private VideoCapabilities snapshotResolution = null;

        // provide snapshots or not
        private bool provideSnapshots = false;

        private Thread thread = null;
        private ManualResetEvent stopEvent = null;

        private VideoCapabilities[] videoCapabilities;
        private VideoCapabilities[] snapshotCapabilities;

        private bool needToSetVideoInput = false;
        private bool needToSimulateTrigger = false;
        private bool needToDisplayPropertyPage = false;
        private bool needToDisplayCrossBarPropertyPage = false;
        private IntPtr parentWindowForPropertyPage = IntPtr.Zero;

        // video capture source object
        private object sourceObject = null;

        // time of starting the DirectX graph
        private DateTime startTime = new DateTime();

        // dummy object to lock for synchronization
        private object sync = new object();

        // flag specifying if IAMCrossbar interface is supported by the running graph/source object
        private bool? isCrossbarAvailable = null;

        private VideoInput[] crossbarVideoInputs = null;
        private VideoInput crossbarVideoInput = VideoInput.Default;

        // cache for video/snapshot capabilities and video inputs
        private static Dictionary<string, VideoCapabilities[]> cacheVideoCapabilities = new Dictionary<string, VideoCapabilities[]>();

        private static Dictionary<string, VideoCapabilities[]> cacheSnapshotCapabilities = new Dictionary<string, VideoCapabilities[]>();
        private static Dictionary<string, VideoInput[]> cacheCrossbarVideoInputs = new Dictionary<string, VideoInput[]>();

        ///// <summary>
        ///// Current video input of capture card.
        ///// </summary>
        /////
        ///// <remarks><para>The property specifies video input to use for video devices like capture cards
        ///// (those which provide crossbar configuration). List of available video inputs can be obtained
        ///// from <see cref="AvailableCrossbarVideoInputs"/> property.</para>
        /////
        ///// <para>To check if the video device supports crossbar configuration, the <see cref="CheckIfCrossbarAvailable"/>
        ///// method can be used.</para>
        /////
        ///// <para><note>This property can be set as before running video device, as while running it.</note></para>
        /////
        ///// <para>By default this property is set to <see cref="VideoInput.Default"/>, which means video input
        ///// will not be set when running video device, but currently configured will be used. After video device
        ///// is started this property will be updated anyway to tell current video input.</para>
        ///// </remarks>
        /////
        //public VideoInput CrossbarVideoInput
        //{
        //    get { return crossbarVideoInput; }
        //    set
        //    {
        //        needToSetVideoInput = true;
        //        crossbarVideoInput = value;
        //    }
        //}

        ///// <summary>
        ///// Available inputs of the video capture card.
        ///// </summary>
        /////
        ///// <remarks><para>The property provides list of video inputs for devices like video capture cards.
        ///// Such devices usually provide several video inputs, which can be selected using crossbar.
        ///// If video device represented by the object of this class supports crossbar, then this property
        ///// will list all video inputs. However if it is a regular USB camera, for example, which does not
        ///// provide crossbar configuration, the property will provide zero length array.</para>
        /////
        ///// <para>Video input to be used can be selected using <see cref="CrossbarVideoInput"/>. See also
        ///// <see cref="DisplayCrossbarPropertyPage"/> method, which provides crossbar configuration dialog.</para>
        /////
        ///// <para><note>It is recomended not to call this property immediately after <see cref="Start"/> method, since
        ///// device may not start yet and provide its information. It is better to call the property
        ///// before starting device or a bit after (but not immediately after).</note></para>
        ///// </remarks>
        /////
        //public VideoInput[] AvailableCrossbarVideoInputs
        //{
        //    get
        //    {
        //        if (crossbarVideoInputs == null)
        //        {
        //            lock (cacheCrossbarVideoInputs)
        //            {
        //                if ((!string.IsNullOrEmpty(deviceMoniker)) && (cacheCrossbarVideoInputs.ContainsKey(deviceMoniker)))
        //                {
        //                    crossbarVideoInputs = cacheCrossbarVideoInputs[deviceMoniker];
        //                }
        //            }

        //            if (crossbarVideoInputs == null)
        //            {
        //                if (!IsRunning)
        //                {
        //                    // create graph without playing to collect available inputs
        //                    WorkerThread(false);
        //                }
        //                else
        //                {
        //                    for (int i = 0; (i < 500) && (crossbarVideoInputs == null); i++)
        //                    {
        //                        Thread.Sleep(10);
        //                    }
        //                }
        //            }
        //        }
        //        // don't return null even if capabilities are not provided for some reason
        //        return (crossbarVideoInputs != null) ? crossbarVideoInputs : new VideoInput[0];
        //    }
        //}

        ///// <summary>
        ///// Specifies if snapshots should be provided or not.
        ///// </summary>
        /////
        ///// <remarks><para>Some USB cameras/devices may have a shutter button, which may result into snapshot if it
        ///// is pressed. So the property specifies if the video source will try providing snapshots or not - it will
        ///// check if the camera supports providing still image snapshots. If camera supports snapshots and the property
        ///// is set to <see langword="true"/>, then snapshots will be provided through <see cref="SnapshotFrame"/>
        ///// event.</para>
        /////
        ///// <para>Check supported sizes of snapshots using <see cref="SnapshotCapabilities"/> property and set the
        ///// desired size using <see cref="SnapshotResolution"/> property.</para>
        /////
        ///// <para><note>The property must be set before running the video source to take effect.</note></para>
        /////
        ///// <para>Default value of the property is set to <see langword="false"/>.</para>
        ///// </remarks>
        /////
        //public bool ProvideSnapshots
        //{
        //    get { return provideSnapshots; }
        //    set { provideSnapshots = value; }
        //}

        ///// <summary>
        ///// New frame event.
        ///// </summary>
        /////
        ///// <remarks><para>Notifies clients about new available frame from video source.</para>
        /////
        ///// <para><note>Since video source may have multiple clients, each client is responsible for
        ///// making a copy (cloning) of the passed video frame, because the video source disposes its
        ///// own original copy after notifying of clients.</note></para>
        ///// </remarks>
        /////
        //public event NewFrameEventHandler NewFrame;

        ///// <summary>
        ///// Snapshot frame event.
        ///// </summary>
        /////
        ///// <remarks><para>Notifies clients about new available snapshot frame - the one which comes when
        ///// camera's snapshot/shutter button is pressed.</para>
        /////
        ///// <para>See documentation to <see cref="ProvideSnapshots"/> for additional information.</para>
        /////
        ///// <para><note>Since video source may have multiple clients, each client is responsible for
        ///// making a copy (cloning) of the passed snapshot frame, because the video source disposes its
        ///// own original copy after notifying of clients.</note></para>
        ///// </remarks>
        /////
        ///// <seealso cref="ProvideSnapshots"/>
        /////
        //public event NewFrameEventHandler SnapshotFrame;

        ///// <summary>
        ///// Video source error event.
        ///// </summary>
        /////
        ///// <remarks>This event is used to notify clients about any type of errors occurred in
        ///// video source object, for example internal exceptions.</remarks>
        /////
        //public event VideoSourceErrorEventHandler VideoSourceError;

        ///// <summary>
        ///// Video playing finished event.
        ///// </summary>
        /////
        ///// <remarks><para>This event is used to notify clients that the video playing has finished.</para>
        ///// </remarks>
        /////
        //public event PlayingFinishedEventHandler PlayingFinished;

        /// <summary>
        /// Video source.
        /// </summary>
        ///
        /// <remarks>Video source is represented by moniker string of video capture device.</remarks>
        ///
        public virtual string Source
        {
            get { return deviceMoniker; }
            set
            {
                deviceMoniker = value;

                videoCapabilities = null;
                snapshotCapabilities = null;
                crossbarVideoInputs = null;
                isCrossbarAvailable = null;
            }
        }

        /// <summary>
        /// Received frames count.
        /// </summary>
        ///
        /// <remarks>Number of frames the video source provided from the moment of the last
        /// access to the property.
        /// </remarks>
        ///
        public int FramesReceived
        {
            get
            {
                int frames = framesReceived;
                framesReceived = 0;
                return frames;
            }
        }

        /// <summary>
        /// Received bytes count.
        /// </summary>
        ///
        /// <remarks>Number of bytes the video source provided from the moment of the last
        /// access to the property.
        /// </remarks>
        ///
        public long BytesReceived
        {
            get
            {
                long bytes = bytesReceived;
                bytesReceived = 0;
                return bytes;
            }
        }

        /// <summary>
        /// State of the video source.
        /// </summary>
        ///
        /// <remarks>Current state of video source object - running or not.</remarks>
        ///
        public bool IsRunning
        {
            get
            {
                if (thread != null)
                {
                    // check thread status
                    if (thread.Join(0) == false)
                        return true;

                    // the thread is not running, free resources
                    Free();
                }
                return false;
            }
        }

        /// <summary>
        /// Obsolete - no longer in use
        /// </summary>
        ///
        /// <remarks><para>The property is obsolete. Use <see cref="VideoResolution"/> property instead.
        /// Setting this property does not have any effect.</para></remarks>
        ///
        [Obsolete]
        public Size DesiredFrameSize
        {
            get { return Size.Empty; }
            set { }
        }

        /// <summary>
        /// Obsolete - no longer in use
        /// </summary>
        ///
        /// <remarks><para>The property is obsolete. Use <see cref="SnapshotResolution"/> property instead.
        /// Setting this property does not have any effect.</para></remarks>
        ///
        [Obsolete]
        public Size DesiredSnapshotSize
        {
            get { return Size.Empty; }
            set { }
        }

        /// <summary>
        /// Obsolete - no longer in use.
        /// </summary>
        ///
        /// <remarks><para>The property is obsolete. Setting this property does not have any effect.</para></remarks>
        ///
        [Obsolete]
        public int DesiredFrameRate
        {
            get { return 0; }
            set { }
        }

        /// <summary>
        /// Video resolution to set.
        /// </summary>
        ///
        /// <remarks><para>The property allows to set one of the video resolutions supported by the camera.
        /// Use <see cref="VideoCapabilities"/> property to get the list of supported video resolutions.</para>
        ///
        /// <para><note>The property must be set before camera is started to make any effect.</note></para>
        ///
        /// <para>Default value of the property is set to <see langword="null"/>, which means default video
        /// resolution is used.</para>
        /// </remarks>
        ///
        public VideoCapabilities VideoResolution
        {
            get { return videoResolution; }
            set { videoResolution = value; }
        }

        /// <summary>
        /// Snapshot resolution to set.
        /// </summary>
        ///
        /// <remarks><para>The property allows to set one of the snapshot resolutions supported by the camera.
        /// Use <see cref="SnapshotCapabilities"/> property to get the list of supported snapshot resolutions.</para>
        ///
        /// <para><note>The property must be set before camera is started to make any effect.</note></para>
        ///
        /// <para>Default value of the property is set to <see langword="null"/>, which means default snapshot
        /// resolution is used.</para>
        /// </remarks>
        ///
        public VideoCapabilities SnapshotResolution
        {
            get { return snapshotResolution; }
            set { snapshotResolution = value; }
        }

        /// <summary>
        /// Video capabilities of the device.
        /// </summary>
        ///
        /// <remarks><para>The property provides list of device's video capabilities.</para>
        ///
        /// <para><note>It is recomended not to call this property immediately after <see cref="Start"/> method, since
        /// device may not start yet and provide its information. It is better to call the property
        /// before starting device or a bit after (but not immediately after).</note></para>
        /// </remarks>
        ///
        //public VideoCapabilities[] VideoCapabilities
        //{
        //    get
        //    {
        //        if (videoCapabilities == null)
        //        {
        //            lock (cacheVideoCapabilities)
        //            {
        //                if ((!string.IsNullOrEmpty(deviceMoniker)) && (cacheVideoCapabilities.ContainsKey(deviceMoniker)))
        //                {
        //                    videoCapabilities = cacheVideoCapabilities[deviceMoniker];
        //                }
        //            }

        //            if (videoCapabilities == null)
        //            {
        //                if (!IsRunning)
        //                {
        //                    // create graph without playing to get the video/snapshot capabilities only.
        //                    // not very clean but it works
        //                    WorkerThread(false);
        //                }
        //                else
        //                {
        //                    for (int i = 0; (i < 500) && (videoCapabilities == null); i++)
        //                    {
        //                        Thread.Sleep(10);
        //                    }
        //                }
        //            }
        //        }
        //        // don't return null even capabilities are not provided for some reason
        //        return (videoCapabilities != null) ? videoCapabilities : new VideoCapabilities[0];
        //    }
        //}

        /// <summary>
        /// Snapshot capabilities of the device.
        /// </summary>
        ///
        /// <remarks><para>The property provides list of device's snapshot capabilities.</para>
        ///
        /// <para>If the array has zero length, then it means that this device does not support making
        /// snapshots.</para>
        ///
        /// <para>See documentation to <see cref="ProvideSnapshots"/> for additional information.</para>
        ///
        /// <para><note>It is recomended not to call this property immediately after <see cref="Start"/> method, since
        /// device may not start yet and provide its information. It is better to call the property
        /// before starting device or a bit after (but not immediately after).</note></para>
        /// </remarks>
        ///
        /// <seealso cref="ProvideSnapshots"/>
        ///
        //public VideoCapabilities[] SnapshotCapabilities
        //{
        //    get
        //    {
        //        if (snapshotCapabilities == null)
        //        {
        //            lock (cacheSnapshotCapabilities)
        //            {
        //                if ((!string.IsNullOrEmpty(deviceMoniker)) && (cacheSnapshotCapabilities.ContainsKey(deviceMoniker)))
        //                {
        //                    snapshotCapabilities = cacheSnapshotCapabilities[deviceMoniker];
        //                }
        //            }

        //            if (snapshotCapabilities == null)
        //            {
        //                if (!IsRunning)
        //                {
        //                    // create graph without playing to get the video/snapshot capabilities only.
        //                    // not very clean but it works
        //                    WorkerThread(false);
        //                }
        //                else
        //                {
        //                    for (int i = 0; (i < 500) && (snapshotCapabilities == null); i++)
        //                    {
        //                        Thread.Sleep(10);
        //                    }
        //                }
        //            }
        //        }
        //        // don't return null even capabilities are not provided for some reason
        //        return (snapshotCapabilities != null) ? snapshotCapabilities : new VideoCapabilities[0];
        //    }
        //}

        /// <summary>
        /// Source COM object of camera capture device.
        /// </summary>
        ///
        /// <remarks><para>The source COM object of camera capture device is exposed for the
        /// case when user may need get direct access to the object for making some custom
        /// configuration of camera through DirectShow interface, for example.
        /// </para>
        ///
        /// <para>If camera is not running, the property is set to <see langword="null"/>.</para>
        /// </remarks>
        ///
        public object SourceObject
        {
            get { return sourceObject; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoCaptureDevice"/> class.
        /// </summary>
        ///
        public VideoCaptureDevice2()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoCaptureDevice"/> class.
        /// </summary>
        ///
        /// <param name="deviceMoniker">Moniker string of video capture device.</param>
        ///
        public VideoCaptureDevice2(string deviceMoniker)
        {
            this.deviceMoniker = deviceMoniker;
        }



        /// <summary>
        /// Signal video source to stop its work.
        /// </summary>
        ///
        /// <remarks>Signals video source to stop its background thread, stop to
        /// provide new frames and free resources.</remarks>
        ///
        public void SignalToStop()
        {
            // stop thread
            if (thread != null)
            {
                // signal to stop
                stopEvent.Set();
            }
        }

        /// <summary>
        /// Wait for video source has stopped.
        /// </summary>
        ///
        /// <remarks>Waits for source stopping after it was signalled to stop using
        /// <see cref="SignalToStop"/> method.</remarks>
        ///
        public void WaitForStop()
        {
            if (thread != null)
            {
                // wait for thread stop
                thread.Join();

                Free();
            }
        }

        /// <summary>
        /// Stop video source.
        /// </summary>
        ///
        /// <remarks><para>Stops video source aborting its thread.</para>
        ///
        /// <para><note>Since the method aborts background thread, its usage is highly not preferred
        /// and should be done only if there are no other options. The correct way of stopping camera
        /// is <see cref="SignalToStop">signaling it stop</see> and then
        /// <see cref="WaitForStop">waiting</see> for background thread's completion.</note></para>
        /// </remarks>
        ///
        public void Stop()
        {
            if (this.IsRunning)
            {
                thread.Abort();
                WaitForStop();
            }
        }

        /// <summary>
        /// Free resource.
        /// </summary>
        ///
        private void Free()
        {
            thread = null;

            // release events
            stopEvent.Close();
            stopEvent = null;
        }



        /// <summary>
        /// Sets a specified property on the camera.
        /// </summary>
        ///
        /// <param name="property">Specifies the property to set.</param>
        /// <param name="value">Specifies the new value of the property.</param>
        /// <param name="controlFlags">Specifies the desired control setting.</param>
        ///
        /// <returns>Returns true on success or false otherwise.</returns>
        ///
        /// <exception cref="ArgumentException">Video source is not specified - device moniker is not set.</exception>
        /// <exception cref="ApplicationException">Failed creating device object for moniker.</exception>
        /// <exception cref="NotSupportedException">The video source does not support camera control.</exception>
        ///
        public bool SetVideoProperty(VideoProcAmpProperty property, int value, VideoProcAmpFlags controlFlags)
        {
            bool ret = true;

            // check if source was set
            if ((deviceMoniker == null) || (string.IsNullOrEmpty(deviceMoniker)))
            {
                throw new ArgumentException("Video source is not specified.");
            }

            lock (sync)
            {
                object tempSourceObject = null;

                // create source device's object
                try
                {
                    tempSourceObject = FilterInfo.CreateFilter(deviceMoniker);
                }
                catch
                {
                    throw new ApplicationException("Failed creating device object for moniker.");
                }

                if (!(tempSourceObject is IAMVideoProcAmp))
                {
                    throw new NotSupportedException("The video source does not support camera control.");
                }

                IAMVideoProcAmp pCamControl = (IAMVideoProcAmp)tempSourceObject;
                int hr = pCamControl.Set(property, value, controlFlags);

                ret = (hr >= 0);

                Marshal.ReleaseComObject(tempSourceObject);
            }

            return ret;
        }

        /// <summary>
        /// Gets the current setting of a camera property.
        /// </summary>
        ///
        /// <param name="property">Specifies the property to retrieve.</param>
        /// <param name="value">Receives the value of the property.</param>
        /// <param name="controlFlags">Receives the value indicating whether the setting is controlled manually or automatically</param>
        ///
        /// <returns>Returns true on success or false otherwise.</returns>
        ///
        /// <exception cref="ArgumentException">Video source is not specified - device moniker is not set.</exception>
        /// <exception cref="ApplicationException">Failed creating device object for moniker.</exception>
        /// <exception cref="NotSupportedException">The video source does not support camera control.</exception>
        ///
        public bool GetVideoProperty(VideoProcAmpProperty property, out int value, out VideoProcAmpFlags controlFlags)
        {
            bool ret = true;

            // check if source was set
            if ((deviceMoniker == null) || (string.IsNullOrEmpty(deviceMoniker)))
            {
                throw new ArgumentException("Video source is not specified.");
            }

            lock (sync)
            {
                object tempSourceObject = null;

                // create source device's object
                try
                {
                    tempSourceObject = FilterInfo.CreateFilter(deviceMoniker);
                }
                catch
                {
                    throw new ApplicationException("Failed creating device object for moniker.");
                }

                if (!(tempSourceObject is IAMVideoProcAmp))
                {
                    throw new NotSupportedException("The video source does not support camera control.");
                }

                IAMVideoProcAmp pCamControl = (IAMVideoProcAmp)tempSourceObject;
                int hr = pCamControl.Get(property, out value, out controlFlags);

                ret = (hr >= 0);

                Marshal.ReleaseComObject(tempSourceObject);
            }

            return ret;
        }

        /// <summary>
        /// Gets the range and default value of a specified camera property.
        /// </summary>
        ///
        /// <param name="property">Specifies the property to query.</param>
        /// <param name="minValue">Receives the minimum value of the property.</param>
        /// <param name="maxValue">Receives the maximum value of the property.</param>
        /// <param name="stepSize">Receives the step size for the property.</param>
        /// <param name="defaultValue">Receives the default value of the property.</param>
        /// <param name="controlFlags">Receives a member of the <see cref="CameraControlFlags"/> enumeration, indicating whether the property is controlled automatically or manually.</param>
        ///
        /// <returns>Returns true on success or false otherwise.</returns>
        ///
        /// <exception cref="ArgumentException">Video source is not specified - device moniker is not set.</exception>
        /// <exception cref="ApplicationException">Failed creating device object for moniker.</exception>
        /// <exception cref="NotSupportedException">The video source does not support camera control.</exception>
        ///
        public bool GetVideoPropertyRange(VideoProcAmpProperty property, out int minValue, out int maxValue, out int stepSize, out int defaultValue, out VideoProcAmpFlags controlFlags)
        {
            bool ret = true;

            // check if source was set
            if ((deviceMoniker == null) || (string.IsNullOrEmpty(deviceMoniker)))
            {
                throw new ArgumentException("Video source is not specified.");
            }

            lock (sync)
            {
                object tempSourceObject = null;

                // create source device's object
                try
                {
                    tempSourceObject = FilterInfo.CreateFilter(deviceMoniker);
                }
                catch
                {
                    throw new ApplicationException("Failed creating device object for moniker.");
                }

                if (!(tempSourceObject is IAMVideoProcAmp))
                {
                    throw new NotSupportedException("The video source does not support camera control.");
                }

                IAMVideoProcAmp pCamControl = (IAMVideoProcAmp)tempSourceObject;
                int hr = pCamControl.GetRange(property, out minValue, out maxValue, out stepSize, out defaultValue, out controlFlags);

                ret = (hr >= 0);

                Marshal.ReleaseComObject(tempSourceObject);
            }

            return ret;
        }


    }
}