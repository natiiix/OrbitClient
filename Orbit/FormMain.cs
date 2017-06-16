using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NatiTools.xWeb;

namespace Orbit
{
    public partial class FormMain : Form
    {
        // Constants
        private const string SERVER_URL = "http://orbit.6f.sk";
        private const string RESPONSE_END_SIGN = "<!--END-->";
        private const char DATA_SEPARATOR = ';';

        private static string MAC_ADDRESS = GetMacAddress();

        // Static members
        private static DateTime m_dtLastUpdate = DateTime.Now;
        private static double m_latency = 0.0;
        private static Player[] m_players = new Player[0];
        private static Player m_self = new Player(0, 0);

        // Instance members
        Sampler<double> m_samplerLatency = new Sampler<double>(5);
        Sampler<double> m_samplerRenderTime = new Sampler<double>(10);

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            FillFormWithMap();

            // Make labels display "proper" transparent background
            labelFPS.Parent = pictureBoxMap;
            labelLatency.Parent = pictureBoxMap;

            m_samplerLatency.OnSamplerOverflow += SamplerLatency_OnSamplerOverflow;
            m_samplerRenderTime.OnSamplerOverflow += SamplerRenderTime_OnSamplerOverflow;

            // Start updating UI and reading data from server
            timerUpdateUI.Start();
            Task.Run(() => RequestRead());
        }

        private void SamplerLatency_OnSamplerOverflow(object sender, SamplerEventArgs<double> e)
        {
            // Update the average latency variable
            m_latency = e.AverageValue;
        }

        private void SamplerRenderTime_OnSamplerOverflow(object sender, SamplerEventArgs<double> e)
        {
            // Update the FPS counter
            // Avoid division by zero when the total render time is zero
            labelFPS.Text = "FPS: " + (e.AverageValue > 0 ? (1 / e.AverageValue).ToString("0") : "infinite");
        }

        private void RequestMove(int x, int y)
        {
            // Put data into objects
            HTTP.PostData pdId = new HTTP.PostData("id", MAC_ADDRESS);
            HTTP.PostData pdAction = new HTTP.PostData("action", "move");
            HTTP.PostData pdData = new HTTP.PostData("data", x.ToString() + ";" + y.ToString());
            
            // Send the move request
            HTTP.GetPostResponse(SERVER_URL, pdId, pdAction, pdData);
        }
        
        private void FillFormWithMap()
        {
            // Makes the map picture box fill the entire area of the form
            pictureBoxMap.Bounds = ClientRectangle;
        }

        private string ExtractData(string strResponse)
        {
            // Get the part of the response before the end sign
            return strResponse.Substring(0, strResponse.IndexOf(RESPONSE_END_SIGN));
        }

        private void TimerUpdateUI_Tick(object sender, EventArgs e)
        {
            // Start measuring render time
            DateTime dtStart = DateTime.Now;

            // ---- RENDER START ----

            // Update the latency label
            labelLatency.Text = "Latency: " + m_latency.ToString("0") + " ms";

            // Dispose the old map bitmap
            if (pictureBoxMap.Image != null)
            {
                pictureBoxMap.Image.Dispose();
            }
            // Update the map bitmap
            pictureBoxMap.Image = GenerateMap();

            // ---- RENDER END ----

            // Calculate the render time and push it to the sampler
            m_samplerRenderTime.Push((DateTime.Now - dtStart).TotalSeconds);
        }

        private void PictureBoxMap_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Send move request to the server
                Task.Run(() => RequestMove(e.X, e.Y));

                // Move the player locally
                m_self.Move(e.X, e.Y);
            }
        }

        private static string GetMacAddress()
        {
            // https://stackoverflow.com/a/7661829/3043260

            return (from nic in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
                    where nic.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up
                    select nic.GetPhysicalAddress().ToString()
                    ).FirstOrDefault();
        }

        private Bitmap GenerateMap()
        {
            // Create new bitmap
            Bitmap bmpMap = new Bitmap(pictureBoxMap.Width, pictureBoxMap.Height);

            // Fill the bitmap with the background color
            using (Graphics g = Graphics.FromImage(bmpMap))
            {
                g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(0, 0, bmpMap.Width, bmpMap.Height));
            }

            // Draw dots representing players to the map
            foreach (Player p in m_players)
            {
                bmpMap.SetPixel(p.X, p.Y, Color.White);
            }
            
            // Return the generated map bitmap
            return bmpMap;
        }

        private void RequestRead()
        {
            // Put data into objects
            HTTP.PostData pdId = new HTTP.PostData("id", MAC_ADDRESS);
            HTTP.PostData pdAction = new HTTP.PostData("action", "read");

            // Get current time
            DateTime dtSendTime = DateTime.Now;
            // Send the request and process the output
            string strDataTmp = ExtractData(HTTP.GetPostResponse(SERVER_URL, pdId, pdAction));
            // Calculate the server response latency
            m_samplerLatency.Push((DateTime.Now - dtSendTime).TotalMilliseconds);

            bool dataOK = true;

            // Separate rows
            string[] rows = strDataTmp.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            Player[] tempPlayers = new Player[rows.Length];

            for (int i = 0; i < rows.Length; i++)
            {
                // Separate individual coordinates on each row
                string[] coordinates = rows[i].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                // There are two coordinates and their values are supposed to be integer
                if (coordinates.Length == 2 &&
                    int.TryParse(coordinates[0], out int x) &&
                    int.TryParse(coordinates[1], out int y))
                {
                    // Store the player in the array
                    tempPlayers[i] = new Player(x, y);
                }
                else
                {
                    // There is an error in the data string, something isn't right
                    dataOK = false;
                }
            }

            // Use the most recent data
            // When data from older request than the last handled one returns ignore it
            // Check for data string not being broken
            if (m_dtLastUpdate < dtSendTime && dataOK)
            {
                m_dtLastUpdate = dtSendTime;
                m_players = tempPlayers;
            }

            // Send another read request asynchronously
            Task.Run(() => RequestRead());
        }

        private void LabelFPS_MouseClick(object sender, MouseEventArgs e)
        {
            ClickThroughLabel(labelFPS, sender, e);
        }

        private void LabelLatency_MouseClick(object sender, MouseEventArgs e)
        {
            ClickThroughLabel(labelLatency, sender, e);
        }

        private void LabelLocation_MouseClick(object sender, MouseEventArgs e)
        {
            ClickThroughLabel(labelLocation, sender, e);
        }

        private void ClickThroughLabel(Label label, object sender, MouseEventArgs e)
        {
            // Redirect the mouse click event to the map picture box with proper click location information
            MouseEventArgs evArgs = new MouseEventArgs(e.Button, e.Clicks, e.X + label.Location.X, e.Y + label.Location.Y, e.Delta);
            PictureBoxMap_MouseClick(sender, evArgs);
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            // Change the size of the map when the form is resized
            FillFormWithMap();
        }
    }
}
