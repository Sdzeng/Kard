using Kard.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Kard.Core.Dtos
{
    public class TopMediaDto
    {
        private static readonly Regex _regex = new Regex(@"(?'group1'#)([^#]+?)(?'-group1'#)");

        public int EssayMediaCount { get; set; }

        public int EssayLikeNum { get; set; }

        public int EssayId { get; set; }

        public string CdnPath { get; set; }

        public string MediaExtension { get; set; }


        public string FirstTagName
        {
            get
            {
                if ((!this.EssayContent.IsNullOrEmpty()) && _regex.IsMatch(this.EssayContent))
                {
                    var matchCollection = _regex.Matches(this.EssayContent);
                    return matchCollection.First()?.Value.Replace("#","");
                }
                return string.Empty;
            }
        }

        public string EssayContent { get; set; }

        public string Creator { get; set; }

        public string CreatorNikeName { get; set; }

    }
}
