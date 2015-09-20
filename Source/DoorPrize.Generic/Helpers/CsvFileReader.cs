using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;

namespace DoorPrize.Generic
{
    public class CsvFileReader : IEnumerable<string[]>, IDisposable
    {
        private readonly int expectedFields;
        private readonly string nameOnly;

        private StreamReader reader;

        public CsvFileReader(string fileName, int expectedFields)
        {
            Contract.Requires(fileName.IsFileName());
            Contract.Requires(expectedFields >= 1);

            reader = new StreamReader(fileName);

            this.expectedFields = expectedFields;

            nameOnly = Path.GetFileName(fileName);
        }

        public void Dispose()
        {
            if (reader != null)
                reader.Close();

            reader = null;
        }

        public IEnumerator<string[]> GetEnumerator()
        {
            var lineNumber = 0;

            string line;

            while ((line = reader.ReadLine()) != null)
            {
                var fields = line.Split(',');

                if (fields.Length != expectedFields)
                {
                    throw new Exception(string.Format(
                        "Line {0:N} of \"{1}\" contained {2} fields, not {3} as expected!", 
                        lineNumber, nameOnly, fields.Length, expectedFields));
                }

                lineNumber++;

                yield return fields;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
