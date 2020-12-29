using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.Generic;
using System.Xml.Serialization;
using ExcelInterop.Domain;

namespace Graph {

	/// <summary>
	/// A simple identifiable vertex.
	/// </summary>
	[DebuggerDisplay( "{ID}" )]
	public class ExcelVertex  : INotifyPropertyChanged {

		public string ID {
			get;
			private set;
		}

	    private ExcelAddress _addr;
	    public ExcelAddress Address => _addr ?? (_addr = new ExcelAddress(Sheet, Cell));

	    [XmlAttribute("Sheet", Namespace = "http://graphml.graphdrawing.org/xmlns")]
        public string Sheet { get; set; }
	    [XmlAttribute("Cell", Namespace = "http://graphml.graphdrawing.org/xmlns")]
        public string Cell { get; set; }
	    [XmlAttribute("Row", Namespace = "http://graphml.graphdrawing.org/xmlns")]
        public int Row { get; set; }
	    [XmlAttribute("Col", Namespace = "http://graphml.graphdrawing.org/xmlns")]
        public int Col { get; set; }
	    [XmlAttribute("Formula", Namespace = "http://graphml.graphdrawing.org/xmlns")]
        public string Formula { get; set; }
	    [XmlAttribute("Value", Namespace = "http://graphml.graphdrawing.org/xmlns")]
        public string Value  { get; set; }
	    [XmlAttribute("ValueEval", Namespace = "http://graphml.graphdrawing.org/xmlns")]
        public string ValueEval { get; set; }
	    [XmlAttribute("isBlank", Namespace = "http://graphml.graphdrawing.org/xmlns")]
        public bool isBlank { get; set; }
	    [XmlAttribute("isRange", Namespace = "http://graphml.graphdrawing.org/xmlns")]
        public bool isRange { get; set; }
	    [XmlAttribute("isFormula", Namespace = "http://graphml.graphdrawing.org/xmlns")]
        public bool isFormula { get; set; }
	    [XmlAttribute("RangeSize", Namespace = "http://graphml.graphdrawing.org/xmlns")]
        public int RangeSize { get; set; }
	    [XmlAttribute("Type", Namespace = "http://graphml.graphdrawing.org/xmlns")]
        public string Type { get; set; }

		public virtual string Caption { get { return " "+ID.Trim()+" "; }  }

		public ExcelVertex( string id ) {
			this.ID = id;
		}

		public virtual string GetColourID() {
			return this.ID.Split(new[] {'!'})[0];
		}

		public virtual string Colour {
			get { return GetColour(this.GetColourID()); }
			set {  }
		}

		private string _highlightFontSize = "12";
		public string HighlightFontSize {
			get { return this._highlightFontSize; }
			set { this._highlightFontSize = value;
			NotifyPropertyChanged("HighlightFontSize");
			}
		}

		//public string MetaData { get; set; }

		public override string ToString()
		{
			return ID;
		}

		public static readonly List<string> RemainingColours = new List<string> { "Red", "GreenYellow", "Yellow", "CornflowerBlue", "Coral", "Gold", "Bisque", "LightPink", "Silver", "SeaGreen", "Aquamarine", "HotPink", "LightSeaGreen", "Beige", "CadetBlue", "Lavender", "PaleTurquoise", "Plum", "MediumSpringGreen", "LightGoldenrodYellow" };

		public static readonly Dictionary<string, string> ColourMap = new Dictionary<string, string>();
		

		public static string GetColour(string value) {
			if (!ColourMap.ContainsKey(value)) {
				if (RemainingColours.Count == 0)
					return "Red";
				ColourMap.Add(value, RemainingColours[0]);
				RemainingColours.RemoveAt(0);
			} 
			return ColourMap[value];
		}

		#region Implementation of INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		protected void NotifyPropertyChanged(string info) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(info));
		}


		#endregion
	}
}