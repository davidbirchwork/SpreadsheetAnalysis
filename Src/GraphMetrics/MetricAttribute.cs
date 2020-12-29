using System;
using System.ComponentModel.Composition;

namespace GraphMetrics {
    /// <summary>
    /// The Meta data interface used for MEF binding - implemented by <see cref="MetricAttribute"/>
    /// </summary>
    public interface IMetricAttribute {

        /// <summary>
        /// Gets the name of the metric.
        /// </summary>
        /// <value>The name of the metric.</value>
        string MetricName { get; }

        /// <summary>
        /// Gets the metric description.
        /// </summary>
        /// <value>The metric description.</value>
        string MetricDescription { get; }        

    }

    // ReSharper disable UnusedMember.Global
    /// <summary>
    /// This Attribute is for annotating <see cref="IMetricAttribute"/> definitions for MEF export
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MetricAttribute : ExportAttribute, IMetricAttribute {
        public string MetricName { get; set; }
        public string MetricDescription { get; set; }

        public MetricAttribute(string name, string description)
            : base(typeof(IMetric)) {
            this.MetricName = name;
            this.MetricDescription = description;
        }
    }
}