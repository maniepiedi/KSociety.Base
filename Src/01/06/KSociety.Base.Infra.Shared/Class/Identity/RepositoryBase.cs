﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using KSociety.Base.Infra.Shared.Csv;
using KSociety.Base.Infra.Shared.Interface;
using KSociety.Base.Infra.Shared.Interface.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.Infra.Shared.Class.Identity
{
    public abstract class RepositoryBase<TContext, TEntity, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> : IRepositoryBase<TEntity>
        where TContext : DbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
        where TEntity : class
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>
        where TUserToken : IdentityUserToken<TKey>
    {
        private TContext _dataContext;
        private readonly ILoggerFactory _loggerFactory;

        protected TContext DataContext => _dataContext ??= DatabaseFactory.Get();

        protected readonly DbSet<TEntity> DataBaseSet;

        protected readonly ILogger<RepositoryBase<TContext, TEntity, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>> Logger;

        protected IDatabaseFactory<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> DatabaseFactory { get; private set; }

        protected bool Exists => DataContext.Exists();

        protected RepositoryBase(ILoggerFactory loggerFactory, IDatabaseFactory<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> databaseFactory)
        {
            _loggerFactory = loggerFactory;
            DatabaseFactory = databaseFactory;
            DataBaseSet = DataContext.Set<TEntity>();

            Logger = _loggerFactory.CreateLogger<RepositoryBase<TContext, TEntity, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>>();
        }

        public virtual EntityEntry<TEntity> Add(TEntity entity)
        {
            if (!Exists)
            {
                Logger.LogWarning("Database not exists!");
                return null;
            }

            var result = DataBaseSet.Add(entity);
            Logger.LogTrace("RepositoryBase Add: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + entity.GetType().FullName + ")" + " State: " + result.State);
            return result;
        }

        public virtual async ValueTask<EntityEntry<TEntity>> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (!Exists)
            {
                Logger.LogWarning("Database not exists!");
                return null;
            }
            var result = await DataBaseSet.AddAsync(entity, cancellationToken).ConfigureAwait(false);
            Logger.LogTrace("RepositoryBase AddAsync: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + entity.GetType().FullName + ")" + " State: " + result.State);
            return result;
        }

        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            if (!Exists)
            {
                Logger.LogWarning("Database not exists!");
            }
            else
            {

                DataBaseSet.AddRange(entities);
                Logger.LogTrace("RepositoryBase AddRange: " + GetType().FullName + "." +
                                System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }
        }

        public virtual async ValueTask AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            if (!Exists)
            {
                Logger.LogWarning("Database not exists!");
            }
            else
            {
                await DataBaseSet.AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
                Logger.LogTrace("RepositoryBase AddRangeAsync: " + GetType().FullName + "." +
                                System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }
        }

        public virtual EntityEntry<TEntity> Update(TEntity entity)
        {
            if (!Exists)
            {
                Logger.LogWarning("Database not exists!");
                return null;
            }
            var result = DataBaseSet.Update(entity);
            Logger.LogTrace("RepositoryBase Update: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + entity.GetType().FullName + ")" + " State: " + result.State);
            return result;
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entities)
        {
            if (!Exists)
            {
                Logger.LogWarning("Database not exists!");
            }
            else
            {

                DataBaseSet.UpdateRange(entities);
                Logger.LogTrace("RepositoryBase UpdateRange: " + GetType().FullName + "." +
                                System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }
        }

        public virtual EntityEntry<TEntity> Delete(TEntity entity)
        {
            if (!Exists)
            {
                Logger.LogWarning("Database not exists!");
                return null;
            }
            var result = DataBaseSet.Remove(entity);
            Logger.LogTrace("RepositoryBase Delete: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + entity.GetType().FullName + ")" + " State: " + result.State);
            return result;
        }

        public virtual void DeleteRange(IEnumerable<TEntity> entities)
        {
            if (!Exists)
            {
                Logger.LogWarning("Database not exists!");
            }
            else
            {

                DataBaseSet.RemoveRange(entities);
                Logger.LogTrace("RepositoryBase DeleteRange: " + GetType().FullName + "." +
                                System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }
        }

        public virtual IEnumerable<EntityEntry<TEntity>> Delete(Expression<Func<TEntity, bool>> where)
        {
            Logger.LogTrace("RepositoryBase Delete: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + where.GetType().FullName + ")");
            if (!Exists)
            {
                Logger.LogWarning("Database not exists!");
                return null;
            }

            IEnumerable<TEntity> objects = DataBaseSet.Where(@where).AsEnumerable();
            IEnumerable<EntityEntry<TEntity>> output = new List<EntityEntry<TEntity>>();
            foreach (var obj in objects)
            {
                output.Append(DataBaseSet.Remove(obj));
            }

            return output;

        }

        public TEntity Find(params object[] keyObject)
        {
            Logger.LogTrace("RepositoryBase Find: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + keyObject.GetType().FullName + ")");
            if (Exists) return DataBaseSet.Find(keyObject);
            Logger.LogWarning("Database not exists!");
            return null;

        }

        public async ValueTask<TEntity> FindAsync(CancellationToken cancellationToken = default, params object[] keyObject)
        {
            Logger.LogTrace("RepositoryBase FindAsync: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + keyObject.GetType().FullName + ")");
            if (Exists) return await DataBaseSet.FindAsync(keyObject).ConfigureAwait(false);
            Logger.LogWarning("Database not exists!");
            return null;

        }

        public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> filter)
        {
            Logger.LogTrace("RepositoryBase Query: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + filter.GetType().FullName + ")");
            if (Exists) return DataBaseSet.Where(filter);
            Logger.LogWarning("Database not exists!");
            return null;

        }

        public IQueryable<TEntity> QueryObjectGraph(Expression<Func<TEntity, bool>> filter, string children)
        {
            Logger.LogTrace("RepositoryBase QueryObjectGraph: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + filter.GetType().FullName + "," + children + ")");
            if (Exists) return DataBaseSet.Include(children).Where(filter);
            Logger.LogWarning("Database not exists!");
            return null;

        }

        public IQueryable<TEntity> FindAll()
        {
            Logger.LogTrace("RepositoryBase FindAll: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            if (Exists) return DataBaseSet;
            Logger.LogWarning(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + " Database not exists!");
            return null;
        }

        public void ImportCsv(string fileName)
        {
            Logger.LogTrace("RepositoryBase ImportCsv: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            var result = ReadCsv<TEntity>.Import(_loggerFactory, fileName);
            if (!result.Any()) return;
            DeleteRange(FindAll());

            AddRange(result);
        }

        public async ValueTask ImportCsvAsync(string fileName, CancellationToken cancellationToken = default)
        {
            Logger.LogTrace("RepositoryBase ImportCsvAsync: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            var result = ReadCsv<TEntity>.ImportAsync(_loggerFactory, fileName);

            DeleteRange(FindAll());

            await foreach (var entity in result.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                await AddAsync(entity, cancellationToken).ConfigureAwait(false);
            }

        }

        public void ExportCsv(string fileName)
        {
            Logger.LogTrace("RepositoryBase ExportCsv: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            WriteCsv<TEntity>.Export(_loggerFactory, fileName, FindAll());
        }

        public async ValueTask ExportCsvAsync(string fileName, CancellationToken cancellationToken = default)
        {
            Logger.LogTrace("RepositoryBase ExportCsvAsync: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            await WriteCsv<TEntity>.ExportAsync(_loggerFactory, fileName, FindAll()).ConfigureAwait(false);
        }

        public void Dispose()
        {
            DataContext.Dispose();
        }
    }
}
