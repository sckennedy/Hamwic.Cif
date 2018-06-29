using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Hamwic.Cif.Web.TagHelpers
{
    public class FileIconTagHelper : TagHelper
    {
        public string Filename {get; set;}
        private static readonly IDictionary<string, string> ExtensionMap = new Dictionary<string, string>
        {
            {"xls", "fa-file-excel-o"},
            {"xlsm", "fa-file-excel-o"},
            {"xlsx", "fa-file-excel-o"},
            {"xlw", "fa-file-excel-o"},
            {"txt", "fa-file-text-o"},
            {"rtf", "fa-file-text-o"},
            {"csv", "fa-file-text-o"},
            {"odf", "fa-file-pdf-o"},
            {"doc", "fa-file-word-o"},
            {"docx", "fa-file-word-o"},
            {"docm", "fa-file-word-o"},
            {"gif", "fa-file-image-o"},
            {"jpg", "fa-file-image-o"},
            {"jpeg", "fa-file-image-o"},
            {"png", "fa-file-image-o"},
            {"svg", "fa-file-image-o"},
            {"ppt", "fa-file-powerpoint-o"},
            {"pptx", "fa-file-powerpoint-o"},
            {"zip", "fa-file-zip-o"},
            {"tar", "fa-file-zip-o"},
            {"7z", "fa-file-zip-o"}
        };

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {

            var extension = (Path.GetExtension(Filename) ?? string.Empty).Replace(".", "").ToLowerInvariant();
            if (!ExtensionMap.ContainsKey(extension))
                output.PostContent.SetHtmlContent("<i class='fa fa-file-o'></i>");
            else
                output.PostContent.SetHtmlContent($"<i class='fa {ExtensionMap[extension]}'></i>");
        }
    }
}