﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.XPath;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;
using Microsoft.VisualStudio.Text.Adornments;

namespace VsctCompletion.Completion.Providers
{
    public class GuidSymbolIdProvider : ICompletionProvider
    {
        private readonly IAsyncCompletionSource _source;
        private readonly ImageElement _icon;
        private static IEnumerable<CompletionItem> _knownIds, _knownMonikers;

        public GuidSymbolIdProvider(IAsyncCompletionSource source, ImageElement icon)
        {
            _source = source;
            _icon = icon;
        }

        public IEnumerable<CompletionItem> GetCompletions(XmlDocument doc, XPathNavigator navigator, Func<string, CompletionItem> CreateItem)
        {
            var list = new List<CompletionItem>();

            string guid = navigator.GetAttribute("guid", "");

            if (guid == "ImageCatalogGuid")
            {
                list.AddRange(GetKnownMonikers(_source, _icon));
            }
            else if (guid == "guidSHLMainMenu")
            {
                list.AddRange(GetKnownIds(_source, _icon));
            }
            else
            {
                XmlNodeList ids = doc.SelectNodes("//GuidSymbol[@name='" + guid + "']//IDSymbol");

                foreach (XmlNode symbol in ids)
                {
                    XmlAttribute name = symbol.Attributes["name"];

                    if (name != null)
                    {
                        list.Add(CreateItem(name.Value));
                    }
                }
            }

            return list;
        }

        private IEnumerable<CompletionItem> GetKnownMonikers(IAsyncCompletionSource source, ImageElement icon)
        {
            if (_knownMonikers == null)
            {
                var list = new List<CompletionItem>();

                foreach (string name in KnownMonikersList.KnownMonikerNames)
                {
                    var item = new CompletionItem(name, source, icon);
                    item.Properties.AddProperty("knownmoniker", name);
                    list.Add(item);
                }

                _knownMonikers = list;
            }

            return _knownMonikers;
        }

        private IEnumerable<CompletionItem> GetKnownIds(IAsyncCompletionSource source, ImageElement icon)
        {
            if (_knownIds == null)
            {
                _knownIds = KnownIdList.KnownIds.Select(k => new CompletionItem(k, source, icon));
            }

            return _knownIds;
        }
     }
}
