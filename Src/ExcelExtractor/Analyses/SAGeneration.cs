using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using LinqExtensions.Extensions;
using NCalcExcel;

namespace ExcelExtractor.Analyses {
    public static class SAGeneration {
        public static void CreateSArun(string cellMapFileName, string IExcelFile, string fileName) {
            //return this.Sheet + "\t" + this.Cell + "\t" + this.FriendlyName + "\t" + this.HighFormula + "\t" + this.LowFormula + "\t" + this._defaultvalue;
            CellMap[] cells = CellMap.ReadTSV(cellMapFileName);
            if (cells != null) {
                // then we can dump them all to a SA file.
                StringBuilder sb = new StringBuilder();

                // header
                sb.AppendLine("Excel Sensitivity Analysis Save file");
                sb.AppendLine("App Created by David Birch");
                sb.AppendLine("You may add your own content on any rows outside of the BEGIN END blocks");
                sb.AppendLine("You may add extra columns to the VarMappingsTable ONLY");
                sb.AppendLine("When you save the file as CSV please ensure that you use tabs to delimit");
                sb.AppendLine("Config(BEGIN)");
                sb.AppendLine("IExcelFile\t" + IExcelFile);
                sb.AppendLine("Config(END)");
                // writes
                sb.AppendLine("WriteValuesTable(BEGIN)");
                sb.AppendLine("Sheet\tCell\tExpression");
                foreach (CellMap cell in cells) {
                    sb.AppendLine(cell.Sheet + "\t" + cell.Cell + "\t" + cell.FriendlyName);
                }

                sb.AppendLine("WriteValuesTable(END)");
                // varmapping
                sb.AppendLine("VarMappingsTable(BEGIN)");
                sb.AppendLine("Variable Names\tIn SA\tDefault\tLow\tHigh");
                foreach (CellMap cell in cells) {
                    sb.AppendLine(cell.FriendlyName + "\t" + "YES" + "\t" + cell.AccessDefaultValue() + "\t" +
                                  cell.LowFormula + "\t" + cell.HighFormula);
                }

                sb.AppendLine("VarMappingsTable(END)");
                // reads - empty
                sb.AppendLine("ReadValuesTable(BEGIN)");
                sb.AppendLine("Output Name\tSheet\tCell");
                sb.AppendLine("ReadValuesTable(END)");


                File.WriteAllText(fileName, sb.ToString());

            }
        }

        private static Dictionary<string, XElement> EndCells = new Dictionary<string, XElement>();

        private static void FindEndCellsFromTree(XElement root) {
            if (!root.HasElements) {
                string key = root.AttributeValue("Name");
                if (!EndCells.ContainsKey(key)) {
                    EndCells.Add(key, root);
                }
            }
            else {
                foreach (XElement element in root.Elements()) {
                    FindEndCellsFromTree(element);
                }
            }
        }
        
        private static void OldCreateSACode(string cellMapFileName, string excelFile, string fileName, XElement root,
            bool includeUnMappedCells,ExpressionFactory factory) {

            //return this.Sheet + "\t" + this.Cell + "\t" + this.FriendlyName + "\t" + this.HighFormula + "\t" + this.LowFormula + "\t" + this._defaultvalue;
            CellMap[] mappings = CellMap.ReadTSV(cellMapFileName);
            if (mappings != null) {
                FindEndCellsFromTree(root);

                // now we go through and create cell maps for new ones also... 
                List<CellMap> mappedCells = new List<CellMap>();
                List<CellMap> unMappedCells = new List<CellMap>();
                List<int> usedMaps = new List<int>();

                foreach (KeyValuePair<string, XElement> endCell in EndCells) {
                    string endcellName = endCell.Key;
                    string value = endCell.Value.AttributeValue("Value");
                    if (value == "" || value == "-") {
                        value = "0";
                    }

                    int foundindex = mappings.FirstIndexOf(map => map.GetName().Equals(endcellName));
                    CellMap found = foundindex == -1 ? null : mappings[foundindex];
                    usedMaps.Add(foundindex);

                    if (found != null) {
                        found.PutDefaultValue(value);
                        mappedCells.Add(found);
                    }
                    else {
                        CellMap cellMap = new CellMap();
                        string cellname = endCell.Value.AttributeValue("Name");
                        cellMap.Sheet = cellname.Split('!')[0];
                        cellMap.Cell = cellname.Split('!')[1];

                        cellMap.FriendlyName = "UNKNOWN_" + cellname;
                        cellMap.LowFormula = "value";
                        cellMap.HighFormula = "value";
                        cellMap.PutDefaultValue(value);

                        unMappedCells.Add(cellMap);
                    }
                }

                // save any unused maps
                {
                    List<CellMap> unusedMappings = new List<CellMap>();
                    for (int i = 0; i < mappings.Length; i++) {
                        if (!usedMaps.Contains(i)) {
                            unusedMappings.Add(mappings[i]);
                        }
                    }

                    StringBuilder asb = new StringBuilder();
                    foreach (CellMap unusedMapping in unusedMappings) {
                        asb.AppendLine(unusedMapping.ToTSV());
                    }

                    File.WriteAllText("UnusedMappings.txt", asb.ToString());
                }

                // save unmapped cells seperately                    
                StringBuilder bsb = new StringBuilder();
                foreach (CellMap unusedMapping in unMappedCells) {
                    bsb.AppendLine(unusedMapping.ToTSV());
                }

                File.WriteAllText("CellsNotInMapping.txt", bsb.ToString());

                List<CellMap> allcells = new List<CellMap>();
                allcells.AddRange(mappedCells);
                if (includeUnMappedCells) {
                    allcells.AddRange(unMappedCells);
                }

                // then we can dump them all to a SA file.  // evaling high / low expr's. 
                StringBuilder sb = new StringBuilder();

                // header
                sb.AppendLine("Excel Sensitivity Analysis Save file");
                sb.AppendLine("App Created by David Birch");
                sb.AppendLine("You may add your own content on any rows outside of the BEGIN END blocks");
                sb.AppendLine("You may add extra columns to the VarMappingsTable ONLY");
                sb.AppendLine("When you save the file as CSV please ensure that you use tabs to delimit");
                sb.AppendLine("Config(BEGIN)");
                sb.AppendLine("IExcelFile\t" + excelFile);
                sb.AppendLine("Config(END)");
                // writes
                sb.AppendLine("WriteValuesTable(BEGIN)");
                sb.AppendLine("Sheet\tCell\tExpression");
                foreach (CellMap cell in allcells) {
                    sb.AppendLine(cell.Sheet + "\t" + cell.Cell + "\t" + cell.FriendlyName);
                }

                sb.AppendLine("WriteValuesTable(END)");
                // varmapping
                sb.AppendLine("VarMappingsTable(BEGIN)");
                sb.AppendLine("Variable Names\tIn SA\tDefault\tLow\tHigh");
                foreach (CellMap cell in allcells) {
                    sb.AppendLine(cell.FriendlyName + "\t" + "YES" + "\t" + cell.AccessDefaultValue() + "\t" +
                                  cell.EvalLow(factory) + "\t" + cell.EvalHigh(factory));
                }

                sb.AppendLine("VarMappingsTable(END)");
                // reads - empty
                sb.AppendLine("ReadValuesTable(BEGIN)");
                sb.AppendLine("Output Name\tSheet\tCell");
                sb.AppendLine("ReadValuesTable(END)");


                File.WriteAllText(fileName, sb.ToString());

            }
        }
    }

}

