using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IOrderServices
    {
        Task<bool> UpdateStatus(int id, string status);
    }
}