using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ClosedXML.Excel;
using ExcelInterop.Domain;

namespace Excel_Interop_ClosedXML.SpreadUtil
{
    public static class SpreadColourer
    {
        private class GroupColours {
            public XLColor NextColor() {
                _pointer = (_pointer + 1) % _colours.Count;
                return _colours[_pointer];
            }

            private int _pointer = -1;
            private readonly List<XLColor> _colours = new List<XLColor>() {
                // from color brewer
                XLColor.FromArgb(247, 251, 255),
                XLColor.FromArgb(222, 235, 247),
                XLColor.FromArgb(198, 219, 239),
                XLColor.FromArgb(158, 202, 225),
                XLColor.FromArgb(107, 174, 214),
                XLColor.FromArgb(66, 146, 198),
                XLColor.FromArgb(33, 113, 181),
                XLColor.FromArgb(8, 81, 156),
                XLColor.FromArgb(8, 48, 107),
                XLColor.FromArgb(229, 245, 224),
                XLColor.FromArgb(199, 233, 192),
                XLColor.FromArgb(161, 217, 155),
                XLColor.FromArgb(116, 196, 118),
                XLColor.FromArgb(65, 171, 93),
                XLColor.FromArgb(35, 139, 69),
                XLColor.FromArgb(0, 109, 44),
                XLColor.FromArgb(0, 68, 27),
                XLColor.FromArgb(240, 240, 240),
                XLColor.FromArgb(217, 217, 217),
                XLColor.FromArgb(189, 189, 189),
                XLColor.FromArgb(150, 150, 150),
                XLColor.FromArgb(115, 115, 115),
                XLColor.FromArgb(82, 82, 82),
                XLColor.FromArgb(37, 37, 37),
                XLColor.FromArgb(0, 0, 0),
                XLColor.FromArgb(254, 230, 206),
                XLColor.FromArgb(253, 208, 162),
                XLColor.FromArgb(253, 174, 107),
                XLColor.FromArgb(253, 141, 60),
                XLColor.FromArgb(241, 105, 19),
                XLColor.FromArgb(217, 72, 1),
                XLColor.FromArgb(166, 54, 3),
                XLColor.FromArgb(127, 39, 4),
                XLColor.FromArgb(239, 237, 245),
                XLColor.FromArgb(218, 218, 235),
                XLColor.FromArgb(188, 189, 220),
                XLColor.FromArgb(158, 154, 200),
                XLColor.FromArgb(128, 125, 186),
                XLColor.FromArgb(106, 81, 163),
                XLColor.FromArgb(84, 39, 143),
                XLColor.FromArgb(63, 0, 125)
            };
        }

        public static void Colour(string filename, string newFilename, Dictionary<string, List<ExcelAddress>> groupsDictionary) {
            if (!File.Exists(filename)) throw new ArgumentNullException(nameof(filename), "Cannot access file");
            if (File.Exists(newFilename)) File.Delete(newFilename);
            File.Copy(filename, newFilename);
             Thread.Sleep(250);
                
            var wb = new XLWorkbook(newFilename);
            var colourBook = wb.Worksheets.Add("ColourKey");

            int r = 1;

            GroupColours colourer = new GroupColours();
            foreach (var group in groupsDictionary.OrderBy(a=>a.Value.First().WorkSheet)) { // try an order so colours do not overlap on worksheet... 
                var colour = colourer.NextColor();

                colourBook.Cell(r, 1).Value = "'" + group.Key;
                colourBook.Cell(r, 1).Style.Fill.BackgroundColor = colour;
                colourBook.Cell(r, 2).Style.Fill.BackgroundColor = colour;

                string formula = "=sum(";
                foreach (var address in group.Value) {
                    if (address.IsRange()) {
                        foreach (var cell in ExcelAddress.ExpandRangeToExcelAddresses(address)) {
                            formula = ColourCell(formula, cell.ToString(), wb, colour);
                        }
                    }
                    else {
                        formula = ColourCell(formula, address.ToString(), wb, colour);
                    }
                }

                formula += ")";

                colourBook.Cell(r, 2).FormulaA1 = formula;

                r++;
            }

            wb.Save();
        }

        private static string ColourCell(string formula, string cellAddress, XLWorkbook wb, XLColor colour) {
            formula += cellAddress + ",";
            var cell = wb.CellFromFullAddress(cellAddress, out var _);
            cell.Style.Fill.BackgroundColor = colour;
            return formula;
        }
    }
}
