using API.Filters;
using API.Types;

namespace API.Data
{
    public interface IDataRepository
    {
        public Task<User> GetUser(int id, CancellationToken cancellationToken = default(CancellationToken));
        public Task<User> FindUserByEmail(string email, CancellationToken cancellationToken = default(CancellationToken));
        public Task<ICollection<User>> GetUsers(UserFilters filters, CancellationToken cancellationToken = default(CancellationToken));
        public Task<ICollection<BlogPost>> GetPosts(PostFilters filters, CancellationToken cancellationToken = default(CancellationToken));
        public Task<BlogPost> GetPost(int id, CancellationToken cancellationToken = default(CancellationToken));
        public Task<int?> GetNextPostId(int id, CancellationToken cancellationToken = default(CancellationToken));
        public Task<int?> GetPreviousPostId(int id, CancellationToken cancellationToken = default(CancellationToken));
        public void Add<TEntity>(TEntity entity) where TEntity : class;

        public void Delete<TEntity>(TEntity entity) where TEntity : class;
        public Task<bool> SaveAll(CancellationToken cancellationToken = default(CancellationToken));
    }
}