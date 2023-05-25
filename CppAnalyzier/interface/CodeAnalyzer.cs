using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace @interface
{
    internal class CodeAnalyzer
    {
        string pattern = @"(\+)|(\-)|(L)|(l)|(\d)";
        private AnalyzerState AnalyzerState = AnalyzerState.FIRST_SYMBOL;
        private int ColumnCount = 0;
        private int errorCounter = 0;
        private bool hasEndLiteral = false;
        RichTextBox richTextBox;
        bool firstDigit = false;
        private List<char> lexems = new List<char>();


        public CodeAnalyzer(string text, RichTextBox richTextBox)
        {
            this.richTextBox = richTextBox;
            AppendInfo("S0");
            foreach (char match in text)
            {
                lexems.Add(match);
                ColumnCount++;

                if (AnalyzerState == AnalyzerState.FIRST_SYMBOL)
                {
                    AppendInfo("S1");
                    if (match == '+' || match == '-' || char.IsDigit(match))
                    {
                        if (char.IsDigit(match))
                        {
                            AppendInfo("S2");
                            firstDigit = true;
                        }
                    }
                    else if (char.ToLowerInvariant(match) == 'l')
                    {
                        AppendError("Число не может начинаться с литерала l");
                        hasEndLiteral = true;
                        errorCounter++;
                    }
                    else
                    {
                        AppendError("В числе не может быть что-либо кроме цифр, плюса, минуса или литерала l ");
                        errorCounter++;
                    }
                    AnalyzerState = AnalyzerState.DIGIT;
                }
                else if (AnalyzerState == AnalyzerState.DIGIT) 
                {
                    if (char.IsDigit(match))
                    {

                            AppendInfo("S2");


                    }
                    else if (match == '+' || match == '-') 
                    {
                        AppendError("Знак числа может быть задан только в его начале");
                        errorCounter++;
                    }
                    else if (char.ToLowerInvariant(match) == 'l')
                    {
                        if (ColumnCount == 2 && !firstDigit)
                        {
                            AppendError("Число не может быть без цифр");
                            hasEndLiteral = true;
                            errorCounter++;
                        }
                        else
                        {
                            AnalyzerState = AnalyzerState.END_DIGIT;
                            hasEndLiteral = true;
                            AppendInfo("S3");
                        }
                    }
                    else
                    {
                        AppendError("В числе не может быть что-либо кроме цифр, плюса, минуса или литерала l ");
                        errorCounter++;
                    }
                }
                else if (AnalyzerState == AnalyzerState.END_DIGIT)
                {
                    if (char.IsDigit(match))
                    {
                        AppendError("После окончания числа не может быть цифр");
                        errorCounter++;
                    }
                    else if (match == '+' || match == '-')
                    {
                        AppendError("Знак числа может быть задан только в его начале");
                        errorCounter++;
                    }
                    else if (char.ToLowerInvariant(match) == 'l')
                    {
                        AppendError("В числе не может быть больше одного литерала l");
                        errorCounter++;
                    }
                    else
                    {
                        AppendError("В числе не может быть что-либо кроме цифр, плюса, минуса или литерала l ");
                        errorCounter++;
                    }
                }
            }
            if (errorCounter == 0 && hasEndLiteral)
            {
                AppendInfo("TRUE");
            }
           
            if (!hasEndLiteral) 
            {
                AppendError("Конец числа должен быть литералом l или L");
            }
        }

        private void AppendError(string error = "")
        {
            richTextBox.AppendText($"{error}{", ошибка в позиции "}{ColumnCount}{Environment.NewLine}");
        }

        private void AppendInfo(string info = "")
        {
            richTextBox.AppendText($"{info}{Environment.NewLine}");
        }
    }
}
