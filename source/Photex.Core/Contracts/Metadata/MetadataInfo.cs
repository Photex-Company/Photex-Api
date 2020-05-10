using System.Collections.Generic;
using ExifLibrary;

namespace Photex.Core.Contracts.Metadata
{
    public static class MetadataInfo
    {
        public enum MetadataName
        {
            DocumentName = 269,
            ImageDescription = 270,
            Make = 271,
            Model = 272,
            Software = 461,
            ModifyDate = 462,
            Artist = 473,
            HostComputer = 474,
            Copyright = 33432,
            GPSInfo = 34853,
            DateTimeOriginal = 36867,
            CreateDate = 36868,
            UserComment = 37510,
            XPTitle = 40091,
            XPComment = 40092,
            XPAuthor = 40093,
            XPKeywords = 40094,
            OwnerName = 42032
        }

        public static IDictionary<MetadataName, string> MetadataDescriptions
            => new Dictionary<MetadataName, string>
            {
                { MetadataName.DocumentName, "The name of the document from which this image was scanned" },
                { MetadataName.ImageDescription, "A character string giving the title of the image. It may be a comment such as \"1988 company picnic\" or the like. Two-bytes character codes cannot be used. When a 2-bytes code is necessary, the Exif Private tag <UserComment> is to be used." },
                { MetadataName.Make, "The manufacturer of the recording equipment. This is the manufacturer of the DSC, scanner, video digitizer or other equipment that generated the image. When the field is left blank, it is treated as unknown." },
                { MetadataName.Model, "The model name or model number of the equipment. This is the model name or number of the DSC, scanner, video digitizer or other equipment that generated the image. When the field is left blank, it is treated as unknown." },
                { MetadataName.Software, "This tag records the name and version of the software or firmware of the camera or image input device used to generate the image. The detailed format is not specified, but it is recommended that the example shown below be followed. When the field is left blank, it is treated as unknown." },
                { MetadataName.ModifyDate, "The date and time of image creation. In Exif standard, it is the date and time the file was changed." },
                { MetadataName.Artist, " This tag records the name of the camera owner, photographer or image creator. The detailed format is not specified, but it is recommended that the information be written as in the example below for ease of Interoperability. When the field is left blank, it is treated as unknown" },
                { MetadataName.HostComputer, "This tag records information about the host computer used to generate the image." },
                { MetadataName.Copyright, "Copyright information. In this standard the tag is used to indicate both the photographer and editor copyrights. It is the copyright notice of the person or organization claiming rights to the image. The Interoperability copyright statement including date and rights should be written in this field; e.g., \"Copyright, John Smith, 19xx. All rights reserved.\"" },
                { MetadataName.GPSInfo, "" },
                { MetadataName.DateTimeOriginal, "The date and time when the original image data was generated. For a digital still camera the date and time the picture was taken are recorded." },
                { MetadataName.CreateDate, " The date and time when the image was stored as digital data." },
                { MetadataName.UserComment, "A tag for Exif users to write keywords or comments on the image besides those in <ImageDescription>, and without the character code limitations of the <ImageDescription> tag." },
                { MetadataName.XPTitle, "Title tag used by Windows, encoded in UCS2" },
                { MetadataName.XPComment, "Comment tag used by Windows, encoded in UCS2" },
                { MetadataName.XPAuthor, "Author tag used by Windows, encoded in UCS2" },
                { MetadataName.XPKeywords, "Keywords tag used by Windows, encoded in UCS2" },
                { MetadataName.OwnerName, "This tag records the owner of a camera used in photography as an ASCII string." }
            };

        public static IDictionary<MetadataName, string> MetadataValues
            => new Dictionary<MetadataName, string>
            {
                { MetadataName.DocumentName, "0x010d" },
                { MetadataName.ImageDescription, "0x010e" },
                { MetadataName.Make, "0x010f" },
                { MetadataName.Model, "0x0110" },
                { MetadataName.Software, "0x0131" },
                { MetadataName.ModifyDate, "0x0132" },
                { MetadataName.Artist, "0x013b" },
                { MetadataName.HostComputer, "0x013c" },
                { MetadataName.Copyright, "0x8298" },
                { MetadataName.GPSInfo, "0x8825" },
                { MetadataName.DateTimeOriginal, "0x9003" },
                { MetadataName.CreateDate, "0x9004" },
                { MetadataName.UserComment, "0x9286" },
                { MetadataName.XPTitle, "0x9c9b" },
                { MetadataName.XPComment, "0x9c9c" },
                { MetadataName.XPAuthor, "0x9c9d" },
                { MetadataName.XPKeywords, "0x9c9e" },
                { MetadataName.OwnerName, "0xa430" }
            };

        public static IDictionary<MetadataName, ExifTag> MetadataTags
            => new Dictionary<MetadataName, ExifTag>
            {
                { MetadataName.DocumentName, ExifTag.DocumentName },
                { MetadataName.ImageDescription, ExifTag.ImageDescription },
                { MetadataName.Make, ExifTag.Make },
                { MetadataName.Model, ExifTag.Model },
                { MetadataName.Software, ExifTag.Software },
                { MetadataName.ModifyDate, ExifTag.DateTime },
                { MetadataName.Artist, ExifTag.Artist },
                { MetadataName.HostComputer, ExifTag.HostComputer },
                { MetadataName.Copyright, ExifTag.Copyright },
                { MetadataName.GPSInfo, ExifTag.GPSAreaInformation },
                { MetadataName.DateTimeOriginal, ExifTag.DateTimeOriginal },
                { MetadataName.CreateDate, ExifTag.DateTimeDigitized },
                { MetadataName.UserComment, ExifTag.UserComment },
                { MetadataName.XPTitle, ExifTag.WindowsTitle },
                { MetadataName.XPComment, ExifTag.WindowsComment },
                { MetadataName.XPAuthor, ExifTag.WindowsAuthor },
                { MetadataName.XPKeywords, ExifTag.WindowsKeywords },
                { MetadataName.OwnerName, ExifTag.CameraOwnerName }
            };
    }
}

