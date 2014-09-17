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
    }
}
