using Graph.Compound;

namespace GraphGUI.Compound.ColourGraph {
    /// <summary>
    /// A compound vertex that fixes its colour and contains a value
    /// </summary>
    public class CompoundTaggedVertex : CompoundVertex{
        
        public object Tag { get; set; }        

        public CompoundTaggedVertex(string id,object tag,string realID) : base(id,realID) {            
            this.Tag = tag;            
            this._color = "#ADD8E6";

        }

        private string _color;
        public override string Colour {
            get { return this._color; }
            set {
                this._color = value;
                NotifyPropertyChanged("Colour");
            }
        }
    }
}
