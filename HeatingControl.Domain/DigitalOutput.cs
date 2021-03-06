﻿using System;

namespace Domain
{
    public class DigitalOutput
    {
        public int DigitalOutputId { get; set; }

        public string ProtocolName { get; set; }

        public string OutputDescriptor { get; set; }

        [Obsolete]
        public int DeviceId { get; set; }
    }
}
