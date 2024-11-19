using System.Collections.Generic;

namespace Connexien.Lib.Communication
{
    public interface IEmailParameters
    {
        List<string> Attachments { get; set; }

        string BodyText { get; set; }

        string EmailAddress { get; set; }

        string Subject { get; set; }
    }
}