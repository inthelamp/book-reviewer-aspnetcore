using Microsoft.JSInterop;

namespace BookReviewer.Helpers
{
    public static class RichEditorHelper
    {
        [JSInvokable]
        public static string GetContentText(string content)
        {
            // Removes image data
            const string imageStartTag = "{\"image\":\"data:image";
            const string imageEndTag = "\"}";            
            while (content.Contains(imageStartTag))
            {
                int imageStartIndex = content.IndexOf(imageStartTag);
                int imageEndIndex = content.IndexOf(imageEndTag, imageStartIndex + imageStartTag.Length);
                content = content.Remove(imageStartIndex, imageEndIndex + imageEndTag.Length - imageStartIndex);
            }

            // Removes other tags
            content = content.Replace("\"ops\"", String.Empty).Replace("\"insert\"", String.Empty);
            content = content.Replace("\"attributes\"", String.Empty);        
            content = content.Replace("\"underline\":true,", String.Empty).Replace("\"blockquote\":true,", String.Empty);
            content = content.Replace("\"italic\":true,", String.Empty).Replace("\"bold\":true,", String.Empty);     
            content = content.Replace("\"code-block\":true,", String.Empty);                  
            content = content.Replace("\"underline\":true", String.Empty).Replace("\"blockquote\":true", String.Empty);
            content = content.Replace("\"italic\":true", String.Empty).Replace("\"bold\":true", String.Empty);     
            content = content.Replace("\"code-block\":true", String.Empty);             
            content = content.Replace("\"header\":1", String.Empty).Replace("\"header\":2", String.Empty).Replace("\"header\":3", String.Empty);                       
            content = content.Replace("[", String.Empty).Replace("]", String.Empty).Replace("\\n", String.Empty);
            content = content.Replace(":", String.Empty).Replace("\\", String.Empty).Replace("\"", String.Empty).Replace("{", String.Empty);
            content = content.Replace("},", String.Empty).Replace("}", String.Empty);

            return content.Trim();
        }
    }
}