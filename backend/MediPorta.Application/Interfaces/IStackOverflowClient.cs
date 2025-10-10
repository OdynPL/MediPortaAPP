using MediPorta.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediPorta.Application.Interfaces
{
    public interface IStackOverflowClient
    {
        Task<List<StackOverflowTag>> GetTagsAsync(int count);
    }
}
