using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Application.Interface
{
    public interface IAimlService
    {
        public Task<string> GetChatResponseAsync(string userPrompt);
    }
}
