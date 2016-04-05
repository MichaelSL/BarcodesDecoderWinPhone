using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodesDecoder
{
    public class Telemetry
    {
        public const string BARCODE_SCANNED = "events/BARCODE_SCANNED";
        public const string BARCODE_DECODED = "events/BARCODE_DECODED";
        public const string REVERTING_BARCODE_DEFINITIONS_TO_EN = "events/REVERTING_BARCODE_DEFINITIONS_TO_EN";


		public const string BARCODE_SCAN_FAILED = "BARCODE_SCAN_FAILED";
		public const string CAPTURE_DEVICE_INIT_FAILED = "CAPTURE_DEVICE_INIT_FAILED";
		public const string START_CAPTURE_FAILED = "START_CAPTURE_FAILED";
		public const string FOCUSING_FAILED = "FOCUSING_FAILED";
		public const string READING_BARCODE_DEFINITIONS_FAILED = "READING_BARCODE_DEFINITIONS_FAILED";
		public const string APPLICATION_STOP_ERROR = "APPLICATION_STOP_ERROR";
	}
}
