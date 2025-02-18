using Humanizer;
using System;

namespace MyFirestoreApi.Services
{
    public class NumberToEnglishWordConverter
    {
        public string ConvertToWords(double number)
        {
            if (number < 0) throw new ArgumentException("Number must be non-negative");

            // Round number to handle two decimal places
            double wholeNumber = Math.Floor(number);
            string numberInWords = ((int)wholeNumber).ToWords();

            // Handle decimal part
            double decimalPart = Math.Round((number - wholeNumber) * 100, 0);
            string decimalPartInWords = ((int)decimalPart).ToWords();

            string bath = "bath";

            return decimalPart > 0 ? $"{numberInWords} {decimalPartInWords} {bath}" : $"{numberInWords} {bath}";
        }
    }

    public class NumberToThaiWordConverter
    {
        private static readonly string[] Units = { "ศูนย์", "หนึ่ง", "สอง", "สาม", "สี่", "ห้า", "หก", "เจ็ด", "แปด", "เก้า" };
        private static readonly string[] Tens = { "", "สิบ", "ยี่สิบ", "สามสิบ", "สี่สิบ", "ห้าสิบ", "หกสิบ", "เจ็ดสิบ", "แปดสิบ", "เก้าสิบ" };
        private static readonly string[] Thousands = { "", "พัน", "หมื่น", "แสน", "ล้าน" };

        public string ConvertToWords(double number)
        {
            if (number < 0)
                throw new ArgumentException("Number must be non-negative");

            // Round number to handle two decimal places
            number = Math.Round(number, 2);

            // Convert whole number part
            int wholeNumber = (int)Math.Floor(number);
            string words = ConvertWholeNumberToWords(wholeNumber);

            // Handle decimal part if necessary
            int decimalPart = (int)Math.Round((number - wholeNumber) * 100);
            if (decimalPart > 0)
            {
                words += "บาท"; 
                words += $"{ConvertWholeNumberToWords(decimalPart)}สตางค์"; 
            }
            else
            {
                words += "บาทถ้วน";
            }

            return words;
        }

        private string ConvertWholeNumberToWords(int number)
        {
            if (number == 0) return "ศูนย์";

            string words = "";

            for (int i = 0; number > 0 && i < Thousands.Length; i++)
            {
                int sectionValue = number % 10000;
                number /= 10000;

                if (sectionValue > 0)
                {
                    string sectionWords = ConvertSectionToWords(sectionValue);
                    if (i > 0)
                        sectionWords += Thousands[i];
                    words = sectionWords + words;
                }
            }

            return words;
        }

        private string ConvertSectionToWords(int number)
        {
            if (number == 0) return "";

            string words = "";

            // Handle thousands
            if (number >= 1000)
            {
                int thousands = number / 1000;
                words += Units[thousands] + "พัน";
                number %= 1000;
            }

            // Handle hundreds
            if (number >= 100)
            {
                int hundreds = number / 100;
                words += Units[hundreds] + "ร้อย";
                number %= 100;
            }

            // Handle tens
            if (number >= 10)
            {
                int tens = number / 10;
                words += Tens[tens];
                number %= 10;
            }

            // Handle units
            if (number > 0)
            {
                if (number == 1 && words.Length > 0)
                    words += "เอ็ด"; // Special case for "หนึ่ง" after "สิบ"
                else
                    words += Units[number];
            }

            return words;
        }
    }
}
