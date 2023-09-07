using LegalGen.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegalGen.Domain.Dtos
{
    public class AiChatResponse
    {
        public List<ResearchBookAiChatDto?> ResearchBooks {  get; set; }
        public List<LegalInformationAiChatDto?> LegalInformations { get; set; }
    }
}
