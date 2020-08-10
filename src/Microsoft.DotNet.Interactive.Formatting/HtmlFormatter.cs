﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Html;
using static Microsoft.DotNet.Interactive.Formatting.PocketViewTags;

namespace Microsoft.DotNet.Interactive.Formatting
{
    public static class HtmlFormatter
    {
        public static bool PreformatEmbeddedPlainText { get; set; } = false;

        static HtmlFormatter()
        {
            Formatter.Clearing += (obj, sender) => PreformatEmbeddedPlainText = false;
        }

        public static ITypeFormatter GetBestFormatterFor(Type type) =>
            Formatter.GetBestFormatterFor(type, MimeType);

        public static ITypeFormatter GetDefaultFormatterForAnyObject(Type type, bool includeInternals = false) =>
            FormattersForAnyObject.GetFormatter(type, includeInternals);

        public static ITypeFormatter GetDefaultFormatterForAnyEnumerable(Type type) =>
            FormattersForAnyEnumerable.GetFormatter(type, false);

        public const string MimeType = "text/html";

        internal static PocketView Table(
            List<IHtmlContent> headers,
            List<IHtmlContent> rows) =>
            table(
                thead(
                    tr(
                        headers ?? new List<IHtmlContent>())),
                tbody(
                    rows));

        internal static ITypeFormatter[] DefaultFormatters { get; } = DefaultHtmlFormatterSet.DefaultFormatters;

        internal static FormatterTable FormattersForAnyObject =
            new FormatterTable(typeof(HtmlFormatter<>), nameof(HtmlFormatter<object>.CreateForAnyObject));

        internal static FormatterTable FormattersForAnyEnumerable =
            new FormatterTable(typeof(HtmlFormatter<>), nameof(HtmlFormatter<object>.CreateForAnyEnumerable));

        internal static IHtmlContent DisplayEmbeddedObjectAsPlainText(object value)
        {
            var html = value.ToDisplayString(PlainTextFormatter.MimeType).HtmlEncode();
            // See https://github.com/dotnet/interactive/issues/697
            if (PreformatEmbeddedPlainText)
            {
                var pre = new Tag("pre");
                pre.HtmlAttributes["text-align"] = "left";
                html = pre.Containing(html);
            }
            return html;

        }
    }

}
}