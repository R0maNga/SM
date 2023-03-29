﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public abstract class AbstractRepository<T> where T : class
    {
        private readonly DbSet<T> _dbSet;
        private readonly OrderServiceContext _context;

        public AbstractRepository(DbSet<T>  dbSet, OrderServiceContext context)
        {
            _dbSet = dbSet;
            _context = context;
        }
        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public virtual void Create(T item)
        {
            _dbSet.Add(item);
        }

        public virtual void Update(T item)
        {
            _dbSet.Update(item);
        }

        public virtual void Delete(T item)
        {
            _dbSet.Remove(item);
        }

    }
}
