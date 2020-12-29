namespace ExcelFunctionExtractor {
    class OldCode {

        /*
         * old extraction routine
         *
             private void BeginExecution() {
           while (this.WorkElements.Count>0) {
           XElement element = this.WorkElements.Pop();
           XAttribute unproccessed = element.Attribute("Unprocessed");
           if (unproccessed != null) {
           unproccessed.Remove();
           }
           
           // ReSharper disable PossibleNullReferenceException
           ExcelAddress address = new ExcelAddress(element.Attribute("Name").Value);
           // ReSharper restore PossibleNullReferenceException
           
           // deal with ranges... 
           
           if (address.CellName.Contains(":")) { // deal with ranges
           List<string> addresses = ExcelAddress.ExpandRangeToCellList(address);
           AddReferences(element, address, addresses);
           element.Add(new XAttribute("Formula", "RANGE"));
           } else {
           
           string formula;
           
           try {
           formula = this.ExcelObject.ReadFormula(address);
           // test it is a valid cell name
           new CellName(address.CellName);
           } catch (ArgumentException) { // its a named cell
           try {
           element.Add(new XAttribute("KnownAs", address.CellName));
           address = this.ExcelObject.FindName(address.CellName);
           //element.Add(new XAttribute("NameGivenByParent", element.Attribute("Name").Value));
           element.SetAttributeValue("Name", address);
           formula = this.ExcelObject.ReadFormula(address);
           } catch (Exception e) {
           throw e;
           }
           }                    
           element.Add(new XAttribute("Value", this.ExcelObject.ReadValue(address)));
           if (element.Attribute("KnownAs") == null) {
           string givenName = this.ExcelObject.ReadGivenName(address);
           if (!string.IsNullOrEmpty(givenName)) {
           element.Add(new XAttribute("KnownAs", givenName));
           }
           }
           
           if (string.IsNullOrWhiteSpace(formula)) {
           element.Add(new XAttribute("Formula", "BLANK"));
           } else {
           string sanitisedFormula = ExcelFormula.SanitiseExcelFormula(formula, new List<Tuple<string,string>>());
           element.Add(new XAttribute("Formula", sanitisedFormula));
           
           if (formula.Contains("=") && formula[0] == '=') {
           element.Add(new XAttribute("IsFormula","true"));
           List<string> references = this.Factory.ExtractVars(sanitisedFormula);
           AddReferences(element, address, references);
           }
           }
           }
           }    
           
           // finished - clean up and present results and show it to the user
           this.ExcelObject.Close(false);
           
           this.BtnRefreshClick(this, null);
           
           // now lets add the links - now that they have been calculated.  
           
           AddChildren(this.Tree.DataSource as XElement);            
           
           // ReSharper disable PossibleNullReferenceException
           IEnumerable<XElement> errors = this.KnownCells.Where(pair => pair.Value.Attribute("Formula")== null || pair.Value.Attribute("Formula") != null && pair.Value.Attribute("Formula").Value == "ERROR" ).Select( pair => pair.Value);
           
           MessageBox.Show("Finished - found # " + this.KnownCells.Count +" errors: "+errors.Count());
           
           this.BtnRefreshClick(this, null);
           
           ValidateTree(this.Tree.DataSource as XElement);
           
           MessageBox.Show("validated tree");
           
           if (errors.Count() != 0) {
           MessageBox.Show("ERR");
           } else {
           // print out some results!
           
           // all known cells
           XElement root = new XElement("KnownCells");
           foreach (KeyValuePair<string, XElement> knownCell in this.KnownCells.Where(pair => !pair.Value.Attribute("Formula").Value.Equals("RANGE"))) {                    
           root.Add(new XElement("CELL",knownCell.Value.Attributes()));
           }
           root.Save(GetBasePath(this.Tree.DataSource as XElement)+"_Elements.xml");
           
           // print the whole tree                
           (this.Tree.DataSource as XElement).Save(GetBasePath(this.Tree.DataSource as XElement) + "_FormulaTree.xml");
           
           // try to extract all end values
           root = new XElement("EndValues");
           foreach (KeyValuePair<string, XElement> knownCell in 
           this.KnownCells.Where(pair => !pair.Value.Attribute("Formula").Value.Equals("RANGE")
           && (pair.Value.Attribute("IsFormula") == null
           || !pair.Value.HasElements))) {
           root.Add(new XElement("CELL", knownCell.Value.Attributes()));
           }
           root.Save(GetBasePath(this.Tree.DataSource as XElement) + "_EndValues.xml");
           
           MessageBox.Show(string.Format("Result is {0}", EvaluateCell(this.Tree.DataSource as XElement)));
           
           // now lets write out a CSV for input to excel....
           
           //  <CELL NameGivenByParent="H14" Name="Canary!H14" Value="0" KnownAs="Zone10_population" Formula="0" />./*
           /* this.Sheet = values[0];
           this.Cell = values[1];
           this.FriendlyName = values[2];            
           this.LowFormula = values[3];
           this.HighFormula = values[4];
           * /
           
           var cells = from cell in root.Elements()
           select new {
           Sheet = cell.AttributeValue("Name").Split(new[] {'!'})[0],
           Cell  = cell.AttributeValue("Name").Split(new[] {'!'})[1],
           FriendlyName = cell.AttributeValue("KnownAs"),
           LowFormula = "low",
           HighFormula = "high",
           Value = cell.AttributeValue("Value"),
           Formula = "'"+cell.AttributeValue("Formula")
           };
           
           String file = cells.Aggregate("Sheet,Cell,FriendlyName,LowValue,HighValue,Value,Formula",
           (acc, cell) => string.Format("{0}\n{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}",
           acc, cell.Sheet, cell.Cell, cell.FriendlyName, cell.LowFormula,
           cell.HighFormula, cell.Value, cell.Formula));
           
           File.WriteAllText(GetBasePath(this.Tree.DataSource as XElement) + "_Sensitivity_Setup.csv",file);
           
           
           }
           // ReSharper restore PossibleNullReferenceException
           }
           
         */

        /*
         *      private object EvaluateCell(XElement cell) {
            //try to make the entire formula :)
            gccount = 0;
            this.VariableMap = new ConcurrentDictionary<string, object>();
            string theFormula = GetFormula(cell,0,false);            
            
            File.WriteAllText(GetBasePath(cell)+"_formula.txt", theFormula.Replace("\n", ""));            
            GC.Collect();
            MessageBox.Show(string.Format("created formula - internal var map contains {0} items", this.VariableMap.Count));

            Dictionary<string,object> parammap = this.VariableMap.ToDictionary(valuePair => valuePair.Key, valuePair => valuePair.Value);
            object result = this.Factory.Evaluate(GetFormula(cell, 0, true), parammap);
            if (cell.HasAttribute("CalculatedAs")) {
                cell.RemoveAttribute("CalculatedAs");
            }
            cell.Add(new XAttribute("CalculatedAs", result.ToString()));

            GC.Collect();
            return result;
        }

               private string GetBasePath(XElement cell) {
            return Path.ChangeExtension(this.ExcelFileAddress.Text, null) + "_" + cell.AttributeValue("Name").Replace("!", "_").Replace(":", "_");
        }

         */
    }
}
