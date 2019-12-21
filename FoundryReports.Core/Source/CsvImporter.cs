﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FoundryReports.Core.Products;

namespace FoundryReports.Core.Source
{
    public class CsvImporter
    {
        public async IAsyncEnumerable<IMold> ImportMoldsFromCsv(string csvPath)
        {
            var moldTable = await ReadCsvAsStringTable(csvPath);

            foreach (var line in moldTable)
            {
                if (line.Count < 4)
                    continue;

                var name = line[0];
                var currentUsages = AsInt(line[1]);
                var maxUsages = AsInt(line[2]);
                var castingCellAmount = AsDecimal(line[3]);

                var newMold = new Mold
                {
                    Name = name,
                    CurrentUsages = currentUsages,
                    MaxUsages = maxUsages,
                    CastingCellAmount = castingCellAmount
                };

                yield return newMold;
            }
        }

        public async IAsyncEnumerable<IProduct> ImportProductsFromCsv(string csvPath)
        {
            var productTable = await ReadCsvAsStringTable(csvPath);
            var usedMolds = new List<string>();

            foreach (var line in productTable)
            {
                // the first line is expected to name the products
                if (usedMolds.Count == 0)
                {
                    // the very first column is empty, as it is the column for the product names
                    usedMolds.AddRange(line.Skip(1));
                }

                if (line.Count < 1)
                    continue;

                var name = line[0];
                var moldValues = line.Skip(1).ToList();

                var moldRequirements = ToRequirements(moldValues, usedMolds);

                var newProduct = new Product {Name = name};
                foreach (var moldRequirement in moldRequirements)
                {
                    newProduct.MoldRequirements.Add(moldRequirement);
                }

                yield return newProduct;
            }
        }

        private IEnumerable<IMoldRequirement> ToRequirements(IList<string> moldValues, IList<string> moldNames)
        {
            var count = Math.Min(moldValues.Count, moldNames.Count);

            for (var i = 0; i < count; i++)
            {
                var moldName = moldNames[i];
                var moldValue = AsDecimal(moldValues[i]);
                var correspondingMold = new MoldDummy {Name = moldName };

                yield return new MoldRequirement(correspondingMold, moldValue);
            }
        }

        private decimal AsDecimal(string stringRepresentation)
        {
            // always use period as limiter to avoid culture problems
            stringRepresentation = stringRepresentation.Replace(',', '.');
            if (decimal.TryParse(stringRepresentation, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture,
                out var asDecimal))
            {
                return asDecimal;
            }

            return default;
        }

        private int AsInt(string stringRepresentation)
        {
            if (int.TryParse(stringRepresentation, out var asInt))
            {
                return asInt;
            }

            return default;
        }

        private async Task<List<List<string>>> ReadCsvAsStringTable(string csvPath)
        {
            var table = new List<List<string>>();

            const char csvDelimiter = ';';

            await foreach (var line in ReadLinesOfFile(csvPath))
            {
                table.Add(line.Split(csvDelimiter).ToList());
            }

            return table;
        }

        private async IAsyncEnumerable<string> ReadLinesOfFile(string filePath)
        {
            await Task.Delay(500);
            if (!File.Exists(filePath))
                yield break;

            string[] lines;

            try
            {
                lines = await File.ReadAllLinesAsync(filePath);
            }
            catch (Exception)
            {
                // log exception here. (which is omitted within the scope of this project)
                yield break;
            }

            foreach (var line in lines)
            {
                yield return line;
            }
        }
    }
}