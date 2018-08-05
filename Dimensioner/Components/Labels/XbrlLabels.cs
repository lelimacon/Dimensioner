using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Dimensioner.Components.Labels
{
    public class XbrlLabels : IEnumerable<Label>
    {
        private readonly List<Label> _labels;
        private List<Label.RoleEnum> _roles;
        private List<CultureInfo> _cultures;

        public int Count => _labels.Count;

        public IEnumerable<Label.RoleEnum> AvailableRoles =>
            _roles ?? (_roles = _labels.Select(l => l.LabelRole).Distinct().ToList());

        public IEnumerable<CultureInfo> AvailableCultures =>
            _cultures ?? (_cultures = _labels.Select(l => l.Culture).Distinct().ToList());

        public string Standard
        {
            get => GetText();
            set => SetText(value);
        }

        public XbrlLabels(List<Label> labels = null)
        {
            _labels = labels ?? new List<Label>();
        }

        public void Add(Label label)
        {
            _labels.Add(label);
            _roles = null;
            _cultures = null;
        }

        /// <summary>
        ///     Returns a new instance without modifying the existing object.
        /// </summary>
        /// <param name="labels"></param>
        /// <returns></returns>
        public XbrlLabels Concat(XbrlLabels labels)
        {
            return new XbrlLabels(_labels.Concat(labels._labels).ToList());
        }

        /// <summary>
        ///     Finds the text for this label.
        /// </summary>
        /// <param name="role">The role to look for.</param>
        /// <param name="fallback">Fallback to default role. Defaults to false.</param>
        /// <returns>A string of the specified or default role representing this label.</returns>
        public string GetText(Label.RoleEnum role = Label.RoleEnum.Standard, bool fallback = false)
        {
            var label = _labels.FirstOrDefault(l => l.LabelRole == role);
            if (label == null && fallback)
                label = _labels.FirstOrDefault(l => l.LabelRole == Label.RoleEnum.Standard);
            return label?.Text?.Trim() ?? "?";
        }

        public string GetText(Label.RoleEnum role, Label.RoleEnum fallback)
        {
            var label = _labels.FirstOrDefault(l => l.LabelRole == role);
            if (label == null)
                label = _labels.FirstOrDefault(l => l.LabelRole == fallback);
            return label?.Text?.Trim() ?? "?";
        }

        public void SetText(string value, Label.RoleEnum role = Label.RoleEnum.Standard, bool fallback = false)
        {
            var label = _labels.FirstOrDefault(l => l.LabelRole == role);
            if (label == null && fallback)
                label = _labels.FirstOrDefault(l => l.LabelRole == Label.RoleEnum.Standard);
            if (label?.Text == null)
                return;
            label.Text = value;
        }

        public override string ToString()
        {
            return GetText();
        }

        public IEnumerator<Label> GetEnumerator()
        {
            return _labels.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _labels.GetEnumerator();
        }
    }
}
