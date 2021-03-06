using System;
using System.Text.RegularExpressions;

public static class Normalizer
{
    public static string Normalize(string orgString)
    {
        //去除空白,换行    
        var stringArray = orgString.Trim().Split("\n");
        string rtn = "";
        foreach (var item in stringArray)
        {
            rtn += item.Trim();
        }
        //表示项目编号的数字归一化  => []
        rtn = NormalizeItemListNumber(rtn);
        return rtn;
    }

    public static string NormalizeTextResult(this string orgString)
    {
        //HTML符号的过滤
        if (orgString.Contains("&amp;"))
        {
            orgString = orgString.Replace("&amp;", "&");
        }
        if (orgString.Contains("&nbsp;"))
        {
            orgString = orgString.Replace("&nbsp;", " ");
        }
        if (orgString.Contains("&lt;"))
        {
            orgString = orgString.Replace("&lt;", "<");
        }
        if (orgString.Contains("&gt;"))
        {
            orgString = orgString.Replace("&gt;", ">");
        }
        orgString = orgString.TrimEnd("。".ToCharArray());
        orgString = orgString.TrimEnd("；".ToCharArray());
        return orgString;
    }

    public static string NormalizeKey(this string orgString)
    {
        if (!String.IsNullOrEmpty(orgString))
        {
            orgString = orgString.Trim().Replace(" ", "").ToLower();
        }
        return orgString;
    }

    public static string NormalizeNumberResult(this string orgString)
    {
        if (!String.IsNullOrEmpty(orgString))
        {
            orgString = orgString.Trim().Replace(",", "");
        }
        return orgString;
    }

    public static string NormailizeDate(string orgString, string keyword = "")
    {
        orgString = orgString.Trim().Replace(",", "");
        var NumberList = RegularTool.GetNumberList(orgString);
        if (NumberList.Count == 6)
        {
            String Year = NumberList[3];
            String Month = NumberList[4];
            String Day = NumberList[5];
            int year; int month; int day;
            if (int.TryParse(Year, out year) && int.TryParse(Month, out month) && int.TryParse(Day, out day))
            {
                var d = new DateTime(year, month, day);
                return d.ToString("yyyy-MM-dd");
            }
        }
        if (NumberList.Count == 5)
        {
            if (orgString.IndexOf("年") != -1 && orgString.IndexOf("月") != -1 && orgString.IndexOf("日") != -1)
            {
                String Year = NumberList[0];
                String Month = NumberList[3];
                String Day = NumberList[4];
                int year; int month; int day;
                if (int.TryParse(Year, out year) && int.TryParse(Month, out month) && int.TryParse(Day, out day))
                {
                    if (month <= 12 && day <= 31)
                    {
                        var d = new DateTime(year, month, day);
                        return d.ToString("yyyy-MM-dd");
                    }
                }
            }
        }

        if (orgString.Contains("年") && orgString.Contains("月") && orgString.Contains("月"))
        {
            String Year = Utility.GetStringBefore(orgString, "年");
            String Month = RegularTool.GetValueBetweenString(orgString, "年", "月");
            String Day = Utility.GetStringAfter(orgString, "月").Replace("日", "");
            int year; int month; int day;
            if (int.TryParse(Year, out year) && int.TryParse(Month, out month) && int.TryParse(Day, out day))
            {
                var d = new DateTime(year, month, day);
                return d.ToString("yyyy-MM-dd");
            }
        }

        var SplitChar = new string[] { "/", ".", "-" };
        foreach (var sc in SplitChar)
        {
            var SplitArray = orgString.Split(sc);
            if (SplitArray.Length == 3)
            {
                String Year = SplitArray[0];
                String Month = SplitArray[1];
                String Day = SplitArray[2];
                int year; int month; int day;
                if (int.TryParse(Year, out year) && int.TryParse(Month, out month) && int.TryParse(Day, out day))
                {
                    var d = new DateTime(year, month, day);
                    return d.ToString("yyyy-MM-dd");
                }
            }
        }

        return orgString;
    }

    public static string NormalizerStockNumber(string orgString, string TitleWord)
    {
        if (!String.IsNullOrEmpty(orgString))
        {
            orgString = orgString.Trim().Replace(",", "");
            orgString = orgString.Trim().Replace("，", "");
        }
        orgString = orgString.Replace("不超过", "");
        orgString = orgString.Replace("不低于", "");
        orgString = orgString.Replace("不多于", "");
        orgString = orgString.Replace("不少于", "");

        if (orgString.EndsWith("股"))
        {
            orgString = orgString.Replace("股", "");
        }
        //对于【亿，万】的处理
        if (orgString.EndsWith("万") || TitleWord.Contains("万股"))
        {
            orgString = orgString.Replace("万", "");
            double x;
            if (double.TryParse(orgString, out x))
            {
                orgString = (x * 10_000).ToString();
            }
        }
        if (orgString.EndsWith("亿") || orgString.EndsWith("惩"))  //惩 本次HTML特殊处理
        {
            orgString = orgString.Replace("亿", "");
            orgString = orgString.Replace("惩", "");
            double x;
            if (double.TryParse(orgString, out x))
            {
                orgString = (x * 100_000_000).ToString();
            }
        }
        return orgString;
    }


    public static string[] CurrencyList = { "人民币","港币", "美元", "欧元", "元" };

    public static string NormalizerMoney(string orgString, string TitleWord)
    {
        if (!String.IsNullOrEmpty(orgString))
        {
            orgString = orgString.Trim().Replace(",", "");
            orgString = orgString.Trim().Replace("，", "");
        }
        orgString = orgString.Replace("不超过", "");
        orgString = orgString.Replace("不低于", "");
        orgString = orgString.Replace("不多于", "");
        orgString = orgString.Replace("不少于", "");

        foreach (var Currency in CurrencyList)
        {
            if (orgString.EndsWith(Currency))
            {
                orgString = orgString.Replace(Currency, "");
                orgString = orgString.Trim();
                break;
            }
        }
        //对于【亿，万】的处理
        if (orgString.EndsWith("万") || TitleWord.Contains("万元"))
        {
            orgString = orgString.Replace("万", "");
            double x;
            if (double.TryParse(orgString, out x))
            {
                orgString = (x * 10_000).ToString();
            }
        }
         if (orgString.EndsWith("亿") || orgString.EndsWith("惩"))  //惩 本次HTML特殊处理
        {
            orgString = orgString.Replace("亿", "");
            orgString = orgString.Replace("惩", "");
            double x;
            if (double.TryParse(orgString, out x))
            {
                orgString = (x * 100_000_000).ToString();
            }
        }
        if (orgString.EndsWith(".00")) orgString = orgString.Substring(0, orgString.Length - 3);
        orgString = orgString.Trim();
        return orgString;
    }

    public static string NormalizeItemListNumber(string orgString)
    {
        //（1）  => [1]
        RegexOptions ops = RegexOptions.Multiline;
        Regex r = new Regex(@"\（(\d+)\）", ops);
        if (r.IsMatch(orgString))
        {
            orgString = r.Replace(orgString, "<$1>");
            return orgString;
        }

        //4 、   => [4]
        r = new Regex(@"(\d+)\ \、", ops);
        if (r.IsMatch(orgString))
        {
            orgString = r.Replace(orgString, "<$1>");
            return orgString;
        }

        //（1）、 => [4]
        new Regex(@"\（(\d+)\）、", ops);
        if (r.IsMatch(orgString))
        {
            orgString = r.Replace(orgString, "<$1>");
            return orgString;
        }

        //4、    => [4]
        r = new Regex(@"(\d+)\、", ops);
        if (r.IsMatch(orgString))
        {
            orgString = r.Replace(orgString, "<$1>");
            return orgString;
        }

        return orgString;
    }

}