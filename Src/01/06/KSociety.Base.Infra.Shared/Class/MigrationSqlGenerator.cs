﻿using System;
using System.IO;
using System.Linq;
using KSociety.Base.InfraSub.Shared.Class;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace KSociety.Base.Infra.Shared.Class
{
    //No Abstract
    public class MigrationSqlGenerator : SqlServerMigrationsSqlGenerator
    {
        //It must be public
        public MigrationSqlGenerator(
            MigrationsSqlGeneratorDependencies dependencies,
            //IMigrationsAnnotationProvider migrationsAnnotations)
            IRelationalAnnotationProvider migrationsAnnotations)
            : base(dependencies, migrationsAnnotations)
        {

        }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "Just because")]
        protected override void Generate(
            MigrationOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            if (operation is CreateViewOperation createViewOperation)
            {
                Generate(createViewOperation, builder);
            }
            else
            {
                base.Generate(operation, model, builder);
            }
        }

        private void Generate(
            CreateViewOperation operation,
            MigrationCommandListBuilder builder)
        {
            var sqlHelper = Dependencies.SqlGenerationHelper;
            //var stringMapping = Dependencies.TypeMappingSource.FindMapping(typeof(string));

            var assembly = AssemblyTool.GetAssemblyByName(operation.AssemblyName);
            //var assembly = Assembly.GetExecutingAssembly();
            //Console.WriteLine("Generate: " + assembly.FullName);
            string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(operation.ResourceSqlFileName));

            //Assembly.GetManifestResouceNames

            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            using StreamReader reader = new StreamReader(stream ?? throw new InvalidOperationException());
            string result = reader.ReadToEnd();
            //.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);

            builder.AppendLines(result)
                .AppendLine(sqlHelper.StatementTerminator)
                .EndCommand();
        }
    }
}