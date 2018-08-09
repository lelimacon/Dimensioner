using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using Dimensioner.Components.Arcroles;
using Dimensioner.Components.Calculations;
using Dimensioner.Components.Definitions;
using Dimensioner.Components.Elements;
using Dimensioner.Components.Labels;
using Dimensioner.Components.Presentations;
using Dimensioner.Components.Roles;
using Dimensioner.Components.Tables;

namespace Dimensioner.Cli
{
    internal class Program
    {
        //const string EntryPath = @"..\..\..\..\..\DATA\TAXO\LDT\2.0.1\www.srb.europa.eu\eu\fr\xbrl\fws\res\eu-806-2014\2017-10-10\mod\ldt-con.xsd";
        //const string EntryPath = "https://www.ifrs.org/-/media/feature/standards/taxonomy/2018/ifrst_2018-03-16.zip?la=en&hash=0AEB90D8569C648C9433398AC0E1D5C006FAFCEC";
        const string EntryPath = "https://raw.githubusercontent.com/SunSpecOrangeButton/solar-taxonomy/v1.2/core/solar_all_2018-03-31_r01.xsd";

        private static void Main(string[] args)
        {
            var w = new Stopwatch();

            IWebProxy proxy = WebRequest.DefaultWebProxy;
            proxy.Credentials = CredentialCache.DefaultCredentials;
            var config = new ReaderConfiguration
            {
                UseCache = true
            };
            XbrlSchemaSet schemaSet = null;
            var labelReader = new LabelReader();
            var genericLabelReader = new GenericLabelReader();

            using (var reader = new TaxonomyReader(config)
                    .Register<ElementReader>()
                    .Register<RoleReader>()
                    .Register<ArcroleReader>()
                    .Register<DefinitionReader>()
                    .Register<CalculationReader>()
                    .Register<PresentationReader>()
                    .Register<TableReader>()
                    .Register<TableGroupReader>()
                    .Register(labelReader)
                    .Register(genericLabelReader))
            {
                Console.WriteLine($"Reading taxonomy at {EntryPath}");
                Console.WriteLine();

                w.Start();
                schemaSet = reader.Read(EntryPath);
                schemaSet.Compile();
                w.Stop();

                // Print errors.
                if (reader.Errors.Any())
                {
                    Console.WriteLine($"{reader.Errors.Count} component reader errors");
                    foreach (var error in reader.Errors)
                        Console.WriteLine($"  - {error.ComponentType.Name} : {error.Message}");
                    Console.WriteLine();
                }

                // Print elapsed.
                Console.WriteLine($"Read {schemaSet.Schemas.Count()} schemas in {w.Elapsed}");
                foreach (var tracker in reader.ComponentReaders)
                    Console.WriteLine($"  - {tracker.Reader.GetType().Name} : {tracker.Elapsed}");
                Console.WriteLine();
            }

            if (schemaSet == null)
                return;

            PrintElements(schemaSet, labelReader, genericLabelReader);
            //ExportLinkbase(schemaSet);
            //PrintTypes(schemaSet);

            Console.ReadLine();
        }

        private static void ExportLinkbase(XbrlSchemaSet schemaSet)
        {
            // Print linkbase.
            Definition definition = schemaSet.Components<Definition>().First();
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string savePath = Path.Combine(desktopPath, "test-def.xml");

            Console.WriteLine($"Exporting definition {definition.Role?.Uri}");
            Console.WriteLine($"- Defined in {definition.Schema.Path}");
            Console.WriteLine();

            /*
            byte[] linkbaseContent;
            using (var stream = new MemoryStream())
            {
                definition.Write(stream);
                linkbaseContent = stream.GetBuffer();
            }
            Console.WriteLine($"Example definition linkbase output: {savePath}");
            //Console.WriteLine(Encoding.UTF8.GetString(linkbaseContent));
            File.WriteAllBytes(savePath, linkbaseContent);
            */

            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                definition.Write(stream);
            }
        }

        private static void PrintElements(XbrlSchemaSet schemaSet, LabelReader labelReader,
            GenericLabelReader genericLabelReader)
        {
            // Retrieve elements.
            var elements = schemaSet.Components<XbrlElement>().ToList();
            var elementTypes = elements.Select(e => e.Raws.Type)
                .OrderBy(t => t?.ToString()).ToLookup(e => e).ToList();
            var roles = schemaSet.Components<Role>().ToList();
            var arcroles = schemaSet.Components<Arcrole>().ToList();
            var definitions = schemaSet.Components<Definition>().ToList();
            var calculations = schemaSet.Components<Calculation>().ToList();
            var presentations = schemaSet.Components<Presentation>().ToList();
            var tables = schemaSet.Components<Table>().ToList();
            var tableGroups = schemaSet.Components<TableGroup>().ToList();
            var allComps = schemaSet.Schemas
                .SelectMany(s => s.Components())
                .ToList();
            var labelizable = allComps
                .OfType<ILabelized>()
                .ToList();
            var labelized = labelizable
                .Where(l => l.Labels?.Any() == true)
                .ToList();

            // Print component details.
            Console.WriteLine("Components:");
            Console.WriteLine($"- {allComps.Count} in schema set / {schemaSet.Components().Count()} in schemas");
            Console.WriteLine($"- {elements.Count} elements");
            Console.WriteLine($"- {roles.Count} roles");
            Console.WriteLine($"- {arcroles.Count} arcroles");
            Console.WriteLine($"- {definitions.Count} definitions");
            Console.WriteLine($"- {calculations.Count} calculations");
            Console.WriteLine($"- {presentations.Count} presentations");
            Console.WriteLine($"- {tables.Count} rendering tables");
            Console.WriteLine($"- {tableGroups.Count} table groups");
            Console.WriteLine($"- {labelized.Count}/{labelizable.Count} labelizable components");
            Console.WriteLine(
                $"  - {labelized.OfType<XbrlElement>().Count()}/{labelizable.OfType<XbrlElement>().Count()} of which are elements");
            Console.WriteLine(
                $"  - {labelized.OfType<Table>().Count()}/{labelizable.OfType<Table>().Count()} of which are tables");
            Console.WriteLine(
                $"  - {labelized.OfType<Breakdown>().Count()}/{labelizable.OfType<Breakdown>().Count()} of which are breakdowns");
            Console.WriteLine(
                $"  - {labelized.OfType<Components.Tables.DefinitionNode>().Count()}/{labelizable.OfType<Components.Tables.DefinitionNode>().Count()} of which are definition nodes (rule or aspect)");
            Console.WriteLine($"  - {labelReader.OrphanLabels.Count} orphan labels");
            Console.WriteLine($"  - {genericLabelReader.OrphanLabels.Count} orphan generic labels");
            Console.WriteLine();
        }

        private static void PrintTypes(XbrlSchemaSet schemaSet)
        {
            var elements = schemaSet.Components<XbrlElement>().ToList();
            var elementTypes = elements.Select(e => e.Raws.Type)
                .OrderBy(t => t?.ToString()).ToLookup(e => e).ToList();
            Console.WriteLine("Element types:");
            foreach (var type in elementTypes)
                Console.WriteLine($"[{type.Count(),4}] {type.Key}");
            Console.WriteLine();
        }
    }
}
