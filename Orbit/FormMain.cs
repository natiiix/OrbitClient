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
        private const string SERVER_URL = "http://orbit.6f.sk";
        private const string RESPONSE_END_SIGN = "<!--END-->";
        private const char DATA_SEPARATOR = ';';
        private const int MAP_WIDTH = 500;
        private const int MAP_HEIGHT = 400;

        private static string MAC_ADDRESS = GetMacAddress();

        private static DateTime dtLastUpdate = DateTime.Now;
        private static string strLatency = "Latency";
        private static Bitmap bmpMap;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            timerUpdate.Start();
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

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            labelLatency.Text = strLatency;
            pictureBoxMap.Image = bmpMap;
        }

        private void timerGetMap_Tick(object sender, EventArgs e)
        {
            Task.Run(() => UpdateMap());
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
            return (
                from nic in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
                where nic.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up
                select nic.GetPhysicalAddress().ToString()
            ).FirstOrDefault();
        }

        private void UpdateMap()
        {
            string strData = RequestRead();

            if (strData == null)
            {
                return;
            }

            Bitmap mapTmp = new Bitmap(MAP_WIDTH, MAP_HEIGHT);

            using (Graphics g = Graphics.FromImage(mapTmp))
            {
                g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(0, 0, MAP_WIDTH, MAP_HEIGHT));
            }

            string[] rows = strData.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string strRow in rows)
            {
                string[] coordinates = strRow.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                if (coordinates.Length == 2)
                {
                    if (int.TryParse(coordinates[0], out int x) && int.TryParse(coordinates[1], out int y))
                    {
                        mapTmp.SetPixel(x, y, Color.White);
                    }
                }
            }

            if (bmpMap != null)
            {
                bmpMap.Dispose();
            }

            bmpMap = mapTmp;
        }

        private string RequestRead()
        {
            // Put data into objects
            HTTP.PostData pdId = new HTTP.PostData("id", MAC_ADDRESS);
            HTTP.PostData pdAction = new HTTP.PostData("action", "read");

            // Get current time
            DateTime dtSendTime = DateTime.Now;
            // Send the request and process the output
            string strDataTmp = ExtractData(HTTP.GetPostResponse(SERVER_URL, pdId, pdAction));
            // Calculate the server response latency
            TimeSpan tsLatency = (DateTime.Now - dtSendTime);

            string strLatencyTemp = "Latency: " + tsLatency.TotalMilliseconds.ToString("0") + " ms";

            // Make sure to use the most recent data
            // When data from older request than the last handled one returns ignore it
            if (dtLastUpdate < dtSendTime)
            {
                dtLastUpdate = dtSendTime;
                strLatency = strLatencyTemp;
                return strDataTmp;
            }
            // Data are outdated
            else
            {
                return null;
            }
        }
    }
}
