using System;
using System.Collections.Generic;
using System.Text;

namespace BackgroundManagement.Models.Commands
{
    public class CommandDetails
    {
        public List<string> CommandElementList { get; set; } = new List<string>();

        public string GetFullCommand()
        {
            if (this.CommandElementList.Count == 0)
                return String.Empty;

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < this.CommandElementList.Count; i++)
            {
                sb.Append($"{(i > 0 ? " " : "")}{this.CommandElementList[i]}");
            }

            return sb.ToString();
        }
    }
}
