using CondoVision.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Data
{
    public class ForumRepository : GenericRepository<ForumPost>, IForumRepository
    {
        public ForumRepository(DataContext context) : base(context)
        {
        }

        public async Task<List<ForumPost>> GetRecentPostsAsync(int take = 5)
        {
            return await base.GetRecentAsync(take); 
        }

       
        public async Task<List<(string AuthorName, string Content, DateTime CreatedAt)>> GetRecentPostsAsTupleAsync(int take = 5)
        {
            var posts = await GetRecentPostsAsync(take);
            return posts.Select(p => (p.AuthorName, p.Content, p.CreatedAt)).ToList();
        }


    }
}
