using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.Event
{
    public class EventDto
    {
    }
    public class AddPointsDto
    {
        public Guid UserId { get; set; }
        public int Points { get; set; }
    }

    public class AddTurnsDto
    {
        public Guid UserId { get; set; }
        public int TurnCount { get; set; }
    }
}
