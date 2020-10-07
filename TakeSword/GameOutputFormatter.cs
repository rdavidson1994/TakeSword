using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace TakeSword
{
    public class ConsoleOutputFormatter : IGameOutputFormatter
    {
        public IVerbalAI<PhysicalActor>? VerbalAI { get; set; }
        public string FormatString(FormattableString formattableString)
        {
            var pieces = Regex.Split(formattableString.Format, @"{\d+}");
            object?[] arguments = formattableString.GetArguments() ?? new object?[] { };
           
            var argumentCount = formattableString.ArgumentCount;
            List<string> outList = new List<string>();
            for (int i=0; i<argumentCount; i++)
            {
                outList.Add(pieces[i]);
                object? argument = arguments[i];
                if (argument is GameObject gameObject)
                {
                    if (VerbalAI != null)
                    {
                        outList.Add(gameObject.DisplayName(VerbalAI.Actor));
                    }
                    else
                    {
                        outList.Add(gameObject.DisplayName(null));
                    }
                }
                else if (argument != null)
                {
                    outList.Add(argument.ToString() ?? "");
                }
                else
                {
                    outList.Add("");
                }
            }
            outList.Add(pieces[argumentCount]);
            return string.Join("", outList);
        }
    }

    ////TBD...
    ////Will transform the arguments and format string into a sequence of JSON messages
    ////
    //public class JsonOutputFormatter : IGameOutputFormatter
    //{
    //    public IVerbalAI VerbalAI { get; set; }

    //    public string FormatString(FormattableString formattableString)
    //    {
    //        var pieces = Regex.Split(formattableString.Format, @"{\d+}");
    //        var arguments = formattableString.GetArguments();
    //        var argumentCount = formattableString.ArgumentCount;
    //        List<string> outList = new List<string>();
    //        for (int i = 0; i < argumentCount; i++)
    //        {
    //            var representation = new
    //            {
    //                Type = "text",
    //                Text = pieces[i]
    //            };
    //            string json = JsonConvert.SerializeObject(representation);
    //            outList.Add(json);
    //            object argument = arguments[i];
    //            if (argument is GameObject gameObject)
    //            {
    //                outList.Add(gameObject.DisplayName(VerbalAI.GetActor()));
    //            }
    //            else
    //            {
    //                outList.Add(argument.ToString());
    //            }
    //        }
    //    }
    //}
}
