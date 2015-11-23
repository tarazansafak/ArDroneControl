using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AR.Drone.Client;
using AR.Drone.Client.Command;
using AR.Drone.Client.Configuration;
using AR.Drone.Data;
using AR.Drone.Data.Navigation;
using AR.Drone.Data.Navigation.Native;
using AR.Drone.Media;
using AR.Drone.Video;
using AR.Drone.Avionics;
using AR.Drone.Avionics.Objectives;
using AR.Drone.Avionics.Objectives.IntentObtainers;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;


namespace AR.Drone.WinApp
{
    public partial class MainForm : Form
    {
        private const string ARDroneTrackFileExt = ".ardrone";
        private const string ARDroneTrackFilesFilter = "AR.Drone track files (*.ardrone)|*.ardrone";

        private readonly DroneClient _droneClient;
        private readonly List<PlayerForm> _playerForms;
        private readonly VideoPacketDecoderWorker _videoPacketDecoderWorker;
        private Settings _settings;
        private VideoFrame _frame;
        private Bitmap _frameBitmap;
        private uint _frameNumber;
        private NavigationData _navigationData;
        private NavigationPacket _navigationPacket;
        private PacketRecorder _packetRecorderWorker;
        private FileStream _recorderStream;
        private Autopilot _autopilot;

        private Bitmap capturedImage;
        private bool isSensitiveMode = false;
        private float flightSensitivityConst = 0.9f;




        Image<Bgr, Byte> currentFrame;
        Capture grabber;
        HaarCascade face;
        Image<Gray, byte> result;
        Image<Gray, byte> gray = null;
        int t;




        public MainForm()
        {
            InitializeComponent();

            face = new HaarCascade("haarcascade_frontalface_default.xml");
         //   grabber = new Capture();
          //  grabber.QueryFrame();
          //  Application.Idle += new EventHandler(FrameGrabber);

            this.WindowState = FormWindowState.Maximized;

            _videoPacketDecoderWorker = new VideoPacketDecoderWorker(PixelFormat.BGR24, true, OnVideoPacketDecoded);
            _videoPacketDecoderWorker.Start();

            _droneClient = new DroneClient("192.168.1.1");
            _droneClient.NavigationPacketAcquired += OnNavigationPacketAcquired;
            _droneClient.VideoPacketAcquired += OnVideoPacketAcquired;
            _droneClient.NavigationDataAcquired += data => _navigationData = data;

            tmrStateUpdate.Enabled = true;
            tmrVideoUpdate.Enabled = true;

            _playerForms = new List<PlayerForm>();

            _videoPacketDecoderWorker.UnhandledException += UnhandledException;

            _droneClient.Start();

        }


        private void UnhandledException(object sender, Exception exception)
        {
            MessageBox.Show(exception.ToString(), "Unhandled Exception (Ctrl+C)", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Text += Environment.Is64BitProcess ? " [64-bit]" : " [32-bit]";
        }

        protected override void OnClosed(EventArgs e)
        {
            if (_autopilot != null)
            {
                _autopilot.UnbindFromClient();
                _autopilot.Stop();
            }

            StopRecording();

            _droneClient.Dispose();
            _videoPacketDecoderWorker.Dispose();

            base.OnClosed(e);
        }

        private void OnNavigationPacketAcquired(NavigationPacket packet)
        {
            if (_packetRecorderWorker != null && _packetRecorderWorker.IsAlive)
                _packetRecorderWorker.EnqueuePacket(packet);

            _navigationPacket = packet;
        }

        private void OnVideoPacketAcquired(VideoPacket packet)
        {
            if (_packetRecorderWorker != null && _packetRecorderWorker.IsAlive)
                _packetRecorderWorker.EnqueuePacket(packet);
            if (_videoPacketDecoderWorker.IsAlive)
                _videoPacketDecoderWorker.EnqueuePacket(packet);
        }

        private void OnVideoPacketDecoded(VideoFrame frame)
        {
            _frame = frame;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _droneClient.Stop();
        }

        //void FrameGrabber(object sender, EventArgs e)
        //{


        //    currentFrame = grabber.QueryFrame().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);

        //    gray = currentFrame.Convert<Gray, Byte>();

        //    MCvAvgComp[][] facesDetected = gray.DetectHaarCascade(
        //    face,
        //    1.2,
        //    10,
        //    Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
        //    new Size(20, 20));

        //    Action for each element detected
        //    foreach (MCvAvgComp f in facesDetected[0])
        //    {
        //        t = t + 1;
        //        result = currentFrame.Copy(f.rect).Convert<Gray, byte>().Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
        //        currentFrame.Draw(f.rect, new Bgr(Color.Red), 2);
        //    }
        //    t = 0;


        //    imageBoxFrameGrabber.SizeMode = PictureBoxSizeMode.CenterImage;
        //    imageBoxFrameGrabber.Image = currentFrame;



        //}

        private void tmrVideoUpdate_Tick(object sender, EventArgs e)
        {
            if (_frame == null || _frameNumber == _frame.Number)
                return;
            _frameNumber = _frame.Number;

            if (_frameBitmap == null)
                _frameBitmap = VideoHelper.CreateBitmap(ref _frame);
            else
                VideoHelper.UpdateBitmap(ref _frameBitmap, ref _frame);


                Image<Bgr, Byte> My_Image = new Image<Bgr, byte>(_frameBitmap);
                gray = My_Image.Convert<Gray, byte>();
                                     

                MCvAvgComp[][] facesDetected = gray.DetectHaarCascade(
                face,
                1.2,
                10,
                Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
                new Size(20, 20));

                //Action for each element detected
                foreach (MCvAvgComp f in facesDetected[0])
                {
                    t = t + 1;
                    result = My_Image.Copy(f.rect).Convert<Gray, byte>().Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                    My_Image.Draw(f.rect, new Bgr(Color.Red), 2);
                }
                t = 0;

            imageBoxFrameGrabber.Image = My_Image;
            pbVideo.Image = _frameBitmap;
            imageBoxFrameGrabber.SizeMode = PictureBoxSizeMode.CenterImage;
            pbVideo.SizeMode = PictureBoxSizeMode.CenterImage;
        }

        private void tmrStateUpdate_Tick(object sender, EventArgs e)
        {
            tvInfo.BeginUpdate();

            TreeNode node = tvInfo.Nodes.GetOrCreate("ClientActive");
            node.Text = string.Format("Client Active: {0}", _droneClient.IsActive);

            node = tvInfo.Nodes.GetOrCreate("Navigation Data");
            if (_navigationData != null) DumpBranch(node.Nodes, _navigationData);

            node = tvInfo.Nodes.GetOrCreate("Configuration");
            if (_settings != null) DumpBranch(node.Nodes, _settings);

            TreeNode vativeNode = tvInfo.Nodes.GetOrCreate("Native");

            NavdataBag navdataBag;
            if (_navigationPacket.Data != null && NavdataBagParser.TryParse(ref _navigationPacket, out navdataBag))
            {
                var ctrl_state = (CTRL_STATES)(navdataBag.demo.ctrl_state >> 0x10);
                node = vativeNode.Nodes.GetOrCreate("ctrl_state");
                node.Text = string.Format("Ctrl State: {0}", ctrl_state);

                var flying_state = (FLYING_STATES)(navdataBag.demo.ctrl_state & 0xffff);
                node = vativeNode.Nodes.GetOrCreate("flying_state");
                node.Text = string.Format("Ctrl State: {0}", flying_state);

                DumpBranch(vativeNode.Nodes, navdataBag);
            }
            tvInfo.EndUpdate();


        }

        private void DumpBranch(TreeNodeCollection nodes, object o)
        {
            Type type = o.GetType();

            foreach (FieldInfo fieldInfo in type.GetFields())
            {
                TreeNode node = nodes.GetOrCreate(fieldInfo.Name);
                object value = fieldInfo.GetValue(o);

                DumpValue(fieldInfo.FieldType, node, value);
            }

            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                TreeNode node = nodes.GetOrCreate(propertyInfo.Name);
                object value = propertyInfo.GetValue(o, null);

                DumpValue(propertyInfo.PropertyType, node, value);
            }
        }

        private void DumpValue(Type type, TreeNode node, object value)
        {
            if (value == null)
                node.Text = node.Name + ": null";
            else
            {
                if (type.Namespace.StartsWith("System") || type.IsEnum)
                    node.Text = node.Name + ": " + value;
                else
                    DumpBranch(node.Nodes, value);
            }
        }


        //private void btnReadConfig_Click(object sender, EventArgs e)
        //{
        //    Task<Settings> configurationTask = _droneClient.GetConfigurationTask();
        //    configurationTask.ContinueWith(delegate(Task<Settings> task)
        //        {
        //            if (task.Exception != null)
        //            {
        //                Trace.TraceWarning("Get configuration task is faulted with exception: {0}", task.Exception.InnerException.Message);
        //                return;
        //            }

        //            _settings = task.Result;
        //        });
        //    configurationTask.Start();
        //}


        private void StopRecording()
        {
            if (_packetRecorderWorker != null)
            {
                _packetRecorderWorker.Stop();
                _packetRecorderWorker.Join();
                _packetRecorderWorker = null;
            }
            if (_recorderStream != null)
            {
                _recorderStream.Dispose();
                _recorderStream = null;
            }
        }

        private void btnStopRecording_Click(object sender, EventArgs e)
        {
            StopRecording();
        }

        private void btnReplay_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog { DefaultExt = ARDroneTrackFileExt, Filter = ARDroneTrackFilesFilter })
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    StopRecording();

                    var playerForm = new PlayerForm { FileName = dialog.FileName };
                    playerForm.Closed += (o, args) => _playerForms.Remove(o as PlayerForm);
                    _playerForms.Add(playerForm);
                    playerForm.Show(this);
                }
            }
        }

        // Make sure '_autopilot' variable is initialized with an object
        private void CreateAutopilot()
        {
            if (_autopilot != null) return;

            _autopilot = new Autopilot(_droneClient);
            _autopilot.OnOutOfObjectives += Autopilot_OnOutOfObjectives;
            _autopilot.BindToClient();
            _autopilot.Start();
        }

        // Event that occurs when no objectives are waiting in the autopilot queue
        private void Autopilot_OnOutOfObjectives()
        {
            _autopilot.Active = false;
        }

        // Create a simple mission for autopilot
        private void CreateAutopilotMission()
        {
            _autopilot.ClearObjectives();

            // Do two 36 degrees turns left and right if the drone is already flying
            if (_droneClient.NavigationData.State.HasFlag(NavigationState.Flying))
            {
                const float turn = (float)(Math.PI / 5);
                float heading = _droneClient.NavigationData.Yaw;

                _autopilot.EnqueueObjective(Objective.Create(2000, new Heading(heading + turn, aCanBeObtained: true)));
                _autopilot.EnqueueObjective(Objective.Create(2000, new Heading(heading - turn, aCanBeObtained: true)));
                _autopilot.EnqueueObjective(Objective.Create(2000, new Heading(heading, aCanBeObtained: true)));
            }
            else // Just take off if the drone is on the ground
            {
                _autopilot.EnqueueObjective(new FlatTrim(1000));
                _autopilot.EnqueueObjective(new Takeoff(3500));
            }

            // One could use hover, but the method below, allows to gain/lose/maintain desired altitude
            _autopilot.EnqueueObjective(
                Objective.Create(3000,
                    new VelocityX(0.0f),
                    new VelocityY(0.0f),
                    new Altitude(1.0f)
                )
            );

            _autopilot.EnqueueObjective(new Land(5000));
        }

        // Activate/deactive autopilot

        private void Main_Form_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F:
                    _droneClient.FlatTrim();
                    break;
                case Keys.Back:
                    if (!isSensitiveMode)
                    {
                        flightSensitivityConst = 0.25f;
                        isSensitiveMode = true;
                    }
                    else
                    {
                        flightSensitivityConst = 0.9f;
                        isSensitiveMode = false;
                    }

                    break;
                case Keys.W:
                    _droneClient.Progress(FlightMode.Progressive, pitch: -flightSensitivityConst);
                    break;
                case Keys.S:
                    _droneClient.Progress(FlightMode.Progressive, pitch: flightSensitivityConst);
                    break;
                case Keys.A:
                    _droneClient.Progress(FlightMode.Progressive, roll: -flightSensitivityConst);
                    break;
                case Keys.D:
                    _droneClient.Progress(FlightMode.Progressive, roll: +flightSensitivityConst);
                    break;
                case Keys.Up:
                    _droneClient.Progress(FlightMode.Progressive, gaz: flightSensitivityConst);
                    break;
                case Keys.Down:
                    _droneClient.Progress(FlightMode.Progressive, gaz: -flightSensitivityConst);
                    break;
                case Keys.Left:
                    _droneClient.Progress(FlightMode.Progressive, yaw: -flightSensitivityConst);
                    break;
                case Keys.Right:
                    _droneClient.Progress(FlightMode.Progressive, yaw: +flightSensitivityConst);
                    break;
                case Keys.E:
                    _droneClient.Takeoff();
                    break;
                case Keys.Space:
                    _droneClient.Land();
                    break;
                case Keys.C:
                    var configuration = new Settings();
                    configuration.Video.Channel = VideoChannelType.Next;
                    _droneClient.Send(configuration);
                    break;
                case Keys.R:
                    string path = string.Format("ttu_flight_{0:yyyy_MM_dd_HH_mm}" + ARDroneTrackFileExt, DateTime.Now);
                    using (var dialog = new SaveFileDialog { DefaultExt = ARDroneTrackFileExt, Filter = ARDroneTrackFilesFilter, FileName = path })
                    {
                        if (dialog.ShowDialog(this) == DialogResult.OK)
                        {
                            StopRecording();

                            _recorderStream = new FileStream(dialog.FileName, FileMode.OpenOrCreate);
                            _packetRecorderWorker = new PacketRecorder(_recorderStream);
                            _packetRecorderWorker.Start();
                        }
                    }
                    break;
            }
        }

        private void Main_Form_KeyUp(object sender, KeyEventArgs e)
        {
            _droneClient.Hover();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            capturedImage = new Bitmap(200, 100);
            capturedImage = this._frameBitmap;
            this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            this.pictureBox1.Image = _frameBitmap;

        }



    }
}