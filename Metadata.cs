using FotoFly;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vault
{
    public static class Metadata
    {

        public static string ReadMetadata(string filename)
        {
            try
            {

                JpgPhoto image = new JpgPhoto(filename);
                image.ReadMetadata();
                ReadOnlyCollection<string> tag = image.Metadata.Tags.ToReadOnlyCollection();
                SD.Garbage.ClearRAM.Clear();
                string tags = "";
                foreach (string s in tag)
                {
                    tags += s + ";";
                }
                return tags;
            }
            catch
            {
                return "";
            }
        }

        //public static void WriteMetadata(string filename, System.Collections.ObjectModel.ReadOnlyCollection<string> tags)
        //{
        //    File.Copy(filename, "Copied.jpg", true);
        //    JpgPhoto image = new JpgPhoto("Copied.jpg");
        //    image.ReadMetadata();
        //    image.Metadata.Tags = new TagList(tags);
        //    image.WriteMetadata(filename);
        //    File.Delete("Copied.jpg");

        //}

        //internal static void WriteMetadata(string filename, List<string> tags)
        //{
        //    try
        //    {
        //        File.Copy(filename, "Copied.jpg", true);
        //        JpgPhoto image = new JpgPhoto("Copied.jpg");
        //        image.ReadMetadata();

        //        TagList taglist = new TagList();
        //        taglist.AddRange(tags);
        //        image.Metadata.Tags = taglist;

        //        image.WriteMetadata(filename);

        //    }
        //    catch (Exception ee)
        //    {
        //        MessageBox.Show(ee.Message);
        //    }
        //}
    }
}
