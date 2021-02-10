﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StadevParser.Models
{
    public class PCInfoModel
    {
        public string Type { get; set; }
        public string OperatingSystem { get; set; }
        public int Number { get; set; }
        public string HardwareType { get; set; }
        public string SoftwareVersion { get; set; }
        public string PrimaryIP { get; set; }
        public string ReceiptPrinter { get; set; }
        public string CustomerDisplay { get; set; }
        public string BarcodeScanner { get; set; }
        public string LevelGauge { get; set; }
        public string TouchScreen { get; set; }
        public int NumSerialPorts { get; set; }
        public List<SerialDeviceModel> SerialDevices { get; set; }
        public string UPS { get; set; }
        public int SerialPortsUsed 
        {
            get 
            {
                int numDevices = SerialDevices
                    .Where(number => number.Device != "Unknown")
                    .Count();

                return numDevices;
            } 
        }
    }
}
