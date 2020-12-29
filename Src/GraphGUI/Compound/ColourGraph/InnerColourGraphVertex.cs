using System.Collections.Generic;
using System.Linq;
using Graph;

namespace GraphGUI.Compound.ColourGraph {
    public class InnerColourGraphVertex : ExcelVertex {
        private readonly Dictionary<string, double> _graphValues;
        public string _value;
        public new string Value {
            get { return "   " + this._value.Trim() + "   "; }
            set { this._value = value; }
        }
        public object _tag;
        public object Tag {
            get { return "   " + this._tag.ToString().Trim() + "   "; }
            set { this._tag = value; }
        }

        public InnerColourGraphVertex(string id,string value,Dictionary<string,double> graphValues,object tag) : base(id) {
            this.Value = value;
            this.Tag = tag;
            this._graphValues = graphValues;
            this.Value1 = 10;
            this.Value2 = 20;
            this.Value3 = 30;
            this.Value4 = 40;
            this.Value5 = 50;
        }

        private string _color;
        public override string Colour {
            get { return this._color; }
            set {
                this._color = value;
                NotifyPropertyChanged("Colour");
            }
        }

        public void SetValuesTo(List<string> graphValues) {
            string key1 = GetNext(graphValues);
            this.Value1 = key1 != null ? this._graphValues[key1] : 0;
            string key2 = GetNext(graphValues);
            this.Value2 = key2 != null ? this._graphValues[key2] : 0;
            string key3 = GetNext(graphValues);
            this.Value3 = key3 != null ? this._graphValues[key3] : 0;
            string key4 = GetNext(graphValues);
            this.Value4 = key4 != null ? this._graphValues[key4] : 0;
            string key5 = GetNext(graphValues);
            this.Value5 = key5 != null ? this._graphValues[key5] : 0;
        }

        private string GetNext(List<string> graphValues) {
            string key = null;
            bool stop = graphValues.Count() == 0;
            while (!stop) {
                stop = graphValues.Count() == 0;
                key = null;
                if (!stop) {
                    key = graphValues[0];
                    graphValues.RemoveAt(0);
                    //key = graphValues.Take(1).FirstOrDefault();
                    stop = this._graphValues.ContainsKey(key);
                }                
            }

            return key;

        }

        #region graphbars

        private double _value1;
        public double Value1 {
            get { return _value1; }
            set {
                _value1 = value;
                NotifyPropertyChanged("Value1");
            }
        }

        private double _value2;
        public double Value2 {
            get { return _value2; }
            set {
                _value2 = value;
                NotifyPropertyChanged("Value2");
            }
        }
        private double _value3;
        public double Value3 {
            get { return _value3; }
            set {
                _value3 = value;
                NotifyPropertyChanged("Value3");
            }
        }
        private double _value4;
        public double Value4 {
            get { return _value4; }
            set {
                _value4 = value;
                NotifyPropertyChanged("Value4");
            }
        }
        private double _value5;
        public double Value5 {
            get { return _value5; }
            set {
                _value5 = value;
                NotifyPropertyChanged("Value5");
            }
        }


        #endregion
    }
}
