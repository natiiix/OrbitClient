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
        private const int RENDER_TIME_SAMPLES = 7;

        private static string MAC_ADDRESS = GetMacAddress();

        // Static members
        private static DateTime m_dtLastUpdate = DateTime.Now;
        private static double m_latency = 0.0;
        private static Player[] m_players = new Player[0];
        private static bool m_readComplete = true;
        //private static Player m_self = new Player(0, 0);

        // Instance members
        private double[] m_renderTimes = new double[RENDER_TIME_SAMPLES];
        private int m_renderTimesIdx = 0;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            timerUpdateUI.Start();
            timerGetMap.Start();
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

        private string ExtractData(string strResponse)
        {
            // Get the part of the response before the end sign
            return strResponse.Substring(0, strResponse.IndexOf(RESPONSE_END_SIGN));
        }

        private void timerUpdateUI_Tick(object sender, EventArgs e)
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

            // Calculate the render time
            TimeSpan tsRenderTime = (DateTime.Now - dtStart);

            // Update the render time display
            m_renderTimes[m_renderTimesIdx++] = tsRenderTime.TotalSeconds;

            // Index overflow
            if (m_renderTimesIdx == RENDER_TIME_SAMPLES)
            {
                // Reset the index
                m_renderTimesIdx = 0;

                // Get the total render time
                double totalRenderTime = m_renderTimes.Sum();

                string strFps = string.Empty;

                // Avoid division by zero when the total render time is zero
                if (totalRenderTime > 0.0)
                {
                    strFps = (RENDER_TIME_SAMPLES / totalRenderTime).ToString("0");
                }
                else
                {
                    strFps = "infinite";
                }

                // Update the FPS counter
                labelFPS.Text = "FPS: " + strFps;
            }
        }

        private void timerGetMap_Tick(object sender, EventArgs e)
        {
            if (m_readComplete)
            {
                Task.Run(() => RequestRead());
            }
        }

        private void pictureBoxMap_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Send move request to the server
                Task.Run(() => RequestMove(e.X, e.Y));
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
            m_readComplete = false;

            // Put data into objects
            HTTP.PostData pdId = new HTTP.PostData("id", MAC_ADDRESS);
            HTTP.PostData pdAction = new HTTP.PostData("action", "read");

            // Get current time
            DateTime dtSendTime = DateTime.Now;
            // Send the request and process the output
            string strDataTmp = ExtractData(HTTP.GetPostResponse(SERVER_URL, pdId, pdAction));
            // Calculate the server response latency
            TimeSpan tsLatency = (DateTime.Now - dtSendTime);

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
                m_latency = tsLatency.TotalMilliseconds;
                m_players = tempPlayers;
            }

            m_readComplete = true;
        }
    }
}
