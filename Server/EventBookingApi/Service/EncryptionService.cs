using System;
using System.Security.Cryptography;
using System.Text;
using EventBookingApi.Interface;
using EventBookingApi.Model;

namespace EventBookingApi.Service;

public class EncryptionService : IEncryptionService
    {
        public  async Task<EncryptModel> EncryptData(EncryptModel data)
        {
            data.EncryptedData = BCrypt.Net.BCrypt.HashPassword(data.Data);
            return data;
        }
    }
    
