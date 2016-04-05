using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodesDecoder
{
    public class Telemetry
    {
        public static string BARCODE_SCANNED = "events/BARCODE_SCANNED";
        public static string BARCODE_DECODED = "events/BARCODE_DECODED";
        public static string REVERTING_BARCODE_DEFINITIONS_TO_EN = "events/REVERTING_BARCODE_DEFINITIONS_TO_EN";


		public static string BARCODE_SCAN_FAILED = "BARCODE_SCAN_FAILED";
		public static string CAPTURE_DEVICE_INIT_FAILED = "CAPTURE_DEVICE_INIT_FAILED";
		public static string START_CAPTURE_FAILED = "START_CAPTURE_FAILED";
		public static string FOCUSING_FAILED = "FOCUSING_FAILED";
		public static string READING_BARCODE_DEFINITIONS_FAILED = "READING_BARCODE_DEFINITIONS_FAILED";

	}
}
