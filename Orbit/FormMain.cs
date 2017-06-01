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
using NatiTools.xCSharp;

namespace Orbit
{
    public partial class FormMain : Form
    {
        private const string SERVER_URL = "http://orbit.6f.sk";
        private const string RESPONSE_END_SIGN = "<!--END-->";
        private const char DATA_SEPARATOR = ';';

        private static string strOutput = string.Empty;
        private static string strLatency = "Latency";
        private static DateTime dtLastUpdate = DateTime.Now;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            timerUpdate.Start();
        }

        private void UpdateTranslation()
        {
            // Put data into objects
            HTTP.PostData pdType = new HTTP.PostData("type", "send");
            HTTP.PostData pdData = new HTTP.PostData("data", StringTools.Base64Encode(textBoxInput.Text));

            // Get current time
            DateTime dtSendTime = DateTime.Now;
            // Send the request and process the output
            string strOutputTemp = string.Join("\n", ExtractData(HTTP.GetPostResponse(SERVER_URL, pdType, pdData)));
            // Calculate the server response latency
            TimeSpan tsLatency = (DateTime.Now - dtSendTime);

            string strLatencyTemp = "Latency: " + tsLatency.TotalMilliseconds.ToString("0") + " ms";

            // Make sure to use the most recent data
            // When data from older request than the last handled one returns ignore it
            if (dtLastUpdate < dtSendTime)
            {
                dtLastUpdate = dtSendTime;

                strOutput = strOutputTemp;
                strLatency = strLatencyTemp;
            }
        }

        private string[] ExtractData(string strResponse)
        {
            // Get the part of response before the end sign
            string strData = strResponse.Substring(0, strResponse.IndexOf(RESPONSE_END_SIGN));
            // Decode response string from Base64
            string strDecoded = StringTools.Base64Decode(strData);
            // Split the data by separator character and return the resulting string array
            return strDecoded.Split(new char[] { DATA_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            textBoxOutput.Text = strOutput;
            labelLatency.Text = strLatency;
        }

        private void textBoxInput_TextChanged(object sender, EventArgs e)
        {
            Task.Run(() => UpdateTranslation());
        }
    }
}
