using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OneNoteDuplicatesRemover
{
    public static class Utils
    {
        public static string ConvertToHexString(byte[] computedHash)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < computedHash.Length; i++)
            {
                sb.Append(computedHash[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
