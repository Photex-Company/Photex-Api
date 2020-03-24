using System;

namespace Photex.Core.Exceptions
{
    public class WrongImageFormatException : Exception
    {
        public WrongImageFormatException()
             : base("Image is not in correct format.")
        {  }
    }
}
