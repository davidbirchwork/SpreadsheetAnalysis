using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Extractor_Test.TestConfig {
    /// <summary>
    /// Allows you to tag a test that it should be run on up to N of the files in the test directory
    /// </summary>
    public class TestFileDataAttribute : Attribute, ITestDataSource {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string _path;
        private readonly int _limit;
        private readonly string _prefix;
        private readonly string _format;
        private readonly string _wrapper;

        public TestFileDataAttribute(string path, int limit, string prefix, string format = "D2", string wrapper = "") {
            _path = path;
            _limit = limit;
            _prefix = prefix;
            _format = format;
            _wrapper = wrapper;
        }

        public IEnumerable<object[]> GetData(MethodInfo methodInfo) {
            try {
                
                DirectoryInfo d = new DirectoryInfo(_path);
                string directoryParent = d.Parent?.FullName;

                var ids = Enumerable.Range(1, _limit).Select(i => _wrapper+i.ToString(_format)+_wrapper).ToList();

                var files = Directory.EnumerateFiles(_path, "*.xlsx")
                    .Where(f => !f.Contains('~'))
                    .Where(f => ids.Any(f.Contains))
                    .Where(f=> f.Contains(_prefix))
                    .Select(f => new object[] { Path.IsPathFullyQualified(_path) ? f : directoryParent + Path.DirectorySeparatorChar + f})
                    .ToList();

                Log.Info("we found " + files.Count + " to test");
                return files;
            }
            catch (Exception e) {
                Assert.Fail("exception " + e);
                return null;
            }
        }

        public string GetDisplayName(MethodInfo methodInfo, object[] data) {
            return data == null ? null : string.Format(CultureInfo.CurrentCulture, "{0} ({1})", methodInfo.Name, string.Join(",", data));
        }
    }
}