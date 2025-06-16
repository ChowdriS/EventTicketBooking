using System;

namespace EventBookingApi.Interface;

public interface IRepository<K, T> where T : class
{
    public Task<T> Add(T item);
    public Task<T> GetById(K key);
    public Task<IEnumerable<T>> GetAll();
    public Task<T> Update(K key, T item);
    public Task<T> Delete(K key);
}