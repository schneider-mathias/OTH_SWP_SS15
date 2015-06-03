#region Copyright information
// <summary>
// <copyright file="ScenarioMatrixExporter.cs">Copyright (c) 2015</copyright>
// 
// <creationDate>22/04/2015</creationDate>
// 
// <professor>Prof. Dr. Kurt Hoffmann</professor>
// <studyCourse>Angewandte Informatik</studyCourse>
// <branchOfStudy>Industrieinformatik</branchOfStudy>
// <subject>Software Projekt</subject>
// </summary>
#endregion
using System;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using GraphFramework.Interfaces;

namespace UseCaseAnalyser.Model.Model
{
    /// <summary>
    /// class to export the scenario matrix to a file
    /// </summary>
    public static class ScenarioMatrixExporter
    {

        private const string ExcelExtension = ".xlsx";

        /// <summary>
        /// exports the scenario matrix of the use case graph to the specfified file (.xlsx) 
        /// </summary>
        /// <param name="useCaseGraph">use case graph whose scenario matrix should be exported</param>
        /// <param name="file">file info of the file to save the scenario matrix to</param>
        public static void ExportScenarioMatrix(UseCaseGraph useCaseGraph, FileInfo file)
        {
            //check if given file is corrupted
            ValidateFile(file);

            SpreadsheetDocument document = SpreadsheetDocument.Create(file.FullName, SpreadsheetDocumentType.Workbook);
                ScenarioMatrixExporter.CreateScenarioMatrix(useCaseGraph, document, 1);
            document.Close();
        }

        /// <summary>
        /// Exports the scenario matrix of all use case graphs to the specfified file (.xlsx)
        /// Each Use Case is a new excel sheet 
        /// </summary>
        /// <param name="useCaseGraphs"></param>
        /// <param name="file">file info of the file to save the scenario matrix to</param>
        public static void ExportScenarioMatrix(UseCaseGraph[] useCaseGraphs, FileInfo file)
        {
            //check if given file is corrupted
            ValidateFile(file);

            using (SpreadsheetDocument document = SpreadsheetDocument.Open(file.FullName, true))
            {
                for (UInt32Value i = 0; i < useCaseGraphs.Length; i++)
                {
                    var useCaseGraph = useCaseGraphs[i];
                    ScenarioMatrixExporter.CreateScenarioMatrix(useCaseGraph, document, i);
                }
            }
        }

        
        // ReSharper disable once UnusedParameter.Local
        // [Mathias Schneider] needed for checking for exceptions
        /// <summary>
        /// Helper method to check if file validates preconditions.
        /// </summary>
        /// <param name="file">FileInfo that should be checked.</param>
        private static void ValidateFile(FileInfo file)
        {
            if (file == null || file.FullName.Equals(ExcelExtension))
                throw new InvalidOperationException("Please specify an output file name.");
            if (!string.Equals(file.Extension, ExcelExtension))
                throw new InvalidOperationException("Wrong file type, please specify a *" + ExcelExtension + ".");
        }

        private static void CreateScenarioMatrix(UseCaseGraph useCaseGraph, SpreadsheetDocument spreadsheetDoc, UInt32Value index)
        {
            string ucName = useCaseGraph.GetAttributeByName("Name").Value as string;
            if (string.IsNullOrEmpty(ucName)) throw new InvalidOperationException("Use case is corrupt!");

            // Add a WorkbookPart to the document.
            WorkbookPart workbookPart = spreadsheetDoc.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            // Add a WorksheetPart to the WorkbookPart.
            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            // Add Sheets to the Workbook.
            Sheets sheets = spreadsheetDoc.WorkbookPart.Workbook.AppendChild(new Sheets());

            // Append a new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet()
            {
                Id = spreadsheetDoc.WorkbookPart.
                    GetIdOfPart(worksheetPart),
                SheetId = index,
                Name = ucName
            };

            sheets.Append(sheet);

            int varColCount = GetNumberOfVariants(useCaseGraph);

            workbookPart.Workbook.Save();
        }

        private static int GetNumberOfVariants(UseCaseGraph useCaseGraph)
        {
            int number = 0;
            int lastNumber = 0;
            IGraph[] scenarios = useCaseGraph.Scenarios.ToArray();                       
            for (int i = 0; i < scenarios.Length; i++)
            {
                INode[] nodes = scenarios[i].Nodes.ToArray();
                UseCaseGraph.NodeTypeAttribute lastNodeType = (UseCaseGraph.NodeTypeAttribute) nodes[0].GetAttributeByName(
                    NodeAttributes.NodeType.AttributeName()).Value;

                foreach (var node in scenarios[i].Nodes)
                {
                    IAttribute attr = node.GetAttributeByName(NodeAttributes.NodeType.AttributeName());
                    if(attr == null) throw new NotImplementedException();
                    if ((UseCaseGraph.NodeTypeAttribute) attr.Value == UseCaseGraph.NodeTypeAttribute.VariantNode)
                    {
                        if (lastNodeType != (UseCaseGraph.NodeTypeAttribute)attr.Value)
                            number++;
                    }

                    lastNodeType = (UseCaseGraph.NodeTypeAttribute)attr.Value;
                }

                lastNumber = (number > lastNumber) ? number : lastNumber;
                number = 0;
            }

            return lastNumber;
        }
    }
}