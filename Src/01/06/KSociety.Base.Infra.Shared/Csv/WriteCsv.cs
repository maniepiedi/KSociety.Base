﻿using CsvHelper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace KSociety.Base.Infra.Shared.Csv
{
    public class WriteCsv<TEntity>
        where TEntity : class
    {
        private static ILogger _logger;

        public static TEntity[] Write(ILoggerFactory loggerFactory, string fileName)
        {
            _logger = loggerFactory.CreateLogger("WriteCsv");

            var csvFileName = @"." + fileName + @".csv";
            _logger.LogTrace("WriteCsv csvFileName: " + csvFileName);
            var assembly = Assembly.GetCallingAssembly();
            var resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(csvFileName));
            _logger.LogTrace("WriteCsv resourceName: " + resourceName);

            try
            {
                using var stream = assembly.GetManifestResourceStream(resourceName);
                using var streamReader = new StreamReader(stream ?? throw new InvalidOperationException());
                var reader = new CsvReader(streamReader, Configuration.CsvConfiguration);

                return reader.GetRecords<TEntity>().ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WriteCsv: ");
            }
            return null;
        }

        public static bool Export(ILoggerFactory loggerFactory, string fileName, IEnumerable<TEntity> records)
        {
            _logger = loggerFactory.CreateLogger("ExportCsv");

            try
            {
                using var streamWriter = new StreamWriter(fileName, false, System.Text.Encoding.UTF8);
                var writer = new CsvWriter(streamWriter, Configuration.CsvConfigurationWrite);
                //writer.Configuration.AutoMap<>();
                writer.WriteRecords(records);
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "WriteCsv: ");
            }

            return false;
        }

        public static async ValueTask<bool> ExportAsync(ILoggerFactory loggerFactory, string fileName, IEnumerable<TEntity> records)
        {
            _logger = loggerFactory.CreateLogger("ExportAsyncCsv");

            try
            {
                await using var streamWriter = new StreamWriter(fileName, false, System.Text.Encoding.UTF8);
                var writer = new CsvWriter(streamWriter, Configuration.CsvConfigurationWrite);

                await writer.WriteRecordsAsync(records).ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "WriteCsv: ");
            }

            return false;
        }
    }
}
