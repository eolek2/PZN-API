using System.Net;
using API.Filters;
using API.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace API.Data
{
    public class DataRepository : IDataRepository
    {
        private DataContext _dataContext;
        public DataRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            _dataContext.Add<TEntity>(entity);
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            DbSet<TEntity> set = _dataContext.Set<TEntity>();

            EntityEntry<TEntity> dbEntityEntry = _dataContext.Entry(entity);
        
            if (dbEntityEntry.State != EntityState.Deleted)
            {
                dbEntityEntry.State = EntityState.Deleted;
            }
            else
            {
                set.Attach(entity);
                set.Remove(entity);
            }
        }

        public async Task<int?> GetNextPostId(int id, CancellationToken cancellationToken = default)
        {
            var current = await _dataContext.Posts.Where(x=>x.Id == id).FirstOrDefaultAsync();
            if(current != null)
            {
                var date = current.CreationDate;
                if(current.UpdateDate.HasValue)
                    date = current.UpdateDate.Value;

                var sort = new List<KeyValuePair<int,DateTime>>();
                sort.AddRange(await _dataContext.Posts.Where(x=>!x.UpdateDate.HasValue && x.CreationDate > date).Select(x=> new KeyValuePair<int,DateTime>(x.Id, x.CreationDate)).ToListAsync());
                sort.AddRange(await _dataContext.Posts.Where(x=>x.UpdateDate.HasValue && x.UpdateDate.Value > date).Select(x=> new KeyValuePair<int,DateTime>(x.Id, x.UpdateDate.Value)).ToListAsync());

                sort = sort.OrderBy(x=>x.Value).ToList();
                    
                if(sort.Any())
                {
                    return sort.Select(x=>x.Key).First();
                }

                return null;
            }

            return null;
        }

        public async Task<BlogPost> GetPost(int id, CancellationToken cancellationToken = default)
        {
            return await _dataContext.Posts.FirstOrDefaultAsync(x=>x.Id == id, cancellationToken);
        }

        public async Task<ICollection<BlogPost>> GetPosts(PostFilters filters, CancellationToken cancellationToken = default)
        {
            var postsQuery = _dataContext.Posts.AsQueryable();

            if(filters.UserId.HasValue)
                postsQuery = postsQuery.Where(x=>x.UserId == filters.UserId.Value);

            if(filters.FromDate.HasValue)
                postsQuery = postsQuery.Where(x=>x.CreationDate >= filters.FromDate.Value 
                || x.UpdateDate.HasValue && x.UpdateDate.Value >= filters.FromDate.Value);

            
            if(filters.ToDate.HasValue)
                postsQuery = postsQuery.Where(x=>x.CreationDate <= filters.ToDate.Value
                || x.UpdateDate.HasValue && x.UpdateDate.Value <= filters.ToDate.Value);
            

            var posts = await postsQuery.ToListAsync(cancellationToken);

            var sort = new List<KeyValuePair<int,DateTime>>();
            sort.AddRange(posts.Where(x=>!x.UpdateDate.HasValue).Select(x=> new KeyValuePair<int,DateTime>(x.Id, x.CreationDate)).ToList());
            sort.AddRange(posts.Where(x=>x.UpdateDate.HasValue).Select(x=> new KeyValuePair<int,DateTime>(x.Id, x.UpdateDate.Value)).ToList());

            sort = sort.OrderByDescending(x=>x.Value).ToList();

            var finalQuery = from sv in sort
                        join p in posts on sv.Key equals p.Id
                        select p;

            return finalQuery.ToList();
        }

        public async Task<int?> GetPreviousPostId(int id, CancellationToken cancellationToken = default)
        {
            var current = await _dataContext.Posts.Where(x=>x.Id == id).FirstOrDefaultAsync();
            if(current != null)
            {
                var date = current.CreationDate;
                if(current.UpdateDate.HasValue)
                    date = current.UpdateDate.Value;

                var sort = new List<KeyValuePair<int,DateTime>>();
                sort.AddRange(await _dataContext.Posts.Where(x=>!x.UpdateDate.HasValue && x.CreationDate < date).Select(x=> new KeyValuePair<int,DateTime>(x.Id, x.CreationDate)).ToListAsync());
                sort.AddRange(await _dataContext.Posts.Where(x=>x.UpdateDate.HasValue && x.UpdateDate.Value < date).Select(x=> new KeyValuePair<int,DateTime>(x.Id, x.UpdateDate.Value)).ToListAsync());

                sort = sort.OrderByDescending(x=>x.Value).ToList();
                    
                if(sort.Any())
                {
                    return sort.Select(x=>x.Key).First();
                }

                return null;
            }

            return null;
        }

        public async Task<User> GetUser(int id, CancellationToken cancellationToken = default)
        {
            return await _dataContext.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<User> FindUserByEmail(string email, CancellationToken cancellationToken = default)
        {
            return await _dataContext.Users.FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower(), cancellationToken);
        }

        public async Task<ICollection<User>> GetUsers(UserFilters filters, CancellationToken cancellationToken = default)
        {
            var query = _dataContext.Users.AsQueryable();

            if(filters.UserId.HasValue)
                query = query.Where(x=>x.Id == filters.UserId.Value);
            
            if(filters.Age.HasValue)
            {
                int year = DateTime.Now.Year - 50;
                DateTime from = new DateTime(year-1, DateTime.Now.Month, DateTime.Now.Day);
                DateTime to = new DateTime(year, DateTime.Now.Month, DateTime.Now.Day);
                query = query.Where(x=>x.DateOfBirth >= from && x.DateOfBirth <= to);
            }
                
            
            if (!String.IsNullOrWhiteSpace(filters.Email))
                query = query.Where(x=>x.Email.ToLower() == filters.Email.ToLower());

            if (!String.IsNullOrWhiteSpace(filters.FirstName))
                query = query.Where(x=>x.FirstName.ToLower() == filters.FirstName.ToLower());

            if (!String.IsNullOrWhiteSpace(filters.LastName))
                query = query.Where(x=>x.LastName.ToLower() == filters.LastName.ToLower());

            if (!String.IsNullOrWhiteSpace(filters.UserName))
                query = query.Where(x=>x.UserName.ToLower() == filters.UserName.ToLower());

            if (filters.FromDate.HasValue)
                query = query.Where(x=>x.DateOfBirth >= filters.FromDate.Value);

            if (filters.ToDate.HasValue)
                query = query.Where(x=>x.DateOfBirth <= filters.ToDate.Value);

            var ret = await query.ToListAsync(cancellationToken);

            if(ret.Count > 25)
                ret = ret.Take(25).ToList();

            for(int i =0; i<ret.Count(); i++)
            {
                if(String.IsNullOrWhiteSpace(ret[i].AvatarUrl))
                    continue;

                string url = ret[i].AvatarUrl;

                try
                {
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                    req.Timeout = 3000;
                    var response = await req.GetResponseAsync();
                }
                catch
                {
                    ret[i].AvatarUrl = String.Empty;
                }
            }

            return ret;
        }

        public async Task<bool> SaveAll(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }
    }
}