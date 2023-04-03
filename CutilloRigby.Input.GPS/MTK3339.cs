/*
Copyright 2010 Thomas W. Holtquist
www.skewworks.com
 * 
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.IO.Ports;
using System.Text;

using Microsoft.SPOT;

namespace raican.Sensors.GPS
{

    public class MTK3339
    {

        #region Variables

        private SerialPort _comm;

        private string _receivedDataFragment;

        private string _latitude;
        private double _mapLatitude;

        private string _longitude;
        private double _mapLongitude;

        private Fix _fixType;

        private double _speed;
        private double _course;

        private DateTime _date;

        #endregion

        #region Constructors
        public MTK3339(string COMPort)
        {
            // Create COM Port
            _comm = new SerialPort(COMPort, 9600);
            _comm.Open();

            SerialWrite(PMTK_API_SET_NMEA_OUTPUT_RMC);
            SerialWrite(PMTK_SET_NMEA_UPDATERATE_5HZ);

            _comm.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
        }

        #endregion

        #region Properties

        public double Course
        {
            get { return _course; }
        }
        public bool HasFix
        {
            get { return _fixType == Fix.Active; }
        }
        public Fix FixType
        {
            get { return _fixType; }
        }
        public string LatitudeDMS
        {
            get { return _latitude; }
        }
        public string LongitudeDMS
        {
            get { return _longitude; }
        }
        public double LatitudeDD
        {
            get { return _mapLatitude; }
        }
        public double LongitudeDD
        {
            get { return _mapLongitude; }
        }
        public double SpeedKnots
        {
            get { return _speed; }
        }
        public double SpeedMPH
        {
            get { return _speed * 1.15077945; }
        }
        public double SpeedKPH
        {
            get { return _speed * 1.852; }
        }
        public DateTime Date
        {
            get { return _date; }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Reads Serial data on DataReceived event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] _buffer;
            int _bytesRead;
            string _dataBuffer;
            string[] _line;
            string[] data;

            try
            {
                _buffer = new byte[_comm.BytesToRead];

                if (_buffer.Length == 0)
                    return;

                _bytesRead = _comm.Read(_buffer, 0, _buffer.Length);
                _dataBuffer = _receivedDataFragment + new string(UTF8Encoding.UTF8.GetChars(_buffer));

                _line = _dataBuffer.Split('\n'); // Split in to lines. LF is removed, CR remains.
                if (_line.Length == 0)
                    return;

                _receivedDataFragment = _line[_line.Length - 1]; // Last (incomplete) line is written back to buffer.

                for (int _l = 0; _l < _line.Length - 1; _l++)
                {
                    // Line should end in CR if valid NMEA, LF is removed in earlier split.
                    if (_line[_l].Substring(_line[_l].Length - 1) != "\r")
                        continue;
                    _line[_l] = _line[_l].Substring(0, _line[_l].Length - 1);
                    
                    if (_line[_l].Substring(_line[_l].Length - 3, 1) == "*") // Ignore (Assume correct) and Remove Checksum.
                        _line[_l] = _line[_l].Substring(0, _line[_l].Length - 3);

                    // Split line
                    data = _line[_l].Split(',');

                    // Proccess command
                    switch (data[0])
                    {
                        case "$PMTK001": // Ack, do nothing
                            break;
                        case "$GPRMC": // Recommended Minimum Data
                            ParseRMCData(data);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception)
            {
                Debug.Print("Parse error");
                _receivedDataFragment = string.Empty;
            }
        }

        private DateTime ParseTime(string Date, string UTC)
        {
            int _day;
            int _month;
            int _year;
            int _hour;
            int _minute;
            int _second;

            try
            {
                _day = int.Parse(Date.Substring(0, 2));
                _month = int.Parse(Date.Substring(2, 2));
                _year = 2000 + int.Parse(Date.Substring(4, 2));

                _hour = int.Parse(UTC.Substring(0, 2));
                _minute = int.Parse(UTC.Substring(2, 2));
                _second = int.Parse(UTC.Substring(4, 2));

                return new DateTime(_year, _month, _day, _hour, _minute, _second, 0);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
        private void ParseLatitude(string Value, string NSIndicator)
        {
            double decimalDegrees;
            string degreesMinutesSeconds;

            double _seconds = double.Parse(Value.Substring(4, 4));
            decimalDegrees = _seconds;
            decimalDegrees = (double.Parse(Value.Substring(2, 2)) + decimalDegrees) / 60;
            decimalDegrees = double.Parse(Value.Substring(0, 2)) + decimalDegrees;


            degreesMinutesSeconds = Value.Substring(0, 2) + "º ";
            degreesMinutesSeconds += Value.Substring(2, 2) + "' ";
            degreesMinutesSeconds += ((int)(_seconds * 60)).ToString() + "\" ";

            // Put negative if South
            if (NSIndicator == "S")
                decimalDegrees = -decimalDegrees;

            _latitude = degreesMinutesSeconds + NSIndicator;
            _mapLatitude = decimalDegrees;
        }
        private void ParseLongitude(string Value, string EWIndicator)
        {
            double decimalDegrees;
            string degreesMinutesSeconds;

            double _seconds = double.Parse(Value.Substring(5, 4));
            decimalDegrees = _seconds;
            decimalDegrees = (double.Parse(Value.Substring(3, 2)) + decimalDegrees) / 60;
            decimalDegrees = double.Parse(Value.Substring(0, 3)) + decimalDegrees;


            degreesMinutesSeconds = Value.Substring(0, 3) + "º ";
            degreesMinutesSeconds += Value.Substring(3, 2) + "' ";
            degreesMinutesSeconds += ((int)(_seconds * 60)).ToString() + "\" ";

            // Put negative if South
            if (EWIndicator == "W")
                decimalDegrees = -decimalDegrees;

            _longitude = degreesMinutesSeconds + EWIndicator;
            _mapLongitude = decimalDegrees;
        }
        private void ParseRMCData(string[] Data)
        {
            if (Data.Length != 13)
                return;

            // Get Date/Time of Fix
            _date = ParseTime(Data[9], Data[1]);

            // Is fix valid?
            _fixType = Data[2] == "A" ? Fix.Active : Fix.Void;

            if (!HasFix) //If No Fix, exit.
                return;

            // Latitude
            if (Data[3] != string.Empty && Data[4] != string.Empty)
                ParseLatitude(Data[3], Data[4]);

            // Longitude
            if (Data[5] != string.Empty && Data[6] != string.Empty)
                ParseLongitude(Data[5], Data[6]);

            // Speed (in Knots)
            if (Data[7] != string.Empty)
                _speed = double.Parse(Data[7]);

            // Course (in degrees)
            if (Data[8] != string.Empty)
                _course = double.Parse(Data[8]);
        }

        private void SerialWrite(string Value)
        {
            byte[] _buffer = UTF8Encoding.UTF8.GetBytes(Value);
            _comm.Write(_buffer, 0, _buffer.Length);
        }

        #endregion

        #region Constants
        private const string PMTK_SET_NMEA_UPDATERATE_1HZ = "$PMTK220,1000*1F\r\n";
        private const string PMTK_SET_NMEA_UPDATERATE_5HZ = "$PMTK220,200*2C\r\n";
        private const string PMTK_SET_NMEA_UPDATERATE_10HZ = "$PMTK220,100*2F\r\n";

        private const string PMTK_API_SET_NMEA_OUTPUT_RMC = "$PMTK314,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0*29\r\n";
        private const string PMTK_API_SET_NMEA_OUTPUT_RMCGGA = "$PMTK314,0,1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0*28\r\n";
        private const string PMTK_API_SET_NMEA_OUTPUT_ALL = "$PMTK314,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0*28\r\n";
        private const string PMTK_API_SET_NMEA_OUTPUT_NONE = "$PMTK314,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0*28\r\n";

        #endregion

        #region Enumerations
        public enum Fix
        {
            Void = 0,
            Active = 1,
        }

        #endregion

    }
}
