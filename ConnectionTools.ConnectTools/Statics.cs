//using System.Text.RegularExpressions;

//namespace ConnectTools
//{
//  public static class Statics
//  {

//    public static bool FitsMask(this string sFileName, string sFileMask)
//    {
//      if (string.IsNullOrWhiteSpace(sFileMask))
//        return true;
//      Regex mask = new Regex(sFileMask.Replace(".", "[.]").Replace("*", ".*").Replace("?", ".").Replace("\\", "\\\\"));
//      return mask.IsMatch(sFileName);
//    }

//  }
//}


