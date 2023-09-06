using LegalGen.Domain.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegalGen.Domain.Interfaces
{
    public interface IEmailService
    {
        void SendEmail(EmailMessage message);
    }
}
