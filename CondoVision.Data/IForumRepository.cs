using CondoVision.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Data
{
    public interface IForumRepository : IGenericRepository<ForumPost>
    {
        Task<List<ForumPost>> GetRecentPostsAsync(int take = 5);

        Task<List<(string AuthorName, string Content, DateTime CreatedAt)>> GetRecentPostsAsTupleAsync(int take = 5);
    }
}
