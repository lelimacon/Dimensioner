using System;
using Dimensioner.Utils;

namespace Dimensioner
{
    public class Href
    {
        private readonly string _originalString;

        public string AbsolutePath { get; }
        public string DocumentUri { get; }
        public string ResourceId { get; }

        public Href(string href)
            : this(null, href)
        {
        }

        public Href(string basePath, string href)
        {
            _originalString = href ?? throw new ArgumentNullException(nameof(href));
            string[] hrefSplit = href.Split(new[] {'#'});
            DocumentUri = hrefSplit[0];
            if (hrefSplit.Length > 1)
                ResourceId = hrefSplit[1];
            if (!string.IsNullOrEmpty(basePath))
                AbsolutePath = LocalUrlResolver.Resolve(basePath, DocumentUri);
            else
                AbsolutePath = DocumentUri;
            AbsolutePath = string.IsNullOrEmpty(basePath)
                ? DocumentUri
                : LocalUrlResolver.Resolve(basePath, DocumentUri);
        }

        public override bool Equals(object obj)
        {
            var href = obj as Href;
            if (href == null)
                return false;
            return href.AbsolutePath == AbsolutePath && href.ResourceId == ResourceId;
        }

        public static bool operator ==(Href left, Href right)
        {
            if (ReferenceEquals(left, null))
                return ReferenceEquals(right, null);
            return left.Equals(right);
        }

        public static bool operator !=(Href left, Href right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return (AbsolutePath?.GetHashCode() ?? 1)
                   ^ (7 * (ResourceId?.GetHashCode() ?? 1));
        }

        public override string ToString()
        {
            return _originalString;
        }

        public string Format()
        {
            return $"{AbsolutePath}#{ResourceId}";
        }
    }
}
