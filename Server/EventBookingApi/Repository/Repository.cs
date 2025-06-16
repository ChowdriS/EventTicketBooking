using System;
using EventBookingApi.Context;
using EventBookingApi.Interface;

namespace EventBookingApi.Repository;

public  abstract class Repository<K, T> : IRepository<K, T> where T:class
{
    protected readonly EventContext _eventContext;

    public Repository(EventContext eventContext)
    {
        _eventContext = eventContext;
    }
    public async Task<T> Add(T item)
    {
        _eventContext.Add(item);
        await _eventContext.SaveChangesAsync();
        return item;
    }

    public async Task<T> Delete(K key)
    {
        var item = await GetById(key);
        if (item != null)
        {
            _eventContext.Remove(item);
            await _eventContext.SaveChangesAsync();
            return item;
        }
        throw new Exception("No such item found for deleting");
    }

    public abstract Task<T> GetById(K key);


    public abstract Task<IEnumerable<T>> GetAll();


    public async Task<T> Update(K key, T item)
    {
        var myItem = await GetById(key);
        if (myItem != null)
        {
            _eventContext.Entry(myItem).CurrentValues.SetValues(item);
            await _eventContext.SaveChangesAsync();
            return item;
        }
        throw new Exception("No such item found for updation");
    }
}

