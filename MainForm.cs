using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using SharedCoreLib.AudioProcessing;

namespace shazam
{
    public partial class MainForm : Form
    {
        ShazamClient client;
        Microphone microphone;
        byte[] audioData;
        int bytesRead = 0;
        int counter = 10;

        public MainForm()
        {
            InitializeComponent();
            client = new ShazamClient();
            client.OnRecongnitionStateChanged += ShazamStateChanged;
            microphone = Microphone.Default;
            if (Microphone.All.Count == 0)
            {
                MessageBox.Show("There are no recording devices on this computer.", "Error");
                Application.Exit();
            }
            FrameworkDispatcher.Update();
        }

        public void ShazamStateChanged(ShazamRecognitionState State, ShazamResponse response)
        {
            switch (State)
            {
                case ShazamRecognitionState.Sending:
                    this.statusLabel.Text = "Sending...";
                    break;
                case ShazamRecognitionState.Matching:
                    this.statusLabel.Text = "Matching...";
                    break;
                case ShazamRecognitionState.Done:
                    this.statusLabel.Text = "Click to Recognize";
                    button1.Enabled = true;
                    if (response.Tag != null)
                    {
                        if (response.Tag.Track != null)
                            MessageBox.Show("Title: " + response.Tag.Track.Title + "\r\nArtist: " + response.Tag.Track.Artist, "Hey!");
                        else
                            MessageBox.Show("Song not found :-(", "Hey!");
                    }
                    else
                        MessageBox.Show("Song not found :-(", "Hey!");
                    break;
                case ShazamRecognitionState.Failed:
                    button1.Enabled = true;
                    if (response.Exception.Message != null && response.Exception.Message != "")
                        this.statusLabel.Text = "Failed! Message: " + response.Exception.Message;
                    else
                        this.statusLabel.Text = "Failed!";
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            audioData = new byte[Microphone.Default.GetSampleSizeInBytes(TimeSpan.FromSeconds(10.0))];
            bytesRead = 0;
            counter = 10;
            microphone.Start();
            timer.Start();
            recordTimer.Start();
            this.statusLabel.Text = "Listening... "+counter;
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnimationSpeed = 10;
            button1.Enabled = false;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            ProcessPCMAudio(microphone.SampleRate,16,1);
            string str = Encoding.UTF8.GetString(audioData);
            microphone.Stop();
            client.DoRecognition(audioData, MicrophoneRecordingOutputFormatType.PCM);
            microphone.Stop();
            timer.Stop();
            recordTimer.Stop();
            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.MarqueeAnimationSpeed = 0;
        }


        private void ProcessPCMAudio(int sampleRate, short numBitsPerSample, short numChennels)
        {
                int length = audioData.Length;
                WaveFile.WaveHeader waveHeader = new WaveFile.WaveHeader(length, sampleRate, numBitsPerSample, numChennels, false);
                MemoryStream memoryStream = WaveFile.WriteWaveFile(waveHeader, audioData);
                audioData = memoryStream.GetBuffer();
        }

        private void recordTimer_Tick(object sender, EventArgs e)
        {
            counter--;
            statusLabel.Text = "Listening... " + counter;
            bytesRead += Microphone.Default.GetData(audioData, bytesRead, (audioData.Length - bytesRead));
        }
    }
}
