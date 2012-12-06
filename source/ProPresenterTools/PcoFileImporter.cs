using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ProPresenter.BO.Common;
using ProPresenter.BO.RenderingEngine.Entities;
using ProPresenter.BO.RenderingEngine.Entities.Interface;

namespace ProPresenterTools
{
    internal class PcoFileImporter
    {
        private static readonly string[] delimiterString = new[] { "\r\n\r\n", "\n\n" };

        public RVPresentationParameters PresentationParams { get; private set; }

        public PcoFileImporter(RVPresentationParameters parameter)
        {
            this.PresentationParams = parameter;
        }

        public RVPresentation GetImportedPresentation(string text, string name)
        {
            RVPresentation presentation = null;

            if (!string.IsNullOrEmpty(text))
            {
                text = Helper.TranslateLabels(text);

                text = Regex.Replace(text, @"(?!\r)\n", "\r\n");
                text = text.Replace("\r\r\n", "\r\n");
                
                presentation = new RVPresentation(name, this.PresentationParams.Height, this.PresentationParams.Width);
                presentation.Category = this.PresentationParams.SelectedCategory;
                
                string[] strArray = delimiterString[0] != "\r\n"
                    ? text.Split(delimiterString, StringSplitOptions.RemoveEmptyEntries)
                    : text.Split(delimiterString, StringSplitOptions.None);

                StringBuilder stringBuilder = new StringBuilder();
                int num = 0;

                RVSlideGroup group = null;
                
                foreach (string part in strArray)
                {
                    if (!string.IsNullOrEmpty(part))
                    {
                        string trimmed = part.TrimStart(new char[0]);

                        if (Helper.IsValidLabel(trimmed))
                        {
                            string slideText = stringBuilder.ToString();
                            stringBuilder.Clear();
                            
                            if (!string.IsNullOrEmpty(slideText))
                                this.AddNewSlide(presentation, slideText, group);

                            group = new RVSlideGroup();

                            if (trimmed.IndexOf("\r\n") < 0)
                            {
                                group.SetLabel(trimmed);
                                num = 0;
                            }
                            else
                            {
                                group.SetLabel(trimmed.Substring(0, trimmed.IndexOf("\r\n")));

                                stringBuilder.AppendLine(trimmed.Substring(trimmed.IndexOf("\r\n") + 2));
                                num = 1;
                            }

                            presentation.AddSlideGroup(group);
                        }
                        else
                        {
                            stringBuilder.Append(part);
                            ++num;

                            if (num == this.PresentationParams.DelemetersPerSlide)
                            {
                                string slideText = stringBuilder.ToString();
                                stringBuilder.Clear();
                                num = 0;

                                this.AddNewSlide(presentation, slideText, group);
                            }
                            else
                            {
                                stringBuilder.AppendLine();
                            }
                        }
                    }
                }

                this.AddNewSlide(presentation, stringBuilder.ToString(), group);
            }

            return presentation;
        }

        private void AddNewSlide(IRVPresentation importedPresentation, string slideText, RVSlideGroup slideGroup = null)
        {
            if (slideGroup != null)
            {
                slideGroup.AddSlide(this.GetNewSlide(slideText));
            }
            else
            {
                importedPresentation.AddSlide(this.GetNewSlide(slideText));
            }
        }

        private RVDisplaySlide GetNewSlide(string slideText)
        {
            RVDisplaySlide slide = RVDisplaySlide.CreateSlide(true);
            
            RVText rvText = slide.PrimaryTextElement ?? RVText.GetDefaultTextElement(slideText);
            rvText.PlainText = slideText;
            slide.Add(rvText);
            
            return slide;
        }

        private static class Helper
        {
            private static readonly string[] labels = new[]
            {
              "verse",
              "chorus",
              "pre-chorus",
              "bridge",
              "misc"
            };

            public static string TranslateLabels(string text)
            {
                text = text
                    .Replace("strophe", "verse")
                    .Replace("Strophe", "verse")
                    .Replace("refrain", "chorus")
                    .Replace("Refrain", "chorus");

                text = Regex.Replace(text, @"(\d)+. verse", @"verse $1");

                return text;
            }

            public static bool IsValidLabel(string suggestedLabel)
            {
                return Helper.labels.Any(label => suggestedLabel.StartsWith(label, StringComparison.InvariantCultureIgnoreCase));
            }
        }
    }
}
