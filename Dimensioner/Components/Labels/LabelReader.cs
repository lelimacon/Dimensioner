using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Dimensioner.Utils;

namespace Dimensioner.Components.Labels
{
    public class LabelReader : TaxonomyComponentReader
    {
        private readonly ConcurrentBag<KeyValuePair<Href, Label>> _labels;
        private readonly ConcurrentBag<Label> _orphanLabels;

        public IReadOnlyCollection<Label> OrphanLabels => _orphanLabels;

        public LabelReader()
        {
            _labels = new ConcurrentBag<KeyValuePair<Href, Label>>();
            _orphanLabels = new ConcurrentBag<Label>();
        }

        public override IEnumerable<TaxonomyComponent> Read(Linkbase linkbase, XDocument document)
        {
            // Filter on label linkbases.
            if (linkbase.Role != Ns.XbrlLabelLinkbaseReferenceRole)
                return null;

            // Read labels.
            var linkNodes = document.Root.Children(Ns.Link, "labelLink");
            foreach (XElement linkNode in linkNodes)
            {
                var labels = linkNode.Children(Ns.Link, "label")
                    .Select(l => new XlinkNode(linkbase.Path, l))
                    .ToDictionary(l => l.Label);
                var locs = linkNode.Children(Ns.Link, "loc")
                    .Select(l => new XlinkNode(linkbase.Path, l))
                    .ToDictionary(l => l.Label);
                var labelArcs = linkNode.Children(Ns.Link, "labelArc")
                    .Select(l => new XlinkNode(linkbase.Path, l))
                    .ToList();
                foreach (XlinkNode arc in labelArcs)
                {
                    var locNode = locs[arc.From];
                    var labelNode = labels[arc.To];
                    Label label = new Label(labelNode.XNode);
                    _labels.Add(new KeyValuePair<Href, Label>(locNode.Href, label));
                }
            }

            return new List<TaxonomyComponent>();
        }

        public override IEnumerable<TaxonomyComponent> PostProcess(XbrlSchemaSet schemaSet)
        {
            // Assign all labels to the components, if found.
            foreach (var labelEncaps in _labels)
            {
                Href href = labelEncaps.Key;
                Label label = labelEncaps.Value;
                var componenent = schemaSet.GetComponent(href) as ILabelized;
                if (componenent == null)
                {
                    _orphanLabels.Add(label);
                    continue;
                }
                if (componenent.Labels == null)
                    componenent.Labels = new XbrlLabels();
                componenent.Labels.Add(label);
            }

            return new List<TaxonomyComponent>();
        }
    }
}
